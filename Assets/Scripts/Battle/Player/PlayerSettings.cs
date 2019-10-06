using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : BattleObjectSettings
{
	[Header("Player Stuff")]
	public float MovementSpeed;
	public float Inertia;

	[Header("Push Away")]
	public float PushAwayRadius;
	public float PushAwayForce;

	[Header("Health")]
	public float Health;

	[Header("Projectiles")]
	public float FireRateDuration;
	public float Damage;
	public ProjectileSettings Weapon1;
	public override string VisualsPath => "Player/PlayerVisuals";

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
	{
		return new Player(this, room, position);
	}
}