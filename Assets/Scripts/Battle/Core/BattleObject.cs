using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public abstract class BattleObject
{
	/// <summary>
	/// Immutable settings for battle objects.
	/// </summary>
	public readonly BattleObjectSettings Settings;

	/// <summary>
	/// Battle object that spawned this, null if spawned as part of the level.
	/// </summary>
	public readonly BattleObject Parent;

	/// <summary>
	/// Room that this object currently belongs.
	/// </summary>
	private Room room;

	/// <summary>
	/// To which side does this object belong?
	/// </summary>
	public OwnerId Owner;

	/// <summary>
	/// Position of the object inside the current room.
	/// </summary>
	public float2 Position;

	/// <summary>
	/// Physical velocity of the object.
	/// </summary>
	public float2 Velocity;

	/// <summary>
	/// Physical size of the object.
	/// </summary>
	public float2 Size;

    /// <summary>
    /// Is the object currently active? Will be removed from the list of active objects in room if not.
    /// </summary>
    public bool IsActive = true;

	/// <summary>
	/// Time ticked since this object has been added to the battle.
	/// </summary>
	public float TimeSinceInit;

	/// <summary>
	/// If not set, next act will invoke OnFirstAct,
	/// </summary>
	private bool isFirstTickDone;

	/// <summary>
	/// Parent level of my room.
	/// </summary>
	public GameWorld Level => room.World;

	/// <summary>
	/// 
	/// </summary>
	public bool WasPartOfTheRoom;

	/// <summary>
	/// Player convenience getter.
	/// </summary>
	public Player Player => room.World.Player;

	public float DistanceToPlayer => distance(Position, Player.Position);

	/// <summary>
	/// Room that this object currently belongs.
	/// </summary>
	public Room Room
	{
		get { return room; }
		set
		{
			if (room != value && value != null)
			{
				room?.OnObjectLeft(this);
				room = value;
				room?.OnObjectAdded(this);
			}
		}
	}

	public BattleObject(Room room, BattleObjectSettings settings, BattleObject parent, float2 position) :
		this(room, settings, parent != null ? parent.Owner : OwnerId.Neutral, position)
	{
		Parent = parent;
	}

	public BattleObject(Room room, BattleObjectSettings settings, OwnerId owner, float2 position)
	{
		if (settings == null)
			throw new ArgumentNullException($"Tried to spawn object {GetType()} with null settings!");

		Owner = owner;
		Settings = settings;
		Size = settings.Size;
		Position = position;
		Room = room;
		SendViewEvent(ViewEventType.Init);
	}

	/// <summary>
	/// Action done by this object on main game loop tick.
	/// </summary>
	public void Act(float dT)
	{
		if (!isFirstTickDone)
		{
			OnFirstAct();
			isFirstTickDone = true;
		}

		OnAct(dT);

		TimeSinceInit += dT;
	}

	/// <summary>
	/// Custom logic to be done on the first tick.
	/// </summary>
	protected virtual void OnFirstAct() {}

	/// <summary>
	/// Custom logic to be done on every tick.
	/// </summary>
	protected virtual void OnAct(float dT) {}

	/// <summary>
	/// Deal damage to this object.
	/// </summary>
	public virtual void DealDamage(float damage, BattleObject source) {}

	/// <summary>
	/// Check if object should be destroyed.
	/// </summary>
	public virtual void CheckHealth() {}

	/// <summary>
	/// Called when object is dead.
	/// </summary>
	public void Deactivate()
	{
		if (IsActive)
		{
			IsActive = false;
			OnDeactivate();
			SendViewEvent(ViewEventType.End);
		}
	}

	/// <summary>
	/// Callback invoked whenever object collides with other object.
	/// </summary>
	public virtual void OnCollisionWith(BattleObject other) {}

	/// <summary>
	/// Callback invoked when object collides with the wall.
	/// </summary>
	public virtual void OnCollisionWithWall() {}

	/// <summary>
	/// Custom logic to be done when objects gets removed from the battle.
	/// </summary>
	protected virtual void OnDeactivate() {}

	/// <summary>
	/// Dispatches a view event into the pipe, so view can handle stuff in the view pass.
	/// </summary>
	public void SendViewEvent(ViewEventType eventType, object data = null)
	{
		Room.World.ViewEventPipe.SendEvent(this, eventType, data);
	}

	#region Physics

	public void Push(float2 direction, float intensity)
	{
		Velocity += math.normalize(direction) * intensity / Settings.Mass;
	}

	public void PushFrom(float2 source, float intensity)
	{
		Push(normalizesafe(Position - source), intensity);
	}

	public float2 RandomDirection => room.World.RandomDirection;

	#endregion

	#region Projectiles

	public Projectile FireProjectile(float2 position, float damage, ProjectileSettings projectileType)
	{
		var projectile = new Projectile(Room, damage, projectileType, this, Position);
		projectile.DirectAt(position);
		return projectile;
	}

	#endregion

	#region Helpers

	public bool IsPlayer => Owner == OwnerId.Player;
	public bool IsEnemy => Owner == OwnerId.Enemy;
	public bool IsNeutral => Owner == OwnerId.Neutral;

	/// <summary>
	/// Converts 2D position + room position into a position assignable to transform.
	/// </summary>
	public Vector3 Position3D => Room.Position3D + new Vector3(Position.x, Position.y, Position.y * 0.25f);

	/// <summary>
	/// Gets a list of all objects in my room, in radius, excluding me.
	/// </summary>
	public List<Unit> AllUnitsInRadius(float radius, bool includeSelf = false)
	{
		var list = new List<Unit>();

		foreach(var unit in room.AllUnits)
		{
			if (unit == this && !includeSelf)
			{
				continue;
			}

			if (distance(unit.Position, Position) <= radius)
			{
				list.Add(unit);
			}
		}

		return list;
	}

	/// <summary>
	/// Gets a closest objecy in my room, in radius, excluding me.
	/// </summary>
	public Unit ClosestUnitInRadius(float radius)
	{
		Unit closest = null;
		var minDist = radius*radius;

		foreach(var unit in room.AllUnits)
		{
			if (unit == this)
			{
				continue;
			}

			var dist = distancesq(unit.Position, Position);
			if (minDist > dist)
			{
				minDist = dist;
				closest = unit;
			}
		}

		return closest;
	}

	#endregion Helpers
}

public enum OwnerId
{
	Player = 0,
	Enemy = 1,
	Neutral = 2
}

public enum CollisionType
{
	NoCollision = 0,
	HitDetectionOnly = 1,
	Radius = 2,
}