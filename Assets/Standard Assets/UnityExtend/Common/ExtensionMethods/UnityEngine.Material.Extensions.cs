using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialExtensions
{
	public static void SetKeyword(this Material mat, string name, bool value)
	{
		if (value)
		{
			mat.EnableKeyword(name);
		}
		else
		{
			mat.DisableKeyword(name);
		}
	}
}