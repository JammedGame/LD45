using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Immutable Settings used to create animations.
/// </summary>
[System.Serializable]
public struct NAnimationSettings
{
	public bool IsLooping;
	public bool IsPingPong;
	public float Duration;
	public float Delay;
	public float TotalDuration;
	public float StartValue;
	public float TargetValue;
	public Func<float, float> Easing;
	public Action<float> Action;
	public Action Callback;
	public AnimationCurve Curve;
	public bool PlayOnce => !IsPingPong && !IsLooping;
}

public class NAnimation : CustomYieldInstruction
{
	/* internal state, changes over time. */
	float progress;
	float timeSinceStart;
	bool firstTickDone;
	bool stopped;
	bool isFinished;
	float delay;
	float timeScale = 1;

	/* immutable settings */
	private NAnimationSettings settings;

	/* special condition - when set, animation will terminate if this object is destroyed. */
	public NLazy<UnityEngine.Object> target;

	internal void Initialize(NAnimationSettings settings, UnityEngine.Object owner = null)
	{
		this.settings = settings;
		progress = 0f;
		timeSinceStart = 0f;
		delay = settings.Delay;
		firstTickDone = false;
		stopped = false;
		isFinished = false;
		timeScale = 1f;
		RefreshCurveDuration();
		if (owner != null) { this.target = owner; }
	}

	/// <summary>
	/// Pool to save on GC.
	/// </summary>
	internal static class Pool
	{
		static readonly Queue<NAnimation> pool = new Queue<NAnimation>();

		static Pool()
		{
			for(int i = 0; i < 100; i++)
				pool.Enqueue(new NAnimation());
		}

		public static NAnimation Fetch(ref NAnimationSettings settings, UnityEngine.Object owner = null)
		{
			var fetched = pool.Count > 0 ? pool.Dequeue() : new NAnimation();
			fetched.Initialize(settings, owner);
			return fetched;
		}

		public static void Release(NAnimation animation)
		{
			if (animation == null) { return; }
			animation.target.ClearValue();
			pool.Enqueue(animation);
		}
	}

	public void RefreshCurveDuration()
	{
		var curve = settings.Curve;
		if (curve == null) return;
		settings.Duration = curve.Duration();
	}

	/// <summary>
	/// Advances animation for dT seconds. Progress value will be changed.
	/// Returns false if animation is finished.
	/// </summary>
	public bool Tick(float dT)
	{
		/* check alive. */
		if (IsFinished)
		{
			return false;
		}

		delay -= dT;
		if (delay > 0f)
		{
			return true;
		}

		/* always start with clean zero */
		if (!firstTickDone)
		{
			firstTickDone = true;
			settings.Action.Dispatch(Value);
			return true;
		}

		/* update progress */
		timeSinceStart += dT;
		progress += dT * timeScale / settings.Duration;

		/* what to do if reached boundaries? */
		if (progress < 0f || progress > 1f)
		{
			/* ping pong animation - non looping terminate at progress < 0, looping ones wait for total duration. */
			if (settings.IsPingPong)
			{
				timeScale = -timeScale;

				/* _ping pong once_ animations terminate  */
				if (!settings.IsLooping && progress < 0f)
				{
					progress = 0f;
					isFinished = true;
				}

				// handle ping pong looping - just fold over and change direction
				if (progress < 0f) { progress = 0 - progress; }
				if (progress > 1f) { progress = 2 - progress; }
			}
			else if (settings.IsLooping)
			{
				/* rewind progress when loop end is reached. */
				while (progress > 1f) { progress -= 1f; }
				while (progress < 0f) { progress += 1f; }
			}
			else if (settings.PlayOnce)
			{
				/* one time animations - terminate if reached the end. */
				if (progress > 1f)
				{
					progress = 1f;
					isFinished = true;
				}
			}

			// read stats from animation curve, in case animation was being adjusted on the fly.
			RefreshCurveDuration();
		}

		/* check if loop was capped by "TotalDuration" */
		if (settings.TotalDuration > 0f && timeSinceStart > settings.TotalDuration)
		{
			isFinished = true;
		}

		/* execute actual animation action */
		try
		{
			settings.Action?.Invoke(Value);
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}

		return !IsFinished;
	}

	/// <summary>
	/// Manually stop current action.
	/// </summary>
	public void Stop(bool skipCallbacks = false)
	{
		if (!skipCallbacks)
		{
			DispatchCallback();
		}

		stopped = true;
	}

	/// <summary>
	/// Value of the animation at current progress.
	/// </summary>
	public float Value
	{
		get
		{
			var valueProgress = settings.Curve != null ? settings.Curve.Evaluate(progress * settings.Duration) : settings.Easing(progress);
			if (settings.StartValue == 0f && settings.TargetValue == 0f) { return valueProgress; }
			return Mathf.Lerp(settings.StartValue, settings.TargetValue, valueProgress);
		}
	}

	/// <summary>
	/// Whether this animation has been finished, by either: time expiration, manual stopping, or destruction of its owner (if one was set).
	/// </summary>
	public bool IsFinished
	{
		get { return stopped || isFinished || settings.Duration <= 0f || (target.IsAssigned && target.Value == null); }
	}

	/// <summary>
	/// Returns true if not finished.
	/// </summary>
	public bool IsPlaying => !IsFinished;

	/// <summary>
	/// Allows for using of NAnimations in coroutines.
	/// </summary>
	public override bool keepWaiting => !IsFinished;

	/// <summary>
	/// Called when animation gets finished
	/// </summary>
	internal void DispatchCallback()
	{
		if (!stopped)
			settings.Callback.Dispatch();
	}
}