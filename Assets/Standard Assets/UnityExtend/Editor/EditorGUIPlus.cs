using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Extension of EditorGUI.
/// </summary>
public static partial class EditorGUIPlus
{
	/// <summary>
	/// Draw a visible separator in addition to adding some padding.
	/// </summary>
	public static void DrawSeparator(float height = 27f, float lineThickness = 2f)
	{
		GUILayout.Space(height);

		if (Event.current.type == EventType.Repaint)
		{
			DrawSeparator(GUILayoutUtility.GetLastRect());
		}
	}

	/// <summary>
	/// Draw a visible separator in addition to adding some padding.
	/// </summary>
	public static void DrawSeparator(Rect rect, float lineThickness = 2f)
	{
		if (Event.current.type == EventType.Repaint)
		{
			var tex = EditorGUIUtility.whiteTexture;
			rect.y += rect.height * 0.5f;
			rect.width = Screen.width - rect.x * 2;
			rect.height = lineThickness;

			var oldColor = GUI.color;
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
				GUI.DrawTexture(rect, tex);
			GUI.color = oldColor;
		}
	}

	public static bool ToggleButton(Rect rect, bool oldValue, GUIContent texture)
	{
		var newValue = GUI.Toggle(rect, oldValue, texture, EditorStyles.toolbarButton);
		return newValue != oldValue;
	}

	public static void ToggleDuo(string label, SerializedProperty left, SerializedProperty right, string leftLabel, string rightLabel)
	{
		ToggleDuo(EditorGUILayout.GetControlRect(), label, left, right, leftLabel, rightLabel);
	}

	public static void ToggleDuo(Rect rect, string label, SerializedProperty left, SerializedProperty right,
		string leftLabel, string rightLabel)
	{
		EditorGUI.LabelField(rect.TakeEditorLabel(), label);

		left.boolValue = GUI.Toggle(rect.Take(24f), left.boolValue, leftLabel, EditorStyles.miniButtonLeft);
		right.boolValue = GUI.Toggle(rect.Take(24f), right.boolValue, rightLabel, EditorStyles.miniButtonRight);
	}
}