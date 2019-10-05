using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class MobSettings : BattleObjectSettings
{
	[Header("Mob Stuff")]
	public float MovementSpeed;
	public float AggroRange;

	[Header("Health")]
	public float Health;

	[Header("Patrolling Stuff")]
	public float PatrollingDistance;
	public float PatrollingPause;

	public override string VisualsPath => "Mobs/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
	{
		return new Mob(this, room, position);
	}
}