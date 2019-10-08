using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public class Player : Unit
{
    public int GearState;
    public bool ReadyToGoToNextRoom;
    public float DamageBonus;
    public float HealthBonus;
    public float MovementSpeedBonus;
    public bool HasEyes;
    public readonly PlayerSettings PlayerSettings;
    public AnimationType CurrentAnimation;
    public float attackProgress;

    public int SkillPoints;

    public string LoreToTell;
    public string DialogToTell;
    public string MonologToTell;

    public Player(PlayerSettings settings, Room initialRoom, float2 initialPosition) : base(initialRoom, settings.Health, settings, OwnerId.Player, initialPosition)
    {
        this.GearState = Game.GameState.GearState;
        this.DamageBonus = Game.GameState.BonusDamage;
        this.HealthBonus = Game.GameState.BonusHealth;
        this.MovementSpeedBonus = Game.GameState.BonusSpeed;
        this.HasEyes = Game.GameState.HasEyes;
        PlayerSettings = settings;
        SkillPoints = Game.GameState.SkillPoints;
        this.MonologToTell = initialRoom.RoomData.MonologToTell;
    }
    public float TotalDamage => PlayerSettings.Damage + DamageBonus * 0.4f;
    public float TotalHealth => PlayerSettings.Health + HealthBonus * 3;
    public float TotalSpeed => PlayerSettings.MovementSpeed + MovementSpeedBonus * 15;
    public float SkillDamage => PlayerSettings.Damage * 5 + DamageBonus;
    public float SkillHealth => PlayerSettings.Health / 2 + HealthBonus;
    public float SkillSpeed => PlayerSettings.MovementSpeed / 15 + MovementSpeedBonus;
    float InertiaLol;
    protected override void OnAct(float dT)
    {
        // a bit less intertia than other stuff

        CurrentAnimation = AnimationType.Idle;

        if (TimeSinceInit < InertiaLol)
            Velocity *= PlayerSettings.Inertia;

        // move around of arrow keys
        var movement = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movement.x != 0 || movement.y != 0)
        {
            Velocity += normalize(movement) * TotalSpeed * dT;
            InertiaLol = TimeSinceInit + 0.15f;
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
        if (Input.GetKey(KeyCode.Mouse0)  && TotalDamage > 0)
        {
            CurrentAnimation = AnimationType.Attack;
            if (attackProgress <= 0f)
            {
                var mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Room.Position3D;
                var mousePos = float2(mousePos3D.x, mousePos3D.y);
                FireProjectile(mousePos, TotalDamage, PlayerSettings.Weapon1);
                attackProgress = PlayerSettings.FireRateDuration;
            }
        }
    }

    public override void OnCollisionWithWall()
    {
        if (Room.CanProgressToNextRoom())
        {
            if (distance(Room.DoorPosition, Position) < 1)
            {
                if (Room.NextRoom == null)
                {
                    ReadyToGoToNextRoom = true;
                }
                else
                {
                    Room = Room.NextRoom;
                    Velocity = 0;
                    Position = Room.EntryPosition * 0.9f;
                    if(!string.IsNullOrEmpty(Room.RoomData.MonologToTell)) {
                        MonologToTell = Room.RoomData.MonologToTell;
                    }
                }
            }
            else if (Room.PreviousRoom != null
                && distance(Room.EntryPosition, Position) < 1)
            {
                Room = Room.PreviousRoom;
                Velocity = 0;
                Position = Room.DoorPosition * 0.9f;
            }
        }
    }

    public override void DealDamage(float damage, BattleObject source)
    {
        InertiaLol = 0;
        base.DealDamage(damage, source);
    }
}