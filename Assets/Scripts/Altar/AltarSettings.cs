using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class AltarSettings : BattleObjectSettings
{
    public string Item;

    public override string VisualsPath => "Altar/Altar";

    public override BattleObject SpawnIntoRoom(Room room, float2 position)
    {
        return new Altar(room, this, position);
    }
}
