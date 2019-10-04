public static class FloatExtensions
{
	public static bool Approximately(this float value, float otherValue)
	{
		return (value - otherValue) < 0.0001f && (value - otherValue) > -0.0001f;
	}
}