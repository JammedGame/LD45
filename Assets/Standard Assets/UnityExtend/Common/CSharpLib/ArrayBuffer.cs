using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// This improved version of the System.Collections.Generic.List that doesn't release the buffer on Clear(), resulting in better performance and less garbage collection.
/// </summary>

public class ArrayBuffer<T> : IEnumerable<T>, IList<T>
{
	const int MinimumBufferSize = 4;

	/// <summary>
	/// Direct access to the buffer. Note that you should not use its 'Length' parameter, but instead use ArrayBuffer.size.
	/// </summary>

	public T[] buffer;

	/// <summary>
	/// Buffer length.
	/// </summary>
	private int buffer_length;

	/// <summary>
	/// Direct access to the buffer's size. Note that it's only public for speed and efficiency. You shouldn't modify it.
	/// </summary>

	public int Count;

	/// <summary>
	/// IList implementation.
	/// </summary>
	int ICollection<T>.Count => Count;

	/// <summary>
	/// IList implementation.
	/// </summary>
	bool ICollection<T>.IsReadOnly => false;

	/// <summary>
	/// For 'foreach' functionality.
	/// </summary>

	public IEnumerator<T> GetEnumerator() => ArrayBufferEnumerator.Fetch(this);

	IEnumerator IEnumerable.GetEnumerator() => ArrayBufferEnumerator.Fetch(this);

	/// <summary>
	/// Convenience function. I recommend using .buffer instead.
	/// </summary>
	public T this[int i]
	{
		get { return buffer[i]; }
		set { buffer[i] = value; }
	}

	/// <summary>
	/// Helper function that expands the size of the array, maintaining the content.
	/// </summary>
	void AllocateMore()
	{
		int newSize = MinimumBufferSize;
		if (buffer != null) { newSize = Mathf.Max(buffer.Length << 1, MinimumBufferSize); }
		Array.Resize(ref buffer, newSize);
	}

	/// <summary>
	/// Clear the array by resetting its size to zero. Note that the memory is not actually released.
	/// </summary>
	public void Clear()
	{
		// We need to explicitley release reference
		if (Count > 0) { Array.Clear(buffer, 0, Count); }
		Count = 0;
	}

	/// <summary>
	/// Clear the array and release the used memory.
	/// </summary>
	public void Release() { Count = 0; buffer = null; }

	/// <summary>
	/// Add the specified item to the end of the list.
	/// </summary>
	public void Add(T item)
	{
		if (buffer == null || Count == buffer.Length) AllocateMore();
		buffer[Count++] = item;
	}

	/// <summary>
	/// Add the specified item to the end of the list, if not already contained.
	/// </summary>
	public void AddUnique(T item)
	{
		if (Contains(item)) { return; }
		Add(item);
	}

	/// <summary>
	/// Concatenates contents of this ArrayBuffer to the specified ArrayBuffer.
	/// </summary>
	public void CopyTo(ArrayBuffer<T> dest)
	{
		if (buffer == null || Count == 0) { return; }
		if (dest.buffer == null || dest.buffer.Length < Count + dest.Count) Array.Resize(ref dest.buffer, Count + dest.Count);
		Array.Copy(buffer, 0, dest.buffer, dest.Count, Count);
		dest.Count = Count + dest.Count;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (buffer == null || Count == 0) { return; }
		if (array == null || array.Length < Count + arrayIndex) Array.Resize(ref array, Count + arrayIndex);
		Array.Copy(buffer, 0, array, arrayIndex, Count);
	}

	/// <summary>
	/// Fills buffer with the specified value.
	/// </summary>
	public void FillWith(T value, int amount)
	{
		if (buffer == null || buffer.Length < amount) Array.Resize(ref buffer, amount);
		for (int i = 0; i < amount; i++) { buffer[i] = value; }
		if (Count < amount) Count = amount;
	}

	/// <summary>
	/// Insert an item at the specified index, pushing the entries back.
	/// </summary>
	public void Insert(int index, T item)
	{
		if (buffer == null || Count == buffer.Length) AllocateMore();

		if (index < Count)
		{
			Array.Copy(buffer, index, buffer, index + 1, Count - index);
			buffer[index] = item;
			++Count;
		}
		else Add(item);
	}

	/// <summary>
	/// Returns 'true' if the specified item is within the list.
	/// </summary>
	public bool Contains(T item)
	{
		if (buffer == null) return false;
		return IndexOf(item) != -1;
	}

