using System;
using GameConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Game
{
	public static GameTicker ActiveGame;
	public static Player Player => ActiveGame?.GameWorld.Player;
	public static readonly SettingsCache<PlayerSettings> PlayerSettings = new SettingsCache<PlayerSettings>("Player/PlayerSettings");
	public static readonly GameState GameState = new GameState();

	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/Reboot _F12")]
#endif
	public static void Reboot()
	{
		GameState.Level = 1;
		SceneManager.LoadScene("Game");
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