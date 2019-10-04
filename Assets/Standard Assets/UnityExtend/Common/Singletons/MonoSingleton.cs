using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null) { instance = GameObject.FindObjectOfType<T>(); }
			return instance;
		}
	}

	public void OnDestroy()
	{
		if (instance == this) { instance = null; }
	}
}