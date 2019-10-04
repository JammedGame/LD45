using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SearchableDropDown
{
	/// <summary>
	/// Cache of the hash to use to resolve the ID for the drawer.
	/// </summary>
	private static readonly int idHash = "SearchableEnumAttributeDrawer".GetHashCode();

	public static void Show(Rect position, string label, IList<string> options, int current, Action<int> onSelectionMade)
	{
		Show(position, new GUIContent(label), options, current, onSelectionMade);
	}

	public static void Show(Rect position, GUIContent label, IList<string> options, int current, Action<int> onSelectionMade)
	{
		// array index.
		if (current >= options.Count || current <= 0)
		{
			current = 0;
		}

		int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);
		position = EditorGUI.PrefixLabel(position, id, label);

		if (DropdownButton(id, position, new GUIContent(options[current])))
		{
			SearchablePopup.Show(position, options, current, onSelectionMade);
		}
	}

	public static void Show(Rect position, GUIContent label, Func<IList<string>> optionsFunc, string current, Action<int> onSelectionMade)
	{
		// array index.
		int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);
		position = EditorGUI.PrefixLabel(position, id, label);

		if (DropdownButton(id, position, new GUIContent(current)))
		{
			var options = optionsFunc();
			SearchablePopup.Show(position, options, options.IndexOf(current), onSelectionMade);
		}
	}

	public static void Show(string label, string[] options, int current, Action<int> onSelectionMade)
	{
		try
		{
			Show(EditorGUILayout.GetControlRect(), label, options, current, onSelectionMade);
		}
		catch(Exception)
		{
			/* ignore weird editor errors */
			return;
		}
	}


	/// <summary>
	/// A custom button drawer that allows for a controlID so that we can
	/// sync the button ID and the label ID to allow for keyboard
	/// navigation like the built-in enum drawers.
	/// </summary>
	public static bool DropdownButton(int id, Rect position, GUIContent content)
	{
		Event current = Event.current;
		switch (current.type)
		{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition) && current.button == 0)
				{
					Event.current.Use();
					return true;
				}

				break;
			case EventType.KeyDown:
				if (GUIUtility.keyboardControl == id && current.character == '\n')
				{
					Event.current.Use();
					return true;
				}

				break;
			case EventType.Repaint:
				EditorStyles.popup.Draw(position, content, id, false);
				break;
		}

		return false;
	}
}