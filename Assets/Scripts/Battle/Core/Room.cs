
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Room
{
	public readonly GameWorld World;
	public readonly RoomPreset RoomData;
	public Room NextRoom { get; private set; }
	public Room PreviousRoom { get; private set; }
	public readonly int X, Y;
	public readonly float2 Position;
	public readonly Vector3 Position3D;
	public readonly float Width, Height;
	public readonly List<BattleObject> AllObjects = new List<BattleObject>();
	public readonly List<Unit> AllUnits = new List<Unit>();
	public float2 DoorPosition;
	public float2 EntryPosition;

	public Room(GameWorld gameWorld, int x, int y, RoomPreset data)
	{
		RoomData = data;
		World = gameWorld;
		X = x;
		Y = y;

		// position room
		var gameSettings = GameSettings.Instance;
		Width = gameSettings.RoomWidth;
		Height = gameSettings.RoomHeight;
		Position3D.x = X * (Width + gameSettings.RoomWidthPadding);
		Position3D.y = Y * (Height + gameSettings.RoomHeightPadding);
		data.SpawnStuffIntoRooms(this);
	}

	public void SetNextRoom(Room nextRoom)
	{
		if (nextRoom == null) return;

		NextRoom = nextRoom;

		if (NextRoom.X == X + 1)
		{
			DoorPosition = new float2(GameSettings.Instance.RoomWidth / 2f, 0f);
		}
		else if (NextRoom.Y == Y + 1)
		{
			DoorPosition = new float2(0f, GameSettings.Instance.RoomHeight / 2f);
		}
	}

	public void SetRoomBefore(Room roomBefore)
	{
		if (roomBefore == null) return;

		PreviousRoom = roomBefore;

		if (PreviousRoom.X == X - 1)
		{
			EntryPosition = new float2(-GameSettings.Instance.RoomWidth / 2f, 0f);
		}
		else if (PreviousRoom.Y == Y - 1)
		{
			EntryPosition = new float2(0f, -GameSettings.Instance.RoomHeight / 2f);
		}
	}

	public bool CanProgressToNextRoom()
	{
		foreach(var unit in AllUnits)
		{
			if (unit.Owner == OwnerId.Enemy) return false;
		}

		return true;
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
		AllUnits.RemoveAll(x => !x.IsActive);
	}
}
