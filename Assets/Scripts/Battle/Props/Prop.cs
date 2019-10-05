using Unity.Mathematics;

public class Prop : BattleObject
{
    public float Health;
	public PropSettings PropSettings;

    public Prop(Room room, PropSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
    {
        Health = settings.Health;
		PropSettings = settings;
    }

    public override void DealDamage(float damage, BattleObject source)
    {
        if (PropSettings.IsVulnerable)
        {
            Health -= damage;
        }
    }

	protected override void OnDeactivate() {
		base.OnDeactivate();
		if(PropSettings.spawnOnDeath!=null) {
			PropSettings.spawnOnDeath.SpawnIntoRoom(Room, Position);
		}
	}

    public override void CheckHealth()
    {
        if (Health <= 0)
        {
            Deactivate();
        }
    }
}