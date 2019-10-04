using System;
using System.Collections.Generic;

public static class CSharpLib
{
	/// <summary>
	/// Returns number of elements in an array which are not null.
	/// </summary>
	public static int CountNotNull<T>(this T[] array)
	{
		int count = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null) { count++; }
		}
		return count;
	}

	/// <summary>
	/// Returns array of elements except nulls.
	/// </summary>
	public static T[] TrimNulls<T>(this T[] array)
	{
		int countNotNull = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null) { countNotNull++; }
		}

		var result = new T[countNotNull];
		for (int i = 0, j = 0; i < array.Length; i++)
		{
			if (array[i] != null) { result[j++] = array[i]; }
		}

		return result;
	}

	/// <summary>
	/// Enques a collection.
	/// </summary>
	public static void AddRange<T>(this Queue<T> queue, IList<T> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			queue.Enqueue(items[i]);
		}
	}

	/// <summary>
	/// Returns null if key does not exist.
	/// </summary>
	public static V Get<K, V>(this Dictionary<K, V> dictionary, K key)
	{
		dictionary.TryGetValue(key, out V value);
		return value;
	}

	static readonly Random rng = new Random();

	/// <summary>
	/// Shuffles a list.
	/// </summary>
	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}

public static class SafeDispatch
{
	public static void Dispatch(this Action action)
	{
		try { action?.Invoke(); }
		catch(System.Exception e) { LogException(e); }
	}

	public static void Dispatch<T1>(this Action<T1> action, T1 arg1)
	{
		try { action?.Invoke(arg1); }
		catch(System.Exception e) { LogException(e); }
	}

	public static void Dispatch<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		try { action?.Invoke(arg1, arg2); }
		catch(System.Exception e) { LogException(e); }
	}

	public static void Dispatch<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		try { action?.Invoke(arg1, arg2, arg3); }
		catch(System.Exception e) { LogException(e); }
	}

	public static void Foreach<T1>(this IEnumerable<T1> list, Action<T1> action)
	{
		foreach(var obj in list)
			try { action(obj); }
			catch(Exception e) { LogException(e); }
	}

	public static void Foreach<T1>(this IList<T1> list, Action<T1> action)
	{
		for(int i = 0; i < list.Count; i++)
			try { action(list[i]); }
			catch(Exception e) { LogException(e); }
	}

	public static void Foreach<T1, T2>(this IList<T1> list, Action<T1, T2> action, T2 args)
	{
		for(int i = 0; i < list.Count; i++)
			try { action(list[i], args); }
			catch(Exception e) { LogException(e); }
	}

	public static void LogException(Exception e)
	{
		UnityEngine.Debug.LogException(e);
	}
}