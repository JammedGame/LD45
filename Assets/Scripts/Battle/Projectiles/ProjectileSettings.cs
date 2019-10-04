using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class ProjectileSettings : BattleObjectSettings
{
	[Header("Projectile")]
	public float MovementSpeed;
	public float PushOnImpact;
	public override string VisualsPath => "Projectiles/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
	{
		return new Projectile(room, this, null, position);
	}
}
