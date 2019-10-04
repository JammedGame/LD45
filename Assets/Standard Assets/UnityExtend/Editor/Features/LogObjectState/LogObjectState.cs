using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameConsole;

public static class GameObjectCommands
{
	#region Game object manipulation

	[ExecutableCommand]
	public static string CreateGameObject(string gameObjectName)
	{
		new GameObject(gameObjectName);
		return gameObjectName;
	}

	[ExecutableCommand]
	[CommandSuggestion(0, typeof(ListAllGameObjects))]
	public static string MoveGameObject(string gameObjectName, float x, float y, float z)
	{
		var go = GameObject.Find(gameObjectName);
		if (go == null) return "ERROR: Game object by the name " + gameObjectName + " not found.";

		go.transform.position = new Vector3(x, y, z);

		return "";
	}

	[ExecutableCommand]
	[CommandSuggestion(0, typeof(ListAllGameObjects))]
	public static string DumpGameObject(string gameObjectName)
	{
		var go = GameObject.Find(gameObjectName);
		if (go == null) return "ERROR: Game object by the name " + gameObjectName + " not found.";

		var appender = new StringBuilder();
		appender.AppendLine("=============================");
		appender.AppendLine(go.name);

		LogObject(go, appender);
		foreach (var component in go.GetComponents<Component>()) { LogObject(component, appender); }

		return appender.ToString();
	}

	public static void LogObject(object component, StringBuilder appender)
	{
		appender.AppendLine("--------------");
		appender.AppendLine(component.GetType().Name);

		foreach (var member in GetAllFieldsAndProperties(component.GetType(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			try
			{
				var fieldInfo = member as FieldInfo;
				if (fieldInfo != null) appender.AppendLine(member.Name + ": " + fieldInfo.GetValue(component));

				var property = member as PropertyInfo;
				if (property != null && property.GetIndexParameters().Length == 0 &&
				    property.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0)
				{
					appender.AppendLine(member.Name + ": " + property.GetValue(component, null));
				}
			}
			catch (Exception)
			{
				appender.AppendLine(member.Name + " [failed to read value]");
			}
		}

		var renderer = component as Renderer;
		if (renderer != null) { LogObject(renderer.sharedMaterial, appender); }
	}

	public static string LogObject(object component)
	{
		if (component == null) return "null";

		var sb = new StringBuilder();
		LogObject(component, sb);
		return sb.ToString();
	}

	private static List<MemberInfo> GetAllFieldsAndProperties(Type type, BindingFlags flags)
	{
		var all = new List<MemberInfo>();
		all.AddRange(type.GetFields(flags));
		all.AddRange(type.GetProperties(flags));
		return all;
	}

	public static Type GetTypeFromClassName(string className)
	{
		Type t = null;
		foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			t = asm.GetType(className);
			if (t != null) break;
		}

		return t;
	}

	#endregion
}

public static class ListAllGameObjects
{
	public static List<string> GetSuggestions()
	{
		List<string> results = new List<string>();
		var allGameObjects = GameObject.FindObjectsOfType<GameObject>();

		foreach (var go in allGameObjects)
		{
			results.Add(go.GetPath());
		}

		return results;
	}

	public static string GetPath(this GameObject current)
	{
		return current.transform.GetPath();
	}

	public static string GetPath(this Transform current)
	{
		if (current.parent == null) return "/" + current.name;
		return current.parent.GetPath() + "/" + current.name;
	}
}
