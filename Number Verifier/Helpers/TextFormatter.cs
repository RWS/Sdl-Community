using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Helpers
{
	public class TextFormatter
	{
		private readonly NumberVerifierMain _numberVerifierMain;

		public TextFormatter(NumberVerifierMain numberVerifierMain)
		{
			_numberVerifierMain = numberVerifierMain;
		}

		// Format text so the 'No-break space'/'Thin space'/'Space'/'Narrow no break space' separators will be removed from the text, when the options are enabled
		// and Target or Source has NoSeparator option enabled:
		// E.g: source text: 2300 with option 'No separator' and target text: 2 300 with option 'No-break space'
		public string FormatTextSpace(string separators, string text)
		{
			if ((separators.Contains("u00A0") || separators.Contains("u2009") || separators.Contains("u0020") || separators.Contains("u202F"))
			    && (_numberVerifierMain.VerificationSettings.TargetNoSeparator || _numberVerifierMain.VerificationSettings.SourceNoSeparator))
			{
				foreach (var separator in separators.Split('\\'))
				{
					if (!string.IsNullOrEmpty(separator))
					{
						// replace the text space only for the enabled separator
						switch (separator)
						{
							case "u00A0":
								if (separators.Contains("u00A0"))
								{
									text = text.Replace("\u00A0", string.Empty);
								}
								continue;
							case "u2009":
								if (separators.Contains("u2009"))
								{
									text = text.Replace("\u2009", string.Empty);
								}
								continue;
							case "u0020":
								if (separators.Contains("u0020"))
								{
									text = text.Replace("\u0020", string.Empty);
								}
								continue;

							case "u202F":
								if (separators.Contains("u202F"))
								{
									text = text.Replace("\u202F", string.Empty);
								}
								continue;
							default:
								continue;
						}
					}
				}
			}
			return text;
		}

		// Format text so the comma/period will be removed from the text when the TargetNoSeparator or SourceNoSeparator is enabled,
		// so the number can be validated entirely and it won't be split based on the , or .
		// E.g: source text: 1,300 with option 'Comma separator' and target text: 1300 with option 'No separator'
		public string FormatTextForNoSeparator(string text, bool isSource, SeparatorModel separatorModel)
		{
			// Replace separator when for both source AND target, the 'NoSeparator' option is enabled
			// AND user enables also one of the option: SourceThousandsComma/SourceThousandsPeriod when the text is source and
			// TargetThousandsComma/TargetThousandsPeriod when the text is target
			var verificationSettings = _numberVerifierMain.VerificationSettings;
			if (isSource && verificationSettings.SourceNoSeparator && verificationSettings.TargetNoSeparator
			    && (verificationSettings.SourceThousandsComma || verificationSettings.SourceThousandsPeriod)
			    || !isSource && verificationSettings.TargetNoSeparator && verificationSettings.SourceNoSeparator
			    && (verificationSettings.TargetThousandsComma || verificationSettings.TargetThousandsPeriod))
			{
				text = GetFormattedText(separatorModel, text);
				return text;
			}
			// When for both source AND target settings, only the 'No separator' is checked, do not format the text
			if (verificationSettings.TargetNoSeparator && verificationSettings.SourceNoSeparator)
			{
				return text;
			}
			// When text is source and the verification is for Target 'NoSeparator' OR the text is target and the verification is for Source 'NoSeparator', then format the text
			if (isSource && verificationSettings.TargetNoSeparator || !isSource && verificationSettings.SourceNoSeparator)
			{
				text = GetFormattedText(separatorModel, text);
			}
			return text;
		}

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

		public string GetSeparators(string separators)
		{
			var separatorsBuilder=  new StringBuilder();
			if (separators != null)
			{
				foreach (var separator in separators)
				{
					separatorsBuilder.Append($"\\{separator}");
				}
			}

			return separatorsBuilder.ToString();
		}

		// Format text for the thousand numbers (ex: 1,200) or for the combination of thousand-decimal numbers (ex: 1,200.31)
		public string FormatText(SeparatorModel separatorModel, NormalizedNumber normalizedNumber, string text, string separators, bool isSource)
		{
			text = FormatTextSpace(normalizedNumber.ThousandSeparators, text);
			text = FormatTextForNoSeparator(text, isSource, separatorModel);
			normalizedNumber.Separators = separators;

			return text;
		}
		
		// Get the formatted text after the thousand separator is removed
		private string GetFormattedText(SeparatorModel separatorModel, string text)
		{
			if (separatorModel.LengthCommaOrCustomSep >= 3)
			{
				// replace the "," or custom separator only when it located at thousand position
				separatorModel.CustomSeparators = $"{separatorModel.CustomSeparators},";
				text = GetReplacedText(text, separatorModel.CustomSeparators);
			}

			if (separatorModel.LengthPeriodOrCustomSep >= 3)
			{
				// replace the "." or custom separator only when it located at thousand position
				separatorModel.CustomSeparators = $"{separatorModel.CustomSeparators}.";
				text = GetReplacedText(text, separatorModel.CustomSeparators);
			}

			return text;
		}

		// Return the replaced text based on the custom separators and specific conditions
		private string GetReplacedText(string text, string customSeparators)
		{
			if (string.IsNullOrEmpty(text)) return text;

			foreach (var customSeparator in customSeparators)
			{
				var separator = customSeparator.ToString();
				var formattedSeparator = $@"\{separator}";
				if (Regex.IsMatch(text, formattedSeparator))
				{
					// remove the thousand separator, ex: 1,200,50 / 1*200.50 / 1.200
					var indexOfSeparator = text.IndexOf(separator, StringComparison.Ordinal);
					text = text.Remove(indexOfSeparator, 1).Insert(indexOfSeparator, string.Empty);
				}
			}

			return text;
		}
	}
}