using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberTexts
	{
		private readonly NumberFormattingSettings _formattingSettings;
		private readonly string _digitClass = "[０-９0-9٠-٩]";

		public NumberTexts(string text, NumberFormattingSettings formattingSettings)
		{
			_formattingSettings = formattingSettings;

			var thousandSeparators = formattingSettings.ThousandSeparators;
			var decimalSeparators = formattingSettings.DecimalSeparators;

			NumberParser = new NumberParser(thousandSeparators, decimalSeparators);

			var textMatches = GetTextAreas(text);
			SetTextAreas(textMatches);
		}

		public NumberParser NumberParser { get; }
		public List<NumberText> Texts { get; } = new();

		public NumberText this[int key] => key >= Texts.Count ? null : Texts[key];

		public List<(int, Comparer)> IndexesOf(NumberText other)
		{
			var indexList = new List<(int, Comparer)>();
			for (var i = 0; i < Texts.Count; i++)
			{
				var comparisonResult = Texts[i].Compare(other);
				indexList.Add((i, comparisonResult));
			}

			return indexList;
		}

		private void AddNumberText(List<string> separators, int startIndex, int endIndex, string sign, NumberToken token)
		{
			var numberText = new NumberText
			{
				Text = token.Text,
				ActualSeparators = separators,
				StartIndex = startIndex,
				Length = endIndex,
				Sign = sign,
				Token = token,
				Errors = new Dictionary<NumberText.ErrorLevel, List<Error>>
				{
					[NumberText.ErrorLevel.SegmentPairLevel] = new(),
					[NumberText.ErrorLevel.TextAreaLevel] = GetTextAreaLevelErrors(token)
				}
			};

			if (numberText.IsValidNumber) numberText.Normalized = numberText.Token.Normalize();

			Texts.Add(numberText);
		}

		private List<Error> GetTextAreaLevelErrors(NumberToken numberToken) =>
			numberToken.NumberParts.Where(p => p.Type == NumberPart.NumberType.Invalid).Select(p => new Error
			{
				Message = p.Message,
				ErrorLevel = NumberText.ErrorLevel.TextAreaLevel
			}).ToList();

		private List<Match> GetTextAreas(string text)
		{
			var allSeparatorsRegex = _formattingSettings.NumberAreaSeparators.ToRegexPattern();

			var alphanumericsStartingPattern = "[A-Za-z]+[+−–-]?\\d*";
			var alphanumericsEndingPattern = "\\d*[+−–-]?[A-Z]+";

			var textAreaPattern = new Regex($"(?<!{alphanumericsStartingPattern})((?<Sign>[+−–-](?={_digitClass}|{allSeparatorsRegex}))?((?<Separators>{allSeparatorsRegex})?{_digitClass}+)*)(?!{alphanumericsEndingPattern})");

			var textMatches = new List<Match>();
			if (text is not null) textMatches = textAreaPattern.Matches(text)
				.Cast<Match>()
				.Where(match => !string.IsNullOrWhiteSpace(match.Value) && Regex.Match(match.Value, "\\d").Success)
				.ToList();

			textMatches.ExcludeRanges(_formattingSettings.ExcludedRanges);
			return textMatches;
		}

		private void SetTextAreas(List<Match> textMatches)
		{
			textMatches.ForEach(
				stm =>
				{
					var separators = stm.Groups["Separators"].Captures.Cast<Capture>().Select(c => c.Value).Where(c => !string.IsNullOrEmpty(c)).ToList();

					var sIndex = stm.Index;
					var length = stm.Length;

					var sign = stm.Groups["Sign"].Captures.Cast<Capture>().Select(c => c.Value).FirstOrDefault();
					var token = NumberParser.Parse(stm.Value, _formattingSettings.OmitLeadingZero);
					AddNumberText(separators, sIndex, length, sign, token);
				});
		}
	}
}