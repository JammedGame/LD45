using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class SkillPointSettings : BattleObjectSettings
{

	public float Health;
	public bool IsVulnerable;
	public BattleObjectSettings spawnOnDeath;

	public override string VisualsPath
		=> "SkillPoint/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
		=> new SkillPoint(room, this, OwnerId.Neutral, position);
}