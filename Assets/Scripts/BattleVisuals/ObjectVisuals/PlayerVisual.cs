using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : BattleObject3D
{
    public override void Sync(float dT)
    {
        base.Sync(dT);
        
        var player = (Player)Data;
        GetComponent<Animator>().SetInteger("ActionType", (int)player.CurrentAnimation);
    }
}