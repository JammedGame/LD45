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
	[UnityEditor.MenuItem("Game/GoToNextLevel _F11")]
#endif
	public static void GoToNextLevel()
	{
		GameState.Level++;
		GameState.SkillPoints = Player.SkillPoints;
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

	public void Reset()
	{
		Level = 1;
		SkillPoints = 0;
	}
}