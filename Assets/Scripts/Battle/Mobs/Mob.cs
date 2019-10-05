using Unity.Mathematics;
using static Unity.Mathematics.math;

public class Mob : Unit
{
	public readonly MobSettings MobSettings;
	public AnimationType CurrentAnimation;
	
	public float2 PatrolTarget;
	public float PatrolingPauseTimeLeft;

	public Mob(MobSettings mobSettings, Room room, float2 position) : base(room, mobSettings.Health, mobSettings, OwnerId.Enemy, position)
	{
		MobSettings = mobSettings;
		PatrolTarget = position;
	}

	protected override void OnAct(float dT)
	{
		base.OnAct(dT);

		Velocity *= 0.98f;

		var distanceToPlayer = DistanceToPlayer;
		if (distanceToPlayer > MobSettings.AggroRange)
		{
			PatrolAct(dT);
		}
		else
		{
			AggroAct(dT);
		}
	}

	public void PatrolAct(float dT)
	{
		var distToPatrolTarget = distance(Position, PatrolTarget);
		if (distToPatrolTarget <= 0.2f)
		{
			PatrolingPauseTimeLeft = MobSettings.PatrollingPause;
			PatrolTarget = Position + RandomDirection * MobSettings.PatrollingDistance;
		}

		if (PatrolingPauseTimeLeft > 0f)
		{
			CurrentAnimation = AnimationType.Idle;
			PatrolingPauseTimeLeft -= dT;
		}
		else
		{
			CurrentAnimation = AnimationType.Move;
			MoveTowards(PatrolTarget, dT);
		}
	}

	public override void OnCollisionWithWall()
	{
		base.OnCollisionWithWall();
		PatrolTarget = Position;
	}

	public override void OnCollisionWith(BattleObject obj)
	{
		base.OnCollisionWith(obj);
		if (obj is Mob || obj is Prop)
		{
			PatrolTarget = Position;
		}
	}

	public void AggroAct(float dT)
	{
		CurrentAnimation = AnimationType.Move;
		MoveTowards(Player.Position, dT);
	}

	public void MoveTowards(float2 targetPos, float dT)
	{
		var direction = normalize(targetPos - Position);
		Velocity += direction * dT * MobSettings.MovementSpeed;
	}
}

public enum MobState
{
	Idle = 0,
	Patrolling = 1,
	InAggro = 2
}

public enum AnimationType
{
	Idle = 0,
	Move = 1,
	Attack = 2
}