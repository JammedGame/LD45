using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Serialized state of the object.
/// </summary>
public abstract class BattleObjectSettings : ScriptableObject
{
	[Header("Physics")]
	public ColliderShape ColliderType = ColliderShape.Circle;
	public float2 Size;
	public bool IsStatic;
	[HideIf("IsStatic")] public float Density = 1;

	public float Mass => Size.x*Size.y * Density;

	/// <summary>
	/// Spawns object into game logic.
	/// </summary>
	public abstract BattleObject SpawnIntoRoom(Room room, float2 position);

	/// <summary>
	/// Loads visuals for this object.
	/// </summary>
	public BattleObject3D LoadVisuals() => Resources.Load<BattleObject3D>(VisualsPath);

	/// <summary>
	/// Path for the corresponding visual.
	/// </summary>
	public abstract string VisualsPath { get; }

	/// <summary>
	/// Validate values.
	/// </summary>
	public virtual void OnValidate()
	{
		if (Density <= 0.01f) Density = 0.01f;
		if (Size.x <= 0.01f) Size.x = 0.01f;
		if (Size.y <= 0.01f) Size.y = 0.01f;
	}
}