using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static partial class EditorGUIPlus
{
	private static readonly KeyCode[] UpDownCodes = { KeyCode.UpArrow, KeyCode.DownArrow };
	private static readonly KeyCode[] ArrowCodes = { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };

	private const int UpDownSpeed = 10;
	private static int currentUpDownSpeedFactor;

	public static TextEditor ActiveTextEditor => typeof(EditorGUI).GetField("s_RecycledEditor", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as TextEditor;

	public static float UpDownFloatField(string label, float value)
	{
		var newval = EditorGUILayout.FloatField(label, value);
		newval = HandleKeyboardUpDown(newval);
		return newval;
	}

	public static float UpDownFloatField(Rect rect, string label, float value)
	{
		var newval = EditorGUI.FloatField(rect, label, value);
		newval = HandleKeyboardUpDown(newval);
		return newval;
	}

	/// <summary>
	/// Capture keyboard events for that last created control. Callback has one argument: KeyCode.
	/// </summary>
	public static void KeyboardHandler(KeyCode[] keyCodesToHandle, Action<KeyCode> callback)
	{
		if (!GUI.enabled) return;
		var lastId = GetLastControlId();

		if (!keyCodesToHandle.Contains(Event.current.keyCode) || Event.current.type == EventType.Layout) return;

		if (GUIUtility.keyboardControl != lastId) return;

		var eventType = Event.current.GetTypeForControl(GUIUtility.keyboardControl);
		if (eventType == EventType.KeyUp) currentUpDownSpeedFactor = 0;
		if (eventType == EventType.KeyDown || eventType == EventType.Used) callback?.Invoke(Event.current.keyCode);
		Event.current.Use();
	}

	/// <summary>
	/// Should be called right after IntField if we want to be able to change its value by one using arrows on the keyboard
	/// </summary>
	/// <param name="startValue"></param>
	/// <returns>starrtValue, modified by one if there was a keyboard arrow presses</returns>
	public static int HandleKeyboardUpDown(int startValue)
	{
		KeyboardHandler(UpDownCodes, keyCode =>
		{
			var txtEditor = ActiveTextEditor;
			var dir = 1;
			if (keyCode == KeyCode.DownArrow) dir = -1;
			currentUpDownSpeedFactor++;
			startValue += dir * (currentUpDownSpeedFactor / UpDownSpeed + 1);
			GUI.changed = true;
			if (txtEditor == null) return;
			txtEditor.text = "" + startValue;
			txtEditor.SelectAll();
		});
		return startValue;
	}

	/// <summary>
	/// Should be called right after FloatField if we want to be able to change its value by one using arrows on the keyboard
	/// </summary>
	/// <param name="startValue"></param>
	/// <returns>starrtValue, modified by one if there was a keyboard arrow presses</returns>
	public static float HandleKeyboardUpDown(float startValue)
	{
		KeyboardHandler(UpDownCodes, keyCode =>
		{
			var txtEditor = ActiveTextEditor;

			var dir = 1f;
			if (keyCode == KeyCode.DownArrow) dir = -1f;
			currentUpDownSpeedFactor++;
			startValue += dir * (currentUpDownSpeedFactor / UpDownSpeed + 1);
			startValue = (float)System.Math.Round(startValue, 2);

			GUI.changed = true;
			if (txtEditor == null) return;
			txtEditor.text = "" + startValue;
			txtEditor.SelectAll();
		});
		return startValue;
	}

	/// <summary>
	/// Should be called right after drawing a scene handler if we want to be able to change its position using arrows on the keyboard
	/// </summary>
	/// <param name="baseXSpeed"></param>
	/// <param name="baseYSpeed"></param>
	/// <returns>position, modified if there was a keyboard arrow press</returns>
	public static Vector3 HandleKeyboardArrows(Vector3 position, float baseXSpeed, float baseYSpeed)
	{
		KeyboardHandler(ArrowCodes, keyCode =>
		{
			var ydir = 0f;
			var xdir = 0f;
			switch (keyCode)
			{
				case KeyCode.LeftArrow: xdir = -1f; break;
				case KeyCode.RightArrow: xdir = 1f; break;
				case KeyCode.UpArrow: ydir = -1f; break;
				case KeyCode.DownArrow: ydir = 1f; break;
			}
			currentUpDownSpeedFactor++;
			var acceleration = currentUpDownSpeedFactor / UpDownSpeed + 1;
			position.x += xdir * baseXSpeed * acceleration;
			position.y += ydir * baseYSpeed * acceleration;
		});
		return position;
	}

	private static int GetLastControlId()
	{
		var lastControlIdField = typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);
		if (lastControlIdField != null) return (int)lastControlIdField.GetValue(null);

		Debug.LogError("Compatibility with Unity broke: can't find s_LastControlID field in EditorGUIUtility");
		return 0;
	}
}