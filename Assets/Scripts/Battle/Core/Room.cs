
using System.Collections.Generic;
using UnityEngine;

public class Room
{
	public readonly GameWorld World;
	public readonly RoomData RoomData;
	public readonly Vector3 Position3D;
	public readonly float Width, Height;
	public readonly List<BattleObject> AllObjects = new List<BattleObject>();
	public readonly List<Unit> AllUnits = new List<Unit>();

	public Room(GameWorld gameWorld, RoomData data)
	{
		RoomData = data;
		World = gameWorld;

		// position room
		var gameSettings = GameSettings.Instance;
		Width = gameSettings.RoomWidth;
		Height = gameSettings.RoomHeight;
		Position3D.x = data.X * (Width + gameSettings.RoomWidthPadding);
		Position3D.y = data.Y * (Height + gameSettings.RoomHeightPadding);

		// spawn objects
		data.RoomPreset.StuffInRoom.ForEach(x => x.ObjectType.SpawnIntoRoom(this, x.Position));
	}

	public void OnObjectAdded(BattleObject battleObject)
	{
		if (battleObject.Room == this)
		{
			AllObjects.Add(battleObject);
			if (battleObject is Unit unit) AllUnits.Add(unit);
		}
	}

	public void OnObjectLeft(BattleObject battleObject)
	{
		AllObjects.Remove(battleObject);
		if (battleObject is Unit unit) AllUnits.Remove(unit);
	}

	public void Tick(float dT)
	{
		TickObjects(dT);
		TickCollision(dT);
		CleanUpDeadStuff();
	}

	private void TickObjects(float dT)
	{
		for(int i = 0; i < AllObjects.Count; i++)
		{
			AllObjects[i].Act(dT);
		}
	}

	private void TickCollision(float dT)
	{
		PhysicsJob.Run(PhysicsSettings.Instance, this, dT);
	}

	private void CleanUpDeadStuff()
	{
		for(int i = 0; i < AllObjects.Count; i++)
		{
			AllObjects[i].CheckHealth();
		}

		AllObjects.RemoveAll(x => !x.IsActive);
	}
}
