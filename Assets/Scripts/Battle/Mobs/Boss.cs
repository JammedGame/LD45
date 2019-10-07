using System;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Boss1 : Mob
{
    private Room _Room;
    private float _SpawnProgress;
    public Boss1(MobSettings mobSettings, Room room, float2 position) : base(mobSettings, room, position)
	{
        this._Room = room;
	}
    public override Projectile FireProjectile(float2 position, float damage, ProjectileSettings projectileType)
	{
		var projectile1 = new Projectile(Room, damage, projectileType, this, new Vector2(Position.x - 1.5f, Position.y));
		projectile1.DirectAt(new Vector2(position.x - 1.5f, position.y));
        var projectile2 = new Projectile(Room, damage, projectileType, this, new Vector2(Position.x + 1.6f, Position.y));
		projectile2.DirectAt(new Vector2(position.x + 1.6f, position.y));
		return projectile1;
	}
    protected override void OnAct(float dT)
	{
		if (TimeSinceInit < MobSettings.SpawnDuration)
		{
			CurrentAnimation = WasPartOfTheRoom ? AnimationType.Idle : AnimationType.Spawn;
			return;
		}

		Velocity *= 0.98f;
		AttackProgress -= dT;
        _SpawnProgress -= dT;
        if(_SpawnProgress > 0) return;
		var distanceToPlayer = DistanceToPlayer;
		if (distanceToPlayer <= MobSettings.AttackRange || AttackProgress > 0f)
		{
			AttackAct(dT);
		}
		else if (distanceToPlayer <= MobSettings.AggroRange || Enraged)
		{
            if(UnityEngine.Random.Range(0f, 1f) < 0.01f)
            {
                Debug.Log("Spawn");
                Spawn();
            }
            else AggroAct(dT);
		}
		else
		{
            PatrolAct(dT);
		}
	}
    private void Spawn()
    {
        _SpawnProgress = 1;
        CurrentAnimation = AnimationType.Minion;
        MobSettings.Minion.SpawnIntoRoom(this._Room, new Vector2(Position.x, Position.y - 2f));
    }
}