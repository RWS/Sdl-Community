using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public static class ValidatorExtensions
	{
		public static string AddLeadingZero(this string normalized, bool hasSign)
		{
			normalized = normalized.Insert(hasSign ? 1 : 0, "0");
			return normalized;
		}

		public static void ExcludeRanges(this List<Match> textMatches, List<ExcludedRange> excludedRanges)
		{
			var exclusionList = new List<Match>();
			foreach (var match in textMatches)
			{
				if (excludedRanges.Contain(match.Index, match.Index + match.Length - 1)) exclusionList.Add(match);
			}

			textMatches.RemoveAll(tm => exclusionList.Contains(tm));
		}

		public static bool Contain(this List<ExcludedRange> ranges, int left, int right)
			=> ranges?.Any(range => range.Contains(left, right)) ?? false;

		public static List<ExcludedRange> MergeAdjacentRanges(this List<ExcludedRange> ranges)
		{
			ranges = ranges.OrderBy(r => r.LeftLimit).ToList();
			var limits = new Stack<ExcludedRange>();

			var allLimits = new List<ExcludedRange>();
			ranges.ForEach(r =>
			{
				allLimits.Add(new ExcludedRange { LeftLimit = r.LeftLimit });
				allLimits.Add(new ExcludedRange { RightLimit = r.RightLimit });
			});

			var newRanges = new List<ExcludedRange>();
			foreach (var limit in allLimits)
			{
				if (limits.Count == 0)
				{
					limits.Push(limit);
					continue;
				}

				if (IsLeftLimit(limits.Peek()))
				{
					if (IsRightLimit(limit))
					{
						if (limits.Count == 1)
						{
							newRanges.Add(new ExcludedRange
							{
								LeftLimit = limits.Peek().LeftLimit,
								RightLimit = limit.RightLimit
							});
						}
						limits.Pop();
					}
					else
					{
						limits.Push(limit);
					}
				}
			}

			return newRanges;
		}

		private static bool IsRightLimit(ExcludedRange limit)
		{
			return limit.RightLimit != -1;
		}

		private static bool IsLeftLimit(ExcludedRange range)
		{
			return range.LeftLimit != -1;
		}

		public static bool IsLeadingZeroOmitted(NumberToken numberToken)
		{
			var numberText = numberToken.Text;
			return numberToken.HasSign
				? numberToken.Text[1].ToString().Equals(numberToken.DecimalSeparator)
				: numberText[0].ToString().Equals(numberToken.DecimalSeparator);
		}

		public static string Normalize(this NumberToken numberToken)
		{
			var normalized = "";
			var beforeDecimal = true;
			var previousWasSign = false;
			foreach (var part in numberToken.NumberParts)
			{
				switch (part.Type)
				{
					case NumberPart.NumberType.Number:
						var number = part.Value;
						if (beforeDecimal && numberToken.NumberParts.All(p => p.Type != NumberPart.NumberType.GroupSeparator))
						{
							number = GetNumberWithThousandSeparatorsAdded(part.Value);
						}
						normalized += number;
						break;

					case NumberPart.NumberType.DecimalSeparator:
						normalized += "d";
						beforeDecimal = false;
						break;

					case NumberPart.NumberType.GroupSeparator:
						if (!previousWasSign) normalized += "t";
						break;

					case NumberPart.NumberType.Sign:
						normalized += part.Value == "+" ? "p" : "n";
						previousWasSign = true;
						break;
				}
				if (part.Type != NumberPart.NumberType.Sign && previousWasSign) previousWasSign = false;
			}

			if (IsLeadingZeroOmitted(numberToken))
				normalized = normalized.AddLeadingZero(numberToken.HasSign);

			return normalized.Normalize(System.Text.NormalizationForm.FormKC);
		}

		public static string ToRegexPattern(this List<string> toBeJoined)
		{
			var oneLetterSeparators = new List<string>();
			var longerSeparators = new List<string>();
			foreach (var separator in toBeJoined)
			{
				if (separator.Length == 1) oneLetterSeparators.Add(separator);
				else longerSeparators.Add(separator);
			}

			var joinedListString = "";
			if (oneLetterSeparators.Count > 0)
			{
				joinedListString = string.Join("", oneLetterSeparators);
				joinedListString = $"[{joinedListString}]";
			}

			if (longerSeparators.Count > 0)
			{
				joinedListString = $"({joinedListString}|{string.Join("|", longerSeparators)})";
			}

			return joinedListString;
		}

		private static string GetNumberWithThousandSeparatorsAdded(string normalizedIntegerPart)
		{
			for (var i = normalizedIntegerPart.Length - 3; i > 0; i -= 3)
			{
				normalizedIntegerPart = normalizedIntegerPart.Insert(i, "t");
			}

			return normalizedIntegerPart;
		}
	}
}