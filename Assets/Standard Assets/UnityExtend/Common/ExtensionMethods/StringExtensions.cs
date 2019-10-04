using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Nordeus.Util.CSharpLib
{
	/// <summary>
	/// Static class containing extension methods for string.
	/// </summary>
	public static class StringExtensions
	{
		#region Constants

		private static readonly string[] NewLineDelimiters = { "\r\n", "\n" };

		#endregion

		#region Methods

		/// <summary>
		/// Checks two strings for case-insensitive equality.
		/// </summary>
		/// <param name="firstString">First string to compare</param>
		/// <param name="secondString">Second string to compare</param>
		/// <returns>True if the strings are equal </returns>
		public static bool EqualsIgnoreCase(this string firstString, string secondString)
		{
			return string.Equals(firstString, secondString, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

		/// <summary>
		/// Is stirng multiline?
		/// </summary>
		public static bool IsMultiline(this string value)
		{
			if (value == null) return false;
			return value.IndexOf(Environment.NewLine) != -1;
		}

		/// <summary>
		/// Clip string so it has a maximum number of lines.
		/// Works by finding an nth occurance of end of line character and trims everything afterwards.
		/// </summary>
		/// <param name="value">String to be trimmed.</param>
		/// <param name="maxNumberOfLines">How much lines the return value will have at max.
		/// Doesn't change string if equal or less than 0.</param>
		public static string TrimLines(this string value, int maxNumberOfLines)
		{
			if (value == null)
			{
				return string.Empty;
			}

			if (maxNumberOfLines <= 0)
			{
				return value;
			}

			// find maxNumberOfLines-nth occurance of '\n' in the string.
			int trimIndex = -1;
			for (int i = 0; i < maxNumberOfLines; i++)
			{
				trimIndex = value.IndexOf('\n', trimIndex + 1);

				if (trimIndex == -1) break;
			}

			if (trimIndex >= 0)
			{
				return value.Substring(0, trimIndex);
			}

			return value;
		}

		/// <summary>
		/// Trims input string if it is not null. If null, <see cref="resultIfNull"/> is returned.
		/// </summary>
		public static string TrimIfNotNull(this string value, string resultIfNull = null)
		{
			if (value == null) return resultIfNull;
			return value.Trim();
		}

		public static string TrimAllSpaces(this string str)
		{
			return Regex.Replace(str, @"\s+", string.Empty);
		}

		/// <summary>
		/// Masks the specified string for use in password fields.
		/// </summary>
		/// <param name="showLastCharacter">If set to <c>true</c> last character will not be masked.</param>
		public static string ToPasswordString(this string stringToConvert, bool showLastCharacter)
		{
			if (string.IsNullOrWhiteSpace(stringToConvert)) { return string.Empty; }

			if (showLastCharacter)
			{
				return new string('*', stringToConvert.Length - 1) + stringToConvert[stringToConvert.Length - 1];
			}
			else
			{
				return new string('*', stringToConvert.Length);
			}
		}

		/// <summary>
		/// Checks if string has right to left characters.
		/// </summary>
		public static bool HasRightToLeftCharacters(this string stringForChecking)
		{
			if (stringForChecking == null)
			{
				throw new NullReferenceException();
			}

			for (int index = 0; index < stringForChecking.Length; index++)
			{
				if (stringForChecking[index].IsRightToLeftCharacter())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if character is right to left character.
		/// The check is based on the same check from RTLService class.
		/// </summary>
		public static bool IsRightToLeftCharacter(this char c)
		{
			int num = c;
			return (num <= 0 || num >= sbyte.MaxValue)
					&& (num >= 1424 && num <= 1535 || num >= 1536 && num <= 1791 || (num >= 1872 && num <= 1919 || num >= 2208 && num <= 2303)
						|| (num >= 64336 && num <= 65023 || num >= 65136 && num <= 65279 || (num >= 126464 && num <= 126719 || num >= 69216 && num <= 69247)));
		}

		/// <summary>
		/// Counts occurrences of pattern string in text.
		/// </summary>
		public static int CountStringOccurrences(this string text, string pattern)
		{
			// Loop through all instances of the string 'text'.
			int count = 0;
			int i = 0;

			if (text == null)
			{
				return 0;
			}

			while ((i = text.IndexOf(pattern, i)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}


		private static readonly StringBuilder staticStringBuilder = new StringBuilder();

		/// <summary>
		/// Remove HTML tags from string using char array.
		/// </summary>
		public static string RemoveHTMLTags(this string source)
		{
			lock (staticStringBuilder)
			{
				staticStringBuilder.Length = 0;

				bool inside = false;

				for (int i = 0; i < source.Length; i++)
				{
					char let = source[i];

					if (let == '<')
					{
						inside = true;
						continue;
					}

					if (let == '>')
					{
						inside = false;
						continue;
					}

					if (!inside)
					{
						staticStringBuilder.Append(let);
					}
				}

				return staticStringBuilder.ToString();
			}
		}

		public static string ToMD5(this string source)
		{
			using (MD5 md5Hash = MD5.Create())
			{
				return GetMd5Hash(md5Hash, source);
			}
		}

		public static bool IsLowerCase(this string text)
		{
			if (string.IsNullOrEmpty(text)) { return true; }
			foreach (char c in text)
			{
				if (char.IsLetter(c) && !char.IsLower(c))
				{
					return false;
				}
			}

			return true;
		}

		#region string.Format

		public static String SwapPlaceholders(this string format, Object arg0)
		{
			return String.Format(format, arg0);
		}

		public static String SwapPlaceholders(this string format, Object arg0, Object arg1)
		{
			return String.Format(format, arg0, arg1);
		}

		public static String SwapPlaceholders(this string format, Object arg0, Object arg1, Object arg2)
		{
			return String.Format(format, arg0, arg1, arg2);
		}

		public static string SwapPlaceholders(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		#endregion

		private static string GetMd5Hash(MD5 md5Hash, string input)
		{
			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}

		private static string ReverseWordOrderPerLine(this string text)
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			string[] lines = text.Split(NewLineDelimiters, StringSplitOptions.None);

			for (int i = 0; i < lines.Length; i++)
			{
				string textLine = lines[i];
				string[] wordsInLine = textLine.Split(' ');

				for (int j = wordsInLine.Length - 1; j >= 0; j--)
				{
					stringBuilder.Append(wordsInLine[j]);
					if (j > 0)
					{
						stringBuilder.Append(' ');
					}
				}

				if (i < lines.Length - 1)
				{
					stringBuilder.Append('\n');
				}
			}
			return stringBuilder.ToString();
		}

		public static bool Contains(this string source, string toCheck, StringComparison comparison)
		{
			return source.IndexOf(toCheck, comparison) != -1;
		}

		/// <summary>
		/// Parses string as float, starting from specified index to place where number becomes invalid.
		/// It is culture agnostic (i.e. threats both point and comma as decimal symbols).
		/// Throws exceptions if anything is as not as expected (i.e. no digits is found).
		/// </summary>
		public static float ParseFloat(this string input, int startIndex = 0)
		{
			if (input.Length == 0)
				throw new Exception("Input string cannot be empty");
			if (startIndex < 0 || startIndex >= input.Length)
				throw new IndexOutOfRangeException();
			bool pointFound = false;
			bool pointIsComma = false;
			int endOfFirstPart;
			for (endOfFirstPart = startIndex; endOfFirstPart < input.Length; endOfFirstPart++)
			{
				char c = input[endOfFirstPart];
				if (Char.IsDigit(c))
				{
					continue;
				}
				if (!pointFound && (c == '.' || c == ','))
				{
					pointFound = true;
					if (c == ',') pointIsComma = true;
					continue;
				}
				break;
			}
			if (endOfFirstPart <= startIndex)
			{
				throw new Exception("No digits found in input string '{0}' at starting index {1}".SwapPlaceholders(input, startIndex));
			}
			if (pointIsComma) input = input.Replace(',', '.');

			float result = float.Parse(input.Substring(startIndex, endOfFirstPart - startIndex), CultureInfo.InvariantCulture);
			return result;
		}

		/// <summary>
		/// Very similar to <see cref="ParseFloat"/>, it just doesn't throw exceptions, but returns 0f if an error happens.
		/// </summary>
		public static bool TryParseFloat(this string input, out float result, int startIndex = 0)
		{
			try
			{
				result = input.ParseFloat(startIndex);
				return true;
			}
			catch (Exception)
			{
				result = 0f;
				return false;
			}
		}

		#endregion

		#region RTL languages support
		/// <summary>
		/// If string has right to left characters this method places them in proper position.
		/// </summary>
		/// <returns>String with proper characters position.</returns>
		public static string FixRightToLeftCharactersOrder(this string stringForFixing)
		{
#if USE_RTL_PLUGIN_V_1_0
		if (!stringForFixing.IsNullOrWhiteSpace() && stringForFixing.HasRightToLeftCharacters())
		{
			return RTLService.RTL.Convert(stringForFixing);
		}
#endif
			return stringForFixing;
		}

		/// <summary>
		/// If user is playing in Arabic or Hebrew reverse line words for every line.
		/// </summary>
		/// <returns> Initial text with line words reversed. </returns>
		public static string FixRightToLeftWordOrderPerLine(this string stringForFixing)
		{
			if (!stringForFixing.IsNullOrWhiteSpace() && stringForFixing.HasRightToLeftCharacters())
			{
				return stringForFixing.ReverseWordOrderPerLine();
			}
			return stringForFixing;
		}

		/// <summary>
		/// Converts integer to string with proper sign preffix
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static string ToStringWithSign(this long amount)
		{
			if (amount >= 0)
			{
				return "+" + amount;
			}

			return amount.ToString();
		}

		public static void Clear(this StringBuilder builder)
		{
			builder.Length = 0;
		}

		#endregion
	}
}
