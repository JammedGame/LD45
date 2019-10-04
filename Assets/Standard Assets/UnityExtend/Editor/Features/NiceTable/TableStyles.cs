using UnityEditor;
using UnityEngine;

public static class TableStyles
{
	public const float HorizontalPadding = 10;
	public const float RowHeight = 17;
	public const float RowPadding = 3;
	public static readonly Color StripeColor = new Color(0.4f, 0.4f, 0.4f, 0.1f);

	private static GUIStyle centeredLabel;
    public static GUIStyle CenteredLabel
    {
        get
        {
            if (centeredLabel == null)
            {
                centeredLabel = new GUIStyle(EditorStyles.label);
                centeredLabel.alignment = TextAnchor.MiddleCenter;
            }
            return centeredLabel;
        }
    }

	/// <summary>
	/// Gets default width for table stuff.
	/// </summary>
	public static float GetDefaultWidth(this SerializedProperty property, GUIStyle style = null)
	{
		float width = 120f;
		switch (property.propertyType)
		{
			case SerializedPropertyType.Integer: width = 80; break;
			case SerializedPropertyType.Float: width = 80; break;
			case SerializedPropertyType.Boolean: width = 40; break;
			case SerializedPropertyType.ObjectReference: width = 250; break;
			case SerializedPropertyType.String: width = 250; break;
			case SerializedPropertyType.Vector2: width = 150; break;
			case SerializedPropertyType.Vector3: width = 225; break;
			case SerializedPropertyType.Vector4: width = 240; break;
		}

		if (style != null)
		{
			var minWidth = style.CalcSize(new GUIContent(property.displayName)).x + 18f;
			if (minWidth > width) { return minWidth; }
		}

		return width;
	}}