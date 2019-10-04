using Unity.Mathematics;

public class Prop : BattleObject
{
	public Prop(Room room, BattleObjectSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
	{
	}
}