using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class Comparer
	{
		private readonly NumberText _first;
		private readonly NumberText _second;

		[Flags]
		public enum ResultDescription
		{
			DifferentSequences = 0,
			DifferentValues = 1,
			SameSequence = 2,
			Unlocalised = 4,
			Equal = 8,
			NotNumbers = 16
		}

		private static Dictionary<string, int> ComparisonScoreList { get; } = new()
		{
			[ComparisonConstants.SameNumberOfDigits] = 1,
			[ComparisonConstants.SameDigits] = 1,
			[ComparisonConstants.SameSequenceOfDigits] = 1,
			[ComparisonConstants.SameNumberOfSeparators] = 1,
			[ComparisonConstants.SameSequenceOfGroups] = 1,
			[ComparisonConstants.SameSeparators] = 1,
			[ComparisonConstants.SameSequenceOfSeparators] = 1,
		};

		public Dictionary<string, bool> ScoreList { get; } = new()
		{
			[ComparisonConstants.SameNumberOfDigits] = false,
			[ComparisonConstants.SameDigits] = false,
			[ComparisonConstants.SameSequenceOfDigits] = false,
			[ComparisonConstants.SameNumberOfSeparators] = false,
			[ComparisonConstants.SameSequenceOfGroups] = false,
			[ComparisonConstants.SameSeparators] = false,
			[ComparisonConstants.SameSequenceOfSeparators] = false,
		};

		public ResultDescription Result{ get; set; }

		public int SimilarityDegree
		{
			get
			{
				var keysOfSimilarities = ScoreList.Where(s => s.Value).Select(s => s.Key);

				var score = ComparisonScoreList.Where(item => keysOfSimilarities.Contains(item.Key)).Sum(item => item.Value);
				score -= ComparisonScoreList.Where(item => !keysOfSimilarities.Contains(item.Key)).Sum(item => item.Value);

				return Result.HasFlag(ResultDescription.Equal)
					? SameSequenceScore + 2
					: Result.HasFlag(ResultDescription.Unlocalised) ? SameSequenceScore + 1 : score;
			}
		}

		private static int SameSequenceScore => ComparisonScoreList.Values.Sum();

		public Comparer(NumberText first, NumberText second)
		{
			_first = first;
			_second = second;
		}

		public void AddScorePoints(string similarity)
		{
			ScoreList[similarity] = true;
		}

		public void Compare()
		{
			CompareSequences();
			InterpretResults();
		}

		private void InterpretResults()
		{
			if (SimilarityDegree == SameSequenceScore)
			{
				Result = ResultDescription.SameSequence;
			}

			if (_first.IsValidNumber && _second.IsValidNumber)
			{
				if (_first.Normalized == _second.Normalized) Result = ResultDescription.Equal;
				else Result |= ResultDescription.DifferentValues;
				if (_first.IsAmbiguous() || _second.IsAmbiguous()) Result = ResultDescription.Equal;
			}
			else
			{
				if (!Result.HasFlag(ResultDescription.SameSequence)) return;

				if (_first.IsValidNumber && !_second.IsValidNumber) Result = ResultDescription.Unlocalised;
				if (!_first.IsValidNumber && !_second.IsValidNumber) Result |= ResultDescription.NotNumbers;
			}
		}

		public bool ListEquals<T>(List<T> firstList, List<T> secondList)
		{
			if (firstList.Count != secondList.Count) return false;

			var firstListOrdered = firstList.OrderByDescending(e=>e);
			var secondListOrdered = secondList.OrderByDescending(e=>e);

			return firstListOrdered.SequenceEqual(secondListOrdered);
		}

		private void CompareSequences()
		{
			if (_first.Digits.Length == _second.Digits.Length) AddScorePoints(ComparisonConstants.SameNumberOfDigits);

			if (ListEquals(_first.Digits.ToList(), _second.Digits.ToList())) AddScorePoints(ComparisonConstants.SameDigits);

			if (_first.Digits == _second.Digits) AddScorePoints(ComparisonConstants.SameSequenceOfDigits);
			
			if (_first.ActualSeparators.Count == _second.ActualSeparators.Count) AddScorePoints(ComparisonConstants.SameNumberOfSeparators);

			if (_first.Groups.SequenceEqual(_second.Groups)) AddScorePoints(ComparisonConstants.SameSequenceOfGroups);

			if (ListEquals(_first.ActualSeparators, _second.ActualSeparators)) AddScorePoints(ComparisonConstants.SameSeparators);

			if (_first.ActualSeparators.SequenceEqual(_second.ActualSeparators)) AddScorePoints(ComparisonConstants.SameSequenceOfSeparators);
		}
	}
}