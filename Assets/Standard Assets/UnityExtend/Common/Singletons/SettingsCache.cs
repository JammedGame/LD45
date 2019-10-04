using UnityEngine;

public class SettingsCache<T> where T : ScriptableObject
{
	T cached;
	public readonly string Path;
	public SettingsCache(string path) => Path = path;

	public T Load()
	{
		if (cached == null) { cached = Resources.Load<T>(Path); }
		return cached;
	}

	public static implicit operator T(SettingsCache<T> x)
	{
		return x.Load();
	}
}