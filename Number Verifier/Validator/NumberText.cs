using System;
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

		public enum ErrorLevel
		{
			SegmentPairLevel,
			TextAreaLevel,
			NumberLevel
		}

		public bool CanBeNumber => Errors[ErrorLevel.TextAreaLevel].Count == 0 && AreSeparatorsInValidPositions();

		public string Digits
		{
			get
			{
				var digits = GetWesternArabicNumber(Text);
				ActualSeparators.ForEach(sep => digits = digits.Replace(sep, ""));

				if (!string.IsNullOrWhiteSpace(Sign)) digits = digits.Replace(Sign, "");

				return digits;
			}
		}

		public Dictionary<ErrorLevel, List<Error>> Errors { get; set; } = new()
		{
			[ErrorLevel.SegmentPairLevel] = new List<Error>(),
			[ErrorLevel.TextAreaLevel] = new List<Error>(),
			[ErrorLevel.NumberLevel] = new List<Error>()
		};

		public List<Error> ErrorsList => Errors.Values.SelectMany(e => e).ToList();
		public string[] Groups => GetWesternArabicNumber(Text).Split(ActualSeparators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
		public bool IsValidNumber { get; set; }
		public int Length { get; set; }
		public string Normalized { get; set; }
		public List<string> ActualSeparators { get; set; }
		public string Sign { get; set; }

		public int StartIndex { get; set; }
		public string Text { get; set; }

		public void AddError(ErrorLevel level, string message)
		{
			Errors[level].Add(new Error
			{
				Message = message,
				ErrorLevel = level
			});
		}

		//The only scenario in which a number can be ambiguous is when it has only one and ambivalent separator placed in a thousand position
		//In any other case its meaning is determined
		public bool IsAmbiguous() => Normalized.Contains("u");

		public void SetAsValid(string normalized)
		{
			IsValidNumber = true;
			Normalized = normalized;
		}

		private bool AreSeparatorsInValidPositions()
		{
			return AreThousandsInValidPositions();
		}

		private bool AreThousandsInValidPositions()
		{
			if (ActualSeparators.Count < 2) return true;

			var lastSeparator = Text.IndexOf(ActualSeparators[ActualSeparators.Count - 1]);

			var j = 0;
			for (var i = lastSeparator - 4 - ActualSeparators[j].Length + 1; i >= 0; i -= 4 + ActualSeparators[j].Length - 1)
			{
				if (Text.Substring(i, ActualSeparators[j].Length) != ActualSeparators[j]) return false;
				j++;
			}

			return true;
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