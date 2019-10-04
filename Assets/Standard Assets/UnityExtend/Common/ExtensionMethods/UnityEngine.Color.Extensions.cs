using UnityEngine;

public static class ColorExtensions
{
	public static Color Darken(this Color color, float value)
	{
		Color.RGBToHSV(color, out float h, out float s, out float v);
		v *= value;
		var colorDarkened = Color.HSVToRGB(h, s, v);
		colorDarkened.a = color.a;
		return colorDarkened;
	}
}