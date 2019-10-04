using UnityEngine;

public class ScriptableObject<T> : ScriptableObject where T : ScriptableObject<T>
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				var path = "Settings/" + typeof(T).Name;
				instance = Resources.Load<T>(path);
				if (instance == null)
				{
#if UNITY_EDITOR
					var assetPath = "Assets/Resources/" + path + ".asset";
					var dir = System.IO.Path.GetDirectoryName(assetPath);
					if (!System.IO.Directory.Exists(dir)) { System.IO.Directory.CreateDirectory(dir); }
					instance = ScriptableObject.CreateInstance<T>();
					UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
					instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
					Debug.LogError("Failed to load: " + typeof(T).Name);
#endif
				}
			}
			return instance;
		}
	}
}