using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChestVisual : BattleObject3D
{
    public override void Sync(float dT)
    {
        base.Sync(dT);
        Animator.SetBool("IsOpened", ((Chest)Data).IsOpened);
    }
}