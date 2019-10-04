using System;
using System.Reflection;

public static class ReflectionRecursive
{
	public static MethodInfo GetMethodRecursive(this Type type, string methodName, BindingFlags flags)
	{
		if (type.GetMethod(methodName, flags) is MethodInfo method)
		{
			return method;
		}
		else if (type.BaseType is Type baseType)
		{
			return baseType.GetMethodRecursive(methodName, flags);
		}
		else
		{
			return null;
		}
	}

	public static PropertyInfo GetPropertyRecursive(this Type type, string methodName, BindingFlags flags)
	{
		if (type.GetProperty(methodName, flags) is PropertyInfo property)
		{
			return property;
		}
		else if (type.BaseType is Type baseType)
		{
			return baseType.GetPropertyRecursive(methodName, flags);
		}
		else
		{
			return null;
		}
	}
}