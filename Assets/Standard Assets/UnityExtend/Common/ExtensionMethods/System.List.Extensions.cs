using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Static class containing extension methods for list.
/// </summary>
public static class ListExtensions
{
	/// <summary>
	/// Takes last element of the list, or default(T) if list is empty or null.
	/// </summary>
	public static T Last<T>(this List<T> list)
	{
		return list != null && list.Count > 0 ? list[list.Count - 1] : default(T);
	}

	/// <summary>
	/// Stack like push.
	/// </summary>
	public static void Push<T>(this List<T> list, T element)
	{
		list.Add(element);
	}

	/// <summary>
	/// Stack like push.
	/// </summary>
	public static T Pop<T>(this List<T> list)
	{
		var elem = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return elem;
	}

	/// <summary>
	/// Returns true if collection is null or empty,
	/// otherwise returns false.
	/// </summary>
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	/// <summary>
	/// Gets element of the list, but returns default(T) if index is out of range.
	/// </summary>
	public static T GetSafe<T>(this List<T> collection, int index)
	{
		if (index >= 0 && collection.Count > index)
		{
			return collection[index];
		}

		return default(T);
	}

	/// <summary>
	/// Calls to list.Clear and list.AddRange
	/// </summary>
	public static void ClearAndAdd<T>(this List<T> collection, IEnumerable<T> stuffToAdd)
	{
		collection.Clear();
		collection.AddRange(stuffToAdd);
	}

	/// <summary>
	/// Gets element of the list, but clamps index to list count if out of range.
	/// If list is empty, default(T) is returned.
	/// </summary>
	public static T GetClamped<T>(this List<T> collection, int index)
	{
		var count = collection.Count;
		if (count == 0) { return default(T); }
		if (index <= 0) { return collection[0]; }
		if (index >= count - 1) { return collection[count - 1]; }
		return collection[index];
	}

	/// <summary>
	/// Gets element of the list, but clamps index to list count if out of range.
	/// If list is empty, default(T) is returned.
	/// </summary>
	public static T GetClamped<T>(this T[] collection, int index)
	{
		var count = collection.Length;
		if (count == 0) { return default(T); }
		if (index <= 0) { return collection[0]; }
		if (index >= count - 1) { return collection[count - 1]; }
		return collection[index];
	}

	public static int CountExcept<T>(this List<T> list, T except) where T : class
	{
		int count = 0;
		for(int i = 0; i < list.Count; i++)
		{
			if (list[i] != except) { count++; }
		}
		return count;
	}

	public static bool AddDistinct<T>(this List<T> list, T data) where T : class
	{
		if (list.Contains(data))
		{
			return true;
		}
		else
		{
			list.Add(data);
			return false;
		}
	}
}