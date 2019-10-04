using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
	public GameWorld GameWorld;
	public Room3DController RoomController;
	public BattleViewController ViewController;

	/// <summary>
	/// Creates a new game world and start ticking.
	/// </summary>
	public static GameTicker StartTicking(GameWorldData levelData)
	{
		var gameWorld = new GameWorld(levelData);
		var newTicker = new GameObject("GameWorldTicker").AddComponent<GameTicker>();
		newTicker.GameWorld = gameWorld;
		newTicker.ViewController = new BattleViewController();
		newTicker.RoomController = new Room3DController(gameWorld);
		return newTicker;
	}

	void Update()
	{
		if (GameWorld == null)
		{
			Game.Reboot();
			return;
		}

		var dT = Time.deltaTime;

		GameWorld.Tick(dT);
		ViewController.HandleViewEvents(GameWorld.ViewEventPipe);
		ViewController.SyncEverything(dT);
	}
}
