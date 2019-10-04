using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Allows for delayed calling of stuff on main thread in editor.
/// </summary>
[InitializeOnLoad]
public static class EditorInvoker
{
	static EditorInvoker()
	{
		EditorApplication.update -= Update;
		EditorApplication.update += Update;
	}

	static List<Action> buffered = new List<Action>();
	static List<Action> invoked = new List<Action>();

	static void Update()
	{
		for (int i = 0; i < invoked.Count; i++)
		{
			try { invoked[i](); }
			catch(Exception e) { Debug.LogException(e); }
		}
		invoked.Clear();

		// switch buffers.
		var tmp = invoked;
		invoked = buffered;
		buffered = tmp;
	}

	public static void Invoke(Action action)
	{
		if (action == null) return;
		buffered.Add(action);
	}
}
