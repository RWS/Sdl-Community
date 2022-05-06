using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sdl.Community.NumberVerifier.Helpers
{
	public class TextFormatter
	{
		/// <summary>
		/// Get the builder separators with specific format (used when matching the number based on the specific separator)
		/// </summary>
		/// <param name="separators"></param>
		/// <returns></returns>
		public StringBuilder GetBuilderSeparators(string separators)
		{
			var separatorsBuilder = new StringBuilder();
			var separatorsArray = separators.Split('\\');

			foreach (var separator in separatorsArray)
			{
				if (!string.IsNullOrWhiteSpace(separator))
				{
					separatorsBuilder.Append($@"{separator}?\");
				}
			}
			return separatorsBuilder;
		}

		/// <summary>
		/// Format different types of dates
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string FormatTextDate(string text)
		{
			string[] shortFormats = { "d/M/yy", "dd/MM/yy", "d.M.yy", "dd.MM.yy", "dd/M/yy", "dd.M.yy" };

			string[] longFormats =
			{
				"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
				"M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", "M/d/yyyy h:mm", "M/d/yyyy h:mm", "MM/dd/yyyy hh:mm",
				"M/dd/yyyy hh:mm", "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt", "dd/MM/yyyy hh:mm:ss",
				"d/M/yyyy h:mm:ss", "d/M/yyyy hh:mm tt", "d/M/yyyy hh tt", "d/M/yyyy h:mm", "d/M/yyyy h:mm",
				"dd/M/yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd.M.yyyy", "dd.MM.yyyy", "d.M.yyyy",
				"d.M.yyyy h:mm:ss tt", "d.M.yyyy h:mm tt", "dd.MM.yyyy hh:mm:ss", "d.M.yyyy h:mm:ss",
				"d.M.yyyy hh:mm tt", "d.M.yyyy hh tt", "d.M.yyyy h:mm", "d.M.yyyy h:mm"
			};

			DateTime dateValue;
			if (DateTime.TryParseExact(text, shortFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
			{
				text = dateValue.ToString("dd/MM/yy");
			}

			if (DateTime.TryParseExact(text, longFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
			{
				text = dateValue.ToShortDateString();
			}

			return text;
		}

		/// <summary>
		/// Format the different types of dash
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string FormatDashText(string text)
		{
			//skip the "-" in case of: - 23 (dash, space, number)
			char[] dashSign = { '-', '\u2013', '\u2212' };
			char[] space = { ' ', '\u00a0', '\u2009', '\u202F', '\u0020' };
			var spacePosition = text.IndexOfAny(space);
			var dashPosition = text.IndexOfAny(dashSign);
			if (dashPosition == 0 && spacePosition == 1)
			{
				text = text.Substring(2);
			}

			return text;
		}

		/// <summary>
		/// Format separators for a proper identification of number within the normalization process 
		/// </summary>
		/// <param name="separators"></param>
		/// <returns></returns>
		public string FormatSeparators(IEnumerable<string> separators)
		{
			var separatorsBuilder = new StringBuilder();
			foreach (var item in separators)
			{
				if (!string.IsNullOrEmpty(item))
				{
					// it is required to add \\ for the custom separators, so they will be identified correctly when doing the normalization.
					if (!item.Contains("\\"))
					{
						separatorsBuilder.Append($"\\{item}");
					}
					else
					{
						separatorsBuilder.Append(item);
					}
				}
			}

			return separatorsBuilder.ToString();
		}

		/// <summary>
		/// Get separators
		/// </summary>
		/// <param name="separators"></param>
		/// <returns></returns>
		public string GetSeparators(string separators)
		{
			var separatorsBuilder = new StringBuilder();
			if (separators != null)
			{
				foreach (var separator in separators)
				{
					separatorsBuilder.Append($"\\{separator}");
				}
			}

			return separatorsBuilder.ToString();
		}
		
		/// <summary>
		/// Check if the separators contains one of the space type unicode
		/// </summary>
		/// <param name="separators"></param>
		/// <returns></returns>
		public bool ContainsSpaceSeparator(string separators)
		{
			return separators.Contains("u00A0") || separators.Contains("u2009") || separators.Contains("u0020") || separators.Contains("u202F");
		}

		/// <summary>
		/// Parse the number text when 'No separator' is checked and the number is thousand or thousand-decimal format.
		/// </summary>
		/// <param name="numberText"></param>
		/// <param name="tempNormalized"></param>
		/// <returns></returns>
		public string ParseNoSeparatorNumber(string numberText, StringBuilder tempNormalized)
		{
			if (int.Parse(numberText) >= 1000)
			{
				var position = 0;
				for (var i = numberText.Length - 1; i >= 0; i--)
				{
					if (position > 0 && position == 3)
					{
						tempNormalized.Insert(0, "m");
						//if on the thousand position a number is allocated, then add it back to tempNormalized, at position before the "m"
						if (int.TryParse(numberText[i].ToString(), out _))
						{
							tempNormalized.Insert(0, numberText[i]);
						}
					}
					else
					{
						tempNormalized.Insert(0, numberText[i]);
					}

					position++;
				}

				// insert also the remained first letter back to the number text, in case it was not already added (above at line 157)
				if (tempNormalized.ToString().Length != numberText.Length + 1)
				{
					tempNormalized.Insert(0, numberText[0]);
				}

				return tempNormalized.ToString();
			}

			return numberText;
		}

		public char[] GetSeparatorsChars(string customDecimalSeparators, string customThousandSeparators)
		{
			var decimalSep = customDecimalSeparators.ToCharArray();
			var thousandSep = customThousandSeparators.ToCharArray();
			var separatorsChars = new[] { ',', '.' }.Concat(decimalSep).Concat(thousandSep).ToArray();

			return separatorsChars;
		}

		public string GetAlphanumericsCustomSeparators(string[] separators)
		{
			var customSeparatorsBuilder = new StringBuilder();
			foreach (var separator in separators)
			{
				customSeparatorsBuilder.Append(separator);
			}

			return customSeparatorsBuilder.ToString();
		}

		// When number has the last or first char one of the separators, remove it because we don't need it.
		// Is not corresponding to decimal/thousand separator, it's used as simple punctuation letter (Eg: ,123 or 245, )
		public string RemovePunctuationChar(string text, char[] sepChars, bool omitLeadingZero)
		{
			// we don't want to remove the punctuation marks when OmitLeading Zero is checked so the verification would
			// be processed correctly
			if (omitLeadingZero && sepChars.ToList().Contains(text[0]))
			{
				return text.Trim();
			}
			// if punctuation marks are found at the end of number, remove it (Ex: 245,) because is not part of a thousand or decimal number
			if (text.Length - 1 == text.LastIndexOfAny(sepChars))
			{
				text = text.Remove(text.Length - 1, 1);
			}

			// remove only if the first char is not digit (0-9) and is not representing the negative symbol(−)
			if (!char.IsDigit(text[0]) && text[0].Equals('−'))
			{
				return text.Trim();
			}
			// remove only if the first char is not digit (0-9) and is not representing the negative symbol (-)
			if (!char.IsDigit(text[0]) && text[0].Equals('-'))
			{
				return text.Trim();
			}

			// remove if the first char is not a digit 
			if (!char.IsDigit(text[0]))
			{
				text = text.Remove(0, 1);
			}

			return text.Trim();
		}
	}
}