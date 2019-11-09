using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : BattleObject3D
{
    public override void Sync(float dT)
    {
        base.Sync(dT);

        var player = (Player)Data;

        Animator.SetInteger("GearState", player.GearState);
        Animator.SetInteger("ActionType", (int)player.CurrentAnimation);
    }

    public override void Init(BattleObject data)
    {
        var player = (Player)Data;
    }
}