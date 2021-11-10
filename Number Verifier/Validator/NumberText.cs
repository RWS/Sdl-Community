using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberText
	{
		private readonly Dictionary<char, char> _hindiDictionary = new()
		{
			{ '٠', '0' },
			{ '١', '1' },
			{ '٢', '2' },
			{ '٣', '3' },
			{ '٤', '4' },
			{ '٥', '5' },
			{ '٦', '6' },
			{ '٧', '7' },
			{ '٨', '8' },
			{ '٩', '9' }
		};

		public int StartIndex{ get; set; }
		public int Length{ get; set; }

		public int GetErrorsTotal()
		{
			var errorsTotal = 0;
			Errors.Values.ToList().ForEach(e => errorsTotal += e.Count);

			return errorsTotal;
		}

		public enum ComparisonResult
		{
			DifferentSequence,
			SameSequence,
			DifferentValues,
			Unlocalised,
			Equal
		}

		public enum ErrorLevel
		{
			SegmentPairLevel,
			SegmentLevel,
			TextAreaLevel,
			NumberLevel
		}

		public Dictionary<ErrorLevel, List<Error>> Errors { get; set; } = new()
		{
			[ErrorLevel.SegmentPairLevel] = new List<Error>(),
			[ErrorLevel.SegmentLevel] = new List<Error>(),
			[ErrorLevel.TextAreaLevel] = new List<Error>(),
			[ErrorLevel.NumberLevel] = new List<Error>()
		};

		public bool IsValidNumber { get; set; }

		public bool CanBeNumber => Errors[ErrorLevel.TextAreaLevel].Count == 0;

		public string Normalized { get; set; }

		public List<string> Separators { get; set; }

		public string Digits
		{
			get
			{
				var signature = Text;

				Separators.ForEach(sep => signature = signature.Replace(sep, ""));

				return signature;
			}
		}

		public string Text { get; set; }

		public void AddError(ErrorLevel level, string message, string suggestion = "")
		{
			Errors[level].Add(new Error
			{
				Message = message,
				Suggestion = suggestion
			});
		}

		/// <summary>
		/// Important to note that for comparing target and source numbers, we first have to know that they're valid as numbers to be able to output a corect result
		/// We cannot base our predictions solely on char sequence as separators have different meanings based on language settings
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public ComparisonResult Compare(NumberText other)
		{
			if (other is null) return ComparisonResult.DifferentSequence;

			var sourceWesternArabicNumberText = GetWesternArabicNumber(Text);
			var targetWesternArabicNumberText = GetWesternArabicNumber(other.Text);

			var result = ComparisonResult.DifferentSequence;

			//sequence level
			if (sourceWesternArabicNumberText == targetWesternArabicNumberText) result = ComparisonResult.SameSequence;

			if (result == ComparisonResult.SameSequence &&
				IsValidNumber &&
				!other.IsValidNumber)
				//this condition can only be satisified if we're in "RequireLocalizations" mode; in any other mode, they'd both be valid if one of them is (and they'd be equal), since they both can have the same separators
				result = ComparisonResult.Unlocalised;

			if (result == ComparisonResult.SameSequence &&
				!IsValidNumber &&
				other.IsValidNumber)
				result = ComparisonResult.SameSequence;

			//number level
			if (!IsValidNumber || !other.IsValidNumber) return result;

			result = Normalized == other.Normalized ?
				ComparisonResult.Equal :
				ComparisonResult.DifferentValues;

			if (result != ComparisonResult.Equal &&
				Digits == other.Digits &&
				Separators.Count == 1 &&
				other.Separators.Count == 1)
			{
				//The only scenario in which a number can be ambiguous is when it has only one and ambivalent separator placed in a thousand position
				//In any other case its meaning is determined
				if (Normalized.Contains("u") || other.Normalized.Contains("u"))
				{
					result = ComparisonResult.Equal;
				}
			}

			return result;
		}

		public void SetAsValid(string normalized)
		{
			IsValidNumber = true;
			Normalized = normalized;
		}

		private string GetWesternArabicNumber(string text)
		{
			var westernArabicNumber = text;
			foreach (var easternArabicNumber in _hindiDictionary.Keys)
			{
				var westernArabicDigit = _hindiDictionary[easternArabicNumber];
				westernArabicNumber = westernArabicNumber.Replace(easternArabicNumber, westernArabicDigit);
			}

			return westernArabicNumber;
		}
	}
}