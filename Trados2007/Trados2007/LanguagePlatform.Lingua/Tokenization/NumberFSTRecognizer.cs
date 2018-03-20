using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.Resources;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal class NumberFSTRecognizer : Recognizer
	{
		private FSTRecognizer _FSTRecognizer;

		public static Recognizer Create(Core.Resources.IResourceDataAccessor resourceDataAccessor, 
			System.Globalization.CultureInfo culture, int priority)
		{
			NumberFSTRecognizer result = new NumberFSTRecognizer(culture, priority,
				resourceDataAccessor);

			SetAdditionalOptions(result, culture);
			
			return result;
		}

		public static Recognizer Create(System.Globalization.CultureInfo culture, int priority)
		{
			NumberFSTRecognizer result = new NumberFSTRecognizer(culture, priority);

			SetAdditionalOptions(result, culture);

			return result;
		}

		private static void SetAdditionalOptions(NumberFSTRecognizer result, 
			System.Globalization.CultureInfo culture)
		{
			result.OnlyIfFollowedByNonwordCharacter
				= Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture);
			if (result.AdditionalTerminators == null)
				result.AdditionalTerminators = new Sdl.LanguagePlatform.Core.CharacterSet();
			result.AdditionalTerminators.Add('-'); // TODO other math symbols?
			result.OverrideFallbackRecognizer = true;
		}

		public NumberFSTRecognizer(System.Globalization.CultureInfo culture, int priority)
			: base(TokenType.Number, priority, "Number", "NumberFSTRecognizer")
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Cannot compute number patterns for neutral cultures");
			if (culture.NumberFormat == null)
				throw new ArgumentException("No number format info available for the specified culture");

			LanguagePlatform.Lingua.FST.FST fst = CreateFST(culture, Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture));
			_FSTRecognizer = new FSTRecognizer(fst, culture);
		}

		/// <summary>
		/// Attempts to get the compiled FST from the resources, and if that fails, will create it from scratch 
		/// </summary>
		public NumberFSTRecognizer(System.Globalization.CultureInfo culture, int priority, 
			Core.Resources.IResourceDataAccessor accessor)
			: base(TokenType.Number, priority, "Number", "NumberFSTRecognizer")
		{
			if (culture == null)
				throw new ArgumentNullException();
			if (culture.IsNeutralCulture)
				throw new ArgumentException("Cannot compute number patterns for neutral cultures");
			if (culture.NumberFormat == null)
				throw new ArgumentException("No number format info available for the specified culture");
			if (accessor == null)
				accessor = new ResourceFileResourceAccessor();

			LanguagePlatform.Lingua.FST.FST fst = null;

			bool attemptLoad = true;

			if (attemptLoad && accessor.GetResourceStatus(culture, Core.Resources.LanguageResourceType.NumberFST, true) !=
				Core.Resources.ResourceStatus.NotAvailable)
			{
				// TODO should _Culture be set to the _actual_ culture of the loaded FST, i.e. 
				//  the invariant culture for the generic/canonical one?

				byte[] data = accessor.GetResourceData(culture, Core.Resources.LanguageResourceType.NumberFST, true);
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
						throw new Exception("Internal error: invalid number FST");
					}

					if (consumedLength == 0)
						consumedLength = m.Length;
					else
						System.Diagnostics.Debug.Assert(consumedLength == m.Length);

					Core.Tokenization.NumberToken nt = ParseNumber(s.Substring(m.Index, m.Length),
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

			string fstPattern = Lingua.Tokenization.NumberPatternComputer.ComputeFSTPattern(nfd,
				true, appendWordTerminator);

			LanguagePlatform.Lingua.FST.FST fst = LanguagePlatform.Lingua.FST.FST.Create(fstPattern);

			fst.MakeDeterministic();

#if DEBUG
			bool dump = false;
			if (dump)
				fst.Dump(String.Format("d:/temp/number-fst-{0}.txt", culture.Name));
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
				Core.Tokenization.NumberToken nt = pt.Token as Core.Tokenization.NumberToken;
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

		internal static Core.Tokenization.NumberToken ParseNumber(string surface, string output)
		{
			System.Diagnostics.Debug.Assert(surface != null && output != null
				&& surface.Length == output.Length);

			System.Text.StringBuilder rawSign = new StringBuilder();
			System.Text.StringBuilder rawDecimals = new StringBuilder();
			System.Text.StringBuilder rawFractionals = new StringBuilder();

			char altGrpSep = '\0';
			char altDecSep = '\0';

			Sign signMode = Sign.None;
			NumericSeparator grpSepMode = NumericSeparator.None;
			NumericSeparator decSepMode = NumericSeparator.None;

			int state = 0;
			int l = surface.Length;

			char c;
			char o;

			if (NumberPatternComputer.AllowTrailingSign && l > 0)
			{
				// catch the case of a trailing sign
				o = output[l - 1];
				if (o == '-' || o == '+')
				{
					if (o == '-')
						signMode = Sign.Minus;
					else
						signMode = Sign.Plus;

					rawSign.Append(o);
					// ignore trailing sign when parsing number
					--l;
				}
			}

			for (int p = 0; p < l; ++p)
			{
				c = surface[p];
				o = output[p];

				switch (state)
				{
				case 0: // waiting for digit or sign in the output
					if (o == '-' || o == '+')
					{
						if (o == '-')
							signMode = Sign.Minus;
						else
							signMode = Sign.Plus;

						rawSign.Append(o);
						state = 1;
					}
					else if (o >= '0' && o <= '9')
					{
						// decimal part is not optional, so there will be at least one digit
						//  before any separator
						rawDecimals.Append(o);
						state = 1;
					}
					else
						throw new Exception(String.Format("Unexpected input in {0}/{1} at position {2}", 
							surface, output, p));
					break;

				case 1: // decimal digits
					if (o >= '0' && o <= '9')
					{
						rawDecimals.Append(o);
						state = 1; // remain in state
					}
					else if (o == 'g' || o == 'G')
					{
						if (grpSepMode == NumericSeparator.None)
						{
							// group separator
							if (o == 'g')
							{
								grpSepMode = NumericSeparator.Alternate;
								altGrpSep = c;
							}
							else
								grpSepMode = NumericSeparator.Primary;
						}
					}
					else if (o == 'd' || o == 'D')
					{
						if (decSepMode == NumericSeparator.None)
						{
							// decimal separator
							if (o == 'd')
							{
								decSepMode = NumericSeparator.Alternate;
								altDecSep = c;
							}
							else
								decSepMode = NumericSeparator.Primary;
						}

						state = 2; // fractional part
					}
					else
						throw new Exception(String.Format("Unexpected input in {0}/{1} at position {2}",
							surface, output, p));
					break;

				case 2: // fractional part
					if (o >= '0' && o <= '9')
					{
						rawFractionals.Append(o);
						state = 2; // remain in state
					}
					else
						throw new Exception(String.Format("Unexpected input in {0}/{1} at position {2}",
							surface, output, p));
					break;
				default:
					throw new Exception("Internal error");
				}
			}

			return new NumberToken(surface, grpSepMode, decSepMode,
				altGrpSep, altDecSep, signMode,
				rawSign.Length > 0 ? rawSign.ToString() : null, 
				rawDecimals.Length > 0 ? rawDecimals.ToString() : null, 
				rawFractionals.Length > 0 ? rawFractionals.ToString() : null);
		}
	}
}
