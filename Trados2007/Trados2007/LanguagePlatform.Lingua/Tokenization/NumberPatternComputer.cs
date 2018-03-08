using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	// This is only public so that we can write test cases against it. It's not intended to be
	//  used publicly.
	public class NumberFormatData
	{
		public List<string> Digits;
		public List<SeparatorCombination> SeparatorCombinations;

		public string PositiveSigns;
		public string NegativeSigns;

		public int[] NumberGroupSizes;
		public int NumberNegativePattern;

		public NumberFormatData()
		{
			Digits = new List<string>();
			SeparatorCombinations = new List<SeparatorCombination>();
		}

#if false
		// only needed for canonical pattern computation (not used)

		public void Merge(NumberFormatData other)
		{
			if (other == null)
				throw new ArgumentNullException();

			PositiveSigns = Core.StringUtilities.MergeStrings(PositiveSigns, other.PositiveSigns);
			NegativeSigns = Core.StringUtilities.MergeStrings(NegativeSigns, other.NegativeSigns);

			foreach (string s in other.Digits)
			{
				if (!Digits.Contains(s))
					Digits.Add(s);
			}

			foreach (SeparatorCombination sc in other.SeparatorCombinations)
			{
				if (!SeparatorCombinations.Contains(sc))
					SeparatorCombinations.Add(sc);
			}
		}
#endif

		/// <summary>
		/// <param name="augmentGroupSeparators">If true, blanks/nbsp's will be interchangeable</param>
		/// </summary>
		public bool AddSeparatorCombination(string groupSeparator, string decimalSeparator,
			bool augmentGroupSeparators)
		{
			SeparatorCombination sc = new SeparatorCombination(groupSeparator, decimalSeparator, augmentGroupSeparators);
			if (!SeparatorCombinations.Contains(sc))
			{
				SeparatorCombinations.Add(sc);
				return true;
			}
			else
				return false;
		}

		public string GetCombinedDecimalSeparators()
		{
			HashSet<char> s = new HashSet<char>();
			System.Text.StringBuilder sb = new StringBuilder();

			foreach (SeparatorCombination sc in SeparatorCombinations)
			{
				if (String.IsNullOrEmpty(sc.DecimalSeparators))
					continue;

				for (int p = 0; p < sc.DecimalSeparators.Length; ++p)
				{
					char sep = sc.DecimalSeparators[p];
					if (!s.Contains(sep))
					{
						sb.Append(sep);
						s.Add(sep);
					}
				}
			}

			System.Diagnostics.Debug.Assert(s.Count > 0);

			return sb.ToString();
		}
	}

	public class SeparatorCombination
	{
		public string GroupSeparators;
		public string DecimalSeparators;

		/// <summary>
		/// <param name="augmentGroupSeparators">If true, blanks/nbsp's will be interchangeable</param>
		/// </summary>
		public SeparatorCombination(System.Globalization.CultureInfo culture,
			bool augmentGroupSeparators)
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Invalid argument (neutral culture)");

			Init(culture.NumberFormat.NumberGroupSeparator,
				culture.NumberFormat.NumberDecimalSeparator, augmentGroupSeparators);
		}

		/// <summary>
		/// <param name="augmentGroupSeparators">If true, blanks/nbsp's will be interchangeable</param>
		/// </summary>
		public SeparatorCombination(string groupSeparators, string decimalSeparators,
			bool augmentGroupSeparators)
		{
			Init(groupSeparators, decimalSeparators, augmentGroupSeparators);
		}

		private void Init(string groupSeparators, string decimalSeparators,
			bool augmentGroupSeparators)
		{
			GroupSeparators = groupSeparators;
			DecimalSeparators = decimalSeparators;

			if (GroupSeparators != null && augmentGroupSeparators)
			{
				bool hasBlank = GroupSeparators.IndexOf(' ') >= 0;
				bool hasNbsp = GroupSeparators.IndexOf('\u00A0') >= 0;

				if (hasBlank & !hasNbsp)
					GroupSeparators += '\u00A0';
				if (!hasBlank && hasNbsp)
					GroupSeparators += ' ';
			}
		}

		/// <summary>
		/// true iff one separator is the full stop and the other is the comma
		/// </summary>
		public bool IsSwappable()
		{
			if (!String.IsNullOrEmpty(GroupSeparators)
				&& !String.IsNullOrEmpty(DecimalSeparators)
				&& GroupSeparators.Length == 1
				&& DecimalSeparators.Length == 1)
			{
				char first = GroupSeparators[0];
				char second = DecimalSeparators[0];

				return (first == '.' || first == ',')
					&& (second == '.' || second == ',')
					&& first != second;
			}
			else
				return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != this.GetType())
				return false;
			SeparatorCombination other = (SeparatorCombination)obj;
			return String.Equals(GroupSeparators, other.GroupSeparators)
				&& String.Equals(DecimalSeparators, other.DecimalSeparators);
		}

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode(object)"/>
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

	public class NumberPatternComputer
	{
		public static readonly string DefaultDigits = "0123456789";
		public static readonly string FullWidthDigits = "\uFF10\uFF11\uFF12\uFF13\uFF14\uFF15\uFF16\uFF17\uFF18\uFF19";

		/// <summary>
		/// If true, trailing signs are enabled for cultures where the number format info
		/// suggests that signs may also appear "after" the numeric part, which is the case
		/// for right-to-left ("bidi") cultures. If that's the case, the generated recognizer
		/// will support both leading as well as trailing signs. If false, only leading signs are supported.
		/// </summary>
		internal static readonly bool AllowTrailingSign = true;
		// NOTE: only supported by FSTs, not by RX recognizers

		/// <summary>
		/// If true, non-standard digit groupings (such as in Hindi) are supported. If false,
		/// only the standard three-digit grouping is supported.	
		/// </summary>
		internal static readonly bool SupportNonstandardGrouping = false;
		// NOTE: only supported by FSTs, not by RX recognizers


		/// <summary>
		/// Returns true if the decimal and group separators are not those of en-US
		/// </summary>
		public static bool DoAddENUSSeparators(System.Globalization.CultureInfo culture)
		{
			string lng = culture.TwoLetterISOLanguageName;
			return lng.Equals("fr", StringComparison.OrdinalIgnoreCase);
		}

		public static bool DoAddFullwidthVariantDigits(System.Globalization.CultureInfo culture)
		{
			string lng = culture.TwoLetterISOLanguageName;

			return lng.Equals("ja", StringComparison.OrdinalIgnoreCase)
				|| lng.Equals("zh", StringComparison.OrdinalIgnoreCase)
				|| lng.Equals("ko", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// <param name="augmentWhitespaceGroupSeparators">If true, blanks/nbsp's will be interchangeable</param>
		/// </summary>
		public static NumberFormatData GetNumberFormatData(System.Globalization.CultureInfo culture,
			bool addSeparatorVariants, bool augmentWhitespaceGroupSeparators)
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Cannot compute number format information for neutral cultures");

			string nativeDigits = String.Join("", culture.NumberFormat.NativeDigits);

			if (nativeDigits.Length != 10)
				return null;

			NumberFormatData result = new NumberFormatData();

			result.Digits.Add(nativeDigits);
			if (!String.Equals(nativeDigits, DefaultDigits))
				result.Digits.Add(DefaultDigits);

			if (DoAddFullwidthVariantDigits(culture) &&
				!result.Digits.Contains(FullWidthDigits))
			{
				result.Digits.Add(FullWidthDigits);
			}

			result.NegativeSigns = culture.NumberFormat.NegativeSign;
			result.PositiveSigns = culture.NumberFormat.PositiveSign;
			result.NumberGroupSizes = culture.NumberFormat.NumberGroupSizes;
			result.NumberNegativePattern = culture.NumberFormat.NumberNegativePattern;

			result.AddSeparatorCombination(culture.NumberFormat.NumberGroupSeparator,
				culture.NumberFormat.NumberDecimalSeparator, augmentWhitespaceGroupSeparators);
			result.AddSeparatorCombination(culture.NumberFormat.CurrencyGroupSeparator,
				culture.NumberFormat.CurrencyDecimalSeparator, augmentWhitespaceGroupSeparators);

			if (addSeparatorVariants)
			{
				switch (culture.TwoLetterISOLanguageName.ToLowerInvariant())
				{
				case "fr":
					result.AddSeparatorCombination(",", ".", false);
					break;
				case "pl":
					result.AddSeparatorCombination(".", ",", false);
					break;
				default:
					break;
				}

				for (int p = result.SeparatorCombinations.Count - 1; p >= 0; --p)
				{
					SeparatorCombination sc = result.SeparatorCombinations[p];
					if (sc.IsSwappable())
					{
						// note: swapping separators here
						result.AddSeparatorCombination(sc.DecimalSeparators, sc.GroupSeparators, false);
					}
				}
			}

			return result;
		}

#if false
		public static NumberFormatData GetCanonicalNumberFormatData(bool addSeparatorVariants,
			bool augmentWhitespaceGroupSeparators)
		{
			NumberFormatData result = new NumberFormatData();
			foreach (System.Globalization.CultureInfo ci in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures))
			{
				if (ci.IsNeutralCulture)
					continue;

				NumberFormatData ciData = GetNumberFormatData(ci,
					addSeparatorVariants, augmentWhitespaceGroupSeparators);
				if (ciData == null)
					continue;

				if (result == null)
					result = ciData;
				else
					result.Merge(ciData);
			}
			return result;
		}
#endif

		internal static void AppendDisjunction(System.Text.StringBuilder sb,
			char symbol,
			char output,
			ref bool first)
		{
			if (!first)
				sb.Append("|");
			sb.AppendFormat("<{0}:{1}>", symbol, output);
			first = false;
		}

		private static bool IsBalanced(string s)
		{
			if (String.IsNullOrEmpty(s))
				return true;

			int open = 0;

			for (int p = 0; p < s.Length; ++p)
			{
				if (s[p] == '(')
				{
					++open;
				}
				else if (s[p] == ')')
				{
					--open;
					if (open < 0)
						return false;
				}
			}
			return open == 0;
		}

		internal static void AppendDisjunction(System.Text.StringBuilder sb,
			string symbols,
			char output,
			ref bool first)
		{
			if (!String.IsNullOrEmpty(symbols))
			{
				for (int p = 0; p < symbols.Length; ++p)
					AppendDisjunction(sb, symbols[p], output, ref first);
			}
		}

		internal static void AppendDisjunction(System.Text.StringBuilder sb,
			IList<char> symbols,
			char output,
			ref bool first)
		{
			if (symbols == null || symbols.Count == 0)
				return;

			for (int p = 0; p < symbols.Count; ++p)
				AppendDisjunction(sb, symbols[p], output, ref first);
		}

		private static string ComputeSign(string positiveSigns, string negativeSigns, bool optional)
		{
			System.Text.StringBuilder sb = new StringBuilder();

			sb.Append("(");

			bool first = true;

			AppendDisjunction(sb, positiveSigns, '+', ref first);
			AppendDisjunction(sb, negativeSigns, '-', ref first);

			if (optional)
				sb.Append(")?");
			else
				sb.Append(")");

			return sb.ToString();
		}

		private static string ComputeSingleDigit(IList<string> digitSet)
		{
			StringBuilder singleDigitRX = new StringBuilder();

			bool first = true;

			singleDigitRX.Append("(");

			foreach (string digits in digitSet)
			{
				for (int i = 0; i < 10; ++i)
				{
					AppendDisjunction(singleDigitRX, digits[i], DefaultDigits[i], ref first);
				}
			}

			singleDigitRX.Append(")");

			return singleDigitRX.ToString();
		}

		public static string ComputeFSTPattern(NumberFormatData data,
			bool treatFirstSeparatorsAsPrimarySeparators,
			bool appendWordTerminator)
		{
			// TODO support non-standard groupings (Hindi)
			// TODO support negative patterns (particularly minus sign following numeric part)

			// we also only recognize prefixed signs, but no optional blank between the sign and the number.
			// Other number patters (such as "(x)" for a negative number in US accounting) are unsupported. 

			// ---------------------------------------- compute RX for a single digit

			string dig = ComputeSingleDigit(data.Digits);

			// TODO observe data.NegativeSigns, data.PositiveSigns
			// TODO escape signs just in case they interfere with FST symbols
			string optSign = ComputeSign("+\uFF0B", "-\u2013\u2212\uFF0D", true);
			string reqSign = ComputeSign("+\uFF0B", "-\u2013\u2212\uFF0D", false);

			// --- the ungrouped case (optional group separators)

			StringBuilder ungrouped = new StringBuilder();
			string decimalSeparators = data.GetCombinedDecimalSeparators();

			ungrouped.AppendFormat("({0}+((", dig);
			bool first = true;
			for (int p = 0; p < decimalSeparators.Length; ++p)
			{
				if (p == 0 && treatFirstSeparatorsAsPrimarySeparators)
				{
					AppendDisjunction(ungrouped, decimalSeparators[p], 'D', ref first);
				}
				else
				{
					AppendDisjunction(ungrouped, decimalSeparators[p], 'd', ref first);
				}
			}

			ungrouped.AppendFormat("){0}+)?)", dig);

			System.Diagnostics.Debug.Assert(IsBalanced(ungrouped.ToString()));

			// --- the grouped case

			StringBuilder grouped = new StringBuilder();

			grouped.AppendFormat("{0}({1}{2}?)?(", dig, dig, dig);

			first = true;
			foreach (SeparatorCombination sc in data.SeparatorCombinations)
			{
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(sc.GroupSeparators));
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(sc.DecimalSeparators));

				System.Text.StringBuilder scPattern = new StringBuilder();

				scPattern.Append("(");
				for (int p = 0; p < sc.GroupSeparators.Length; ++p)
				{
					// we expect the same group separators in the number sequence, not _any_
					//  group separator. Can't use sth as "[gs][0-9]{3}".

					char outputSymbol = 'g';
					if (first && treatFirstSeparatorsAsPrimarySeparators)
						outputSymbol = 'G';

					if (p > 0)
						scPattern.Append("|");

					scPattern.AppendFormat("((<{0}:{1}>{2}{3}{4})+)",
						sc.GroupSeparators[p], outputSymbol, dig, dig, dig);
				}
				scPattern.Append(")");
				System.Diagnostics.Debug.Assert(IsBalanced(scPattern.ToString()));

				// the optional fractional part. NOTE: we assume the group and decimal seps are disjunct.

				scPattern.Append("((");
				for (int p = 0; p < sc.DecimalSeparators.Length; ++p)
				{
					char outputSymbol = 'd';
					if (first && treatFirstSeparatorsAsPrimarySeparators)
						outputSymbol = 'D';

					if (p > 0)
						scPattern.Append("|");

					scPattern.AppendFormat("<{0}:{1}>",
						sc.DecimalSeparators[p], outputSymbol);
				}
				scPattern.AppendFormat("){0}+)?", dig);

				System.Diagnostics.Debug.Assert(IsBalanced(scPattern.ToString()));

				if (!first)
					grouped.AppendFormat("|");
				grouped.AppendFormat("({0})", scPattern.ToString());

				if (first)
					first = false;
			}

			grouped.Append(")");

			System.Diagnostics.Debug.Assert(IsBalanced(grouped.ToString()));

			// -----

			StringBuilder result = new StringBuilder();

			if (!AllowTrailingSign || data.NumberNegativePattern < 3)
			{
				// sign precedes numeric part
				result.AppendFormat("({0}({1}|{2}))", optSign,
					grouped.ToString(), ungrouped.ToString());
			}
			else
			{
				// sign follows numeric part, but int.ToString() may generate both forms, 
				//  so we need to build a recognizer which handles both.
				// NOTE using a required sign for leading sign to avoid ambiguous RX/matches
				//  for numbers w/o signs
				result.AppendFormat("(({0}({1}|{2}))|(({1}|{2}){3}))", reqSign,
					grouped.ToString(), ungrouped.ToString(), optSign);
			}

			if (appendWordTerminator)
			{
				// Append "word terminator"
				result.Append("#>");
			}

			return result.ToString();
		}

	}
}
