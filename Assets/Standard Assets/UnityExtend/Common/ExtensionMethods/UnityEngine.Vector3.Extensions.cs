using UnityEngine;

public static class Vector3Extenions
{
	public static bool Approximately(this Vector3 value, Vector3 otherValue)
	{
		return (value.x - otherValue.x) < 0.0001f && (value.x - otherValue.x) > -0.0001f
			&& (value.y - otherValue.y) < 0.0001f && (value.y - otherValue.y) > -0.0001f
			&& (value.z - otherValue.z) < 0.0001f && (value.z - otherValue.z) > -0.0001f;
	}
}