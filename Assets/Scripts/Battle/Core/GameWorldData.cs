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
	public RoomPreset RoomPreset;
	public LoreStoneSettings LoreInRoom;
}
