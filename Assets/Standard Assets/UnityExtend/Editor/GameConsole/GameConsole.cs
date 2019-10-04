using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Text;
using System;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace GameConsole
{
	public class GameConsole : EditorWindow
	{
		#region Inner structures

		public struct AutocompleteOption
		{
			public string Option;
			public string Shown;
			public Color Color;

			public AutocompleteOption(string option)
			{
				Option = option;
				Shown = option;

				Color = Color.white;
			}

			public AutocompleteOption(string option, string shown)
			{
				Option = option;
				Shown = shown;

				Color = Color.white;
			}
		}

		#endregion
		#region Static & consts

		[MenuItem("Window/Game Console &c")]
		public static void GetWindow()
		{
			var window = GetWindow<GameConsole>();
			window.titleContent = new GUIContent("Game Console");
			window.Focus();
			window.focusInput = true;
		}

		#endregion
		#region Fileds

		const int autocompleteLinesHeight = 10;

		string currentCommand = "";
		Vector2 scrollView = Vector2.zero;
		Vector2 autocompleteScroll = Vector2.zero;

		// History and output
		[SerializeField] List<string> commandHistory = new List<string>();

		[SerializeField] List<string> outputList = new List<string>();

		// Autocomplete
		readonly List<AutocompleteOption> options = new List<AutocompleteOption>();
		//private Dictionary<ICommand, List<CommandSuggestionAttribute>> suggestions = new Dictionary<ICommand, List<CommandSuggestionAttribute>>();
		readonly Dictionary<int, List<string>> cachedSuggestions = new Dictionary<int, List<string>>();

		// Drop down selection
		int selectionInHistory = -1;

		int selectionInDropDown = -1;
		bool showDropDown = true;

		bool ShowDropDown { get { return showDropDown && currentCommand.Length > 0; } }

		bool focusInput;
		int currentArg = -1;

		#endregion
		#region Unity methods

		public void OnGUI()
		{
			GUI.color = Color.white;

			HandleInput();

			DrawInput();
			DrawOutput();

			DrawAutocomplete();
		}

		#endregion
		#region Drawing

		public void DrawInput()
		{
			EditorGUI.BeginChangeCheck();

			GUI.SetNextControlName("Input");
			currentCommand = EditorGUILayout.TextField(currentCommand);
			GUI.SetNextControlName("Random");

			if (focusInput)
			{
				GUI.FocusControl("Input");
				focusInput = false;
			}

			if (EditorGUI.EndChangeCheck())
			{
				ContemplateOptions();
			}
		}

		public void DrawOutput()
		{
			scrollView = GUILayout.BeginScrollView(scrollView);
			for (int i = 0; i < outputList.Count; i++)
			{
				if (outputList[i].StartsWith("ERROR:")) GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
				if (outputList[i].StartsWith("WARNING: ")) GUI.color = Color.yellow;

				if (GUILayout.Button(outputList[i], "Label"))
				{
					currentCommand = outputList[i];
					focusInput = true;
				}

				GUI.color = Color.white;
			}

			GUILayout.EndScrollView();
		}

		#endregion
		#region Auto complete

		public void DrawAutocomplete()
		{
			if (!ShowDropDown) return;

			// Drawing
			const float height = autocompleteLinesHeight * 16;
			autocompleteScroll = GUI.BeginScrollView(new Rect(8, 16, position.width - 8, height),
				autocompleteScroll, new Rect(0, 0, position.width - 32, options.Count * 16));

			GUI.Box(new Rect(0, 0, position.width - 16, options.Count * 16), "", "TextField");
			for (int i = 0; i < options.Count; i++)
			{
				if (i == selectionInDropDown)
				{
					GUI.color = new Color(0.3f, 0.75f, 1, 1);
					GUI.Box(new Rect(0, i * 16f, position.width - 16, 16), "", "TextField");
					GUI.color = Color.white;
				}

				GUI.color = options[i].Color;
				if (GUI.Button(new Rect(0, i * 16f, position.width - 16, 16), options[i].Shown, "Label"))
				{
					selectionInDropDown = i;
				}
				GUI.color = Color.white;
			}
			GUI.EndScrollView();
		}

		public void ContemplateOptions()
		{
			CommandExecuter.ParseCommand(currentCommand, out string commandName, out string[] args);

			options.Clear();

			ICommand command = CommandExecuter.FindCommand(x => x != null && x.Name == commandName);
			if (command != null && (args.Length > 0 || currentCommand.EndsWith(" ")))
			{
				currentArg = Mathf.Max(0, args.Length - 1);
				string currentText = "";
				if (currentArg < args.Length)
				{
					currentText = args[currentArg];
				}

				if (currentCommand.EndsWith(" ") && args.Length > 0)
				{
					currentArg += 1;
					currentText = "";
				}

				if (!cachedSuggestions.TryGetValue(currentArg, out List<string> suggestions))
				{
					suggestions = command.GetSuggestions(currentArg);
					cachedSuggestions.Add(currentArg, suggestions);
				}
				else suggestions = cachedSuggestions[currentArg];

				foreach (string s in suggestions)
				{
					if (s.Contains(currentText) || currentText == "") options.Add(new AutocompleteOption(s));
				}
			}
			else
			{
				currentArg = -1;
				cachedSuggestions.Clear();

				// Show commands autocomplete
				foreach (var c in CommandExecuter.CommandListReadOnly)
				{
					if (c != null && c.Name.Contains(commandName))
					{
						StringBuilder additional = new StringBuilder(c.Name);
						foreach (var arg in c.ParamInfo)
						{
							if (arg == null) additional.Append(" unknown");
							else additional.Append(" ").Append(arg.ParameterName);
						}
						var o = new AutocompleteOption(c.Name, additional.ToString());

						options.Add(o);
					}
				}
			}

			selectionInDropDown = -1;
			selectionInHistory = -1;
			showDropDown = true;
		}

		#endregion
		#region Input

		public void ExecuteCommand(string command)
		{
			if (string.IsNullOrEmpty(command)) return;

			outputList.Add(command);
			commandHistory.Insert(0, command);

			EditorInvoker.Invoke(() =>
			{
				try
				{
					object result = CommandExecuter.ExecuteCommandLine(command);
					string text = result == null ? "" : result.ToString();

					if (!string.IsNullOrEmpty(text)) outputList.Add(text);
				}
				catch (Exception e)
				{
					if (e is TargetParameterCountException)
					{
						outputList.Add($"ERROR: parameters do not match {command} command signature");
					}
					else
					{
						outputList.Add("ERROR: " + e.Message);
						Debug.LogException(e);
					}
				}

				Repaint();
			});
		}

		public void HandleInput()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				focusInput = true;

				if (Event.current.keyCode == KeyCode.Return)
				{
					if (selectionInDropDown >= 0 && options.Count > 0) FillCommand();
					ExecuteCommand(currentCommand);

					currentCommand = "";
					Repaint();

					Event.current.Use();
					focusInput = true;
				}
				else if (Event.current.keyCode == KeyCode.UpArrow || Event.current.keyCode == KeyCode.DownArrow)
				{
					int sign = Event.current.keyCode == KeyCode.UpArrow ? 1 : -1;

					if (!ShowDropDown)
					{
						if (commandHistory.Count > 0)
						{
							selectionInHistory = Mathf.Clamp(selectionInHistory + sign, 0, commandHistory.Count - 1);
							currentCommand = commandHistory[selectionInHistory];

							GUI.FocusControl("Random");
							focusInput = true;

							showDropDown = false;
						}
					}
					else
					{
						int count = options.Count;
						if (count >= 0)
						{
							selectionInDropDown = Mathf.FloorToInt(Mathf.Repeat(selectionInDropDown - sign, count));
							// Handle scroll
							float y = selectionInDropDown * 16f;
							const float space = (autocompleteLinesHeight - 1) * 16;

							if (y < autocompleteScroll.y) autocompleteScroll.y = y;
							if (y > autocompleteScroll.y + space) autocompleteScroll.y = y - space;
						}
					}

					Repaint();
					Event.current.Use();
				}
				else if (Event.current.keyCode == KeyCode.Tab)
				{
					if (ShowDropDown)
					{
						FillCommand();
						ContemplateOptions();

						GUI.FocusControl("Random");
						focusInput = true;
					}
					Event.current.Use();
				}
				else if (Event.current.keyCode == KeyCode.Escape)
				{
					showDropDown = false;
					Event.current.Use();

					Repaint();
				}
			}
		}

		public void FillCommand()
		{
			if (currentArg == -1)
			{
				if (selectionInDropDown > -1) currentCommand = options[selectionInDropDown].Option;
				else currentCommand = options[0].Option;
			}
			else
			{
				if (selectionInDropDown > -1) AutocompleteLastArgument(options[selectionInDropDown].Option);
			}
			MoveToLineEnd();
		}

		public void MoveToLineEnd()
		{
			// first frame, onFocus triggers SelectAll.
			// second frame we move the cursor and repaint.
			EditorApplication.update += MoveToLineEndImpl;

			void MoveToLineEndImpl()
			{
				(typeof(EditorGUI).GetField("s_RecycledEditor", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as TextEditor)?.MoveLineEnd();
				Repaint();

				EditorApplication.update -= MoveToLineEndImpl;
			}
		}

		public void AutocompleteLastArgument(string arg)
		{
			int insertIndex = currentCommand.Replace("\\ ", "\\\\").LastIndexOf(' ') + 1;
			currentCommand = currentCommand.Substring(0, insertIndex);
			currentCommand = currentCommand + arg;
		}

		#endregion
		#region Tool Commands

		[ExecutableCommand]
		public static void _clear()
		{
			var console = GetWindow<GameConsole>();
			console.outputList.Clear();
			console.Repaint();
		}

		[ExecutableCommand]
		public static string _help()
		{
			string result = "";
			foreach (var v in CommandExecuter.CommandListReadOnly)
			{
				result += v.Name + "\n";
			}

			return result;
		}

		#endregion
	}
}