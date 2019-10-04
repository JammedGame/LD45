using GameConsole;
using UnityEditor;

public static class RequestRecompile
{
	[ExecutableCommand]
	[MenuItem("Assets/Recompile scripts")]
	public static void RecompileScripts()
	{
		foreach (var path in AssetDatabase.GetAllAssetPaths())
		{
			if (path.StartsWith("Assets") && path.EndsWith(".cs"))
			{
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				break;
			}
		}
	}
}