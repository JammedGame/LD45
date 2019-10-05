using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LoreStone : BattleObject
{
    public readonly LoreStoneSettings LoreSettings;

    public LoreStone(Room room, LoreStoneSettings settings, float2 position) : base(room, settings, OwnerId.Neutral, position)
    {
        LoreSettings = settings;
    }
}
