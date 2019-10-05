using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class ChestSettings : BattleObjectSettings
{
	public List<DropTable> PossibleDrops;

	public override string VisualsPath
		=> "Props/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
		=> new Chest(room, this, position);
}

[Serializable]
public class DropTable
{
	public int Weight = 1;
	[Table] public List<DropTableRow> ItemPool;
}

[Serializable]
public class DropTableRow
{
	public BattleObjectSettings Type;
	public float Proc = 1;
}

public class Chest : BattleObject
{
	public ChestSettings ChestSettings;
	public bool IsOpened;

    public Chest(Room room, ChestSettings settings, float2 position) : base(room, settings, OwnerId.Neutral, position)
    {
		ChestSettings = settings;
    }

	public override void OnCollisionWith(BattleObject other)
	{
		if (!IsOpened && other is Player player && player.Room.CanProgressToNextRoom())
		{
			IsOpened = true;

			if (ChestSettings.PossibleDrops.Count == 1)
			{
				SpawnDrops(ChestSettings.PossibleDrops[0].ItemPool);
			}
			else if (ChestSettings.PossibleDrops.Count > 1)
			{
				var bag = new List<DropTable>();
				foreach(var drop in ChestSettings.PossibleDrops)
					for(int i = 0; i < drop.Weight; i++)
						bag.Add(drop);

				var randomlySelected = UnityEngine.Random.Range(0, bag.Count);
				SpawnDrops(bag[randomlySelected].ItemPool);
			}

		}
	}

	public void SpawnDrops(List<DropTableRow> ItemPool)
	{
		foreach(var stuff in ItemPool)
		{
			var proc = Room.World.RandomGenerator.NextFloat();
			if (proc <= stuff.Proc)
			{
				stuff.Type.SpawnIntoRoom(Room, Position + Size * RandomDirection);
			}
		}
	}
}