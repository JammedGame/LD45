using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
	public GameWorld GameWorld;
	public Room3DController RoomController;
	public BattleViewController ViewController;
	public Camera Camera;
	public float DefaultOrtographicSize;

	/// <summary>
	/// Creates a new game world and start ticking.
	/// </summary>
	public static GameTicker StartTicking(GameWorldData levelData)
	{
		var gameWorld = new GameWorld(levelData);
		var newTicker = new GameObject("GameWorldTicker").AddComponent<GameTicker>();
		newTicker.Camera = Camera.main;
		newTicker.DefaultOrtographicSize = newTicker.Camera.orthographicSize;
		newTicker.GameWorld = gameWorld;
		newTicker.ViewController = new BattleViewController();
		newTicker.RoomController = new Room3DController(gameWorld);
		Game.ActiveGame = newTicker;
		return newTicker;
	}

	void Update()
	{
		if (GameWorld == null)
		{
			Game.Reboot();
			return;
		}

		UpdateCameraZoom();

		var dT = Time.deltaTime;
		var logicPaused = false;

		if (MoveCameraAround(dT))
		{
			// don't update stuff while moving camera
			logicPaused = true;
		}

		if (GameWorld.Player.ReadyToGoToNextRoom)
		{
			logicPaused = true;
			Game.GoToNextLevel();
		}


		if(GameWorld.Player.LoreToTell!=null) {
			logicPaused=true;
		}

		if (!logicPaused) GameWorld.Tick(dT);
		ViewController.HandleViewEvents(GameWorld.ViewEventPipe);
		ViewController.SyncEverything(dT);
	}

	private void UpdateCameraZoom()
	{
		var aspectRatio = Screen.width / (float)Screen.height;
		var referenceAspectRatio = 16f / 9f;

		if (aspectRatio > referenceAspectRatio)
		{
			Camera.orthographicSize = DefaultOrtographicSize;
		}
		else
		{
			Camera.orthographicSize = DefaultOrtographicSize * referenceAspectRatio / aspectRatio;
		}
	}

	private bool MoveCameraAround(float dT)
    {
        var cameraPos = Camera.transform.position;
		var roomPos = GameWorld.Player.Room.Position3D + Vector3.up * 0.3f;
		var targetPos = new Vector3(roomPos.x, roomPos.y, cameraPos.z);

		var dist = Vector3.Distance(targetPos, cameraPos);
		if (dist <= 0.01f) return false;

		Camera.transform.position = Vector3.MoveTowards(cameraPos, targetPos, 40 * dT);
		return true;
    }
}
