using Unity.Mathematics;

public class Prop : BattleObject
{
    public float Health;
    public bool Vulnerable;

    public Prop(Room room, PropSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
    {
        Health = settings.Health;
        Vulnerable = settings.IsVulnerable;
    }

    public override void DealDamage(float damage, BattleObject source)
    {
        if (Vulnerable)
        {
            Health -= damage;
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