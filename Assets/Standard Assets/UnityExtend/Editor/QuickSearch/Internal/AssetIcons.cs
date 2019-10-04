using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace QuickSearch {

	public static class InternalEditorUtility {

		static MethodInfo FindIconForFile;
		static object[] FindIconForFileArgs = new object[1];

		// Referenced from http://answers.unity3d.com/questions/792118/what-are-the-editor-resources-by-name-for-editorgu.html
		public static Texture2D GetIcon (string assetPath)
		{
			if (FindIconForFile == null)
			{
				FindIconForFile = Assembly.GetAssembly(typeof(UnityEditor.Editor)).
					GetType("UnityEditorInternal.InternalEditorUtility", true).
					GetMethod("GetIconForFile");
			}

			FindIconForFileArgs[0] = assetPath;
			return (Texture2D)FindIconForFile.Invoke(null, FindIconForFileArgs);
		}
	}
}
