using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class RoomPreset : ScriptableObject
{
	public string Name = "Room";

	public int PotCount;
	public float ChestSpawnProb;
	public List<MobSettings> FirstWave;
	public List<MobSettings> SecondWave;
	public List<MobSettings> ThirdWave;

	// fixed stuff
	[Table] public List<ObjectInRoomData> StuffInRoom;

	public bool SpawnWave(Room room, int index)
	{
		var levelData = room.World.GameWorldData;

		switch(index)
		{
			case 0:
				return FirstWave.SpawnIntoRoom(room, levelData.MobSpawnPoints, true);
			case 1:
				return SecondWave.SpawnIntoRoom(room, levelData.MobSpawnPoints, false);
			case 2:
				return SecondWave.SpawnIntoRoom(room, levelData.MobSpawnPoints, false);
		}

		return false;
	}

	public void SpawnStuffIntoRooms(Room room)
	{
		var levelData = room.World.GameWorldData;

		// mobs
		SpawnWave(room, 0);
		
		// pots
		var positions = levelData.PotSpawnPoints.GetSpawnPoints(PotCount);
		for(int i = 0; i < PotCount; i++)
		{
			var drop = UnityEngine.Random.Range(0f, 1f) <= ChestSpawnProb ? (BattleObjectSettings)levelData.Chest : (BattleObjectSettings)levelData.Pot;
			drop.SpawnIntoRoom(room, positions[i]);
		}

		// fixed stuff
		foreach(var stuff in StuffInRoom)
		{
			var obj = stuff.ObjectType.SpawnIntoRoom(room, stuff.Position);
			obj.WasPartOfTheRoom = true;
		}
	}
}

[Serializable]
public class MobWave
{
	
}

[Serializable]
public class ObjectInRoomData
{
	public BattleObjectSettings ObjectType;
	public float2 Position;
}

public static class ListUtil
{
	public static T GetRandom<T>(this IList<T> list)
	{
		if (list == null || list.Count == 0) return default(T);
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static List<float2> GetSpawnPoints(this List<float2> positionPool, int count)
	{
		var bag = new List<float2>();
		var finalList = new List<float2>();

		for(int i = 0; i < count; i++)
		{
			// bag prevents objects being spawned into same positions, until pool is exhausted
			if (bag.Count == 0) { bag.AddRange(positionPool); }

			var posIndex = UnityEngine.Random.Range(0, bag.Count);
			finalList.Add(bag[posIndex]);
			bag.RemoveAt(posIndex);			
		}

		return finalList;
	}

	public static bool SpawnIntoRoom<T>(this List<T> listToSpawn, Room room, List<float2> positionsPool, bool isInitialSpawn)
		where T : BattleObjectSettings
	{
		var positions = positionsPool.GetSpawnPoints(listToSpawn.Count);
        for (int i = 0; i < listToSpawn.Count; i++)
		{
            var stuff = listToSpawn[i].SpawnIntoRoom(room, positions[i]);
			stuff.WasPartOfTheRoom = isInitialSpawn; // this controls whether there will be a spawn animation
		}

		// returns true if anything was spawned
		return listToSpawn.Count > 0;
	}
}