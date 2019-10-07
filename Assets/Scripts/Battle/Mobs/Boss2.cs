using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;

public class Boss2 : Mob
{
    private float _SpawnProgress;
    public Boss2(MobSettings mobSettings, Room room, float2 position) : base(mobSettings, room, position)
	{
	}


    protected override void OnAct(float dT)
	{
		if (TimeSinceInit < MobSettings.SpawnDuration)
		{
			CurrentAnimation = WasPartOfTheRoom ? AnimationType.Idle : AnimationType.Spawn;
			return;
		}

		Velocity *= 0.98f;
		AttackProgress -= dT;
        _SpawnProgress -= dT;

        // odmrsti se
        if (_SpawnProgress < 4f)
        {
            CurrentAnimation = AnimationType.Idle;
        }

        if(_SpawnProgress > 0) return;

		var distanceToPlayer = DistanceToPlayer;
		if(UnityEngine.Random.Range(0f, 1f) < 0.8f)
        {
            SpawnMob();
        }
        else
        {
            SpawnBeam();
        }

        // mrsti se
        CurrentAnimation = AnimationType.Minion;
	}

	private void SpawnBeam()
	{
        var pos = Room.World.GameWorldData.MobSpawnPoints.GetRandom() * 2;
        var marusBeamSettings = new ProjectileSettings();
        marusBeamSettings.name = "MarusBeam";
        marusBeamSettings.Size = new float2(0.3f, 0.3f);
        marusBeamSettings.IsStatic = true;
        new MarusBeam(Room, marusBeamSettings, this, pos);
	}

	private void SpawnMob()
    {
        _SpawnProgress = 5;
        CurrentAnimation = AnimationType.Minion;
        MobSettings.Minion.SpawnIntoRoom(Room, new float2(Position.x, Position.y - 2f));
    }
}


public class MarusBeam : BattleObject
{
    public readonly Boss2 Marus;

	public MarusBeam(Room room, BattleObjectSettings settings, Boss2 parent, float2 position) : base(room, settings, parent, position)
	{
        Marus = parent;
	}

    public override void OnCollisionWith(BattleObject other)
    {
        if (other is Player player)
        {
            player.DealDamage(Marus.MobSettings.AttackDamage, Marus);
            player.PushFrom(Position, Marus.MobSettings.AttackPush);
        }
    }
}