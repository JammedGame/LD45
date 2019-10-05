using Unity.Mathematics;

public class SkillPoint : BattleObject
{
    public float Health;
	public SkillPointSettings SkillPointSettings;

    public SkillPoint(Room room, SkillPointSettings settings, OwnerId owner, float2 position) : base(room, settings, owner, position)
    {
        Health = settings.Health;
		SkillPointSettings = settings;
    }

}