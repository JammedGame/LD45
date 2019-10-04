using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;
using GameConsole;

public static class FindReferencesToThisTool
{
	public static List<string> GetDependenciesToAsset(string assetPath)
	{
		var guid = AssetDatabase.AssetPathToGUID(assetPath);
		return GetDependenciesToGUID(guid);
	}

	public static List<string> GetDependenciesToGUID(string guid)
	{
		var allAssets = AssetDatabase.GetAllAssetPaths();
		var dependencies = new List<string>();

		var i = 0;
		var count = (float)allAssets.Length;
		foreach (var path in allAssets)
		{
			if (i++ % 100 == 0)
				EditorUtility.DisplayProgressBar("Finding Dependencies", path, i / count);

			if (!path.CanReference()) continue;

			try
			{
				var found = File.ReadAllText(path).IndexOf(guid, System.StringComparison.Ordinal) != -1;
				if (found) { dependencies.Add(path); }
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}
		EditorUtility.ClearProgressBar();
		return dependencies;
	}

	[MenuItem("Assets/Tools/Find References To This", false, 100)]
	private static void OpenViewer(MenuCommand menuCommand)
	{
		string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		var dependencies = GetDependenciesToAsset(assetPath);

		if (dependencies.Count == 0) { Debug.Log("No dependencies found"); }
		CheckListWindow.Show(dependencies);
	}

	[ExecutableCommand]
	public static string FindPathForGUID(string guid)
	{
		return AssetDatabase.GUIDToAssetPath(guid);
	}

	[ExecutableCommand]
	public static string FindReferencesToGUID(string guid)
	{
		var dependencies = GetDependenciesToGUID(guid);
		if (dependencies.Count == 0) { return "No dependencies found"; }
		CheckListWindow.Show(dependencies);
		return dependencies.Count + " dependencies found";
	}

	[ExecutableCommand]
	public static string FindReferencesToAsset(string path)
	{
		var dependencies = GetDependenciesToAsset(path);
		if (dependencies.Count == 0) { return "No dependencies found"; }
		CheckListWindow.Show(dependencies);
		return dependencies.Count + " dependencies found";
	}

	public static bool CanReference(this string path)
	{
		return path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase)
			|| path.EndsWith(".controller", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase);
	}
}