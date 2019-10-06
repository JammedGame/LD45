using UnityEngine;

public class Room3D : MonoBehaviour
{
	public GameObject EastDoor;
	public GameObject NorthDoor;
	public GameObject WestDoor;
	public GameObject SouthDoor;
	public GameObject Walls;
	public Sprite Frame1;
	public Sprite Frame2;
	public Sprite Frame3;
	public GameObject Floor;
	public Sprite Floor1;
	public Sprite Floor2;
	public Sprite Floor3;

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

		if(room.World.GameWorldData.LevelId == 1) instance.Walls.GetComponent<SpriteRenderer>().sprite = instance.Frame1;
		if(room.World.GameWorldData.LevelId == 2) instance.Walls.GetComponent<SpriteRenderer>().sprite = instance.Frame2;
		if(room.World.GameWorldData.LevelId == 3) instance.Walls.GetComponent<SpriteRenderer>().sprite = instance.Frame3;
		if(room.World.GameWorldData.LevelId == 1) instance.Floor.GetComponent<SpriteRenderer>().sprite = instance.Floor1;
		if(room.World.GameWorldData.LevelId == 2) instance.Floor.GetComponent<SpriteRenderer>().sprite = instance.Floor2;
		if(room.World.GameWorldData.LevelId == 3) instance.Floor.GetComponent<SpriteRenderer>().sprite = instance.Floor3;

		instance.NorthDoor.SetActive(room.NextRoom != null && room.NextRoom.Y == room.Y + 1 );
		instance.EastDoor.SetActive(room.NextRoom != null && room.NextRoom.X == room.X + 1);

		instance.WestDoor.SetActive(room.PreviousRoom != null && room.PreviousRoom.X == room.X - 1 );
		instance.SouthDoor.SetActive(room.PreviousRoom != null && room.PreviousRoom.Y == room.Y - 1);		

		return instance;
	}
}