using Unity.Collections;
using UnityEngine;

public readonly struct NativeAnimationCurve : System.IDisposable
{
	public readonly int length;
	[ReadOnly] public readonly NativeArray<Keyframe> keyFrames;

	public static implicit operator NativeAnimationCurve(AnimationCurve curve) => new NativeAnimationCurve(curve);

	public NativeAnimationCurve(AnimationCurve curve, Allocator allocator = Allocator.TempJob)
	{
		if (curve == null || curve.length == 0)
		{
			Debug.LogWarning("Animation curve is empty!");
			length = 1;
			keyFrames = new NativeArray<Keyframe>(length, allocator);
		}
		else
		{
			length = curve.length;
			keyFrames = new NativeArray<Keyframe>(length, allocator);
			for(int i = 0; i < length; i++)
				keyFrames[i] = curve[i];
		}
	}

	public float Evaluate(float time)
	{
		// find start key
		var startKey = 0;
		for(int i = 1; i < length; i++)
		{
			if (keyFrames[i].time < time)
			{
				startKey = i;
			}
			else
			{
				break;
			}
		}

		var keyframe0 = keyFrames[startKey];

		// is this the beginning?
		if (startKey == 0 && time <= keyframe0.time) return keyframe0.value;
		// is this the end?
		if (startKey + 1 >= length) return keyframe0.value;

		// interpolate to the next key
		var keyframe1 = keyFrames[startKey + 1];
		float timeDelta = keyframe1.time - keyframe0.time;

		float m0 = keyframe0.outTangent * timeDelta;
		float m1 = keyframe1.inTangent * timeDelta;

		float t = (time - keyframe0.time) / timeDelta;
		float t2 = t * t;
		float t3 = t2 * t;

		float a = 2 * t3 - 3 * t2 + 1;
		float b = t3 - 2 * t2 + t;
		float c = t3 - t2;
		float d = -2 * t3 + 3 * t2;

		return a * keyframe0.value + b * m0 + c * m1 + d * keyframe1.value;
	}

	public void Dispose()
	{
		keyFrames.Dispose();
	}
}