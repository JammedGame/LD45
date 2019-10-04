using System;
using GameConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Game
{
	public static readonly SettingsCache<PlayerSettings> PlayerSettings = new SettingsCache<PlayerSettings>("Player/PlayerSettings");

	[ExecutableCommand]
#if UNITY_EDITOR
	[UnityEditor.MenuItem("Game/Reboot _F12")]
#endif
	public static void Reboot()
	{
		SceneManager.LoadScene("Game");
	}
}