using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class PropSettings : BattleObjectSettings
{

	public float Health;
	public bool IsVulnerable;

	public override string VisualsPath
		=> "Props/Visuals/" + name;

	public override BattleObject SpawnIntoRoom(Room room, float2 position)
		=> new Prop(room, this, OwnerId.Neutral, position);
}