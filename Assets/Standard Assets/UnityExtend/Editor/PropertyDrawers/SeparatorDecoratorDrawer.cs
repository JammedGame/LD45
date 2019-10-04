using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDecoratorDrawer : DecoratorDrawer
{
	public override void OnGUI(Rect position)
	{
		EditorGUIPlus.DrawSeparator(position);
	}

	public override float GetHeight() => 30f;
}