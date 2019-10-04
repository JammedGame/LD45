using UnityEngine;

public class Room3D : MonoBehaviour
{
	public Room room { get; private set; }

	/// <summary>
	/// Creates a new room visual
	/// </summary>
	public static Room3D Init(Room room)
	{
		var prefab = Resources.Load<Room3D>($"Rooms/{room.RoomData.Name}");
		var instance = Instantiate(prefab);
		instance.transform.position = room.Position3D + Vector3.forward * Camera.main.farClipPlane;
		instance.transform.localScale = new Vector3(room.Width, room.Height, 1);
		instance.room = room;
		return instance;
	}
}