using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;

public class Boss2 : Mob
{
    private float _SpawnProgress;
    private float _BeamProgress = 0;
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
        _BeamProgress -= dT;

        // odmrsti se
        if (_SpawnProgress < 4f)
        {
            CurrentAnimation = AnimationType.Idle;
        }

        if(_SpawnProgress > 0) return;

		var distanceToPlayer = DistanceToPlayer;
		if(UnityEngine.Random.Range(0f, 1f) < 0.4f || _BeamProgress > 0)
        {
            SpawnMob();
        }
        else if(_BeamProgress <= 0)
        {
            SpawnBeam();
        }

        // mrsti se
        CurrentAnimation = AnimationType.Minion;
	}

	private void SpawnBeam()
	{
        _BeamProgress = 3;
        var marusBeamSettings = new ProjectileSettings();
        marusBeamSettings.name = "MarusBeam";
        marusBeamSettings.Size = new float2(0.5f, 0.5f);
        foreach(var pos in Room.World.GameWorldData.MobSpawnPoints)
        {
            new MarusBeam(Room, marusBeamSettings, this, new Vector2(pos.x * 1.4f, pos.y * 1.4f));
        }
	}

	private void SpawnMob()
    {
        _SpawnProgress = 1.5f;
        CurrentAnimation = AnimationType.Minion;
        MobSettings.Minion.SpawnIntoRoom(Room, new float2(Position.x, Position.y - 2f));
    }
}


public class MarusBeam : BattleObject
{
    private float _Length;
    public readonly Boss2 Marus;

	public MarusBeam(Room room, BattleObjectSettings settings, Boss2 parent, float2 position) : base(room, settings, parent, position)
	{
        _Length = 3;
        Marus = parent;
	}

    protected override void OnAct(float dT)
    {
        base.OnAct(dT);

        _Length -= dT;
        if(_Length < 0)
        {
            Deactivate();
            return;
        }

        Position.x *= 1.005f;
        Position.y *= 1.005f;
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