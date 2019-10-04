using UnityEngine;

public static class GameObjectExtensions
{
	/// <summary>
	/// Add component of specific type to GameObject if doesn't exist already and return it.
	/// If already exists just return it.
	/// </summary>
	public static ComponentType AddComponentIfDoesntExist<ComponentType>(this GameObject gameObject) where ComponentType : UnityEngine.Component
	{
		var component = gameObject.GetComponent<ComponentType>();
		if (component == null)
		{
			return gameObject.AddComponent<ComponentType>();
		}
		else
		{
			return component;
		}
	}

	public static GameObject AddChild(this GameObject go, string name = "")
	{
		var newGO = new GameObject(name);
		newGO.transform.SetParent(go.transform, worldPositionStays: false);
		return newGO;
	}

	public static T AddChild<T>(this GameObject go, string name = "") where T : UnityEngine.Component
	{
		var newGO = new GameObject(name);
		var newT = newGO.AddComponent<T>();
		newGO.transform.SetParent(go.transform, worldPositionStays: false);
		return newT;
	}
}