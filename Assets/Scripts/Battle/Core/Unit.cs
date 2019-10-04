using Unity.Mathematics;
/// <summary>
/// Base class for all characters in the game.
/// </summary>
public abstract class Unit : BattleObject
{
	public Unit(Room room, BattleObjectSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
	{
	}
}