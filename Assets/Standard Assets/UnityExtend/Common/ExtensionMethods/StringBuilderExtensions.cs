using System.Text;

namespace Nordeus.Util.CSharpLib
{
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Extension method that adds string.IndexOf functionality to StringBuilder.
		/// </summary>
		/// <returns>Same as string's version of function - returns zero-based index of char's first occurrence in string builder. Function returns -1 if char is not found.</returns>
		public static int IndexOf(this StringBuilder stringBuilder, char charToFind)
		{
			if (stringBuilder == null) { throw new System.NullReferenceException(); }

			for (int index = 0; index < stringBuilder.Length; index++)
			{
				if (stringBuilder[index] == charToFind) { return index; }
			}

			return -1;
		}

		/// <summary>
		/// StringBuilder extension method that act as string.Substring method.
		/// </summary>
		/// <param name="startIndex">The zero-based starting character position of a substring in stringBuilder instance.</param>
		/// <param name="length">The number of characters in the substring.</param>
		/// <returns>Created substring.</returns>
		public static string Substring(this StringBuilder stringBuilder, int startIndex, int length)
		{
			return stringBuilder.ToString(startIndex, length);
		}

		/// <summary>
		/// StringBuilder extension method that act as string.Substring method.
		/// </summary>
		/// <param name="startIndex">The zero-based starting character position of a substring in stringBuilder instance.</param>
		/// <returns>Created substring.</returns>
		public static string Substring(this StringBuilder stringBuilder, int startIndex)
		{
			return stringBuilder.ToString(startIndex, stringBuilder.Length - startIndex);
		}

		public static StringBuilder ReplaceFirstOccurence(this StringBuilder builder, string oldValue, string newValue)
		{
			if (builder == null)
			{
				return null;
			}
			int indexOfFirstOccurence = builder.ToString().IndexOf(oldValue);
			if (indexOfFirstOccurence != -1)
			{
				builder.Replace(oldValue, newValue, indexOfFirstOccurence, oldValue.Length);
			}
			return builder;
		}
	}
}
