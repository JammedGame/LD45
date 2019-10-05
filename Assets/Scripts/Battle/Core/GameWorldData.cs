using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public class GameWorldData
{
	[Table] public List<RoomData> Rooms;
}

[Serializable]
public class RoomData
{
	public int X;
	public int Y;
	public RoomPreset RoomPreset;
}
