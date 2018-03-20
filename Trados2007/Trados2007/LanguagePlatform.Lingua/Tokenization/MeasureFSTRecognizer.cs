using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.Resources;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class MeasureFSTRecognizer : Recognizer
	{
		private FSTRecognizer _FSTRecognizer;

		public static Recognizer Create(Core.Resources.IResourceDataAccessor resourceDataAccessor, 
			System.Globalization.CultureInfo culture, int priority)
		{
			MeasureFSTRecognizer result = new MeasureFSTRecognizer(culture, priority,
				resourceDataAccessor);

			SetAdditionalOptions(result, culture);

			return result;
		}

		public static Recognizer Create(System.Globalization.CultureInfo culture, int priority)
		{
			MeasureFSTRecognizer result = new MeasureFSTRecognizer(culture, priority);

			SetAdditionalOptions(result, culture);

			return result;
		}

		private static void SetAdditionalOptions(MeasureFSTRecognizer result, 
			System.Globalization.CultureInfo culture)
		{
			result.OnlyIfFollowedByNonwordCharacter
				= true; // otherwise "123 ABC" will be recognized as "123 A" "BC" in Japanese
		}

		public MeasureFSTRecognizer(System.Globalization.CultureInfo culture, int priority)
			: base(TokenType.Measurement, priority, "Measurement", "MeasureFSTRecognizer")
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Cannot compute measurement patterns for neutral cultures");
			if (culture.NumberFormat == null)
				throw new ArgumentException("No number format info available for the specified culture");

			LanguagePlatform.Lingua.FST.FST fst = CreateFST(culture, Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture));
			_FSTRecognizer = new FSTRecognizer(fst, culture);
		}

		/// <summary>
		/// Attempts to get the compiled FST from the resources, and if that fails, will create it from scratch 
		/// </summary>
		public MeasureFSTRecognizer(System.Globalization.CultureInfo culture, int priority, 
			Core.Resources.IResourceDataAccessor accessor)
			: base(TokenType.Measurement, priority, "Measurement", "MeasureFSTRecognizer")
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Cannot compute measurement patterns for neutral cultures");
			if (culture.NumberFormat == null)
				throw new ArgumentException("No number format info available for the specified culture");
			if (accessor == null)
				accessor = new ResourceFileResourceAccessor();

			LanguagePlatform.Lingua.FST.FST fst = null;

			bool attemptLoad = true;

			if (attemptLoad 
				&& accessor.GetResourceStatus(culture, Core.Resources.LanguageResourceType.MeasurementFST, true) !=
				Core.Resources.ResourceStatus.NotAvailable)
			{
				// TODO should _Culture be set to the _actual_ culture of the loaded FST, i.e. 
				//  the invariant culture for the generic/canonical one?

				byte[] data = accessor.GetResourceData(culture, Core.Resources.LanguageResourceType.MeasurementFST, true);
				if (data == null)
					throw new Core.LanguagePlatformException(Core.ErrorCode.ResourceNotAvailable);

				fst = LanguagePlatform.Lingua.FST.FST.Create(data);
			}
			else
				fst = CreateFST(culture, Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture));

			_FSTRecognizer = new FSTRecognizer(fst, culture);
		}

		public override Core.Tokenization.Token Recognize(string s, 
			int from, bool allowTokenBundles, ref int consumedLength)
		{
			consumedLength = 0;

			List<FSTMatch> matches = _FSTRecognizer.ComputeMatches(s, from, false, 0, true);
			if (matches == null || matches.Count == 0)
				return null;

			List<Core.Tokenization.PrioritizedToken> candidates = null;

			foreach (FSTMatch m in matches)
			{
				System.Diagnostics.Debug.Assert(m.Index == from);

				if (VerifyContextConstraints(s, m.Index + m.Length))
				{
					if (m.Output == null || m.Output.Length != m.Length || m.Length == 0)
					{
						throw new Exception("Internal error: invalid measurement FST");
					}

					if (consumedLength == 0)
						consumedLength = m.Length;
					else
						System.Diagnostics.Debug.Assert(consumedLength == m.Length);

					Core.Tokenization.MeasureToken nt = Parse(s.Substring(m.Index, m.Length),
						m.Output);
					if (nt != null)
					{
						if (candidates == null)
							candidates = new List<PrioritizedToken>();
						candidates.Add(new Core.Tokenization.PrioritizedToken(nt, 0));
					}
					else
						System.Diagnostics.Debug.Assert(false, "Expect to be able to parse a number if returned by the FST");
				}
			}

			if (candidates == null || candidates.Count == 0)
				return null;

			EvaluateAndSortCandidates(candidates);
			if (allowTokenBundles && candidates.Count > 1)
			{
				return new Core.Tokenization.TokenBundle(candidates);
			}
			else
			{
				return candidates[0].Token;
			}
		}

		internal static LanguagePlatform.Lingua.FST.FST CreateFST(System.Globalization.CultureInfo culture, 
			bool appendWordTerminator)
		{
			NumberFormatData nfd
				= NumberPatternComputer.GetNumberFormatData(culture, true, true);

			string numberPattern = Lingua.Tokenization.NumberPatternComputer.ComputeFSTPattern(nfd,
				 true, false);

			System.Text.StringBuilder sb = new StringBuilder(numberPattern);
			sb.Append("(");
			bool first = true;
			NumberPatternComputer.AppendDisjunction(sb, Core.CharacterProperties.Blanks, 'U', ref first);
			sb.Append(")?(");

			first = true;
			Core.Wordlist units = Core.Tokenization.PhysicalUnit.GetUnits(culture, false);
			foreach (string unit in units.Items)
			{
				if (first)
					first = false;
				else
					sb.Append("|");

				// append single unit, make sure that first char emits 'U' (in case no whitespace
				//  sep is in the input)
				sb.AppendFormat("(<{0}:U>", FST.FST.EscapeSpecial(unit[0]));
				string remainder = unit.Substring(1);
				if (!String.IsNullOrEmpty(remainder))
				{
					sb.Append(FST.FST.EscapeSpecial(remainder));
				}
				sb.Append(")");
			}

			sb.Append(")");

			if (appendWordTerminator)
			{
				// Append "word terminator"
				sb.Append("#>");
			}

			LanguagePlatform.Lingua.FST.FST fst = LanguagePlatform.Lingua.FST.FST.Create(sb.ToString());

			fst.MakeDeterministic();

#if DEBUG
			bool dump = false;
			if (dump)
				fst.Dump(String.Format("d:/temp/measure-fst-{0}.txt", culture.Name));
#endif

			return fst;
		}

		private void EvaluateAndSortCandidates(List<Core.Tokenization.PrioritizedToken> candidates)
		{
			if (candidates == null || candidates.Count == 0)
				return;

			int basePriority = _Priority;

			foreach (Core.Tokenization.PrioritizedToken pt in candidates)
			{
				Core.Tokenization.MeasureToken nt = pt.Token as Core.Tokenization.MeasureToken;
				System.Diagnostics.Debug.Assert(nt != null);

				int malus = 0;

				if (nt.DecimalSeparator == NumericSeparator.Alternate
					|| nt.GroupSeparator == NumericSeparator.Alternate)
				{
					++malus;
				}

				pt.Priority = _Priority - malus;
			}

			candidates.Sort(delegate(PrioritizedToken a, PrioritizedToken b)
			{
				// sort by decreasing priority
				return b.Priority - a.Priority;
			});
		}

		// NOTE parsing does not yet support canonical number FSTs since the culture
		//  is not inspected for primary/secondary separators. The only source for 
		//  determination whether primary or alternate separators are used is the FST
		//  output. This, however, will always be the alternate separator indicator for
		//  canonical recognizers.

		private Core.Tokenization.MeasureToken Parse(string surface, string output)
		{
			System.Diagnostics.Debug.Assert(surface != null && output != null
				&& surface.Length == output.Length);

			int sep = output.IndexOf('U');
			if (sep <= 0)
				throw new Exception("Invalid measurement format");

			string numericSurface = surface.Substring(0, sep);
			string numericOutput = output.Substring(0, sep);

            char unitSeparator = '\0';

			while (sep < surface.Length && Char.IsWhiteSpace(surface[sep]))
			{
				if (unitSeparator == '\0')
					unitSeparator = surface[sep];
				++sep;
			}
			string unitPart = surface.Substring(sep);

			NumberToken nt = NumberFSTRecognizer.ParseNumber(numericSurface, numericOutput);

			Core.Tokenization.Unit u = Core.Tokenization.PhysicalUnit.Find(unitPart, _FSTRecognizer.Culture);

			MeasureToken value 
				= new Core.Tokenization.MeasureToken(surface, nt, u, unitPart, unitSeparator);

			return value;
		}
	}
}
