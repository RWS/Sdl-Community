using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberValidator
	{
		private List<string> _sourceNumberAreaSeparatorsList;
		private INumberVerifierSettings _settings;
		private List<string> _sourceDecimalSeparators;
		private string _sourceText;
		private List<string> _sourceThousandSeparators;
		private List<string> _targetDecimalSeparators;
		private string _targetText;
		private List<string> _targetThousandSeparators;
		private List<int> _visitedSourceIndexes;
		private List<int> _visitedTargetIndexes;
		private List<string> _targetNumberAreaSeparatorsList;

		public void Verify(string sourceText, string targetText, INumberVerifierSettings settings, out NumberTexts sourceNumberTexts, out NumberTexts targetNumberTexts, List<ExcludedRange> sourceExcludedRanges = null, List<ExcludedRange> targetExcludedRanges = null)
		{
			_settings = settings;
			_sourceText = sourceText;
			_targetText = targetText;

			if (_sourceText is null && _targetText is null)
			{
				sourceNumberTexts = null;
				targetNumberTexts = null;
				return;
			}

			_sourceThousandSeparators = GetAllowedThousandSeparators(true);
			_sourceDecimalSeparators = GetAllowedDecimalSeparators(true).Select(Regex.Unescape).ToList();

			_targetThousandSeparators = GetAllowedThousandSeparators(false);
			_targetDecimalSeparators = GetAllowedDecimalSeparators(false).Select(Regex.Unescape).ToList();

			_sourceNumberAreaSeparatorsList = GetSourceNumberAreaSeparators();
			_targetNumberAreaSeparatorsList = GetTargetNumberAreaSeparators();

			Verify(out sourceNumberTexts, out targetNumberTexts, sourceExcludedRanges, targetExcludedRanges);
		}

		private void AddError(NumberText targetTextArea, Comparer sourceTargetComparison)	
		{
			switch (sourceTargetComparison.Result)
			{
				case Comparer.ResultDescription.DifferentSequences:
					{
						targetTextArea.AddError(NumberText.ErrorLevel.SegmentPairLevel, PluginResources.Error_DifferentSequences);
						break;
					}

				case Comparer.ResultDescription.DifferentValues:
					targetTextArea.AddError(NumberText.ErrorLevel.SegmentPairLevel,
						PluginResources.Error_DifferentValues);
					break;

				case Comparer.ResultDescription.SameSequence | Comparer.ResultDescription.DifferentValues:
					{
						var message = PluginResources.Error_SameSequenceDifferentValues;
						targetTextArea.AddError(NumberText.ErrorLevel.SegmentPairLevel,
							message);

						break;
					}

				case Comparer.ResultDescription.TargetUnlocalised:
					{
						targetTextArea.AddError(NumberText.ErrorLevel.SegmentPairLevel,
							PluginResources.Error_MissingTargetSeparators);

						break;
					}
				case Comparer.ResultDescription.SourceUnlocalised:
					{
						targetTextArea.AddError(NumberText.ErrorLevel.SegmentPairLevel,
							PluginResources.Error_MissingSourceSeparators);

						break;
					}
			}
		}

		private void AddErrorsForUnpairedItems(NumberTexts textAreas, bool isSource)
		{
			for (var i = 0; i < textAreas.Texts.Count; i++)
			{
				var visitedIndexes = isSource ? _visitedSourceIndexes : _visitedTargetIndexes;
				var message = isSource ? PluginResources.Error_NumbersRemoved : PluginResources.Error_NumberAdded;
				if (!visitedIndexes.Contains(i))
					textAreas.Texts[i].AddError(NumberText.ErrorLevel.SegmentPairLevel, message);
			}
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

		private void CheckAlignment(NumberTexts sourceTextAreas, NumberTexts targetTextAreas)
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
				AddError(targetTextAreas[i], sourceTargetComparison);
			}

			for (var i = sourceTextAreas.Texts.Count; i < targetTextAreas.Texts.Count; i++)
			{
				targetTextAreas[i].AddError(NumberText.ErrorLevel.SegmentPairLevel, PluginResources.Error_NumberAdded);
			}
		}

		private void CrossCheck(NumberTexts sourceTextAreas, NumberTexts targetTextAreas)
		{
			_visitedTargetIndexes = new List<int>();
			_visitedSourceIndexes = new List<int>();

			var crossComparisons = new List<ComparisonItem>();
			for (var i = 0; i < targetTextAreas.Texts.Count; i++)
			{
				var sourceIndexList = sourceTextAreas.IndexesOf(targetTextAreas[i]);
				sourceIndexList.ForEach(sourceIndex => crossComparisons.Add(new ComparisonItem
				{
					SourceIndex = sourceIndex.Item1,
					TargetIndex = i,
					Comparer = sourceIndex.Item2
				}));


			}
			crossComparisons = crossComparisons.OrderByDescending(i => i.Comparer.SimilarityDegree).ToList();

			var actualComparisonScores = crossComparisons.Select(cc => cc.Comparer.SimilarityDegree).Distinct();
			foreach (var score in actualComparisonScores)
			{
				var comparisonsOfCurrentResult = crossComparisons.Where(cc => cc.Comparer.SimilarityDegree == score).ToList();

				if (!comparisonsOfCurrentResult.Any()) continue;

				//group comparisons by the element toBeCompared
				var grouped = comparisonsOfCurrentResult.GroupBy(c => c.TargetIndex);
				foreach (var group in grouped)
				{
					//since comparisonResults are ordered descendingly, the first in the group should be of highest similarity -> compare against that one
					var first = group.FirstOrDefault(IndexesNotVisisted);

					if (first == null) continue;

					AddError(targetTextAreas[first.TargetIndex], first.Comparer);
					UpdateVisitedList(first.SourceIndex, first.TargetIndex);
				}
			}

			AddErrorsForUnpairedItems(sourceTextAreas, true);
			AddErrorsForUnpairedItems(targetTextAreas, false);
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

		private List<string> GetTargetNumberAreaSeparators()
		{
			var allSeparators = new List<string>();

			if (!_settings.RequireLocalizations)
			{
				allSeparators.AddRange(_sourceThousandSeparators);
				allSeparators.AddRange(_sourceDecimalSeparators);
			}

			if (!_settings.PreventLocalizations)
			{
				allSeparators.AddRange(_targetThousandSeparators);
				allSeparators.AddRange(_targetDecimalSeparators);
			}

			allSeparators = allSeparators.Distinct().ToList();
			allSeparators.RemoveAll(s => s == Constants.NoSeparator);

			return allSeparators;
		}

		private List<string> GetSourceNumberAreaSeparators()
		{
			var allSeparators = new List<string>();

			allSeparators.AddRange(_sourceThousandSeparators);
			allSeparators.AddRange(_sourceDecimalSeparators);

			allSeparators = allSeparators.Distinct().ToList();
			allSeparators.RemoveAll(s => s == Constants.NoSeparator);

			return allSeparators;
		}

		private NumberFormattingSettings GetNumberFormattingSettings(bool isSource, List<ExcludedRange> excludedRanges) => new()
		{
			NumberAreaSeparators = isSource ? _sourceNumberAreaSeparatorsList : _targetNumberAreaSeparatorsList,
			ThousandSeparators = isSource ? _sourceThousandSeparators : _targetThousandSeparators,
			DecimalSeparators = isSource ? _sourceDecimalSeparators : _targetDecimalSeparators,
			OmitLeadingZero = isSource ? _settings.SourceOmitLeadingZero : _settings.TargetOmitLeadingZero,
			ExcludedRanges = excludedRanges
		};

		private bool IndexesNotVisisted(ComparisonItem first)
		{
			return !_visitedSourceIndexes.Contains(first.SourceIndex) && !_visitedTargetIndexes.Contains(first.TargetIndex);
		}

		private void UpdateVisitedList(int sourceIndex = -1, int targetIndex = -1)
		{
			if (sourceIndex != -1) _visitedSourceIndexes.Add(sourceIndex);
			if (targetIndex != -1) _visitedTargetIndexes.Add(targetIndex);
		}

		private void Verify(out NumberTexts sourceNumberTexts, out NumberTexts targetNumberTexts, List<ExcludedRange> sourceExcludedRanges, List<ExcludedRange> targetExcludedRanges)
		{
			sourceNumberTexts = new NumberTexts(_sourceText, GetNumberFormattingSettings(true, sourceExcludedRanges));
			targetNumberTexts = new NumberTexts(_targetText, GetNumberFormattingSettings(false, targetExcludedRanges));

			if (_settings.CheckInOrder) CheckAlignment(sourceNumberTexts, targetNumberTexts);
			else CrossCheck(sourceNumberTexts, targetNumberTexts);
		}
	}
}