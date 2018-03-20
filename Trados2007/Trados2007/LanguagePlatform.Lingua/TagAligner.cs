using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class TagAligner
	{
		public static TagAssociations AlignPairedTags(IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens, 
			SimilarityMatrix similarityMatrix)
		{
			TagPairs srcPairedTags = FindPairedTags(sourceTokens);
			if (srcPairedTags == null || srcPairedTags.Count == 0)
				return null;

			TagPairs trgPairedTags = FindPairedTags(targetTokens);
			if (trgPairedTags == null || trgPairedTags.Count == 0)
				return null;

			TagAssociations associations = new TagAssociations();

			System.Collections.BitArray processedSrcTags 
				= new System.Collections.BitArray(srcPairedTags.Count);
			System.Collections.BitArray processedTrgTags 
				= new System.Collections.BitArray(trgPairedTags.Count);

			const bool useEndPositions = true;

			if (srcPairedTags.Count > 0 && trgPairedTags.Count > 0)
			{
				int[,] lcsScores = ComputeTagAssociationScores(similarityMatrix, 
					srcPairedTags, trgPairedTags,
					useEndPositions);

				if (lcsScores == null)
					return null;

				while (true)
				{
					// find global row/column maximum

					int maxScore = Int32.MinValue;
					int maxS = -1;
					int maxT = -1;
					bool unique = false;

					for (int s = 0; s < srcPairedTags.Count; ++s)
					{
						if (processedSrcTags[s])
							continue;

						for (int t = 0; t < trgPairedTags.Count; ++t)
						{
							if (processedTrgTags[t])
								continue;

							if (lcsScores[s, t] > maxScore)
							{
								maxScore = lcsScores[s, t];
								maxS = s;
								maxT = t;
								unique = true;
							}
							else if (lcsScores[s, t] == maxScore)
							{
								unique = false;
							}
						}
					}

					if (maxS >= 0)
					{
						if (!unique)
						{
							// disambiguation required? Only if in same row or column - DNC right now.
							// System.Diagnostics.Debug.Assert(false, "Investigate - let Oli know and provide test data");
						}

						// global unique maximum - associate tags
						associations.Add(srcPairedTags[maxS], trgPairedTags[maxT],
							Core.EditDistance.EditOperation.Change);
						processedSrcTags[maxS] = true;
						processedTrgTags[maxT] = true;
					}
					else
					{
						// no global max found anymore
						break;
					}
				}
			}

			for (int p = 0; p < srcPairedTags.Count; ++p)
			{
				if (!processedSrcTags[p])
					// src tag at that position is not associated
					associations.Add(srcPairedTags[p], null);
			}

			for (int p = 0; p < trgPairedTags.Count; ++p)
			{
				if (!processedTrgTags[p])
					associations.Add(null, trgPairedTags[p]);
			}

			return associations;
		}

		/// <summary>
		/// Returns a dictionary of paired tag indices, or null if none exist.
		/// </summary>
		public static TagPairs FindPairedTags(IList<Core.Tokenization.Token> tokens)
		{
			// TODO this is equivalent to Core.Segment.GetTagPairings() - clean up

			TagPairs result = null;

			for (int stp = 0; stp < tokens.Count; ++stp)
			{
				Core.Tokenization.TagToken st = tokens[stp] as Core.Tokenization.TagToken;
				if (st != null && st.Tag.Type == Core.TagType.Start)
				{
					// find the end tag, which is supposed to follow
					int etp = 0;
					for (etp = stp + 1; etp < tokens.Count; ++etp)
					{
						Core.Tokenization.TagToken et = tokens[etp] as Core.Tokenization.TagToken;
						if (et != null && et.Tag.Type == Core.TagType.End && et.Tag.Anchor == st.Tag.Anchor)
						{
							if (result == null)
								result = new TagPairs();
							result.Add(stp, etp, st.Tag.Anchor);
							break;
						}
					}
					System.Diagnostics.Debug.Assert(etp < tokens.Count, "End tag not found");
				}
			}

			return result;
		}

		private static int[,] ComputeTagAssociationScores(SimilarityMatrix similarityMatrix,
			TagPairs srcPairedTags,
			TagPairs trgPairedTags,
			bool useEndPositions)
		{
			// this should pretty much result in first-come first-serve alignment, but we hopefully
			//  get better associations for nested tags

			// foreach src tag, compute LCS to each target tag

			int[,] lcsScores = new int[srcPairedTags.Count, trgPairedTags.Count];

			List<int> sourceTokenPositions = new List<int>();
			List<int> targetTokenPositions = new List<int>();

			TokenIndexLCSScoreProvider scorer
				= new TokenIndexLCSScoreProvider(similarityMatrix, 0.75, true);

			for (int p = 0; p < similarityMatrix.SourceTokens.Count; ++p)
				sourceTokenPositions.Add(p);
			for (int p = 0; p < similarityMatrix.TargetTokens.Count; ++p)
				targetTokenPositions.Add(p);

			SequenceAlignmentComputer<int> aligner
				= new SequenceAlignmentComputer<int>(sourceTokenPositions,
					targetTokenPositions, scorer, null, 1, 1);

			int uptoSource;
			int uptoTarget;

			for (int srcTag = srcPairedTags.Count - 1; srcTag >= 0; --srcTag)
			{
				PairedTag sPt = srcPairedTags[srcTag];

				uptoSource = (useEndPositions ? sPt.End : sPt.Start);

				for (int trgTag = trgPairedTags.Count - 1; trgTag >= 0; --trgTag)
				{
					PairedTag tPt = trgPairedTags[trgTag];
					uptoTarget = (useEndPositions ? tPt.End : tPt.Start);

					List<AlignedSubstring> result
						= aligner.Compute(uptoSource, uptoTarget);

					if (result != null && result.Count > 0)
					{
						System.Diagnostics.Debug.Assert(result.Count == 1);

						// the result is the common subsequence length minus items which were deleted or inserted
						int score = result[0].Score
							- (uptoSource - result[0].Score)
							- (uptoTarget - result[0].Score);

						// penalize large differences in the spanned width, but not if 
						//  we include the end positions in the LCS
						int malus;
						if (useEndPositions)
							malus = 0;
						else
						{
							int srcSpan = GetTagSpan(sPt);
							int trgSpan = GetTagSpan(tPt);

							malus = Math.Abs(srcSpan - trgSpan) / 2;
						}

						lcsScores[srcTag, trgTag] = score - malus;
					}
				}
			}

			return lcsScores;
		}

		/// <summary>
		/// Returns the span (number of spanned tokens) of the tag.
		/// </summary>
		private static int GetTagSpan(PairedTag pt)
		{
			int s = pt.End - pt.Start - 1;
			System.Diagnostics.Debug.Assert(s >= 0);
			return s;
		}

	}
}
