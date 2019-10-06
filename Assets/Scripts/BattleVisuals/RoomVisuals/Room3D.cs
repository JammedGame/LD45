using UnityEngine;

public class Room3D : MonoBehaviour
{
	public GameObject EastDoor;
	public GameObject NorthDoor;
	public GameObject WestDoor;
	public GameObject SouthDoor;

	public Room room { get; private set; }

	/// <summary>
	/// Creates a new room visual
	/// </summary>
	public static Room3D Init(Room room)
	{
		var prefab = Resources.Load<Room3D>($"Rooms/{room.World.GameWorldData.RoomName}");
		var instance = Instantiate(prefab);
		instance.transform.position = room.Position3D + Vector3.forward * Camera.main.farClipPlane;
		instance.room = room;

		instance.NorthDoor.SetActive(room.NextRoom != null && room.NextRoom.Y == room.Y + 1 );
		instance.EastDoor.SetActive(room.NextRoom != null && room.NextRoom.X == room.X + 1);

		instance.WestDoor.SetActive(room.PreviousRoom != null && room.PreviousRoom.X == room.X - 1 );
		instance.SouthDoor.SetActive(room.PreviousRoom != null && room.PreviousRoom.Y == room.Y - 1);

		return instance;
	}
}