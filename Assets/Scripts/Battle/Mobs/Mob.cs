using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Mob : Unit
{
	public readonly MobSettings MobSettings;

	public Mob(MobSettings mobSettings, Room room, float2 position) : base(room, mobSettings, OwnerId.Enemy, position)
	{
		MobSettings = mobSettings;
	}

	protected override void OnAct(float dT)
	{
		base.OnAct(dT);

		Velocity *= 0.98f;

		AggroAct(dT);
	}

	public void AggroAct(float dT)
	{
		var direction = normalize(Player.Position - Position);
		Velocity += direction * dT * MobSettings.MovementSpeed;
	}
}