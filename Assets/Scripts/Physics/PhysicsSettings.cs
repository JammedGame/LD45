using UnityEngine;

public class PhysicsSettings : ScriptableObject<PhysicsSettings>
{
	[Header("Physics Job")]
	public int Passes;
	public float VelocityDragConstant;
	public float VelocityDragRelative;
	public float BounceAbsorb;
}