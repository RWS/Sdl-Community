using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberTexts
	{
		private readonly List<string> _decimalSeparators;
		private readonly List<string> _thousandSeparators;
		private string _digitClass = "[0-9٠-٩]";
		private string _digitClassWithoutZero = "[1-9١-٩]";
		private readonly Regex _signRegex = new("([+−-])");

		public NumberTexts(string text, NumberFormattingSettings formattingSettings)
		{
			_thousandSeparators = formattingSettings.ThousandSeparators;
			_decimalSeparators = formattingSettings.DecimalSeparators;

			//NumberTextArea level - detect possible numbers
			var textMatches = GetTextAreas(text, formattingSettings.NumberAreaSeparators);
			SetTextAreas(textMatches, text);

			//Number level
			CheckTextAreas();
			CheckNumbers(formattingSettings.OmitLeadingZero);
		}

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

		private void AddNumberText(string text, List<string> separators, int startIndex, int endIndex, string sign)
			=> Texts.Add(new NumberText
			{
				Text = text,
				ActualSeparators = separators,
				StartIndex = startIndex,
				Length = endIndex,
				Sign = sign
			});

		private void CheckNumbers(bool omitZero = false)
		{
			//TODO: Change the previous check in the flow to also do this checks - may be more readable
			var thousandSeparatorsList = _thousandSeparators.ToList();
			thousandSeparatorsList.Remove(Constants.NoSeparator);

			foreach (var area in Texts)
			{
				if (!area.CanBeNumber) continue;

				var isSigned = false;
				var usedSign = _signRegex.Match(area.Text[0].ToString());
				if (usedSign.Success)
				{
					isSigned = true;
					area.Text = Regex.Replace(area.Text, _signRegex.ToString(), "");
				}

				//the only scenario in which we have to take omitZero into account is when the number has ONLY ONE separator (and the number starts with it)
				var omitZeroPattern = $"?<={_digitClass}";
				if (omitZero && area.ActualSeparators.Count == 1)
				{
					omitZeroPattern = null;
				}

				var integer = $"(?'Integer'(0|٠|{_digitClassWithoutZero}{_digitClass}*|{_digitClassWithoutZero}{_digitClass}{{0,2}}((?'ThousandSeparators'{thousandSeparatorsList.ToRegexPattern()}){_digitClass}{{3}})*))";
				var fraction = $"(?'Fraction'({omitZeroPattern})((?'DecimalSeparators'{_decimalSeparators.ToRegexPattern()}){_digitClass}+))";

				var pattern = new Regex($"^({integer})?{fraction}?$");
				var realNumberMatch = pattern.Match(area.Text);

				if (isSigned) area.Text = area.Text.Insert(0, usedSign.Value);

				if (!realNumberMatch.Success) continue;

				var normalized = realNumberMatch.Normalize(_thousandSeparators, _decimalSeparators, omitZero).Insert(0, isSigned ? "s" : "");
				area.SetAsValid(
					normalized);
			}
		}

		private void CheckTextAreas()
		{
			//TODO: combine this with CheckNumbers
			foreach (var textPart in Texts)
			{
				var separators = textPart.ActualSeparators;
				var distinctSeparators = separators.Distinct().ToList();
				var distinctSeparatorsTotal = distinctSeparators.Distinct().Count();

				var thousandSeparators = _thousandSeparators.ToList();
				thousandSeparators.Remove(Constants.NoSeparator);
				var ambiguousSeparators = thousandSeparators.Intersect(_decimalSeparators);
				var strictThousand = thousandSeparators.Except(ambiguousSeparators);
				var strictDecimal = _decimalSeparators.Except(ambiguousSeparators);

				switch (distinctSeparatorsTotal)
				{
					case > 2:
						{
							textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.TooManyTypesOfSeparators);
							break;
						}

					case 2:
						{
							if (strictDecimal.Contains(separators[0]))
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.SeparatorAfterDecimal);
							}

							else if (distinctSeparators.All(ds => strictThousand.Contains(ds)))
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.NumberCannotHaveTwoDifferentThousandSeparators);
							}


							else if (!thousandSeparators.Contains(distinctSeparators[0]))
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.Error_ThousandSeparatorNotValid);
							}

							else if (!_decimalSeparators.Contains(distinctSeparators[1]))
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.Error_DecimalSeparatorNotValid);
							}

							break;
						}

					case 1 when separators.Count == 1:
						{
							if (!thousandSeparators.Contains(distinctSeparators[0]) && !_decimalSeparators.Contains(distinctSeparators[0]))
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.SeparatorNotValid);
							}

							if (!_thousandSeparators.Contains(textPart.ActualSeparators[0]) &&
								!_decimalSeparators.Contains(textPart.ActualSeparators[0]))
							{
								textPart.AddError(NumberText.ErrorLevel.NumberLevel, PluginResources.SeparatorNotValid);
							}

							if (Regex.Split(textPart.Text, Regex.Escape(textPart.ActualSeparators[0]))[1].Length < 3 && !_decimalSeparators.Contains(textPart.ActualSeparators[0]))
							{
								textPart.AddError(NumberText.ErrorLevel.NumberLevel, PluginResources.Error_DecimalSeparatorNotValid);
							}

							break;
						}

					case 1 when separators.Count > 1:
						{
							var hasStrictFractionalPart = textPart.Groups[textPart.Groups.Length - 1].Length < 3;
							if (hasStrictFractionalPart)
							{
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel,
									PluginResources.NumberCannotHaveTheSameCharacterAsThousandAndAsDecimalSeparator);
							}

							break;
						}
				}
			}
		}

		private List<Match> GetTextAreas(string text, List<string> allSeparatorsList)
		{
			var allSeparatorsRegex = allSeparatorsList.ToRegexPattern();

			var textAreaPattern = new Regex($"((?<Sign>[+−-](?={_digitClass}|{allSeparatorsRegex}))?(?:(?<Separators>{allSeparatorsRegex})?{_digitClass}+)*)");

			var textMatches = new List<Match>();
			if (text is not null) textMatches = textAreaPattern.Matches(text).Cast<Match>().Where(match => !string.IsNullOrWhiteSpace(match.Value)).ToList();

			return textMatches;
		}

		private void SetTextAreas(List<Match> textMatches, string text)
		{
			textMatches.ForEach(
				stm =>
				{
					var separators = stm.Groups["Separators"].Captures.Cast<Capture>().Select(c => c.Value).Where(c => !string.IsNullOrEmpty(c)).ToList();

					var matchValue = stm.Value;
					var sIndex = stm.Index;
					var length = matchValue.Length;
					
					var firstChar = matchValue[0];
					if (string.IsNullOrWhiteSpace(firstChar.ToString()))
					{
						var misplacedSeparatorPattern = new Regex(Regex.Escape(firstChar.ToString()));
						matchValue = misplacedSeparatorPattern.Replace(matchValue, "", 1);

						if (!misplacedSeparatorPattern.Match(matchValue).Success) separators.Remove(firstChar.ToString());
					}

					var sign = stm.Groups["Sign"].Captures.Cast<Capture>().Select(c => c.Value).FirstOrDefault();
					if (!stm.IsAlphanumeric(text)) AddNumberText(matchValue, separators, sIndex, length, sign);
				});
		}
	}
}