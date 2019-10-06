using Unity.Mathematics;

public class SkillPoint : BattleObject
{
    public SkillPointSettings SkillPointSettings;

    public SkillPoint(Room room, SkillPointSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
    {
        SkillPointSettings = settings;
    }

    public override void OnCollisionWith(BattleObject other)
    {
        if(other is Player player) {
            player.SkillPoints++;
            if(player.SkillPoints==10) {
                player.ShowAvailableUpgrades();
            }
            this.Deactivate();
        }

    }

}