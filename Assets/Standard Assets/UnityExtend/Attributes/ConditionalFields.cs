using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ConditionalFieldAttribute : PropertyAttribute
{
    public enum ComparisonType
    {
        ShowIfEqual,
        HideIfEqual
    }

    public ComparisonType CompareMethod { get; protected set; }

	public readonly string PropertyToCheck;
	public readonly object[] CompareValue;

	public ConditionalFieldAttribute(string propertyToCheck, params object[] compareValue)
	{
		PropertyToCheck = propertyToCheck;
		CompareValue = compareValue;
	}

#if UNITY_EDITOR
	public bool ShouldShow(SerializedProperty property)
	{
		return IsMatch(property) == (CompareMethod == ComparisonType.ShowIfEqual);
	}

	public bool IsMatch(SerializedProperty property)
	{
		if (string.IsNullOrEmpty(PropertyToCheck)) { return true; }

		var conditionProperty = FindPropertyRelative(property, PropertyToCheck);
		if (conditionProperty == null) { return true; }

		if (conditionProperty.propertyType == SerializedPropertyType.Boolean)
		{
			var triggerOnFalse = Array.Exists(CompareValue, x => x.ToString().ToUpper() == "FALSE");
			return triggerOnFalse != conditionProperty.boolValue;
		}

		if (CompareValue != null && CompareValue.Length > 0)
		{
			string conditionPropertyStringValue = AsStringValue(conditionProperty).ToUpper();
			return Array.Exists(CompareValue, x => x.ToString().ToUpper() == conditionPropertyStringValue);
		}

		return true;
	}

	private SerializedProperty FindPropertyRelative(SerializedProperty property, string toGet)
	{
		if (property.depth == 0) return property.serializedObject.FindProperty(toGet);

		var path = property.propertyPath.Replace(".Array.data[", "[");
		var elements = path.Split('.');
		SerializedProperty parent = null;

		for (int i = 0; i < elements.Length - 1; i++)
		{
			var element = elements[i];
			int index = -1;
			if (element.Contains("["))
			{
				index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
				element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
			}

			parent = i == 0 ?
				property.serializedObject.FindProperty(element) :
				parent.FindPropertyRelative(element);

			if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
		}

		return parent.FindPropertyRelative(toGet);
	}

	/// <summary>
	/// Get string representation of serialized property, even for non-string fields
	/// </summary>
	public static string AsStringValue(SerializedProperty property)
	{
		switch (property.propertyType)
		{
			case SerializedPropertyType.String:
				return property.stringValue;

			case SerializedPropertyType.Character:
			case SerializedPropertyType.Integer:
				if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
				return property.intValue.ToString();

			case SerializedPropertyType.ObjectReference:
				return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

			case SerializedPropertyType.Boolean:
				return property.boolValue.ToString();

			case SerializedPropertyType.Enum:
				var index = property.enumValueIndex;
				var names = property.enumNames;
				if (index < 0 || index > names.Length) { return ""; }
				return names[index];

			default:
				return string.Empty;
		}
	}
#endif
}

[AttributeUsage(AttributeTargets.Field)]
public class ShowIfAttribute : ConditionalFieldAttribute
{
	public ShowIfAttribute(string propertyToCheck, params object[] compareValue) : base(propertyToCheck, compareValue) {}
}

[AttributeUsage(AttributeTargets.Field)]
public class HideIfAttribute : ConditionalFieldAttribute
{
	public HideIfAttribute(string propertyToCheck, params object[] compareValue) : base(propertyToCheck, compareValue)
    {
        CompareMethod = ComparisonType.HideIfEqual;
    }
}