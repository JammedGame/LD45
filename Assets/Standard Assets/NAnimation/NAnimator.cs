using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Accessible APIs for NAnimation.
/// </summary>
public class NAnimator : Singleton<NAnimator>
{
	/// <summary>
	/// Keeps track of all active animations
	/// </summary>
	readonly List<NAnimation> animations = new List<NAnimation>();

	/// <summary>
	/// Update all animations each frame.
	/// </summary>
	public NAnimator()
	{
		Invoker.Update -= UpdateAnimations;
		Invoker.Update += UpdateAnimations;
	}

	public void UpdateAnimations()
	{
		var dT = Mathf.Min(Time.deltaTime, 0.1f);
		for(int i = animations.Count - 1; i >= 0; i--)
		{
			/* check for stopped animations */
			var animation = animations[i];
			if (animation.IsFinished)
			{
				animation.DispatchCallback();
				NAnimation.Pool.Release(animation);
				animations.RemoveAt(i);
				continue;
			}

			/* tick animation, call callback */
			animation.Tick(dT);

			/* remove if dead */
			if (animation.IsFinished)
			{
				animation.DispatchCallback();
				NAnimation.Pool.Release(animation);
				animations.RemoveAt(i);
				continue;
			}
		}
	}

	/// <summary>
	/// Stop all animations related to this object
	/// </summary>
	public void StopAllAnimations(Object target, bool skipCallbacks = true)
	{
		if (target == null) { return; }

		foreach(var animation in animations)
		{
			if (animation.target == target) { animation.Stop(skipCallbacks); }
		}
	}

	/// <summary>
	/// Add animation to the loop.
	/// </summary>
	public static NAnimation Animate(Object target, NAnimationSettings animationSettings)
	{
		var newAnimation = NAnimation.Pool.Fetch(ref animationSettings, target);
		Instance.animations.Add(newAnimation);
		return newAnimation;
	}


	public static NAnimation Animate(Action<float> action, Func<float, float> easing, float duration, float delay = 0f, Action callback = null) => Animate(null, action, easing, duration, delay, callback);
	public static NAnimation Animate(Object target, Action<float> action, Func<float, float> easing, float duration, float delay = 0f, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Easing = easing,
			Delay = delay,
			Duration = duration,
			Callback = callback,
		});
	}

	public static NAnimation Animate(Object target, Action<float> action, Func<float, float> easing, float from, float to, float duration, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Easing = easing,
			StartValue = from,
			TargetValue = to,
			Duration = duration,
			Callback = callback,
		});
	}

	public static NAnimation Animate(Action<float> action, AnimationCurve curve, Action callback = null) => Animate(null, action, curve, callback);
	public static NAnimation Animate(Object target, Action<float> action, AnimationCurve curve, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Curve = curve,
			Callback = callback,
		});
	}

	public static NAnimation Loop(AnimationCurve curve, Action<float> action, float totalDuration = 0f, Action callback = null) => Loop(null, action, curve, totalDuration, callback);
	public static NAnimation Loop(Object target, Action<float> action, AnimationCurve curve, float totalDuration = 0f, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Curve = curve,
			IsLooping = true,
			TotalDuration = totalDuration,
			Callback = callback,
		});
	}

	public static NAnimation LoopForever(Action<float> action) => LoopForever(null, action);
	public static NAnimation LoopForever(Object target, Action<float> action)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			IsLooping = true,
			StartValue = 0f,
			TargetValue = float.MaxValue,
			TotalDuration = float.MaxValue,
			Duration = float.MaxValue,
			Easing = EasingFunctions.Linear
		});
	}

	public static NAnimation PingPong(Action<float> action, AnimationCurve curve, Action callback = null) => PingPong(null, action, curve, callback);
	public static NAnimation PingPong(Object target, Action<float> action, AnimationCurve curve, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Curve = curve,
			IsPingPong = true,
			Callback = callback,
		});
	}

	public static NAnimation PingPong(Action<float> action, Func<float, float> curve, float duration, Action callback = null) => PingPong(null, action, curve, duration, callback);
	public static NAnimation PingPong(Object target, Action<float> action, Func<float, float> curve, float duration, Action callback = null)
	{
		return Animate(target, new NAnimationSettings()
		{
			Action = action,
			Easing = curve,
			Duration = duration,
			TotalDuration = duration * 2,
			IsPingPong = true,
			Callback = callback,
		});
	}
}