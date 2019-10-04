using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class GameWorldData
{
	public List<RoomData> Rooms;
}

[Serializable]
public class RoomData
{
	public int X;
	public int Y;
	public string Name => "Room"; // change this to change background visuals
	[Table] public List<ObjectInRoomData> StuffInRoom;
}

[Serializable]
public class ObjectInRoomData
{
	public BattleObjectSettings ObjectType;
	public float2 Position;
}