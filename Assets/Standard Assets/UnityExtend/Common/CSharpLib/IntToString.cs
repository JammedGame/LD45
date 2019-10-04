using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Provides allocation-less Integer.ToStringLookup().
/// </summary>
public static class IntToString
{
	public const int MaxInternedString = 200;

	static readonly string[] cache;
	static readonly Dictionary<int, string> cacheBigNumbers = new Dictionary<int, string>();

	static IntToString()
	{
		cache = new string[MaxInternedString + 1];
		for (int i = 0; i <= MaxInternedString; i++)
		{
			cache[i] = i.ToString();
		}
	}

	public static string ToStringLookup(this int value)
	{
		if (value >= 0 && value <= MaxInternedString)
		{
			return cache[value];
		}

		if (!cacheBigNumbers.TryGetValue(value, out string cached))
		{
			cached = value.ToString(CultureInfo.InvariantCulture);
			cacheBigNumbers[value] = cached;
		}
		return cached;
	}
}