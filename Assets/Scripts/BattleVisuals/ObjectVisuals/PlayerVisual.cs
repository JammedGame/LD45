using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : BattleObject3D
{
    public Light2D Light2D;

    public override void Sync(float dT)
    {
        base.Sync(dT);

        var player = (Player)Data;
        GetComponent<Animator>().SetInteger("ActionType", (int)player.CurrentAnimation);
    }
}