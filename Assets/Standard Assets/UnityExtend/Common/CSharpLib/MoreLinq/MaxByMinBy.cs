using System;
using System.Collections.Generic;

public static partial class MoreLinq
{
	public static T MaxBy<T>(this IList<T> list, Func<T, float> evaluation, float minMaxValue = float.MinValue)
	{
		int index = -1;
		float leadingScore = minMaxValue;
		for(int i = 0; i < list.Count; i++)
		{
			var evalScore = evaluation(list[i]);
			if (evalScore > leadingScore)
			{
				index = i;
				leadingScore = evalScore;
			}
		}

		return index > -1 ? list[index] : default(T);
	}

	public static T MinBy<T>(this IList<T> list, Func<T, float> evaluation)
	{
		int index = -1;
		float leading = float.MaxValue;
		for(int i = 0; i < list.Count; i++)
		{
			var evalScore = evaluation(list[i]);
			if (evalScore < leading)
			{
				index = i;
				leading = evalScore;
			}
		}

		return index > -1 ? list[index] : default(T);
	}

	public static T MaxBy<T>(this IEnumerable<T> source, Func<T, float> evaluation)
	{
		T returnValue = default(T);
		float leadingScore = float.MinValue;
		foreach(var obj in source)
		{
			var evalScore = evaluation(obj);
			if (evalScore > leadingScore)
			{
				returnValue = obj;
				leadingScore = evalScore;
			}
		}

		return returnValue;
	}

	public static T MinBy<T>(this IEnumerable<T> source, Func<T, float> evaluation)
	{
		T returnValue = default(T);
		float leadingScore = float.MaxValue;
		foreach(var obj in source)
		{
			var evalScore = evaluation(obj);
			if (evalScore < leadingScore)
			{
				returnValue = obj;
				leadingScore = evalScore;
			}
		}

		return returnValue;
	}
}