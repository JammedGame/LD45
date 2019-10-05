using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class RoomPreset : ScriptableObject
{
	public string Name = "Room";
	[Table] public List<ObjectInRoomData> StuffInRoom;
}

[Serializable]
public class ObjectInRoomData
{
	public BattleObjectSettings ObjectType;
	public float2 Position;
}