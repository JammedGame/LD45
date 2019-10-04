using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace GameConsole
{
	/// <summary>
	/// This class represents a proxy for a method that can be invoked with
	/// given (string) parameters.
	/// </summary>
	public class InvokableCodeCommand : ICommand
	{
		public MethodInfo MethodInfo { get; private set; }

	    string prefix = "";
	    readonly string name;
	    string cachedName;

		public string NamePrefix
		{
			get { return prefix; }
			set
			{
				prefix = value;
				cachedName = prefix + name;
			}
		}

		public string Name => cachedName;

	    public List<ParameterInfo> ParamInfo { get; private set; }

		public bool HasResult => MethodInfo.ReturnType != typeof(void);

	    /// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public InvokableCodeCommand(MethodInfo methodInfo, string altName = null)
		{
			MethodInfo = methodInfo;

			// Resolve naming
			name = altName ?? methodInfo.Name;
			cachedName = prefix + name;

			ParamInfo = new List<ParameterInfo>();

			foreach (var parameter in methodInfo.GetParameters())
			{
				ParamInfo.Add(new ParameterInfo(parameter.ParameterType, parameter.Name, null));
			}

			object[] attributes = MethodInfo.GetCustomAttributes(typeof(CommandSuggestionAttribute), false);
			for (int i = 0; i < attributes.Length; i++)
			{
				if (!(attributes[i] is CommandSuggestionAttribute s)) continue;

				ParamInfo[s.Argument].CommandSuggestions = s;
			}
		}

		public List<string> GetSuggestions(int argument)
		{
			if (argument >= ParamInfo.Count || argument < 0) return new List<string>();

			var attribute = ParamInfo[argument].CommandSuggestions;
			if (attribute != null)
			{
				return attribute.GetSuggestions();
			}
			else if (ParamInfo[argument].ParameterType.IsEnum)
			{
				return new List<string>(System.Enum.GetNames(ParamInfo[argument].ParameterType));
			}

			return new List<string>();
		}

		/// <summary>
		/// Invokes the command with the given set of parameters
		/// </summary>
		public object Invoke(object[] args)
		{
			if (MethodInfo == null)
			{
				return "ERROR: Invalid command detected";
			}

			return MethodInfo.Invoke(null, BindingFlags.Public, null, args, CultureInfo.CurrentCulture);
		}
	}
}