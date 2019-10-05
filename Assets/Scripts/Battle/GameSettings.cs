using UnityEngine;

public class GameSettings : ScriptableObject<GameSettings>
{
	[Header("Room size")]
	public float RoomWidth;
	public float RoomHeight;
	public float RoomWidthPadding;
	public float RoomHeightPadding;
}