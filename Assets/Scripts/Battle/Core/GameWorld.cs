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
        int x = 0, y = 0;
        Rooms = new List<Room>();
        foreach(var data in gameWorldData.Rooms)
        {
            Rooms.Add(new Room(this, x, y, data.RoomPreset));

            // move up or right
            var proc = UnityEngine.Random.Range(0, 100) > 50;
            if (proc)
                x++;
            else
                y++;
        }

        // set next room refs
        for(int i = 0; i < Rooms.Count - 1; i++)
        {
            Rooms[i].SetNextRoom(Rooms[i + 1]);
        }
        for(int i = 1; i < Rooms.Count; i++)
        {
            Rooms[i].SetRoomBefore(Rooms[i - 1]);
        }

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
