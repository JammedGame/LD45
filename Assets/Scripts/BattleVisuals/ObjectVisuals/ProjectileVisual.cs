using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProjectileVisual : BattleObject3D
{
    public override void Sync(float dT)
    {
        base.Sync(dT);
        
        var proj = (Projectile)Data;

        if (proj.Direction.x != 0 || proj.Direction.y != 0)
            transform.up = new Vector3(proj.Direction.x, proj.Direction.y, 0f);
    }
}