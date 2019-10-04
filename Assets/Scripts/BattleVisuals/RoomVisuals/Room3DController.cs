using System.Collections.Generic;

public class Room3DController
{
	public readonly List<Room3D> AllRooms3D = new List<Room3D>();

	public Room3DController(GameWorld world)
	{
		foreach(var room in world.Rooms)
		{
			Room3D.Init(room);
		}
	}
}