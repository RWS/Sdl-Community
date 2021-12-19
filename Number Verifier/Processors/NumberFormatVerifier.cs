using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;
using Sdl.Community.NumberVerifier.Validator;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Processors
{
	public class NumberFormatVerifier : INumberVerifier
	{
		public NumberFormatVerifier(INumberVerifierSettings settings, ITextGenerator textGenerator)
		{
			VerificationSettings = settings;
			TextGenerator = textGenerator;
		}

		public INumberVerifierSettings VerificationSettings { get; }

		public ITextGenerator TextGenerator { get; }

		public List<ErrorReporting> Verify(ISegmentPair segmentPair, List<ExcludedRange> sourceExcludedRanges = null, List<ExcludedRange> targetExcludedRanges = null)
		{
			var errors = new List<ErrorReporting>();

			if (!VerificationSettings.ReportNumberFormatErrors)
			{
				return errors;
			}

			var sourceText = TextGenerator.GetPlainText(segmentPair.Source, false);
			var sourceNumberTokens = GetNumbersTokens(sourceText,
				VerificationSettings.GetSourceDecimalSeparators(),
				VerificationSettings.GetSourceThousandSeparators(), sourceExcludedRanges);

			var targetText = TextGenerator.GetPlainText(segmentPair.Target, false);
			var targetNumberTokens = GetNumbersTokens(targetText,
				VerificationSettings.GetTargetDecimalSeparators(),
				VerificationSettings.GetTargetThousandSeparators(), targetExcludedRanges);

			foreach (var numberToken in targetNumberTokens.Where(a => !a.Valid))
			{
				errors.AddRange(GetErrorMessages(numberToken, false));
			}

			return errors;
		}

		private IEnumerable<ErrorReporting> GetErrorMessages(NumberToken numberToken, bool isSource)
		{
			var messages = new List<ErrorReporting>();

			foreach (var numberPart in numberToken.NumberParts.Where(a => a.Type == NumberPart.NumberType.Invalid))
			{
				var report = new ErrorReporting
				{
					ErrorMessage = numberPart.Message,
					ExtendedErrorMessage = numberPart.Message,
					ErrorLevel = GetErrorLevel(),
					IsHindiVerification = false,
					InitialSourceNumber = isSource ? numberToken.Text : string.Empty,
					InitialTargetNumber = !isSource ? numberToken.Text : string.Empty,
					SourceNumberIssues = isSource ? numberToken.Text : string.Empty,
					TargetNumberIssues = !isSource ? numberToken.Text : string.Empty
				};

				messages.Add(report);
			}

			return messages;
		}

		private ErrorLevel GetErrorLevel()
		{
			switch (VerificationSettings.NumberFormatErrorType)
			{
				case "Error":
					return ErrorLevel.Error;
				case "Warning":
					return ErrorLevel.Warning;
				default:
					return ErrorLevel.Note;
			}
		}

		private static IEnumerable<NumberToken> GetNumbersTokens(string text, IEnumerable<string> decimalSeparators, IEnumerable<string> groupSeparators, List<ExcludedRange> excludedRanges)
		{
			var numberTokens = new List<NumberToken>();

			var numberSeparators = GetNumberSeparators(
				NormalizeStrings(decimalSeparators),
				NormalizeStrings(groupSeparators));
			var numberParser = new NumberParser(numberSeparators.Count > 0 ? numberSeparators : null);

			var regexNumber = new Regex(@"([\+\-]\s*)?[0-9\.\,]*[Ee]?[\+\-]?\d+", RegexOptions.Singleline);

			var numberMatches = regexNumber.Matches(text).Cast<Match>().ToList();
			numberMatches.ExcludeRanges(excludedRanges);
			foreach (var numberMatch in numberMatches)
			{
				var numberToken = numberParser.Parse(numberMatch.Value);
				numberTokens.Add(numberToken);
			}

			return numberTokens;
		}

		private static List<NumberSeparator> GetNumberSeparators(IEnumerable<string> decimalSeparators, IEnumerable<string> groupSeparators)
		{
			var numberSeparators = GetNumberSeparators(decimalSeparators, NumberSeparator.SeparatorType.DecimalSeparator);
			numberSeparators.AddRange(GetNumberSeparators(groupSeparators, NumberSeparator.SeparatorType.GroupSeparator));

			return numberSeparators;
		}

		private static List<NumberSeparator> GetNumberSeparators(IEnumerable<string> separators, NumberSeparator.SeparatorType type)
		{
			var numberSeparators = new List<NumberSeparator>();
			foreach (var separator in separators.Where(a => !string.IsNullOrEmpty(a)))
			{
				if (numberSeparators.Exists(a => a.Value == separator && a.Type == type))
				{
					continue;
				}

				var numberSeparator = new NumberSeparator
				{
					Type = type,
					Value = separator
				};

				numberSeparators.Add(numberSeparator);
			}

			return numberSeparators;
		}

		private static IEnumerable<string> NormalizeStrings(IEnumerable<string> strings)
		{
			var normalizedSeparators = new List<string>();
			foreach (var text in strings)
			{
				normalizedSeparators.Add(text.Contains("\\u")
					? ConvertCharEncoding(Encoding.Unicode, Encoding.UTF8, Regex.Unescape(text))
					: text);
			}

			return normalizedSeparators;
		}

		private static string ConvertCharEncoding(Encoding unicode, Encoding utf8, string text)
		{
			// Convert the string into a byte array.
			var unicodeBytes = unicode.GetBytes(text);
			// Perform the conversion from one encoding to the other.
			var utf8Bytes = Encoding.Convert(unicode, utf8, unicodeBytes);
			// Convert the new byte[] into a char[] and then into a string.
			var utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
			utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);

			return new string(utf8Chars);
		}
	}
}
