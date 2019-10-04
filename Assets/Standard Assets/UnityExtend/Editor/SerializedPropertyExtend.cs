using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SerializedPropertyExtend
{
	/// <summary>
	/// Wrapped serialized property.
	/// </summary>
	public readonly SerializedProperty SerializedProperty;

	/// <summary>
	/// SerializedProperty.propertyPath value is used during parsing, cached to avoid calling it later.
	/// </summary>
	public readonly string SerializedPropertyPath;

	/// <summary>
	/// Type of the target object.
	/// </summary>
	public readonly Type FieldType;

	/// <summary>
	/// FieldInfo related to this serializedProperty.
	/// </summary>
	public readonly FieldInfo FieldInfo;

	/// <summary>
	/// Pure Value of serialized property.
	/// </summary>
	public readonly object Value;

	/// <summary>
	/// List containg target Value, null if not a list/array element.
	/// </summary>
	public readonly IList List;

	/// <summary>
	/// Indent level of the serialized property in default inspector.
	/// </summary>
	public readonly int IndentLevel;

	/// <summary>
	/// Does it have flatten attribute?
	/// </summary>
	public readonly bool IsFlatten;

	/// <summary>
	/// Constructor does the heavy lifting.
	/// </summary>
	public SerializedPropertyExtend(SerializedProperty property)
	{
		SerializedProperty = property;
		SerializedPropertyPath = property.propertyPath;

		// prepare parsing data.
		FieldInfo field = null;
		System.Object targetObj = property.serializedObject.targetObject;
		System.Type objType = targetObj?.GetType();
		IndentLevel = -1;

		// iterates nodes in property path
		bool deferredIndent = false;
		foreach(var node in SerializedPropertyPath.Split('.'))
		{
			if (deferredIndent)
			{
				IndentLevel++;
				deferredIndent = false;
			}

			var newField = RecursiveFieldSearch.Search(objType, node);
			if (newField == null)
			{
				// is this an array?
				if (node == "Array")
				{
					List = targetObj as IList;
					IndentLevel++;
				}
				// get list element if needed.
				else if (List != null && node.StartsWith("data[", StringComparison.Ordinal) && node.EndsWith("]", StringComparison.Ordinal))
				{
					// get string inside data[xxx]
					var indexString = node.Substring
					(
						startIndex: "data[".Length,
						length: node.Length - "data[]".Length
					);

					if (Int32.TryParse(indexString, out int index) && index >= 0 && index < List.Count)
					{
						targetObj = List[index];
						objType = List.GetType().GetGenericArguments().FirstOrDefault();
						deferredIndent = true;
					}
				}

				continue;
			}
			else
			{
				field = newField;
			}

			targetObj = field.GetValue(targetObj);
			objType = field.FieldType;

			// update indent
			IndentLevel++;
			IndentLevel += IndentAttributeCache.GetIndent(field);
		}

		// finalize data acquired through parsing.
		Value = targetObj;
		FieldInfo = field;
		FieldType = objType;

		// cap in case there were no fields.
		IndentLevel = Mathf.Max(IndentLevel, 0);
	}

	/// <summary>
	/// SerializedPropertyExtend contains SerializedProperty.
	/// </summary>
	public static implicit operator SerializedProperty(SerializedPropertyExtend lhs) => lhs?.SerializedProperty;

	/// <summary>
	/// Logs value and field info.
	/// </summary>
	public override string ToString() => $"{FieldInfo}, value = {Value}";

	/// <summary>
	/// Indent attribute check is cached to avoid reflection.
	/// </summary>
	public static class IndentAttributeCache
	{
		static readonly Dictionary<FieldInfo, int> indentAttributeCache = new Dictionary<FieldInfo, int>();

		public static int GetIndent(FieldInfo field)
		{
			if (!indentAttributeCache.TryGetValue(field, out int isIndented))
			{
				isIndented += field.IsDefined(typeof(IndentAttribute)) ? 1 : 0;
				//isIndented += field.IsDefined(typeof(FlattenAttribute)) ? -1 : 0;
				indentAttributeCache.Add(field, isIndented);
			}
			return isIndented;
		}
	}

	/// <summary>
	/// Field search uses a lot of recursion, cached by (Type, fieldName).
	/// </summary>
	public static class RecursiveFieldSearch
	{
		static readonly Dictionary<(Type, string), FieldInfo> recursiveFieldSearchCache = new Dictionary<(Type, string), FieldInfo>();

		/// <summary>
		/// Get field by searching through type hierarchy.
		/// </summary>
		public static FieldInfo Search(Type type, string fieldName)
		{
			if (type == null)
			{
				return null;
			}

			var key = (type, fieldName);
			if (!recursiveFieldSearchCache.TryGetValue(key, out FieldInfo field))
			{
				field = SearchImpl(type, fieldName);
				recursiveFieldSearchCache.Add(key, field);
			}
			return field;
		}

		static FieldInfo SearchImpl(Type type, string fieldName)
		{
			if (type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) is FieldInfo field)
			{
				return field;
			}
			else if (type.BaseType is Type baseType)
			{
				return SearchImpl(baseType, fieldName);
			}
			else
			{
				return null;
			}
		}
	}
}

public static class SerializedPropertyExtendUtil
{
	public static SerializedPropertyExtend Parse(this SerializedProperty prop)
	{
		if (prop?.serializedObject?.targetObject != null)
			return new SerializedPropertyExtend(prop);
		else
			return null;
	}
}