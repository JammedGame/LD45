using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;

public class Boss3 : Mob
{
    private Room _Room;
    private int _State;
    private float _CooldownTimer;
    private float _PrepareTimer;
    private float _FireTimer;
    private float _CurrentFireTimer;
    private float _FinishTimer;
    private List<Mob> _Minions;
    public Boss3(MobSettings mobSettings, Room room, float2 position) : base(mobSettings, room, position)
	{
        this._Room = room;
        this.CurrentAnimation = AnimationType.Idle;
        this._State = 0;
        this._CurrentFireTimer = 0;
        this._CooldownTimer = 2;
        this._Minions = new List<Mob>();
	}
    protected override void OnAct(float dT)
	{
		if(_State == 0)
        {
            if(_CooldownTimer < 0)
            {
                _State = 1;
                _PrepareTimer = 0.5f;
                CurrentAnimation = AnimationType.AttackPrepare;
                return;
            }
            _CooldownTimer -= dT;
            Spawn();
        }
        else if(_State == 1)
        {
            if(_PrepareTimer < 0)
            {
                _State = 2;
                _FireTimer = 2f;
                CurrentAnimation = AnimationType.Attack;
                return;
            }
            _PrepareTimer -= dT;
        }
        else if(_State == 2)
        {
            if(_FireTimer < 0)
            {
                _State = 3;
                _FinishTimer = 0.5f;
                CurrentAnimation = AnimationType.AttackFinishing;
                return;
            }
            _FireTimer -= dT;
            if(_CurrentFireTimer <= 0)
            {
                FireProjectile(Player.Position, MobSettings.AttackDamage, MobSettings.Projectile);
                _CurrentFireTimer = 0.1f;
            }
            else _CurrentFireTimer -= dT;
        }
        else if(_State == 3)
        {
            if(_FinishTimer < 0)
            {
                _State = 0;
                _CooldownTimer = 2f;
                CurrentAnimation = AnimationType.Idle;
                return;
            }
            _FinishTimer -= dT;
        }
	}
    public override Projectile FireProjectile(float2 position, float damage, ProjectileSettings projectileType)
	{
		var projectile1 = new Projectile(Room, damage, projectileType, this, new Vector2(Position.x - 1.5f, Position.y));
		projectile1.DirectAt(new Vector2(position.x - 1.5f, position.y));
        var projectile2 = new Projectile(Room, damage, projectileType, this, new Vector2(Position.x + 1.5f, Position.y));
		projectile2.DirectAt(new Vector2(position.x + 1.5f, position.y));
		return projectile1;
	}
    private void Spawn()
    {
        bool Exist = false;
        foreach(Mob Minion in _Minions)
        {
            Exist = Exist || Minion.IsActive;
        }
        if(!Exist)
        {
            _Minions = new List<Mob>();
            foreach(var pos in Room.World.GameWorldData.MobSpawnPoints)
            {
                _Minions.Add((Mob)MobSettings.Minion.SpawnIntoRoom(this._Room, pos));
            }
        }
    }
}