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

    protected override void OnAct(float dT)
    {
        base.OnAct(dT);

        Delay -= dT;
        if(Delay > 0) return;
        if (DistanceToPlayer < 1)
        {
            Game.Player.LoreToTell = LoreSettings.Lore;
            Delay = 0.3f;
        }
        else
        {
            Game.Player.LoreToTell = null;
            Delay = 0.3f;
        }
    }

}
