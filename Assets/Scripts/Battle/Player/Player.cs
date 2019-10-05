using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public class Player : Unit
{
    public readonly PlayerSettings PlayerSettings;
    public AnimationType CurrentAnimation;

    public float attackProgress;

    public int SkillPoints;

    public Player(PlayerSettings settings, Room initialRoom, float2 initialPosition) : base(initialRoom, settings.Health, settings, OwnerId.Player, initialPosition)
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
        attackProgress -= dT;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            CurrentAnimation = AnimationType.Attack;
            if (attackProgress <= 0f)
            {
                var mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Room.Position3D;
                var mousePos = float2(mousePos3D.x, mousePos3D.y);
                FireProjectile(mousePos, PlayerSettings.Damage, PlayerSettings.Weapon1);
                attackProgress = PlayerSettings.FireRateDuration;
            }
        }
    }

    public override void OnCollisionWithWall()
    {
        if (Room.CanProgressToNextRoom()
            && Room.NextRoom != null
            && distance(Room.DoorPosition, Position) < 1)
        {
            Room = Room.NextRoom;
            Position = Room.EntryPosition;
        }
    }
}