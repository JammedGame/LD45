using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LoreStone : BattleObject
{
    public float Delay = 0;
    public readonly LoreStoneSettings LoreSettings;

    public bool isRead;

    public LoreStone(Room room, LoreStoneSettings settings, float2 position) : base(room, settings, OwnerId.Neutral, position)
    {
        LoreSettings = settings;
        isRead = settings.isRead;
    }

    public override void OnCollisionWith(BattleObject other)
    {
        if (other is Player player && Room.CanProgressToNextRoom() && !isRead)
        {

            isRead = true;
        }
    }

    protected override void OnAct(float dT)
    {
        base.OnAct(dT);

        Delay -= dT;
        if(Delay > 0) return;
        if (DistanceToPlayer < 1 && Input.GetKeyDown(KeyCode.E))
        {
            Game.Player.LoreToTell = LoreSettings.Lore;
            isRead = true;
            Delay = 0.8f;
        }
        else 
        {
            isRead = false;
        }
    }

}
