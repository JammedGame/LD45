public class Singleton<T> where T : Singleton<T>, new()
{
	static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null) { instance = new T(); }
			return instance;
		}
	}

	/// <summary>
	/// Call this if you want to initialize singleton without doing any action.
	/// </summary>
	public void Touch() { }

	public static void Clear()
	{
		if (instance == null) { return; }
		instance.OnClear();
		instance = null;
	}

	protected virtual void OnClear() { }
}