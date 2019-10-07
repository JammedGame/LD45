using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Altar : BattleObject
{
    public bool Complete = false;
    public readonly AltarSettings AltarSettings;

    public Altar(Room room, AltarSettings settings, float2 position) : base(room, settings, OwnerId.Neutral, position)
    {
        Debug.Log(settings.Item);
        AltarSettings = settings;
    }

    public override void OnCollisionWith(BattleObject other)
	{
		if (!Complete && other is Player player)
		{
			Complete = true;
            if(AltarSettings.Item == "Staff") player.DamageBonus += 5;
            if(AltarSettings.Item == "Lungs") player.MovementSpeedBonus += 3;
            if(AltarSettings.Item == "Liver") player.DamageBonus += 3;
		}
	}
}
