using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class GameWorldData : ScriptableObject
{
	[Table] public List<RoomData> Rooms;
	public List<float2> MobSpawnPoints;
	public List<float2> PotSpawnPoints;
	public PropSettings Pot;
	public ChestSettings Chest;
	public float2 ItemSpawnPoint => float2.zero; 
}

[Serializable]
public class RoomData
{
	public RoomPreset RoomPreset;
	public LoreStoneSettings LoreInRoom;
}
