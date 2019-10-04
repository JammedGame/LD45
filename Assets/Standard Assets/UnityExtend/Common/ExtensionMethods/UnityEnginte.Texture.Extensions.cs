using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.AssetImporters;
using System.Reflection;
using UnityEditor;
#endif

public static class TextureExtensions
{
	// ReSharper disable once CyclomaticComplexity
	public static bool HasAlpha(this TextureFormat format)
	{
		// ARGB or RGBA formats
		return format == TextureFormat.ARGB32 || format == TextureFormat.ARGB4444 || format == TextureFormat.RGBA32
				|| format == TextureFormat.RGBA4444 || format == TextureFormat.RGBAFloat || format == TextureFormat.RGBAHalf
				// ASTC formats
				|| format == TextureFormat.ASTC_RGBA_4x4 || format == TextureFormat.ASTC_RGBA_5x5 || format == TextureFormat.ASTC_RGBA_6x6
				|| format == TextureFormat.ASTC_RGBA_8x8 || format == TextureFormat.ASTC_RGBA_10x10 || format == TextureFormat.ASTC_RGBA_12x12
				// ETC formats
				|| format == TextureFormat.ETC2_RGBA1 || format == TextureFormat.ETC2_RGBA8
				// PVRTC formats
				|| format == TextureFormat.PVRTC_RGBA2 || format == TextureFormat.PVRTC_RGBA4
				// Other
				|| format == TextureFormat.Alpha8 || format == TextureFormat.DXT5
				|| format == TextureFormat.ETC2_RGBA8
				|| format == TextureFormat.BGRA32;
	}

#if UNITY_EDITOR
	public static SourceTextureInformation GetSourceTextureInformation(this Texture texture)
	{
		var path = AssetDatabase.GetAssetPath(texture);

		var importer = AssetImporter.GetAtPath(path);
		if (importer == null) { return null; }

		var methodInfo = importer.GetType().GetMethod("GetSourceTextureInformation", BindingFlags.Instance | BindingFlags.NonPublic);
		if (methodInfo == null) { return null; }

		return methodInfo.Invoke(importer, new object[] {}) as SourceTextureInformation;
	}
#endif
}
