using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Match = System.Text.RegularExpressions.Match;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberValidator
	{
		private List<string> _allSeparatorsList;
		private string _digitClass = "[0-9٠-٩]";
		private string _digitClassWithoutZero = "[1-9١-٩]";
		private INumberVerifierSettings _settings;
		private List<string> _sourceDecimalSeparators;
		private string _sourceText;
		private List<string> _sourceThousandSeparators;
		private List<string> _targetDecimalSeparators;
		private string _targetText;
		private List<string> _targetThousandSeparators;

		public void GetErrors(string sourceText, string targetText, INumberVerifierSettings settings, out NumberTexts sourceNumberTexts, out NumberTexts targetNumberTexts)
		{
			_settings = settings;
			_sourceText = sourceText.Normalize(System.Text.NormalizationForm.FormKC);
			_targetText = targetText.Normalize(System.Text.NormalizationForm.FormKC);

			_sourceThousandSeparators = GetAllowedThousandSeparators(true);
			_sourceDecimalSeparators = GetAllowedDecimalSeparators(true).Select(Regex.Unescape).ToList();

			_targetThousandSeparators = GetAllowedThousandSeparators(false);
			_targetDecimalSeparators = GetAllowedDecimalSeparators(false).Select(Regex.Unescape).ToList();

			_allSeparatorsList = GetAllSeparatorsCombined(_sourceThousandSeparators, _sourceDecimalSeparators, _targetThousandSeparators,
				_targetDecimalSeparators);

			Verify(out sourceNumberTexts, out targetNumberTexts);
		}

		private static void CheckAlignment(NumberTexts sourceTextAreas, NumberTexts targetTextAreas)
		{
			for (var i = 0; i < sourceTextAreas.Texts.Count; i++)
			{
				if (i >= targetTextAreas.Texts.Count)
				{
					var message = PluginResources.Error_NumbersRemoved;
					sourceTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel,
						message);
					continue;
				}

				var sourceTargetComparison = sourceTextAreas[i].Compare(targetTextAreas[i]);
				switch (sourceTargetComparison)
				{
					case NumberText.ComparisonResult.DifferentSequence:
						{
							var betterFitOptions = targetTextAreas.IndexesOf(sourceTextAreas[i]).Select(it => it.Item1);
							var optionsString = string.Join(", ", betterFitOptions);
							optionsString = betterFitOptions.Any() ? $"Segments {optionsString}" : null;

							targetTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel,
								PluginResources.Error_DoesNotCorrespondToItsSourceCounterpart, optionsString);

							break;
						}

					case NumberText.ComparisonResult.SameSequence:
						{
							var message = PluginResources.Error_SameSequencesButDifferentValues;
							targetTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel,
								message);

							break;
						}

					case NumberText.ComparisonResult.DifferentValues:
						targetTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel,
							PluginResources.Error_DoesNotCorrespondToItsSourceCounterpart);
						break;

					case NumberText.ComparisonResult.Unlocalised:
						{
							targetTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel,
								PluginResources.Error_NumberUnlocalised);
							break;
						}
				}
			}
		}

		private static List<string> GetAllSeparatorsCombined(List<string> sourceThousandSeparators, List<string> sourceDecimalSeparators, List<string> targetThousandSeparators, List<string> targetDecimalSeparators)
		{
			var allSeparators = new List<string>();

			allSeparators.AddRange(sourceThousandSeparators);
			allSeparators.AddRange(sourceDecimalSeparators);
			allSeparators.AddRange(targetThousandSeparators);
			allSeparators.AddRange(targetDecimalSeparators);

			allSeparators = allSeparators.Distinct().ToList();

			allSeparators.RemoveAll(s => s == Constants.NoSeparator);
			return allSeparators;
		}

		private static NumberTexts GetTextAreas(List<Match> textMatches)
		{
			var textAreas = new NumberTexts();

			textMatches.ForEach(
				stm =>
				{
					var separators = stm.Groups["Separators"].Captures.Cast<Capture>().Select(c => c.Value).Where(c => !string.IsNullOrEmpty(c)).ToList();

					var matchValue = stm.Captures[0].Value;
					var misplacedSeparator = matchValue[0].ToString();
					if (string.IsNullOrWhiteSpace(misplacedSeparator))
					{
						matchValue = matchValue.Replace(misplacedSeparator, "");
						separators.Remove(misplacedSeparator);
					}

					textAreas[matchValue] = separators;
				});

			return textAreas;
		}

		private void ApplyTargetSettings(List<string> sourceSeparators, List<string> targetSeparators, List<string> separatorsList)
		{
			if (_settings.PreventLocalizations)
			{
				separatorsList.AddRange(sourceSeparators);
			}
			if (_settings.AllowLocalizations)
			{
				separatorsList.AddRange(sourceSeparators);
				separatorsList.AddRange(targetSeparators);
			}
			if (_settings.RequireLocalizations)
			{
				separatorsList.AddRange(targetSeparators);
			}
		}

		private void CheckNumbers(NumberTexts areas, List<string> thousandSeparators, List<string> decimalSeparators, bool omitZero = false)
		{
			var thousandClass = thousandSeparators.ToList();
			thousandClass.Remove(Constants.NoSeparator);

			foreach (var area in areas.Texts)
			{
				if (!area.IsValidTextArea) continue;

				var sign = new Regex("([+−-])");

				var isSigned = false;
				var usedSign = sign.Match(area.Text[0].ToString());
				if (usedSign.Success)
				{
					isSigned = true;
					area.Text = Regex.Replace(area.Text, sign.ToString(), "");
				}

				var firstCharRemoved = false;
				var firstChar = area.Text[0];
				if (!char.IsDigit(firstChar) && !decimalSeparators.Contains(firstChar.ToString()))
				{
					firstCharRemoved = true;
					area.Text = area.Text.Replace(firstChar.ToString(), "");
				}

				//the only scenario in which we have to take omitZero into account is when the number has ONLY ONE separator (and the number starts with it)
				var omitZeroPattern = $"?<={_digitClass}";
				if (omitZero && area.Separators.Count == 1)
				{
					omitZeroPattern = null;
				}

				var integer = $"(?'Integer'(0|٠|{_digitClassWithoutZero}{_digitClass}*|{_digitClassWithoutZero}{_digitClass}{{0,2}}((?'ThousandSeparators'{thousandClass.ToClassOfCharactersString()}){_digitClass}{{3}})*))";
				var fraction = $"(?'Fraction'({omitZeroPattern})((?'DecimalSeparators'{decimalSeparators.ToClassOfCharactersString()}){_digitClass}+))";

				var pattern = new Regex($"^({integer})?{fraction}?$");
				var realNumberMatch = pattern.Match(area.Text);

				if (!firstCharRemoved && isSigned) area.Text = area.Text.Insert(0, usedSign.Value);

				if (realNumberMatch.Success)
				{
					var normalized = realNumberMatch.Normalize(thousandSeparators, decimalSeparators, omitZero).Insert(0, isSigned ? "s" : "");
					area.SetAsValid(
						normalized);
				}
				else
					area.AddError(NumberText.ErrorLevel.NumberLevel, "Number not valid");
			}
		}

		private void CheckTextAreas(NumberTexts numberTexts, List<string> thousandSeparators, List<string> decimalSeparators)
		{
			foreach (var textPart in numberTexts.Texts)
			{
				var separators = textPart.Separators;
				var distinctSeparators = separators.Distinct().ToList();
				var distinctSeparatorsTotal = distinctSeparators.Distinct().Count();

				var ambiguousSeparators = thousandSeparators.Intersect(decimalSeparators);
				var strictThousand = thousandSeparators.Except(ambiguousSeparators);

				switch (distinctSeparatorsTotal)
				{
					case > 2:
						textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.TooManyTypesOfSeparators);
						break;

					case 2 when distinctSeparators[0] != distinctSeparators[1]:
						{
							if (!thousandSeparators.Contains(distinctSeparators[0]))
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.ThousandSeparatorAfterDecimal);
							else if (distinctSeparators.All(ds => strictThousand.Contains(ds)))
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.NumberCannotHaveTwoDifferentThousandSeparators);
							else if (separators.Count(sep => sep == distinctSeparators[1]) > 1)
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.TooManyDecimalSeparators);
							else if (separators[separators.Count - 1] != distinctSeparators[1])
								textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.ThousandSeparatorAfterDecimalSeparator);

							break;
						}
					case 2 when !thousandSeparators.Contains(distinctSeparators[0]):
						{
							textPart.AddError(NumberText.ErrorLevel.TextAreaLevel, PluginResources.TooManyDecimalSeparators);
						}
						break;
				}
			}
		}

		private List<string> GetAllowedDecimalSeparators(bool isSource)
		{
			var sourceDecimalSeparators = _settings.GetSourceDecimalSeparators().ToList();
			var targetDecimalSeparators = _settings.GetTargetDecimalSeparators().ToList();

			var decimalSeparatorsList = new List<string>();
			if (isSource) decimalSeparatorsList = sourceDecimalSeparators;
			else ApplyTargetSettings(sourceDecimalSeparators, targetDecimalSeparators, decimalSeparatorsList);

			decimalSeparatorsList.RemoveAll(string.IsNullOrEmpty);
			return decimalSeparatorsList.Select(Regex.Unescape).Distinct().ToList();
		}

		private List<string> GetAllowedThousandSeparators(bool isSource)
		{
			var sourceThousandSeparators = _settings.GetSourceThousandSeparators().ToList();
			var targetThousandSeparators = _settings.GetTargetThousandSeparators().ToList();

			var thousandSeparatorsList = new List<string>();
			if (_settings.TargetNoSeparator || _settings.SourceNoSeparator) thousandSeparatorsList.Add(Constants.NoSeparator);

			if (isSource) thousandSeparatorsList.AddRange(sourceThousandSeparators);
			else ApplyTargetSettings(sourceThousandSeparators, targetThousandSeparators, thousandSeparatorsList);

			thousandSeparatorsList.RemoveAll(string.IsNullOrEmpty);
			return thousandSeparatorsList.Select(Regex.Unescape).Distinct().ToList();
		}

		private void Verify(out NumberTexts sourceNumberTexts, out NumberTexts targetNumberTexts)
		{
			var allSeparators = _allSeparatorsList.ToClassOfCharactersString();

			var textAreaPattern = new Regex($"((?<Sign>[+−-](?={_digitClass}|{allSeparators}))?(?:(?<=)(?<Separators>{allSeparators})?{_digitClass}+)*)");

			var sourceTextMatches = textAreaPattern.Matches(_sourceText).Cast<Match>().Where(match => !string.IsNullOrWhiteSpace(match.Value)).ToList();
			var targetTextMatches = textAreaPattern.Matches(_targetText).Cast<Match>().Where(match => !string.IsNullOrWhiteSpace(match.Value)).ToList();

			sourceNumberTexts = GetTextAreas(sourceTextMatches);
			targetNumberTexts = GetTextAreas(targetTextMatches);

			//Text-areas-that-can-be-numbers level
			CheckTextAreas(sourceNumberTexts, _sourceThousandSeparators, _sourceDecimalSeparators);
			CheckTextAreas(targetNumberTexts, _targetThousandSeparators, _targetDecimalSeparators);

			//Number level
			CheckNumbers(sourceNumberTexts, _sourceThousandSeparators, _sourceDecimalSeparators, _settings.SourceOmitLeadingZero);
			CheckNumbers(targetNumberTexts, _targetThousandSeparators, _targetDecimalSeparators, _settings.TargetOmitLeadingZero);

			//SegmentPair level - alignment
			CheckAlignment(sourceNumberTexts, targetNumberTexts);

			for (var i = sourceNumberTexts.Texts.Count; i < targetNumberTexts.Texts.Count; i++)
			{
				targetNumberTexts[i].AddError(NumberText.ErrorLevel.SegmentPairLevel, PluginResources.Error_NumberAdded);
			}
		}
	}
}