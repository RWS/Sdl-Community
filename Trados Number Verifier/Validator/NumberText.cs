using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;

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
		}

		public List<string> ActualSeparators { get; set; }

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

		public Dictionary<ErrorLevel, List<Error>> Errors { get; set; }

		public List<Error> ErrorsList => Errors.Values.SelectMany(e => e).ToList();
		public string[] Groups => GetWesternArabicNumber(Text).Split(ActualSeparators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
		public bool IsValidNumber => Token.Valid;
		public int Length { get; set; }
		public string Normalized { get; set; }
		public string Sign { get; set; }

		public int StartIndex { get; set; }
		public string Text { get; set; }
		public NumberToken Token { get; set; }

		public void AddError(ErrorLevel level, string message)
		{
			Errors[level].Add(new Error
			{
				Message = message,
				ErrorLevel = level
			});
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