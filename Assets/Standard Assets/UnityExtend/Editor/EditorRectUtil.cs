using UnityEditor;
using UnityEngine;

public static class EditorRectUtil
{
	public static Rect TakeLine(float height = 18f)
	{
		return EditorGUILayout.GetControlRect(GUILayout.Height(height));
	}

	public static void Space(float height = 5f)
	{
		EditorGUILayout.GetControlRect(GUILayout.Height(height));
	}

	public static Rect TakeLine(this ref Rect rect, float height = 18f)
	{
		var newRect = new Rect(rect.x, rect.y, rect.width, height);
		rect.y += newRect.height;
		rect.height -= newRect.height;
		return newRect;
	}

	public static Rect Take(this ref Rect rect, float width)
	{
		var newRect = new Rect(rect.x, rect.y, width, rect.height);
		rect.x += width;
		rect.width -= width;
		return newRect;
	}

	public static Rect TakeThird(this ref Rect rect)
	{
		return rect.Take(rect.width / 3f);
	}

	public static Rect TakeEditorLabel(this ref Rect rect)
	{
		return rect.Take(EditorGUIUtility.labelWidth);
	}

	public static void DrawEditorLabel(this ref Rect rect, string label)
	{
		var labelRect = rect.Take(EditorGUIUtility.labelWidth);
		EditorGUI.LabelField(labelRect, label);
	}

	public static Rect TakeMiddle(this ref Rect rect, float middle)
	{
		rect.x += (rect.width - middle) * 0.5f;
		rect.width = middle;
		return rect;
	}

	public static Rect TakeRight(this ref Rect rect, float width)
	{
		var newRect = new Rect(rect.xMax - width, rect.y, width, rect.height);
		rect.width -= width;
		return newRect;
	}

	public static void Space(this ref Rect rect, float width)
	{
		rect.Take(width);
	}

	public static void VerticalSpace(this ref Rect rect, float height)
	{
		rect.TakeLine(height);
	}
}