using UnityEngine;

public static class AnimationCurveExtensions
{
	public static float StartTime(this AnimationCurve curve)
	{
		if (curve == null || curve.length == 0) return 0f;

		return curve[0].time;
	}

	public static float EndTime(this AnimationCurve curve)
	{
		if (curve == null || curve.length == 0) return 0f;

		return curve[curve.length - 1].time;
	}

	public static float Duration(this AnimationCurve curve)
	{
		return EndTime(curve) - StartTime(curve);
	}

	public static float EvaluatePercent(this AnimationCurve curve, float percent)
	{
		return curve.Evaluate(percent * curve.Duration());
	}
}