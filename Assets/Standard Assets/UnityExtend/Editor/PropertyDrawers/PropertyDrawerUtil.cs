using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class PropertyDrawerUtil
{
	/// <summary>
	/// Gets attribute linked to this property.
	/// </summary>
	public static IList GetArray(this SerializedProperty property)
	{
		object target = property.serializedObject.targetObject;

		IList list = null;
		object value = target;
		foreach (var node in property.ParsePath())
		{
			try
			{
				value = node.GetValue(value, out list);
			}
			catch(System.Exception e)
			{
				Debug.LogError("Error while accessing: " + node);
				Debug.LogException(e);
				return null;
			}
		}

		// check if this is maybe the main return value.
		return list;
	}

	/// <summary>
	/// Turns property path into a list of nodes.
	/// </summary>
	public static IEnumerable<Node> ParsePath(this SerializedProperty property)
	{
		foreach(var n in property.propertyPath.Replace(".Array.data[", "[").Split('.'))
		{
			yield return new Node(n);
		}
	}

	/// <summary>
	/// Node of SerializedProperty path. Represetns one nested field or array element.
	/// </summary>
	public class Node
	{
		public readonly string fieldName;
		public readonly int arrayIndex;

		public Node(string path)
		{
			fieldName = path;

			// check if node represents list element
			arrayIndex = -1;
			var arrayIndexStart = path.IndexOf('[');
			if (arrayIndexStart != -1)
			{
				arrayIndex = Int32.Parse(path.Substring(arrayIndexStart + 1, path.Length - arrayIndexStart - 2));
				fieldName = fieldName.Substring(0, arrayIndexStart);
			}
		}

		public object GetValue(object value, out IList list)
		{
			if (arrayIndex == -1)
			{
				list = null;
				return GetField(value.GetType(), fieldName).GetValue(value);
			}
			else
			{
				list = GetField(value.GetType(), fieldName).GetValue(value) as IList;
				return list != null && list.Count > arrayIndex && arrayIndex >= 0 ? list[arrayIndex] : value;
			}
		}

		public static FieldInfo GetField(Type type, string fieldName)
		{
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (field != null) { return field; }
			if (type.BaseType != null) { return GetField(type.BaseType, fieldName); }
			return null;
		}

		public override string ToString()
		{
			return fieldName + " " + arrayIndex;
		}
	}
}