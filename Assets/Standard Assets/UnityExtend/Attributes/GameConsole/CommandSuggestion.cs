using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System;

namespace GameConsole
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class CommandSuggestionAttribute : Attribute
	{
	    readonly MethodInfo suggestionInfo;
		public int Argument { get; private set; }

		public CommandSuggestionAttribute(int argument, Type type)
		{
			suggestionInfo = type.GetMethod("GetSuggestions");
			Argument = argument;
		}

		public List<string> GetSuggestions()
		{
			return suggestionInfo.Invoke(null, BindingFlags.Static, null, null, CultureInfo.CurrentCulture) as List<string>;
		}
	}
}