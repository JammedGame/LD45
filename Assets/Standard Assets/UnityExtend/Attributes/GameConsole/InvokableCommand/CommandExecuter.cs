using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameConsole
{
	public static class CommandExecuter
	{
		// TODO: convert this to a dictionary
	    static readonly List<ICommand> commandList;

	    static List<ICommand> commandListReadOnly;
		static bool listNeedsUpdate;

		public static Func<string, ICommand> CustomCommandGetter;

		/// <summary> This returns the command list that should be used to read the command collection. This list is only
		/// a copy of the real list. This is intended to be used as a thread safe reading method. </summary>
		public static List<ICommand> CommandListReadOnly
		{
			get
			{
				if (listNeedsUpdate)
				{
					lock (commandList)
					{
						// We just make a new list, since the existing one might be in use
						commandListReadOnly = new List<ICommand>(commandList);
					}
				}

				return commandListReadOnly;
			}
		}

		static CommandExecuter()
		{
			commandList = GetAllCodeCommandsList();
			commandListReadOnly = new List<ICommand>(commandList);
		}

		/// <summary>
		/// Executes a command line where the command is first word, and the arguments are separated
		/// with whitespaces.
		/// </summary>
		public static object ExecuteCommandLine(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine)) return null;

			ParseCommand(commandLine, out string commandName, out string[] args);

			var command = GetCommand(commandName, args);

			return ExecuteCommand(command, args);
		}

		public static object ExecuteCommand(ICommand command, string[] args)
		{
			if (command == null) return null;
			object[] parsedArgs = ParseArgs(args, command);
			return command.Invoke(parsedArgs);
		}

		/// <summary> Returns the command with the given name </summary>
		public static ICommand GetCommand(string commandName, string[] args)
		{
			lock (commandList)
			{
				if (CustomCommandGetter == null)
				{
					var match = commandList.Find(x => x.Name == commandName && x.ParamInfo.Count == args.Length);
					return match ?? commandList.Find(x => x.Name == commandName);
				}
				return CustomCommandGetter(commandName);
			}
		}

		/// <summary> Finds a command that satisfies the condition </summary>
		public static ICommand FindCommand(Predicate<ICommand> condition)
		{
			lock (commandList)
			{
				return commandList.Find(condition);
			}
		}

		/// <summary> Adds a command to the executer if the command doesn't exist. </summary>
		public static void Add(ICommand command)
		{
			if (command == null) return;
			lock (commandList)
			{
				commandList.Add(command);
				listNeedsUpdate = true;
			}
		}

		/// <summary> Adds the command to the executer, even if it exists. </summary>
		public static void ForceAdd(ICommand command)
		{
			if (command == null) return;
			lock (commandList)
			{
				commandList.Add(command);

				listNeedsUpdate = true;
			}
		}

		/// <summary> Removes a command from the executer </summary>
		public static void Remove(ICommand command)
		{
			lock (commandList)
			{
				commandList.Remove(command);

				listNeedsUpdate = true;
			}
		}

		#region Static methods

		/// <summary>
		/// This method gets all commands from all loaded assemblies.
		/// </summary>
		/// <returns>List of all commands found.</returns>
		public static List<ICommand> GetAllCodeCommandsList()
		{
			List<ICommand> newList = new List<ICommand>();

			foreach (Assembly currentAssembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					var types = currentAssembly.GetTypes();
					for (int i = 0; i < types.Length; i++)
					{
						var type = types[i];
						var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						for (int j = 0; j < methods.Length; j++)
						{
							var method = methods[j];
							var attributes = method.GetCustomAttributes(false);

							if (Array.Find(attributes, x => x.GetType() == typeof(ExecutableCommandAttribute)) is ExecutableCommandAttribute a)
							{
								newList.Add(a.CreateInvokableCommand(method));
							}
						}
					}
				}
				catch(System.Exception e) { Debug.LogException(e); }
			}
			return newList;
		}

		/// <summary> Method that can parse a whole command line to find out the name of the command and its arguments </summary>
		public static void ParseCommand(string command, out string name, out string[] args)
		{
			List<string> split = new List<string>();

			string[] preSplit = command.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < preSplit.Length; i++)
			{
				if (i % 2 == 1) split.Add(preSplit[i]);
				else split.AddRange(preSplit[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
			}

			if (split.Count == 0)
			{
				name = "";
				args = new string[0];
			}
			else if (split.Count == 1)
			{
				name = split[0];
				args = new string[0];
			}
			else
			{
				name = split[0];
				args = new string[split.Count - 1];
				for (int i = 0; i < args.Length; i++) args[i] = split[i + 1];
			}
		}

		/// <summary> Parses a list of arguments into a list of objects of the required type. </summary>
		public static object[] ParseArgs(string[] args, ICommand command)
		{
			object[] result = new object[args.Length];

			for (int i = 0; i < args.Length; i++)
			{
				if (i < command.ParamInfo.Count)
				{
					result[i] = ParseArg(args[i], command.ParamInfo[i].ParameterType);
				}
				else result[i] = args[i];
			}

			return result;
		}

		public static object ParseArg(string arg, Type type)
		{
			return type.IsEnum ? Enum.Parse(type, arg) : Convert.ChangeType(arg, type);
		}

		#endregion
	}
}