using System.Collections.Generic;
using Unity.Collections;

public static class NativeArrayExtensions
{
	public static bool Allocate<T>(this ref NativeArray<T> dst, int count, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
	{
		if (dst.IsCreated && dst.Length == count)
			return false;
		if (dst.IsCreated)
			dst.Dispose();

		dst = new NativeArray<T>(count, allocator, options);
		return true;
	}

	public static NativeList<T> ToNativeList<T>(this IEnumerable<T> src, Allocator allocator) where T : struct
	{
		var list = new NativeList<T>(allocator);
		foreach(var item in src)
			list.Add(item);
		return list;
	}
}