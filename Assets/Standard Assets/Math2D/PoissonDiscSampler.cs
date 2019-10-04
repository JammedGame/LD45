using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using static UnityEngine.Mathf;
using System;

public class PoissonDiscSampler
{
	public static IEnumerable<float2> Sample(Rect rect, int count, uint randomSeed)
	{
		return Sample(rect.width, rect.height, rect.xMin, rect.yMin, count, randomSeed);
	}

	public static IEnumerable<float2> Sample(float rectWidth, float rectHeight, int count, uint randomSeed)
	{
		return Sample(rectWidth, rectHeight, 0, 0, count, randomSeed);
	}

	public static IEnumerable<float2> Sample(float rectWidth, float rectHeight, float xOffset, float yOffset, int count, uint randomSeed)
	{
		var radius = Sqrt(rectWidth * rectHeight * 2 / PI / count * 1.02f);
		return Sample(rectWidth, rectHeight, xOffset, yOffset, radius, count, randomSeed);
	}

	public static IEnumerable<float2> Sample(float rectWidth, float rectHeight, float xOffset, float yOffset, float radius, int count, uint randomSeed)
	{
		if (count <= 0)
			throw new ArgumentException("PoissonDisc sampling count must be positive!");

		var sampler = new PoissonDiscSampler(rectWidth, rectHeight, radius);

		foreach(var pos in sampler.Samples(randomSeed))
		{
			if (count-- <= 0) { break; }
			yield return new float2(pos.x + xOffset, pos.y + yOffset);
		}
	}

	private const int maxAttempts = 30;  // Maximum number of attempts before marking a sample as inactive.

	private readonly float rectWidth, rectHeight;
	private readonly float radius2;  // radius squared
	private readonly float cellSize;
	private readonly Unity.Mathematics.Random random;
	private readonly int gridWidth, gridHeight;
	private readonly float2[,] grid;
	private List<float2> activeSamples = new List<float2>();

	/// Create a sampler with the following parameters:
	///
	/// width:  each sample's x coordinate will be between [0, width]
	/// height: each sample's y coordinate will be between [0, height]
	/// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
	public PoissonDiscSampler(float width, float height, float radius)
	{
		rectWidth = width;
		rectHeight = height;
		radius2 = radius * radius;
		cellSize = radius / Mathf.Sqrt(2);
		gridWidth = Mathf.CeilToInt(width / cellSize);
		gridHeight = Mathf.CeilToInt(height / cellSize);
		grid = new float2[gridWidth, gridHeight];
	}

	/// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
	///   foreach (float2 sample in sampler.Samples()) { ... }
	public IEnumerable<float2> Samples(uint randomSeed)
	{
		var random = new Unity.Mathematics.Random(randomSeed);

		// First sample is choosen randomly
		if (activeSamples.Count == 0)
		{
			yield return AddSample(new float2((float)random.NextDouble() * rectWidth, (float)random.NextDouble() * rectHeight));
		}

		while (activeSamples.Count > 0)
		{
			// Pick a random active sample
			var i = (int) (random.NextDouble() * activeSamples.Count);
			var sample = activeSamples[i];

			// Try `k` random candidates between [radius, 2 * radius] from that sample.
			bool found = false;
			for (int j = 0; j < maxAttempts; j++)
			{
				var angle = 2 * PI * random.NextFloat();
				var r = Sqrt(random.NextFloat() * 3 * radius2 + radius2); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
				var candidate = sample + r * new float2(Cos(angle), Sin(angle));

				// Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
				if (candidate.x >= 0 && candidate.y >= 0 && candidate.x <= rectWidth && candidate.y <= rectHeight && IsFarEnough(candidate))
				{
					found = true;
					yield return AddSample(candidate);
					break;
				}
			}

			// If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
			if (!found)
			{
				activeSamples.RemoveAt(i);
			}
		}
	}

	private bool IsFarEnough(float2 sample)
	{
		var gridPos = GetGridPos(sample);
		int xmin = Mathf.Max(gridPos.x - 2, 0);
		int ymin = Mathf.Max(gridPos.y - 2, 0);
		int xmax = Mathf.Min(gridPos.x + 2, gridWidth - 1);
		int ymax = Mathf.Min(gridPos.y + 2, gridHeight - 1);

		for (int y = ymin; y <= ymax; y++) {
			for (int x = xmin; x <= xmax; x++) {
				float2 s = grid[x, y];
				if (s.x != 0 && s.y != 0)
				{
					float2 d = s - sample;
					if (d.x * d.x + d.y * d.y < radius2) return false;
				}
			}
		}

		return true;

		// Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
		// to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
		// and we might end up with another sample too close from (0, 0). This is a very minor issue.
	}

	/// Adds the sample to the active samples queue and the grid before returning it
	private float2 AddSample(float2 sample)
	{
		activeSamples.Add(sample);
		var gridPos = GetGridPos(sample);
		grid[gridPos.x, gridPos.y] = sample;
		return sample;
	}

	int2 GetGridPos(float2 pos)
	{
		var x = Mathf.Clamp((int)(pos.x / cellSize), 0, gridWidth - 1);
		var y = Mathf.Clamp((int)(pos.y / cellSize), 0, gridHeight - 1);
		return new int2(x, y);
	}
}