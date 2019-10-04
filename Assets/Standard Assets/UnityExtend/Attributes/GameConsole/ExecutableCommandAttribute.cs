using System;
using System.Reflection;

namespace GameConsole
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ExecutableCommandAttribute : Attribute
	{
		public string Prefix = "";
		public Type CustomCommand = null;
		public bool ClassPrefix;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Attribute"/> class.
		/// </summary>
		public ExecutableCommandAttribute(string prefix = "")
		{
			Prefix = prefix;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Attribute"/> class.
		/// </summary>
		public ExecutableCommandAttribute(bool classPrefix)
		{
			ClassPrefix = classPrefix;
		}

		public InvokableCodeCommand CreateInvokableCommand(MethodInfo methodInfo)
		{
			if (ClassPrefix)
			{
				Prefix = methodInfo.DeclaringType.Name + ".";
			}

			InvokableCodeCommand result = null;
			try
			{
				if (CustomCommand == null)
				{
					result = new InvokableCodeCommand(methodInfo);
				}
				else
				{
					result = (InvokableCodeCommand) Activator.CreateInstance(CustomCommand, methodInfo);
				}

				result.NamePrefix = Prefix;
			}
			catch (Exception) {}
			return result;
		}
	}
}