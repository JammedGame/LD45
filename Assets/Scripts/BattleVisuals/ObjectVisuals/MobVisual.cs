using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MobVisual : BattleObject3D
{
    public override void Sync(float dT)
    {
        base.Sync(dT);

        var mob = (Mob)Data;
        GetComponent<Animator>().SetInteger("ActionType", (int)mob.CurrentAction);
    }
}