	/// <summary>
	/// Remove the specified item from the list. Note that RemoveAt() is faster and is advisable if you already know the index.
	/// </summary>
	public bool Remove(T item)
	{
		int loc = IndexOf(item);
		if (loc != -1) { RemoveAt(loc); }
		return loc != -1;
	}

	/// <summary>
	/// Remove an item at the specified index.
	/// </summary>
	public void RemoveAt(int index)
	{
		if (buffer == null || index < 0 || index >= Count) { return; }

		Shift(index, -1);
		Array.Clear(buffer, Count, 1);
	}

	/// <summary>
	/// Removes all elements that match predicate.
	/// </summary>
	/// <param name="match">Predicate to match</param>
	/// <returns>Number of removed elements</returns>
	public int RemoveAll(Predicate<T> match)
	{
		if (buffer == null || match == null) { return 0; }

		int numberOfRemovedElements = 0;

		for (int i = 0; i < Count; i++)
		{
			if (match(buffer[i]))
			{
				// Element should be removed.
				numberOfRemovedElements++;
				continue;
			}

			if (numberOfRemovedElements > 0)
			{
				// Element should not be removed. Reposition it instead.
				buffer[i - numberOfRemovedElements] = buffer[i];
			}
		}

		if (numberOfRemovedElements > 0)
		{
			// Clear last 'numberOfRemovedElements' elements
			Array.Clear(buffer, Count - numberOfRemovedElements, numberOfRemovedElements);
			Count -= numberOfRemovedElements;
		}

		return numberOfRemovedElements;
	}


	/// <summary>
	/// Get index if item. Returns -1 if item is not found.
	/// </summary>
	public int IndexOf(T item)
	{
		if (buffer == null) return -1;

		// return Array.IndexOf<T>(buffer, item, 0, size);
		for (int i = 0; i < Count; i++)
		{
			if (ReferenceEquals(item, buffer[i])) { return i; }
		}

		return -1;
	}

	/// <summary>
	/// Shift buffer data from start location by delta places left or right
	/// </summary>
	void Shift(int start, int delta)
	{
		if (delta < 0)
		{
			start -= delta;
		}

		if (start < Count)
			Array.Copy(buffer, start, buffer, start + delta, Count - start);

		Count += delta;

		if (delta < 0)
			Array.Clear(buffer, Count, -delta);
	}

	public void Sort(IComparer<T> comparer)
	{
		if (buffer != null)
		{
			Array.Sort(buffer, 0, Count, comparer);
		}
	}

	/// <summary>
	/// Make sure list could accept numberOfElements elements.
	/// </summary>
	/// <param name="numberOfElements">The number of elements to allocate.</param>
	public void EnsureCapacity(int numberOfElements)
	{
		int minSize = Mathf.Max(Count, MinimumBufferSize);
		minSize = Mathf.Max(minSize, numberOfElements);
		if (minSize == 0 || buffer != null && buffer.Length >= minSize) { return; }

		int newSize = Mathf.NextPowerOfTwo(minSize);
		Array.Resize(ref buffer, newSize);
	}

	/// <summary>
	/// Remove an item from the end.
	/// </summary>

	public T Pop()
	{
		if (buffer != null && Count != 0)
		{
			T val = buffer[--Count];
			buffer[Count] = default(T);
			return val;
		}
		return default(T);
	}

	public T Find(Func<T, bool> filter)
	{
		for(int i = 0; i < Count; i++)
		{
			if (filter(buffer[i])) return buffer[i];
		}
		return default(T);
	}

	public class ArrayBufferEnumerator : ReusableEnumerator<T, ArrayBufferEnumerator>
	{
		int i;
		int cnt;
		T[] buffer;

		public static ArrayBufferEnumerator Fetch(ArrayBuffer<T> buffer)
		{
			var enumerator = Fetch();
			enumerator.buffer = buffer.buffer;
			enumerator.cnt = buffer.Count;
			enumerator.i = 0;
			return enumerator;
		}

		public override bool MoveNext()
		{
			if (i < cnt)
			{
				Current = buffer[i++];
				return true;
			}

			return false;
		}

		public override void OnDisposed()
		{
			i = 0;
			cnt = 0;
			buffer = null;
		}
	}
}


public static class ArrayBuffer_NativeArrayExtensions
{
	public static void CopyFrom<T>(this NativeArray<T> dst, ArrayBuffer<T> src) where T : struct
	{
		NativeArray<T>.Copy(src.buffer, dst, src.Count);
	}
}