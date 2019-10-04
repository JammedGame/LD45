using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class BitMaskPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		prop.intValue = (int)(object)EditorGUI.EnumFlagsField(position, label, (System.Enum)prop.Parse().Value);
		prop.serializedObject.ApplyModifiedProperties();
	}
}