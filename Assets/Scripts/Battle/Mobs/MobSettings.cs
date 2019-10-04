using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class MobSettings : BattleObjectSettings
{
	[Header("Mob Stuff")]
	public float MovementSpeed;
	public override string VisualsPath => "Mobs/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
	{
		return new Mob(this, room, position);
	}
}