using System;
using GameConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Game
{
	public static readonly GameState GameState = new GameState();
	public static GameTicker ActiveGame;
	public static Player Player => ActiveGame?.GameWorld.Player;
	public static readonly SettingsCache<PlayerSettings> PlayerSettings = new SettingsCache<PlayerSettings>("Player/PlayerSettings");


	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/Reboot _F12")]
#endif
	public static void Reboot()
	{
		GameState.Reset();
		SceneManager.LoadScene("Game");
	}

	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/Skip To End Level _F02")]
#endif
	public static void SkipToFinalRoom()
	{
		var allRooms = Game.Player.Room.World.Rooms;
		Game.Player.Room = allRooms[allRooms.Count - 1];
	}

	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/Kill Everything _F03")]
#endif
	public static void KillEverything()
	{
		foreach(var unit in Game.Player.Room.AllObjects)
		{
			if (unit.Owner == OwnerId.Enemy)
			{
				unit.Deactivate();
			}
		}
	}

	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/GoToNextLevel _F11")]
#endif
	public static void GoToNextLevel()
	{
		GameState.Level++;
		GameState.SkillPoints = Player.SkillPoints;
		GameState.BonusDamage = Player.DamageBonus;
		GameState.BonusHealth = Player.HealthBonus;
		GameState.BonusSpeed = Player.MovementSpeedBonus;
		SceneManager.LoadScene("Game");
	}

	// reset state on enter play mode
#if UNITY_EDITOR
	static Game()
	{
		UnityEditor.EditorApplication.playModeStateChanged += (arg) =>
		{
			GameState.Reset();
		};
	}
#endif
}

public class GameState
{
	public int Level = 1;
	public int SkillPoints;
	public float BonusDamage = -5;
	public float BonusHealth;
	public float BonusSpeed;

	public void Reset()
	{
		Level = 1;
		SkillPoints = 0;
		BonusDamage = -5;
		BonusHealth = 0;
		BonusSpeed = 0;
	}
}