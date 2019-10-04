using System;
using System.Collections.Generic;

namespace GameConsole
{
	/// <summary> Interface that is used to implement a command that can be executed in runtime </summary>
	public interface ICommand
	{
		/// <summary> The name of the command </summary>
		string Name { get; }

		/// <summary> Invokes the command with the given arguments </summary>
		//object Invoke(string[] args);

		/// <summary> Invokes the command with the given arguments </summary>
		object Invoke(object[] args);

		/// <summary> Info of all parameters contained in a list </summary>
		List<ParameterInfo> ParamInfo { get; }

		/// <summary> Returns the list of suggestions for an argument </summary>
		List<string> GetSuggestions(int argument);

		/// <summary> Returns true if the command has a result. TODO: maybe change this to a string, type and
		/// return all out parameters or something in the future...? </summary>
		bool HasResult { get; }
	}

	/// <summary> Container class containing information for a paramete of a command </summary>
	public class ParameterInfo
	{
		public readonly Type ParameterType;
		public readonly string ParameterName;
		public CommandSuggestionAttribute CommandSuggestions;

		public ParameterInfo() { }

		public ParameterInfo(Type type, string name, CommandSuggestionAttribute suggestions)
		{
			ParameterType = type;
			ParameterName = name;
			CommandSuggestions = suggestions;
		}
	}
}