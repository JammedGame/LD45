using Unity.Mathematics;

public class Mob : Unit
{
	public readonly MobSettings MobSettings;

	public Mob(MobSettings mobSettings, Room room, float2 position) : base(room, mobSettings, OwnerId.Enemy, position)
	{
		MobSettings = mobSettings;
	}
}