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

    public GameWorld(GameWorldData levelData)
    {
        Debug.Log("Entering Level " + levelData.LevelId);
        // init basic stuff
        GameWorldData = levelData;
        RandomGenerator = new Unity.Mathematics.Random((uint)GetHashCode());
        ViewEventPipe = new ViewEventsPipe();

        // init rooms
        int x = 0, y = 0;
        Rooms = new List<Room>();

        // do the rooms
        var roomPresets = new List<RoomPreset>();
        roomPresets.Add(LoadRoomPreset("Pause")); // first level always fixed
        roomPresets.AddRange(LoadPresets(1, 5, randomize: levelData.LevelId > 1));
        roomPresets.Add(LoadRoomPreset("Pause"));
        roomPresets.AddRange(LoadPresets(6, 10));
        roomPresets.Add(LoadRoomPreset("Pause"));
        roomPresets.AddRange(LoadPresets(11, 15));
        roomPresets.Add(LoadRoomPreset("Boss"));

        foreach(var roomPreset in roomPresets)
        {
            Rooms.Add(new Room(this, x, y, roomPreset));

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

    public IEnumerable<RoomPreset> LoadPresets(int from, int to, bool randomize = true)
    {
        var pool = new List<int>();
        for(int i = from; i <= to; i++) { pool.Add(i); }

        while(pool.Count > 0)
        {
            var id = randomize 
                ? pool[UnityEngine.Random.Range(0, pool.Count)] 
                : pool[0];

            pool.Remove(id);
            yield return LoadRoomPreset(id);
        }
    }

    public RoomPreset LoadRoomPreset(object id) => 
        Resources.Load<RoomPreset>($"LevelDesign/Level{GameWorldData.LevelId}/Lvl{GameWorldData.LevelId}-{id}");
}
