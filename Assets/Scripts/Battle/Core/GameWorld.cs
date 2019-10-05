using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameWorld
{
    public readonly GameWorldData GameWorldData;
    public readonly List<Room> Rooms;
    public readonly Player Player;
	public readonly ViewEventsPipe ViewEventPipe;
    public Unity.Mathematics.Random RandomGenerator;

    public GameWorld(GameWorldData gameWorldData)
    {
        // init basic stuff
        GameWorldData = gameWorldData;
        RandomGenerator = new Unity.Mathematics.Random((uint)GetHashCode());
        ViewEventPipe = new ViewEventsPipe();

        // init rooms
        Rooms = gameWorldData.Rooms.ConvertAll(roomData => new Room(this, roomData));

        // add player
        Player = new Player
        (
            initialRoom: Rooms[0],
            settings: Game.PlayerSettings,
            initialPosition: float2.zero
        );
    }

	public void Tick(float dT)
	{
        // tick only the room that player is on.
        Player.Room.Tick(dT);
	}

    public float2 RandomDirection => RandomGenerator.NextFloat2Direction();
}
