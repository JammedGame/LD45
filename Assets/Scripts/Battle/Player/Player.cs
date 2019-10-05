using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public class Player : Unit
{
	public readonly PlayerSettings PlayerSettings;
	public AnimationType CurrentAnimation;

	public Player(PlayerSettings settings, Room initialRoom, float2 initialPosition) : base(initialRoom, settings, OwnerId.Player, initialPosition)
	{
		PlayerSettings = settings;
	}

	protected override void OnAct(float dT)
	{
		// a bit less intertia than other stuff
		Velocity *= 0.98f;

		CurrentAnimation = AnimationType.Idle;

		// move around of arrow keys
		var movement = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (movement.x != 0 || movement.y != 0)
		{
			Velocity += normalize(movement) * PlayerSettings.MovementSpeed * dT;
			CurrentAnimation = AnimationType.Move;
		}

		// dummy kick
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (ClosestUnitInRadius(PlayerSettings.PushAwayRadius) is Unit target)
			{
				target.PushFrom(Position, PlayerSettings.PushAwayForce);
			}
		}

		// dummy bullet
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			var mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var mousePos = new float2(mousePos3D.x, mousePos3D.y);
			FireProjectile(mousePos, PlayerSettings.Weapon1);
			CurrentAnimation = AnimationType.Attack;
		}
	}
}