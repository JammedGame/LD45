using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>
/// Base class of all custom editors that need EasyEditor functionalities.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class Editor : UnityEditor.Editor
{
	protected void DrawProperty(string propName, GUIContent content = null)
	{
		EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), content);
	}
}

/// <summary>
/// Replaces default inspector for scriptable objects to enable  EasyEditor functionalities.
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(ScriptableObject), true)]
public class ScriptableObjectEditor : Editor<ScriptableObject>
{
}

/// <summary>
/// Extend custom editor from this to avoid casting target.
/// </summary>
[CanEditMultipleObjects]
public class Editor<T> : Editor where T : UnityEngine.Object
{
	public new T[] targets => base.targets.OfType<T>().ToArray();
	public new T target => (T)base.target;
	public bool isSingleTarget => targets.Length == 1;
	public GameObject[] targetGameObjects => targets.OfType<Component>().Select(x => x.gameObject).ToArray();
}