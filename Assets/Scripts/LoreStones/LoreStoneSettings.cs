using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class LoreStoneSettings : BattleObjectSettings
{
    [TextArea] public string Lore;

    public override string VisualsPath => "LoreStones/LoreStone";

    public override BattleObject SpawnIntoRoom(Room room, float2 position)
    {
        return new LoreStone(room, this, position);
    }
}
