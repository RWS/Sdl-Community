using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{

	public static class TermFinder
	{
		// TODO make this dependent on the length of the tokens?
		// private static float _DICE_THRESHOLD = 0.75f;
		private static float _DICE_THRESHOLD = 0.7f;

		[Flags]
		private enum TokenBoundary
		{
			None = 0,
			AtStart = 1,
			AtEnd = 2
		}

		/// <summary>
		/// Attempts to identify the locations of (substrings of) the search segment in the result segment.
		/// Typically, the search segment is a single search string (such as a concordance search string), 
		/// and the result segment is a transation unit segment. 
		/// </summary>
		/// <param name="search">The search text.</param>
		/// <param name="text">The text segment, usually a sentence or larger block of text.</param>
		/// <param name="culture">The culture the search and text belong to.</param>
		/// <returns>The locations and other information about the coverage of the search segment
		/// in the result segment.</returns>
		public static TermFinderResult FindTerms(string search,
			string text,
			System.Globalization.CultureInfo culture,
			bool expectContinuousMatch)
		{
			if (String.IsNullOrEmpty(search))
				throw new ArgumentNullException("search");
			if (String.IsNullOrEmpty(text))
				throw new ArgumentNullException("text");
			if (culture == null)
				throw new ArgumentNullException("culture");

			Core.Segment searchSegment = new Sdl.LanguagePlatform.Core.Segment(culture);
			Core.Segment textSegment = new Sdl.LanguagePlatform.Core.Segment(culture);

			searchSegment.Add(search);
			textSegment.Add(text);

			Lingua.Tokenization.Tokenizer t =
				new Sdl.LanguagePlatform.Lingua.Tokenization.Tokenizer(Lingua.Tokenization.TokenizerSetupFactory.Create(culture));

			searchSegment.Tokens = t.Tokenize(searchSegment);
			textSegment.Tokens = t.Tokenize(textSegment);

			return FindTerms(searchSegment, textSegment, expectContinuousMatch);
		}

		/// <summary>
		/// Attempts to identify the locations of (substrings of) the search segment in the result segment.
		/// Typically, the search segment is a single search string (such as a concordance search string), 
		/// and the result segment is a transation unit segment. 
		/// </summary>
		/// <param name="searchSegment">The search segment or search word. The segment must be tokenized.</param>
		/// <param name="textSegment">The text segment, usually a sentence or larger block of text. 
		/// The segment must be tokenized.</param>
		/// <returns>The locations and other information about the coverage of the search segment
		/// in the result segment.</returns>
		public static TermFinderResult FindTerms(Core.Segment searchSegment,
			Core.Segment textSegment,
			bool expectContinuousMatch)
		{

			/*
			 * Scoring parameters:
			 * 
			 * - coverage of query string - the more the better
			 * - coverage of query string by the largest fragment?
			 * - number of fragments - the fewer the better
			 * - distance of fragments - the smaller the better
			 * - avg size of fragments? variance? - few large better than many small, even with larger distance
			 * - coverage of fragment relative to the token(s) it spans
			 * - consistent word order, inserted words
			 * 
			 * Delete very small fragments (maybe not in FE languages?)
			 * Number of tokens in query?
			 * */


			if (searchSegment == null)
				throw new ArgumentNullException("searchSegment");
			if (textSegment == null)
				throw new ArgumentNullException("textSegment");

			if (searchSegment.Tokens == null || textSegment.Tokens == null)
				throw new ArgumentException("Segments are expected to be tokenized");

			if (searchSegment.Culture == null || textSegment.Culture == null)
				throw new ArgumentException("At least one segment culture is null");

			if (!Core.CultureInfoExtensions.AreCompatible(searchSegment.Culture, textSegment.Culture))
				throw new ArgumentException("Segment cultures are incompatible");

			if (searchSegment.Tokens.Count == 0 || textSegment.Tokens.Count == 0)
				return null;

			bool usesBlanksAsSeparators = Core.CultureInfoExtensions.UseBlankAsWordSeparator(searchSegment.Culture);

			if (usesBlanksAsSeparators)
			{
				return FindTermsWordBased(searchSegment, textSegment, expectContinuousMatch);
			}
			else
			{
				return FindTermsCharBased(searchSegment, textSegment, expectContinuousMatch);
			}
		}

		private static TermFinderResult FindTermsCharBased(Core.Segment searchSegment,
			Core.Segment textSegment, bool expectContinuousMatch)
		{
			// This should only be used for far-east languages

			// these ranges capture the mapping from a character position in the plain text arrays
			//  to a segment position (run/position pairs)
			List<Core.SegmentPosition> searchSegmentPositions;
			List<Core.SegmentPosition> textSegmentPositions;

			string searchPlain = searchSegment.ToPlain(true, true, out searchSegmentPositions);
			string textPlain = textSegment.ToPlain(true, true, out textSegmentPositions);

			if (searchPlain.Length == 0)
			{
				// TODO may need to look into what may cause such an issue:
				System.Diagnostics.Debug.Assert(false, "Let Oli know and provide test data");
				return null;
			}

			char[] searchPlainArray = searchPlain.ToCharArray();
			char[] textPlainArray = textPlain.ToCharArray();

			int searchPlainLength = searchPlain.Length;
			int textPlainLength = textPlain.Length;

			System.Diagnostics.Debug.Assert(searchPlainLength == searchPlainArray.Length);
			System.Diagnostics.Debug.Assert(textPlainLength == textPlainArray.Length);

			List<AlignedSubstring> lcs = null;

			SubstringAlignmentDisambiguator picker
				= new SubstringAlignmentDisambiguator();

			lcs = SequenceAlignmentComputer<char>.ComputeCoverage(searchPlainArray,
				textPlainArray, new CharSubstringScoreProvider(), picker);

			if (lcs == null || lcs.Count == 0)
				return null;

			TermFinderResult result = new TermFinderResult();
			result.MatchingRanges = new List<Sdl.LanguagePlatform.Core.SegmentRange>();

			List<Core.SegmentPosition> textPositions = new List<Core.SegmentPosition>();

			for (int subIdx = 0; subIdx < lcs.Count; ++subIdx)
			{
				AlignedSubstring sub = lcs[subIdx];

				if (sub.Source.Length != sub.Target.Length)
				{
					// NOTE LCSubseq instead of Substring? Check scorer if this fires
					System.Diagnostics.Debug.Assert(false, "Not supported - let Oli know and provide test data");
					return null;
				}

				for (int p = 0; p < sub.Source.Length; ++p)
				{
					textPositions.Add(textSegmentPositions[sub.Target.Start + p]);
				}
			}

			if (textPositions.Count == 0)
				return null;

			// covered ranges in the text segment:
			result.MatchingRanges = SortAndMelt(textPositions);

			// TODO this does not capture adjacency
			float baseScore = (float)textPositions.Count / (float)searchPlainLength;

#if DEBUG
			bool ok = VerifyRanges(result.MatchingRanges,
				textSegment);
			if (!ok)
			{
				System.Diagnostics.Debug.Assert(false, "Range verification failed");
			}
#endif

			result.Score = (int)(100.0f * baseScore);
			if (result.Score < 0)
				result.Score = 0;
			else if (result.Score > 100)
				result.Score = 100;

			return result;
		}

		private static TermFinderResult FindTermsWordBased(Core.Segment searchSegment,
			Core.Segment textSegment, bool expectContinuousMatch)
		{
			// compute the maximum overlap scores for each token in the source and each token in the text, 
			//  using LCS or ED

			const bool useLcsScoreAdjustment = true;

			int[,] overlaps = ComputeTokenAssociationScores(searchSegment, textSegment);

			int[] maxScores = new int[searchSegment.Tokens.Count];

			TermFinderResult result = new TermFinderResult();
			result.MatchingRanges = new List<Sdl.LanguagePlatform.Core.SegmentRange>();

			System.Collections.BitArray coveredTargetTokens
				= new System.Collections.BitArray(textSegment.Tokens.Count);

			IList<string> srcConcatenatedTokens = new List<string>();
			IList<string> trgConcatenatedTokens = new List<string>();

			int nonwhiteSearchTokens = 0;

			for (int s = 0; s < searchSegment.Tokens.Count; ++s)
			{
				if (!searchSegment.Tokens[s].IsWhitespace)
				{
					++nonwhiteSearchTokens;
					if (useLcsScoreAdjustment)
						srcConcatenatedTokens.Add(searchSegment.Tokens[s].Text.ToLowerInvariant());
				}

				for (int t = 0; t < textSegment.Tokens.Count; ++t)
				{
					if (overlaps[s, t] > 0)
					{
						if (!coveredTargetTokens[t])
						{
							result.MatchingRanges.Add(textSegment.Tokens[t].Span);
							coveredTargetTokens[t] = true;
						}

						if (overlaps[s, t] > maxScores[s])
						{
							System.Diagnostics.Debug.Assert(overlaps[s, t] >= 0 && overlaps[s, t] <= 100);
							maxScores[s] = overlaps[s, t];
						}
					}
				}
			}

			if (nonwhiteSearchTokens == 0)
				return null;

			int tokenOverlapScore = (int)((float)maxScores.Sum() / (float)nonwhiteSearchTokens);

			if (useLcsScoreAdjustment)
			{
				int relevantTextTokens = 0;

				// TODO this won't work really well if the same search token appears
				//  multiple times int the match - the concatenation will include all 
				//  occurrences (which will reduce the LCS score) and also increase the 
				//  text token count (further reducing the LCS score)

				for (int tokenIndex = 0; tokenIndex < textSegment.Tokens.Count; ++tokenIndex)
				{
					if (coveredTargetTokens[tokenIndex])
					{
                        if (trgConcatenatedTokens.Count > 0)
                        {
                            int previousWordTokenIndex = GetPreviousWordTokenIndex(textSegment.Tokens, tokenIndex);
                            if ((previousWordTokenIndex > -1) && (!coveredTargetTokens[previousWordTokenIndex]))
                            {
                                const string UnmatchedToken = "#";
                                trgConcatenatedTokens.Add(UnmatchedToken);
                            }
                        }

                        ++relevantTextTokens;
						trgConcatenatedTokens.Add(textSegment.Tokens[tokenIndex].Text.ToLowerInvariant());
					}
				}

				string srcConcat = string.Join("~", srcConcatenatedTokens.ToArray());
                string trgConcat = string.Join("~", trgConcatenatedTokens.ToArray());

				int lcsOverlapScore = 0;

				if ((expectContinuousMatch || tokenOverlapScore < 100)
					&& srcConcat.Length > 0 && trgConcat.Length > 0)
				{
					List<AlignedSubstring> lcs = SequenceAlignmentComputer<char>.ComputeLongestCommonSubsequence(srcConcat.ToCharArray(),
						trgConcat.ToCharArray(), 1,
						new SimpleCharLSAScoreProvider(), null);

					int lcsOverlap = lcs.Sum(x => x.Length);

					// dice again, this time on the concatenated strings of the tokens, with a 
					//  penalty if the number of tokens differs [0..1]
					float tokenCountDeltaPenalty = 2.0f * (float)Math.Min(nonwhiteSearchTokens, relevantTextTokens)
						/ (float)(nonwhiteSearchTokens + relevantTextTokens);

					// another dice
					// 2009-08-10, OC: reduce token count delta penalty
					lcsOverlapScore = (int)(((75.0f + 25.0f * tokenCountDeltaPenalty)
						* 2.0f * (float)lcsOverlap) / (float)(srcConcat.Length + trgConcat.Length));
					if (lcsOverlapScore < 0)
						lcsOverlapScore = 0;
					if (lcsOverlapScore > 100)
						lcsOverlapScore = 100;

					System.Diagnostics.Debug.Assert(lcsOverlapScore >= 0 && lcsOverlapScore <= 100);
				}

				if (tokenOverlapScore == 100 && lcsOverlapScore > 0)
				{
					// discontinuous/swapped match - not sure how to penalize
					// TODO work out exact scoring
					result.Score = (200 + lcsOverlapScore) / 3;
				}
				else
					result.Score = Math.Max(tokenOverlapScore, lcsOverlapScore);
			}
			else
				result.Score = tokenOverlapScore;

			return result;
		}

        private static int GetPreviousWordTokenIndex(List<Core.Tokenization.Token> tokens, int currentTokenIndex)
        {
            for (int tokenIndex = currentTokenIndex - 1; tokenIndex >= 0; tokenIndex--)
            {
                if (tokens[tokenIndex].IsWord)
                {
                    return tokenIndex;
                }
            }

            return -1;
        }

		private static int[,] ComputeTokenAssociationScores(Core.Segment searchSegment, Core.Segment textSegment)
		{
			int[,] overlaps = new int[searchSegment.Tokens.Count, textSegment.Tokens.Count];

			CaseAwareCharSubsequenceScoreProvider scorer = new CaseAwareCharSubsequenceScoreProvider();
			Core.Tokenization.Token srcToken;
			Core.Tokenization.Token txtToken;

			for (int s = 0; s < searchSegment.Tokens.Count; ++s)
			{
				srcToken = searchSegment.Tokens[s];
				if (srcToken.IsWhitespace || srcToken is Core.Tokenization.TagToken)
					continue;

				for (int t = 0; t < textSegment.Tokens.Count; ++t)
				{
					txtToken = textSegment.Tokens[t];
					if (txtToken.IsWhitespace || txtToken is Core.Tokenization.TagToken)
						continue;

					overlaps[s, t] = 0;

					// TODO relax equality criteria on placeables of the same type

					// TODO does the score include information whether tolower/tobase has
					//  been applied?

					List<AlignedSubstring> alignment
						= SequenceAlignmentComputer<char>.ComputeLongestCommonSubsequence(srcToken.Text.ToCharArray(),
						 txtToken.Text.ToCharArray(), 0, scorer, null);

					if (alignment == null || alignment.Count == 0)
						continue;

					int common = alignment.Sum(x => x.Length);
					if (common == 0)
						continue;

					// dice
					// TODO experiment with other scoring methods? Scoring only relative to query?
					float score = 2.0f * common / (float)(srcToken.Text.Length + txtToken.Text.Length);
					if (score >= _DICE_THRESHOLD)
					{
						// percentage of score
						overlaps[s, t] = (int)(score * 100.0f);
						System.Diagnostics.Debug.Assert(overlaps[s, t] >= 0 && overlaps[s, t] <= 100);
					}
				}
			}

			return overlaps;
		}

		/// <summary>
		/// Verifies that all ranges are "inside" the segment's (text) elements.
		/// </summary>
		private static bool VerifyRanges(IEnumerable<Core.SegmentRange> ranges,
			Core.Segment segment)
		{
			if (ranges == null)
				return true;

			foreach (Core.SegmentRange sr in ranges)
			{
				if (sr == null || sr.From == null || sr.Into == null)
					return false;
				if (sr.From.Index != sr.Into.Index)
					return false;
				if (sr.From.Index < 0 || sr.From.Index >= segment.Elements.Count)
					return false;
				if (sr.From.Position > sr.Into.Position)
					return false;
				Core.Text txt = segment.Elements[sr.From.Index] as Core.Text;
				if (txt == null)
					return false;
				if (sr.From.Position >= txt.Value.Length || sr.Into.Position >= txt.Value.Length)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Sorts the ranges and joins adjacent ranges, if possible. Assumes (but does not
		/// check) that ranges are non-overlapping, and that ranges never cover more than one
		/// element. Resulting ranges will not cross token boundaries.
		/// </summary>
		private static List<Core.SegmentRange> SortAndMelt(List<Core.SegmentPosition> positions)
		{
			List<Core.SegmentRange> result = new List<Core.SegmentRange>();

			positions.Sort(delegate(Core.SegmentPosition a, Core.SegmentPosition b)
			{
				int cmp = a.Index - b.Index;
				if (cmp == 0)
					cmp = a.Position - b.Position;
				return cmp;
			});

			Core.SegmentRange item
				= new Sdl.LanguagePlatform.Core.SegmentRange(positions[0], positions[0].Duplicate());
			result.Add(item);

			for (int p = 1; p < positions.Count; ++p)
			{
				bool adjacent = (positions[p].Index == item.From.Index
					&& positions[p].Position == item.Into.Position + 1);

				if (adjacent)
				{
					item.Into.Position = positions[p].Position;
				}
				else
				{
					item = new Sdl.LanguagePlatform.Core.SegmentRange(positions[p], positions[p].Duplicate());
					result.Add(item);
				}
			}

			return result;
		}


	}

	public class CaseAwareCharSubsequenceScoreProvider
		: ISequenceAlignmentItemScoreProvider<char>
	{
		public int GetAlignScore(char a, char b)
		{
			if (a == b)
				return 2;
			a = Char.ToLowerInvariant(a);
			b = Char.ToLowerInvariant(b);
			if (a == b)
				return 1;

			a = Core.CharacterProperties.ToBase(a);
			b = Core.CharacterProperties.ToBase(b);

			if (a == b)
				return 1;
			return -1;
		}

		public int GetSourceSkipScore(char a)
		{
			return -1;
		}

		public int GetTargetSkipScore(char a)
		{
			return -1;
		}

		public bool MaySkip
		{
			get { return true; }
		}
	}

}
