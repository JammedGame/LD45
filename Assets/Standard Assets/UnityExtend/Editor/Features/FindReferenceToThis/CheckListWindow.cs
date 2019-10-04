using UnityEngine;
using System;
using UnityEditor;
using GameConsole;
using Object = UnityEngine.Object;

using System.Collections.Generic;

public class CheckListWindow : EditorWindow
{
	public List<string> files = new List<string>();
	public List<string> fileComments = new List<string>();
	public List<bool> checkboxes = new List<bool>();

	public List<ButtonInfo> buttons = new List<ButtonInfo>();

	public class ButtonInfo
	{
		public string button;
		public Action<Object> buttonAction;
	}

	public int selectionIndex = -1;

	private Vector2 scroll = Vector2.zero;

	[ExecutableCommand]
	public static string ClearCheckListWindow()
	{
		var window = GetWindow<CheckListWindow>();
		window.Clear();
		return "";
	}

	public static void Show(List<string> f, bool defaultState = true, List<string> comments = null)
	{
		var window = CreateInstance<CheckListWindow>();
		window.Show();
		window.Assign(f, defaultState, comments);		
	}

	public void Assign(List<string> f, bool defaultState = true, List<string> comments = null)
	{
		files.Clear();
		checkboxes.Clear();

		files.AddRange(f);
		for (int i = 0; i < f.Count; i++)
		{
			checkboxes.Add(defaultState);
			if (comments == null || i >= comments.Count) fileComments.Add("");
			else fileComments.Add(comments[i]);
		}
	}

	public void RemoveAt(int index)
	{
		if (index < 0 || index >= files.Count) return;
		files.RemoveAt(index);
		checkboxes.RemoveAt(index);
		fileComments.RemoveAt(index);

		if (selectionIndex >= index) selectionIndex--;
	}

	public void AddButton(string button, Action<Object> action)
	{
		buttons.Add (new ButtonInfo()
		{
			button = button, buttonAction = action
		});
	}
	
	public void Clear()
	{
		files.Clear();
		checkboxes.Clear();
		fileComments.Clear();

		selectionIndex = -1;
	}

	public void OnGUI()
	{
		using (new GUILayout.VerticalScope())
		{
			scroll = GUILayout.BeginScrollView(scroll);

			for (int i = 0; i < files.Count; i++)
			{
				if (i == selectionIndex) GUI.color = new Color(0.3f, 0.3f, 0.9f);
				else GUI.color = Color.white;

				using (new GUILayout.HorizontalScope("Box"))
				{
					GUI.color = Color.white;
					checkboxes[i] = EditorGUILayout.Toggle(checkboxes[i], GUILayout.Width(16));
					if (GUILayout.Button(files[i], "Label"))
					{
						Object clicked = AssetDatabase.LoadAssetAtPath<Object>(files[i]);
						Selection.activeObject = clicked;
						selectionIndex = i;
					}

					GUILayout.FlexibleSpace();
					if (i < fileComments.Count) GUILayout.Label(fileComments[i]);

					foreach (var button in buttons) 
					{
						if (GUILayout.Button(button.button))
						{
							Object clicked = AssetDatabase.LoadAssetAtPath<Object>(files[i]);
							button.buttonAction(clicked);
							UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
						}
					}

					if (GUILayout.Button("Remove"))
					{
						RemoveAt(i);
						i--;
					}
				}
			}

			GUILayout.EndScrollView();
		}
	}

	public List<string> GetCheckedList()
	{
		List<string> result = new List<string>();

		for (int i = 0; i < files.Count; i++)
		{
			if (checkboxes[i]) result.Add(files[i]);
		}

		return result;
	}
}