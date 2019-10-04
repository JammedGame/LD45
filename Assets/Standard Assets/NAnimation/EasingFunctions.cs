using System;
using UnityEngine;
// ReSharper disable ConvertToLambdaExpression

/// <summary>
/// This class contains basic animation's time functions which are used to measure animation's progress in some moment in time.
/// </summary>
public static class EasingFunctions
{
	#region Constants

	const float Pi2 = Mathf.PI * 2f;

	const float PiHalf = Mathf.PI * 0.5f;

	#endregion

	#region Linear Easing Function

	/// <summary>
	/// Linear time function delegate.
	/// </summary>
	public static readonly Func<float, float> Linear = progress => progress;

	/// <summary>
	/// Linear time function delegate.
	/// </summary>
	public static readonly Func<float, float> LinearInOut = progress =>
	{
		return progress < 0.5f ? progress : 1 - progress;
	};

	#endregion

	#region Sine Easing Functions

	/// <summary>
	/// EaseIn time function delegate. Simulates ease in effect.
	/// </summary>
	public static readonly Func<float, float> EaseInSine = progress =>
	{
		return 1f - Mathf.Sin(PiHalf * (1f - progress));
	};

	/// <summary>
	/// EaseOut time function delegate. Simulates ease out effect.
	/// </summary>
	public static readonly Func<float, float> EaseOutSine = progress =>
	{
		return Mathf.Sin(PiHalf * progress);
	};

	/// <summary>
	/// EaseInOut time function delegate. Simulates ease in and out effect.
	/// </summary>
	public static readonly Func<float, float> EaseInOutSine = progress =>
	{
		return progress - Mathf.Sin(progress * Pi2) / Pi2;
	};

	#endregion

	#region Power(N) Easing Functions

	/// <summary>
	/// Quadratic EaseOut time function delegate. Simulates ease out effect.
	/// </summary>
	public static readonly Func<float, float> EaseOutQuadratic = progress =>
	{
		return 1f - (progress - 1f) * (progress - 1f);
	};

	public static readonly Func<float, float> BallisticCurve = t =>
	{
		// 1-(2s-1)^2
		float part = 2f * t - 1;
		return 1f - part * part;
	};

	/// <summary>
	/// Cubic EaseOut time function delegate. Simulates ease out effect.
	/// </summary>
	public static readonly Func<float, float> EaseOutCubic = progress =>
	{
		return 1f + (progress - 1f) * (progress - 1f) * (progress - 1f);
	};

	/// <summary>
	/// Quartic EaseOut time function delegate. Simulates ease out effect.
	/// </summary>
	public static readonly Func<float, float> EaseOutQuartic = progress =>
	{
		return 1f - (progress - 1f) * (progress - 1f) * (progress - 1f) * (progress - 1f);
	};

	public static readonly Func<float, float> EaseInQuartic = progress =>
	{
		return Mathf.Pow(-progress, 4f);
	};

	/// <summary>
	/// Implementation taken from http://api.jqueryui.com/easings/ implementation (v1.11.2).
	/// Copy the following formula [-(2 ^ (8*(x - 1))) * sin(((x-1)*30 - 7.5) * 3.14 / 15)] at http://rechneronline.de/function-graphs/ 
	/// to see how does the function graph looks like with some sample values look like.
	/// </summary>
	public static float EaseInElasticImpl(float progress, float frequency, float phase)
	{
		if (Mathf.Approximately(progress, 0f) || Mathf.Approximately(progress, 1f)) { return progress; }
		return -Mathf.Pow(2, 8 * (progress - 1)) * Mathf.Sin(((progress - 1) * frequency - phase) * Mathf.PI / 15);
	}

	/// <summary>
	/// Invokes <see cref="EaseInElastic"/> with some sample values.
	/// </summary>
	public static readonly Func<float, float> EaseInElastic = progress =>
	{
		return EaseInElasticImpl(progress, 30f, 7.5f);
	};

	/// <summary>
	/// Elastic EaseOut. See also <see cref="EaseInElastic"/>
	/// </summary>
	public static readonly Func<float, float> EaseOutElastic = progress =>
	{
		return 1 - EaseInElastic(1f - progress);
	};

	#endregion

	#region Bounce Easing Functions

	/// <summary>
	/// BounceIn time function delegate. Simulates bounce in effect.
	/// </summary>
	public static readonly Func<float, float> BounceInFunction = progress =>
	{
		return BounceLogic(progress);
	};

	/// <summary>
	/// BounceOut time function delegate. Simulates bounce out effect.
	/// </summary>
	public static readonly Func<float, float> BounceOutFunction = progress =>
	{
		return 1f - BounceLogic(1f - progress);
	};

	/// <summary>
	/// Left-right shake function (useful for telling user he did something wrong)
	/// </summary>
	public static readonly Func<float, float> NoNoFunction = t =>
	{
		t *= 2.5f;

		if (t > 2f) { t -= 2f; }
		if (t > 1f) { t = 2 - t; }

		return 2 * t;
	};

	/// <summary>
	/// Left-right shake rotate function 
	/// </summary>
	public static readonly Func<float, float> RotateNoNoFunction = t =>
	{
		var newT = t;

		if (newT < 1)
		{
			newT = t;
		}
		else if (newT < 3f) { newT = 2f - newT; }
		else { newT = newT - 4f; }

		return newT;
	};

	#endregion

	#region Back easing functions

	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	public static readonly Func<float, float> EaseOutBack = t =>
	{
		return (t - 1) * (t - 1) * (2.70158f * t - 1) + 1;
	};

	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	public static readonly Func<float, float> EaseInBack = progress =>
	{
		return progress * progress * (2.70158f * progress - 1.70158f);
	};

	/// <summary>
	/// Start quickly, slow down towards the middle, then accelerate to the end.
	/// </summary>
	public static readonly Func<float, float> EaseOutIn = progress =>
	{
		return (Mathf.Pow(progress, 3) - 2f * Mathf.Pow(progress, 2) + progress) * 3f + (-2f * Mathf.Pow(progress, 3) + 3f * Mathf.Pow(progress, 2))
				+ (Mathf.Pow(progress, 3) - Mathf.Pow(progress, 2)) * 3f;
	};

	#endregion

	#region Private Methods

	/// <summary>
	/// Bounce logic.
	/// </summary>
	static float BounceLogic(float val)
	{
		// 0.363636f changed from (1/ 2.75f) for speed reasons.
		const float bounceLowLimit = 0.363636f;

		// 0.727272f changed from (2/ 2.75f) for speed reasons.
		const float bounceMidLimit = 0.727272f;

		// 0.909090f changed from (2.5 / 2.75f) for speed reasons.
		const float bounceHighLimit = 0.909090f;

		const float bounceLowFactor = 7.5685f;

		const float bounceFactor = 7.5625f;

		if (val < bounceLowLimit)
		{
			val = bounceLowFactor * val * val;
		}
		else if (val < bounceMidLimit)
		{
			val = bounceFactor * (val -= 0.545454f) * val + 0.75f; // 0.545454f changed from (1.5f / 2.75f) for speed reasons.
		}
		else if (val < bounceHighLimit)
		{
			val = bounceFactor * (val -= 0.818181f) * val + 0.9375f; // 0.818181f changed from (2.25f / 2.75f) for speed reasons.
		}
		else
		{
			val = bounceFactor * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454f changed from (2.625f / 2.75f) for speed reasons.
		}
		return val;
	}

	#endregion
}