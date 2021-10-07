using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Helpers;

namespace Sdl.Community.NumberVerifier.Model
{
	public class Number
	{
		private const string Signs = "+-−";
		private readonly List<string> _decimalSeparators;
		private readonly List<string> _thousandSeparators;
		private string _characterAsString;
		private int _indexOfAddition;

		private Number(List<string> thousandSeparators, List<string> decimalSeparators)
		{
			_thousandSeparators = thousandSeparators;
			_decimalSeparators = decimalSeparators;

			DecimalPlaceFromRight = (-1, null);
		}

		public int DigitCount => DigitsReversed.Count;
		public string Sign { get; set; }

		public (List<int>, string) ThousandSeparatorsPlacesFromRight { get; set; } = (null, null);

		private (int, string) DecimalPlaceFromRight { get; set; }
		private List<char> DigitsReversed { get; } = new List<char>();

		public static List<Number> Parse(string text, List<string> thousandSeparators, List<string> decimalSeparators, bool omitZero)
		{
			text = text.Trim();
			var numberList = new List<Number> { new Number(thousandSeparators, decimalSeparators) };
			var index = 0;

			var betweenNumbers = false;
			for (var i = text.Length - 1; i >= 0; i--)
			{
				var currentCharacter = text[i];
				var currentNumber = numberList[index];
				if (!betweenNumbers)
				{
					betweenNumbers = !currentNumber.AddCharacter(currentCharacter);
				}
				else
				{
					if (!char.IsDigit(currentCharacter)) continue;

					betweenNumbers = false;
					if (!currentNumber.IsEmpty())
					{
						numberList.Add(new Number(thousandSeparators, decimalSeparators));
						index++;
					}

					numberList[index].AddCharacter(currentCharacter);
				}
			}

			if (omitZero)
			{
				numberList.ForEach(AddZero);
			}
			return numberList;
		}

		public void InsertSeparator(List<string> reversedNumber, int skippedDigitsTotal, string separator)
		{
			var index = GetSeparatorPlace(reversedNumber, skippedDigitsTotal);
			reversedNumber.Insert(index, separator);
		}

		public string ToNumber(bool normalize = false)
		{
			var digitsReversed = DigitsReversed.Select(d => d.ToString()).ToList();

			if (Sign != null)
			{
				digitsReversed.Add(normalize ? "m" : Sign);
			}

			if (IsDecimalPlaceSet())
			{
				digitsReversed.Insert(DecimalPlaceFromRight.Item1, normalize ? "m" : DecimalPlaceFromRight.Item2);
			}

			if (normalize && _thousandSeparators.Contains(Constants.NoSeparator))
			{
				InsertSeparatorsAtAllThousandPlaces(digitsReversed);
			}
			else if (ThousandSeparatorsPlacesFromRight != (null, null))
			{
				InsertThousandSeparators(normalize, digitsReversed);
			}

			digitsReversed.Reverse();
			return string.Join("", digitsReversed);
		}

		private static void AddZero(Number currentNumber)
		{
			var isDecimalNumberMissingLeadingZero = currentNumber.IsDecimalPlaceSet() &&
													currentNumber.DecimalPlaceFromRight.Item1 == currentNumber.DigitCount;
			if (isDecimalNumberMissingLeadingZero)
			{
				currentNumber.DigitsReversed.Add('0');
			}
		}

		private static int GetSeparatorPlace(List<string> reversedNumber, int skippedDigitsTotal)
		{
			var index = 0;
			for (; index < reversedNumber.Count; index++)
			{
				var number = reversedNumber[index];
				if (skippedDigitsTotal != 0)
				{
					if (char.IsDigit(char.Parse(number)))
					{
						skippedDigitsTotal--;
					}
					continue;
				}

				break;
			}

			return index;
		}

		/// <summary>
		/// Adds characters to a number
		/// </summary>
		/// <param name="character"></param>
		/// <returns><see langword="true"/> -> the character was added successfully
		/// <para><see langword="false"/> -> character was not part of a number; number ended</para></returns>
		private bool AddCharacter(char character)
		{
			_characterAsString = character.ToString();
			_indexOfAddition = DigitsReversed.Count;
			if (!char.IsDigit(character))
			{
				if (_indexOfAddition > 0 && Signs.Contains(_characterAsString))
				{
					SetSign(_characterAsString);
					return false;
				}

				if (!IsAllowedThousandSeparator() && !IsAllowedDecimalSeparator()) return false;

				if (!IsDecimalPlaceDefined() && !IsValidThousandPlace() && IsAllowedDecimalSeparator())
				{
					SetDecimalPlace();
					return true;
				}

				if (!IsValidThousandPlace() || !IsAllowedThousandSeparator()) return false;

				AddThousandSeparatorPlace();
				if (!IsDecimalPlaceDefined()) SetToNoDecimalPlace();

				return true;
			}

			DigitsReversed.Add(character);
			return true;
		}

		private void AddThousandSeparatorPlace()
		{
			if (ThousandSeparatorsPlacesFromRight == (null, null))
				ThousandSeparatorsPlacesFromRight = (new List<int>(), _characterAsString);

			ThousandSeparatorsPlacesFromRight.Item1.Add((_indexOfAddition));
		}

		private void InsertSeparatorsAtAllThousandPlaces(List<string> digitsReversed)
		{
			var decimalPlaceOffset = IsDecimalPlaceDefined() ? DecimalPlaceFromRight.Item1 : 0;
			for (var i = decimalPlaceOffset + 3; i < digitsReversed.Count - 1; i += 3)
			{
				InsertSeparator(digitsReversed, i, "m");
			}
		}

		private void InsertThousandSeparators(bool normalize, List<string> digitsReversed)
		{
			foreach (var separatorPlaceFromRight in ThousandSeparatorsPlacesFromRight.Item1)
			{
				InsertSeparator(digitsReversed, separatorPlaceFromRight, normalize ? "m" : ThousandSeparatorsPlacesFromRight.Item2);
			}
		}

		private bool IsAllowedDecimalSeparator()
		{
			var currentCharacter = char.TryParse(_characterAsString, out var character)
				? $@"\u{Convert.ToUInt16(character):X4}"
				: _characterAsString;

			return _decimalSeparators.Contains(currentCharacter);
		}

		private bool IsAllowedThousandSeparator()
		{
			var currentCharacter = char.TryParse(_characterAsString, out var character)
				? $@"\u{Convert.ToUInt16(character):X4}"
				: _characterAsString;

			return ThousandSeparatorsPlacesFromRight == (null, null)
				? _thousandSeparators.Contains(currentCharacter)
				: _characterAsString == ThousandSeparatorsPlacesFromRight.Item2;
		}

		private bool IsDecimalPlaceDefined()
		{
			return DecimalPlaceFromRight.Item1 != -1;
		}

		private bool IsDecimalPlaceSet()
		{
			return DecimalPlaceFromRight.Item1 > 0;
		}

		private bool IsEmpty()
		{
			return DigitsReversed.Count == 0;
		}

		private bool IsValidThousandPlace()
		{
			if (_indexOfAddition < 3) return false;
			var offset = IsDecimalPlaceDefined() ? DecimalPlaceFromRight.Item1 : 0;
			return (_indexOfAddition - offset) % 3 == 0;
		}

		private void SetDecimalPlace()
		{
			DecimalPlaceFromRight = (_indexOfAddition, _characterAsString);
		}

		private void SetSign(string character)
		{
			Sign = character;
		}

		private void SetToNoDecimalPlace()
		{
			DecimalPlaceFromRight = (0, null);
		}
	}
}