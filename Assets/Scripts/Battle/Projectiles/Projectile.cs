using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public class Projectile : BattleObject
{
    public readonly ProjectileSettings ProjectileSettings;
    public float Damage;
    public float2 Direction;

	public Projectile(Room room, float damage, ProjectileSettings projectileSettings, BattleObject shooter, float2 position) : base(room, projectileSettings, shooter, position)
	{
        ProjectileSettings = projectileSettings;
        Direction = new float2(1, 0);
        Damage = damage;
	}

    public void DirectAt(float2 targetPosition)
    {
        Direction = normalizesafe(targetPosition - Position);
    }

    protected override void OnAct(float dT)
    {
        base.OnAct(dT);
        Position += Direction * dT * ProjectileSettings.MovementSpeed;
    }

    public override void OnCollisionWith(BattleObject hitObject)
    {
        base.OnCollisionWith(hitObject);

        if (hitObject.Owner != Owner)
        {
            hitObject.Push(Direction, ProjectileSettings.PushOnImpact);
            hitObject.DealDamage(Damage, Parent);
            Deactivate();
        }
    }

    public override void OnCollisionWithWall()
    {
        base.OnCollisionWithWall();
        Deactivate();
    }
}
