using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EditorUpdateProvider
{
	static EditorUpdateProvider()
	{
		EditorApplication.update -= OnUpdate;
		EditorApplication.update += OnUpdate;
	}

	private static void OnUpdate()
	{
		if (Application.isPlaying) { return; }
		foreach(var component in UnityEngine.GameObject.FindObjectsOfType<MonoBehaviour>())
		{
			if (component is EditorUpdate hasUpdate) { hasUpdate.EditorUpdate(); }
		}
	}
}