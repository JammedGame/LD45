using GameConsole;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

public static class EmptyFolderCleaner
{
	[ExecutableCommand]
	public static void DeleteEmptyFolders()
	{
		SeekAndDestroyEmptyFolders("Assets/");
	}

	/// <summary>
	/// Seeks and destroys empty folders. It will consider also the meta files of the empty folders and remove
	/// them as well.
	/// </summary>
	/// <param name="root">The root from where to search</param>
	public static void SeekAndDestroyEmptyFolders(string root)
	{
		// one call would not acount for empty folders that themselves contain empty folders,
		// so call this recursively until no more folders have been found.
		bool foldersDeleted = false;

		try
		{
			// First get all directories
			string[] directoryList = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);

			// Now format the them since they can be in any format
			for (int i = 0; i < directoryList.Length; i++) directoryList[i] = FormatPath(directoryList[i]);

			// Now all paths have a single forward slash format, we will sort them now
			// according to their depth, so we can remove them in that order.
			System.Array.Sort(directoryList, (x, y) =>
			{
				int depthX = GetPathDepth(x);
				int depthY = GetPathDepth(y);

				if (depthX == depthY) return 0;
				else if (depthX < depthY) return -1;
				else return 1;
			});

			// The directories of the highest depths are now in the beginning of the list
			for (int i = 0; i < directoryList.Length; i++)
			{
				var files = Directory.GetFiles(directoryList[i]);
				if (files.Length == 0 || (files.Length == 1 && files[0].EndsWith(".DS_Store")))
				{
					try
					{
						if (files.Length == 1) File.Delete(files[0]);
						Directory.Delete(directoryList[i]);
						File.Delete(directoryList[i] + ".meta");
						foldersDeleted = true;
					}
					catch (System.Exception) { }
				}
			}

			// Check if we should remove the root directory (it is not included into the list)
			if (Directory.GetFiles(root).Length == 0)
			{
				Directory.Delete(root);
				File.Delete(root + ".meta");
			}
		}
		catch (System.Exception)
		{
			UnityEngine.Debug.LogError("Unable to remove empty files from root folder: " + root + ". Check if the path is valid");
		}

		// recurisve call.
		if (foldersDeleted)
		{
			SeekAndDestroyEmptyFolders(root);
		}
		else
		{
			// We should refresh the database
			AssetDatabase.Refresh();
		}
	}

	/// <summary>
	/// Gets the depth of the file or folder. To use this method the path should
	/// be formatted first. It will only count the number of forward slashes which
	/// is the depth of the formatted path.
	/// </summary>
	static int GetPathDepth(string path)
	{
		int depth = 0;
		for (int i = 0; i < path.Length; i++)
		{
			if (path[i] == Path.DirectorySeparatorChar) depth++;
		}
		return depth;
	}

	/// <summary>
	/// Formats the path. It will replace all backslashes with forward slashes and remove
	/// any duplicate slash.
	/// </summary>
	static string FormatPath(string path)
	{
		path = MakeForwardSlash(path);
		return Regex.Replace(path, @"([/])\1+", "/");
	}

	/// <summary>
	/// Replaces all backslash characters in the path with forward slash
	/// </summary>
	static string MakeForwardSlash(string path)
	{
		return path.Replace("\\", "/");
	}
}