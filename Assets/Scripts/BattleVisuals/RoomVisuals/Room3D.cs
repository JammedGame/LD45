using UnityEngine;

public class Room3D : MonoBehaviour
{
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
		return instance;
	}
}