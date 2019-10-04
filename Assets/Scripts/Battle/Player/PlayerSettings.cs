using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : BattleObjectSettings
{
	[Header("Player Stuff")]
	public float MovementSpeed;

	[Header("Push Away")]
	public float PushAwayRadius;
	public float PushAwayForce;

	[Header("Projectiles")]
	public ProjectileSettings Weapon1;
	public override string VisualsPath => "Player/PlayerVisuals";

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
	{
		return new Player(this, room, position);
	}
}