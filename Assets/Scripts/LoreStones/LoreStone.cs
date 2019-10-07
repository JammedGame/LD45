using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LoreStone : BattleObject
{
    public readonly LoreStoneSettings LoreSettings;

    public bool isRead;

    public LoreStone(Room room, LoreStoneSettings settings, float2 position) : base(room, settings, OwnerId.Neutral, position)
    {
        LoreSettings = settings;
        isRead = settings.isRead;
    }

    public override void OnCollisionWith(BattleObject other)
    {
        if (other is Player player && Room.CanProgressToNextRoom())
        {
            player.LoreToTell = LoreSettings.Lore;
            isRead = true;
        }
    }
}
