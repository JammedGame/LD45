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

	public void SpawnWave(Room room, int index)
	{
		var levelData = room.World.GameWorldData;

		switch(index)
		{
			case 0:
				foreach(var mob in FirstWave)
				{
					var newMob = mob.SpawnIntoRoom(room, levelData.MobSpawnPoints.GetRandom());
					newMob.WasPartOfTheRoom = true;
				}
				return;
			case 1:
				foreach(var mob in SecondWave)
					mob.SpawnIntoRoom(room, levelData.MobSpawnPoints.GetRandom());
				return;
			case 2:
				foreach(var mob in ThirdWave)
					mob.SpawnIntoRoom(room, levelData.MobSpawnPoints.GetRandom());
				return;
		}
	}

	public void SpawnStuffIntoRooms(Room room)
	{
		var levelData = room.World.GameWorldData;

		// mobs
		foreach(var mob in FirstWave)
		{
			mob.SpawnIntoRoom(room, levelData.MobSpawnPoints.GetRandom());
		}
		
		// pots
		for(int i = 0; i < PotCount; i++)
		{
			var drop = UnityEngine.Random.Range(0f, 1f) <= ChestSpawnProb ? (BattleObjectSettings)levelData.Chest : (BattleObjectSettings)levelData.Pot;
			drop.SpawnIntoRoom(room, levelData.PotSpawnPoints.GetRandom());
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
}