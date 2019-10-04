using UnityEngine;

public static class Vector2Extenions
{
	public static bool Approximately(this Vector2 value, Vector2 otherValue)
	{
		return (value.x - otherValue.x) < 0.0001f && (value.x - otherValue.x) > -0.0001f
			&& (value.y - otherValue.y) < 0.0001f && (value.y - otherValue.y) > -0.0001f;
	}
}