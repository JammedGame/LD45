using UnityEngine;

public class Room3D : MonoBehaviour
{
	public GameObject RightDoor;
	public GameObject TopDoor;

	public Room room { get; private set; }

	/// <summary>
	/// Creates a new room visual
	/// </summary>
	public static Room3D Init(Room room)
	{
		var prefab = Resources.Load<Room3D>($"Rooms/{room.RoomData.RoomPreset.Name}");
		var instance = Instantiate(prefab);
		instance.transform.position = room.Position3D + Vector3.forward * Camera.main.farClipPlane;
		instance.room = room;

		if (instance.TopDoor)
			instance.TopDoor.SetActive(room.NextRoom != null && room.NextRoom.RoomData.Y == room.RoomData.Y + 1);
		else
			Debug.LogError("STAVI DOOR TEBRA");

		if (instance.RightDoor)
			instance.RightDoor.SetActive(room.NextRoom != null && room.NextRoom.RoomData.X == room.RoomData.X + 1);			
		else
			Debug.LogError("STAVI DOOR TEBRA");

		return instance;
	}
}