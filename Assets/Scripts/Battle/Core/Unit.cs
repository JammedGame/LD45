using Unity.Mathematics;
/// <summary>
/// Base class for all characters in the game.
/// </summary>
public abstract class Unit : BattleObject
{
	public float Health;

	public Unit(Room room, float health, BattleObjectSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
	{
		Health = health;
	}

	public override void DealDamage(float damage, BattleObject source)
	{
		Health -= damage;
	}

	public override void CheckHealth()
	{
		if (Health <= 0)
		{
			Deactivate();
		}
	}
}