using UnityEngine;
using Random = System.Random;

namespace Nordeus.Util.CSharpLib
{
	/// <summary>
	/// Static class containing extension methods for random.
	/// </summary>
	public static class RandomExtensions
	{

		/// <summary>
		/// Gets random int. Assumes input arguments are correct.
		/// </summary>
		/// <param name="random">Random from which to generate the int.</param>
		/// <param name="minValue">Min possible value (inclusive).</param>
		/// <param name="maxValue">Max possible value (inclusive).</param>
		/// <returns>Random int between the arguments passed, including arguments themselves.</returns>
		public static int GetRandomInt(this Random random, int minValue, int maxValue)
		{
			int bound = maxValue - minValue + 1;
			return (int)(random.NextDouble() * bound) + minValue;
		}

		/// <summary>
		/// Returns a random boolean value.
		/// </summary>
		/// <returns>Either true or false, it is a non-deterministic result.</returns>
		public static bool GetRandomBool(this Random random)
		{
			return random.NextDouble() < 0.5;
		}

		public static Vector2 GetRandomVector2(this Random random)
		{
			return new Vector2((float)random.NextDouble(), (float)random.NextDouble());
		}

		public static float GetRandomFloat(this Random random)
		{
			return (float)random.NextDouble();
		}

		public static double GetNormalDistributionRandom(this Random random, double mean, double variance)
		{
			double u1 = random.NextDouble(); // These are uniform (0,1) random doubles.
			double u2 = random.NextDouble();
			double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // Random normal (0,1).
			return mean + variance * randStdNormal; // Random normal (mean, stdDev^2).
		}

		public static float GetNormalDistributionRandom(this Random random, float mean, float variance)
		{
			float u1 = (float)random.NextDouble(); // These are uniform (0,1) random floats.
			float u2 = (float)random.NextDouble();
			float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); // Random normal (0,1).
			return mean + variance * randStdNormal; // Random normal (mean, stdDev^2).
		}

	}
}
