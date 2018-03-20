﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Represents methods used to determine scores for sequence alignment.
	/// </summary>
	/// <typeparam name="T">The type of the elements to align.</typeparam>
	public interface ISequenceAlignmentItemScoreProvider<T>
	{
		// TODO pass positions instead of objects - scorer can then make
		//  return score depending on position of association
		/// <summary>
		/// Computes the score of the alignment between the two objects. The higher
		/// the score, the stronger is the association.
		/// </summary>
		/// <param name="a">The first object</param>
		/// <param name="b">The second object</param>
		/// <returns>The score of the alignment of the two objects.</returns>
		int GetAlignScore(T a, T b);

		/// <summary>
		/// Computes the score of skipping the object in the source sequence. This is typically
		/// a negative  value. If the value is very small, it will in most cases prevent skipping
		/// objects, which means that the substring (not the subsequence) will be computed.
		/// </summary>
		/// <param name="a">The object</param>
		/// <returns>The skip score (usually a negative value)</returns>
		int GetSourceSkipScore(T a);
		/// <summary>
		/// Computes the score of skipping the object in the target sequence. This is typically
		/// a negative  value. If the value is very small, it will in most cases prevent skipping
		/// objects, which means that the substring (not the subsequence) will be computed.
		/// </summary>
		/// <param name="b">The object</param>
		/// <returns>The skip score (usually a negative value)</returns>
		int GetTargetSkipScore(T b);

		/// <summary>
		/// Called once by the sequence aligner to test whether skips are desired
		/// at all (if not, it's a longest common substring aligner; if true, it's 
		/// a longest common subsequence aligner). This is primarily for optimization
		/// purposes as calls to <see cref="GetSourceSkipScore"/> and 
		/// <see cref="GetTargetSkipScore"/> can be avoided.
		/// </summary>
		bool MaySkip { get; }

		/*
		/// <summary>
		/// A test to determine whether the pair of elements may be a valid head (start of 
		/// an aligned subsequence or substring). Will only be called if FilterInvalidHeads
		/// is true. The test will be done after getting the alignment/skip score.
		/// <para>Note that the operation tested may be a skip operation which means that 
		/// either a or b may actually be skipped (i.e. not aligned).</para>
		/// </summary>
		bool IsValidHead(T a, T b);

		/// <summary>
		/// Called once by the sequence aligner to test once whether it should call back into
		/// IsValidHead() at all.
		/// </summary>
		bool FilterInvalidHeads { get; }
		 * 
		*/
	}

	/// <summary>
	/// Represents methods used for sequence alignment, to disambiguate among multiple
	/// alignment paths with the same scores.
	/// </summary>
	public interface IExtensionDisambiguator
	{
		/// <summary>
		/// Used to pick one of multiple possible extension candidates (<paramref name="candidates"/>)
		/// which can extend the already computed sequence alignment (<paramref name="path"/>).
		/// </summary>
		/// <param name="path">The sequence alignment computed so far</param>
		/// <param name="candidates">The list of extension candidates</param>
		/// <returns>A single extension, which must be an element of <paramref name="candidates"/>,
		/// which is then used to extend the alignment in <paramref name="path"/>.
		/// </returns>
		AlignedSubstring PickExtension(List<AlignedSubstring> path, List<AlignedSubstring> candidates);
	}

	/// <summary>
	/// A simple longest common subsequence alignment score provider for character
	/// sequences.
	/// </summary>
	public class SimpleCharLSAScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		// TODO use score 2 for equals, 1 for dia/case insensitive eq

		/// <summary>
		/// Computes the alignment score between the two characters.
		/// </summary>
		/// <param name="a">The first character</param>
		/// <param name="b">The second character</param>
		/// <returns>The alignment score between the two characters.</returns>
		public int GetAlignScore(char a, char b)
		{
			return (a == b ? 2 : -1);
		}

		/// <summary>
		/// The skip score for skipping the character in the source sequence.
		/// </summary>
		/// <param name="a">The character</param>
		/// <returns>The skip score</returns>
		public int GetSourceSkipScore(char a)
		{
			return -1;
		}

		/// <summary>
		/// The skip score for skipping the character in the target sequence.
		/// </summary>
		/// <param name="a">The character</param>
		/// <returns>The skip score</returns>
		public int GetTargetSkipScore(char a)
		{
			return -1;
		}

		/// <summary>
		/// Always returns <c>true</c> (since this class is a longest common subsequence score provider).
		/// </summary>
		public bool MaySkip { get { return true; } }
	}

	/// <summary>
	/// A simple longest common substring alignment score provider for character
	/// sequences.
	/// </summary>
	public class SimpleCharLCSScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		// TODO use score 2 for equals, 1 for dia/case insensitive eq

		/// <summary>
		/// Computes the alignment score between the two characters.
		/// </summary>
		/// <param name="a">The first character</param>
		/// <param name="b">The second character</param>
		/// <returns>The alignment score between the two characters.</returns>
		public int GetAlignScore(char a, char b)
		{
			return (a == b ? 1 : -100000);
		}

		/// <summary>
		/// The skip score for skipping the character in the source sequence. This is a large
		/// negative value.
		/// </summary>
		/// <param name="a">The character</param>
		/// <returns>The skip score</returns>
		public int GetSourceSkipScore(char a)
		{
			return -100000;
		}

		/// <summary>
		/// The skip score for skipping the character in the target sequence. This is a large
		/// negative value.
		/// </summary>
		/// <param name="a">The character</param>
		/// <returns>The skip score</returns>
		public int GetTargetSkipScore(char a)
		{
			return -100000;
		}

		/// <summary>
		/// Always returns <c>false</c> (since this class is a longest common substring score provider).
		/// </summary>
		public bool MaySkip { get { return false; } }
	}

	/// <summary>
	/// A standardized longest common substring score provider.
	/// </summary>
	/// <typeparam name="T">The type of the elements of the sequences to align.</typeparam>
	public class SimpleLCSScoreProvider<T> : ISequenceAlignmentItemScoreProvider<T>
	{
		/// <summary>
		/// Computes the alignment score between the two objects.
		/// </summary>
		/// <param name="a">The first object</param>
		/// <param name="b">The second object</param>
		/// <returns><c>1</c> if the objects are equal, and <c>-100000</c> otherwise.</returns>
		public int GetAlignScore(T a, T b)
		{
			return (a.Equals(b) ? 1 : -100000);
		}

		/// <summary>
		/// Computes the score of skipping the specified object in the source sequence.
		/// </summary>
		/// <param name="a">The object (ignored)</param>
		/// <returns>-100000</returns>
		public int GetSourceSkipScore(T a)
		{
			return -100000;
		}

		/// <summary>
		/// Computes the score of skipping the specified object in the target sequence.
		/// </summary>
		/// <param name="a">The object (ignored)</param>
		/// <returns>-100000</returns>
		public int GetTargetSkipScore(T a)
		{
			return -100000;
		}

		/// <summary>
		/// Always returns <c>false</c>.
		/// </summary>
		public bool MaySkip { get { return false; } }
	}

	/// <summary>
	/// A simple longest common subsequence alignment score provider for character
	/// sequences.
	/// </summary>
	public class CharSubstringScoreProvider : ISequenceAlignmentItemScoreProvider<char>
	{
		/// <summary>
		/// Returns the alignment score for the two characters.
		/// </summary>
		/// <param name="a">The first character</param>
		/// <param name="b">The second character</param>
		/// <returns><c>3</c> if the characters are equal, and <c>-100</c> otherwise</returns>
		public int GetAlignScore(char a, char b)
		{
			if (a == b)
				return 3;
			return -100;
		}

		/// <summary>
		/// Computes the skip score of the character in the source sequence.
		/// </summary>
		/// <param name="a">The character (ignored)</param>
		/// <returns><c>-100</c></returns>
		public int GetSourceSkipScore(char a)
		{
			return -100;
		}

		/// <summary>
		/// Computes the skip score of the character in the target sequence.
		/// </summary>
		/// <param name="a">The character (ignored)</param>
		/// <returns><c>-100</c></returns>
		public int GetTargetSkipScore(char a)
		{
			return -100;
		}

		/// <summary>
		/// Always returns <c>true</c>.
		/// </summary>
		public bool MaySkip { get { return true; } }
	}

	/// <summary>
	/// A generic sequence alignment computer for sequences of elements of a common type.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the sequences to align.</typeparam>
	public class SequenceAlignmentComputer<T>
	{
		private enum Operation
		{
			Align,
			Noise,
			Skip
		}

		private struct Cell
		{
			public int Score;
			public int BackI;
			public int BackJ;
			public Operation Op;
			/// <summary>
			/// The maximum score seen so far in the upper left corner, including this cell. 
			/// Used for LSC only (not for coverage).
			/// </summary>
			public int ULMaxScore;

			/// <summary>
			/// <see cref="object.ToString()"/>
			/// </summary>
			/// <returns>A string representation of the object, for display purposes.</returns>
			public override string ToString()
			{
				return String.Format("{0}->{1}/{2}", Score, BackI, BackJ);
			}
		}

		/// <summary>
		/// Computes the longest local alignment coverage of the two sequences.
		/// </summary>
		/// <param name="source">The source sequence</param>
		/// <param name="target">The target sequence</param>
		/// <param name="scorer">The score provider to use</param>
		/// <param name="picker">An extension disambiguator (may be null)</param>
		public static List<AlignedSubstring> ComputeCoverage(IList<T> source,
			IList<T> target,
			ISequenceAlignmentItemScoreProvider<T> scorer,
			IExtensionDisambiguator picker)
		{
			return ComputeCoverage(source, target, 1, scorer, picker, 0);
		}

		/// <summary>
		/// Computes the longest common subsequence of the two sequences.
		/// </summary>
		/// <param name="source">The source sequence</param>
		/// <param name="target">The target sequence</param>
		/// <param name="minLength">The minimum length of an aligned substring</param>
		/// <param name="scorer">The score provider to use</param>
		/// <param name="picker">An extension disambiguator (may be null)</param>
		public static List<AlignedSubstring> ComputeLongestCommonSubsequence(IList<T> source,
			IList<T> target, int minLength,
			ISequenceAlignmentItemScoreProvider<T> scorer,
			IExtensionDisambiguator picker)
		{
			return ComputeCoverage(source, target, minLength, scorer, picker, 1);
		}

		private static int[,] ComputeScores(IList<T> source, IList<T> target,
			ISequenceAlignmentItemScoreProvider<T> scorer)
		{
			int[,] table = new int[source.Count, target.Count];

			for (int i = 0; i < source.Count; ++i)
			{
				for (int j = 0; j < target.Count; ++j)
				{
					T t1 = source[i];
					T t2 = target[j];

					table[i, j] = scorer.GetAlignScore(t1, t2);
				}
			}
			return table;
		}

		/// <summary>
		/// Computes the longest local alignment coverage of the two sequences.
		/// </summary>
		/// <param name="source">The source sequence</param>
		/// <param name="target">The target sequence</param>
		/// <param name="minLength">The minimum length of an aligned substring</param>
		/// <param name="scorer">The score provider to use</param>
		/// <param name="picker">An extension disambiguator (may be null)</param>
		/// <param name="maxItems">The maximum number of items in the result coverage. If 1, 
		/// no coverage, but only the longest subsequence will be computed. If 0, the full
		/// coverage will be computed.</param>
		public static List<AlignedSubstring> ComputeCoverage(IList<T> source,
			IList<T> target, int minLength,
			ISequenceAlignmentItemScoreProvider<T> scorer,
			IExtensionDisambiguator picker,
			int maxItems)
		{
			SequenceAlignmentComputer<T> aligner = new SequenceAlignmentComputer<T>(source,
				target, scorer, picker, minLength, maxItems);
			return aligner.Compute();
		}

		private IList<T> _Source;
		private IList<T> _Target;

		private ISequenceAlignmentItemScoreProvider<T> _Scorer;
		private IExtensionDisambiguator _Picker;
		private int _MinLength;
		private int _MaxItems;

		private int[,] _AlignmentScores = null;
		private Cell[,] _Table = null;
		private int[] _SourceSkipScores = null;
		private int[] _TargetSkipScores = null;

		/// <summary>
		/// Initializes a new instance with the provided values. Once initialized, 
		/// call <see cref="Compute()"/> to compute and return the actual alignment.
		/// </summary>
		/// <param name="source">The source sequence</param>
		/// <param name="target">The target sequence</param>
		/// <param name="scorer">The score provider to use</param>
		/// <param name="picker">An extension disambiguator (may be null)</param>
		/// <param name="minLength">The minimum length of an aligned substring</param>
		/// <param name="maxItems">The maximum number of items in the result coverage. If 1, 
		/// no coverage, but only the longest subsequence will be computed. If 0, the full
		/// coverage will be computed.</param>
		public SequenceAlignmentComputer(IList<T> source,
			IList<T> target,
			ISequenceAlignmentItemScoreProvider<T> scorer,
			IExtensionDisambiguator picker,
			int minLength,
			int maxItems)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (target == null)
				throw new ArgumentNullException("target");

			if (scorer == null)
				throw new ArgumentNullException("scorer");

			if (minLength <= 0)
				minLength = 1;

			if (maxItems < 0)
				maxItems = 0;

			_MinLength = minLength;
			_MaxItems = maxItems;

			_Source = source;
			_Target = target;
			_Scorer = scorer;
			_Picker = picker;
		}

		/// <summary>
		/// Computes the longest common subsequence of the two sequences used to initialize
		/// this instance.
		/// </summary>
		public List<AlignedSubstring> Compute()
		{
			return this.Compute(_Source.Count, _Target.Count);
		}

		private void ComputeSkipScoreCaches()
		{
			_SourceSkipScores = new int[_Source.Count];
			for (int p = 0; p < _Source.Count; ++p)
				_SourceSkipScores[p] = _Scorer.GetSourceSkipScore(_Source[p]);

			_TargetSkipScores = new int[_Target.Count];
			for (int p = 0; p < _Target.Count; ++p)
				_TargetSkipScores[p] = _Scorer.GetTargetSkipScore(_Target[p]);
		}

		private void ComputeFullTable(bool maySkip)
		{
			for (int i = 1; i <= _Source.Count; ++i)
			{
				for (int j = 1; j <= _Target.Count; ++j)
				{
					_Table[i, j].Score = 0;

					int alignmentScore = _AlignmentScores[i - 1, j - 1];

					int c1 = _Table[i - 1, j - 1].Score + alignmentScore;
					if (c1 > _Table[i, j].Score)
					{
						_Table[i, j].Score = c1;
						_Table[i, j].BackI = i - 1;
						_Table[i, j].BackJ = j - 1;
						_Table[i, j].Op = alignmentScore > 0 ? Operation.Align : Operation.Noise;
					}

					if (maySkip)
					{
						// TODO not sure whether t1 and t2 should not be swapped here

						int c2 = _Table[i - 1, j].Score + _SourceSkipScores[i - 1];
						if (c2 > _Table[i, j].Score)
						{
							_Table[i, j].Score = c2;
							_Table[i, j].BackI = i - 1;
							_Table[i, j].BackJ = j;
							_Table[i, j].Op = Operation.Skip;
						}

						int c3 = _Table[i, j - 1].Score + _TargetSkipScores[j - 1];
						if (c3 > _Table[i, j].Score)
						{
							_Table[i, j].Score = c3;
							_Table[i, j].BackI = i;
							_Table[i, j].BackJ = j - 1;
							_Table[i, j].Op = Operation.Skip;
						}
					}

					int v = _Table[i, j].Score;

					int ulMax = v;

					if (_Table[i - 1, j - 1].ULMaxScore > ulMax)
						ulMax = _Table[i - 1, j - 1].ULMaxScore;

					if (_Table[i - 1, j].ULMaxScore > ulMax)
						ulMax = _Table[i - 1, j].ULMaxScore;

					if (_Table[i, j - 1].ULMaxScore > ulMax)
						ulMax = _Table[i, j - 1].ULMaxScore;

					_Table[i, j].ULMaxScore = ulMax;
				}
			}
		}

		private void ComputeMaximaForCoverage(List<Pair<int>> maxima,
			ref int globalMax,
			int uptoSource, int uptoTarget,
			bool maySkip, bool[,] blocked)
		{
			maxima.Clear();
			globalMax = 0;

			for (int i = 1; i <= uptoSource; ++i)
			{
				for (int j = 1; j <= uptoTarget; ++j)
				{
					_Table[i, j].Score = 0;

					if (blocked != null && blocked[i, j])
						// already aligned
						continue;

					int alignmentScore = _AlignmentScores[i - 1, j - 1];

					int c1 = _Table[i - 1, j - 1].Score + alignmentScore;
					if (c1 > _Table[i, j].Score)
					{
						_Table[i, j].Score = c1;
						_Table[i, j].BackI = i - 1;
						_Table[i, j].BackJ = j - 1;
						_Table[i, j].Op = alignmentScore > 0 ? Operation.Align : Operation.Noise;
					}

					if (maySkip)
					{
						// TODO not sure whether t1 and t2 should not be swapped here

						int c2 = _Table[i - 1, j].Score + _SourceSkipScores[i - 1];
						if (c2 > _Table[i, j].Score)
						{
							_Table[i, j].Score = c2;
							_Table[i, j].BackI = i - 1;
							_Table[i, j].BackJ = j;
							_Table[i, j].Op = Operation.Skip;
						}

						int c3 = _Table[i, j - 1].Score + _TargetSkipScores[j - 1];
						if (c3 > _Table[i, j].Score)
						{
							_Table[i, j].Score = c3;
							_Table[i, j].BackI = i;
							_Table[i, j].BackJ = j - 1;
							_Table[i, j].Op = Operation.Skip;
						}
					}

					if (_Table[i, j].Score > globalMax)
					{
						maxima.Clear();
						globalMax = _Table[i, j].Score;
						maxima.Add(new Pair<int>(i, j));
					}
					else if (_Table[i, j].Score > 0 && _Table[i, j].Score == globalMax)
					{
						maxima.Add(new Pair<int>(i, j));
					}
				}
			}
		}

		private void ComputeMaximaForLCS(List<Pair<int>> maxima, ref int globalMax,
			int uptoSource, int uptoTarget)
		{
			maxima.Clear();
			globalMax = _Table[uptoSource, uptoTarget].ULMaxScore;

			for (int i = uptoSource; i > 0; --i)
			{
				if (_Table[i, uptoTarget].ULMaxScore < globalMax)
					// this doesn't really seem to be hit ever, but won't hurt much either.
					continue;

				for (int j = uptoTarget; j > 0; --j)
				{
					if (_Table[i, j].Score == globalMax)
					{
						maxima.Add(new Pair<int>(i, j));
						return;
					}
				}
			}
		}

		/// <summary>
		/// Computes the longest local alignment coverage of the two sequences used to 
		/// initialize this instance. Unlike <see cref="M:Compute()"/>, you can specify
		/// positions in the sequences up to which to compute the alignment. 
		/// </summary>
		/// <param name="uptoSource">The maximum index to cover in the source sequence (exclusive)</param>
		/// <param name="uptoTarget">The maximum index to cover in the target sequence (exclusive)</param>
		public List<AlignedSubstring> Compute(int uptoSource, int uptoTarget)
		{
			if (uptoSource <= 0 || uptoSource > _Source.Count)
				throw new ArgumentOutOfRangeException("uptoSource");
			if (uptoTarget <= 0 || uptoTarget > _Target.Count)
				throw new ArgumentOutOfRangeException("uptoTarget");

			List<AlignedSubstring> result = new List<AlignedSubstring>();

			int globalMax = 0;
			List<Pair<int>> maxima = new List<Pair<int>>();

			if (_AlignmentScores == null)
				_AlignmentScores = ComputeScores(_Source, _Target, _Scorer);

			bool maySkip = _Scorer.MaySkip;
			if (maySkip)
			{
				if (_SourceSkipScores == null || _TargetSkipScores == null)
				{
					ComputeSkipScoreCaches();
				}
			}

			bool computeCoverage = (_MaxItems != 1);

			if (_Table == null)
			{
				_Table = new Cell[_Source.Count + 1, _Target.Count + 1];
				if (!computeCoverage)
					ComputeFullTable(maySkip);
			}

			bool[,] blocked = null;
			if (computeCoverage)
			{
				blocked = new bool[_Source.Count + 1, _Target.Count + 1];
			}

			do
			{
				if (computeCoverage)
					ComputeMaximaForCoverage(maxima, ref globalMax, uptoSource, uptoTarget, maySkip, blocked);
				else
					ComputeMaximaForLCS(maxima, ref globalMax, uptoSource, uptoTarget);

				if (maxima.Count > 0)
				{
					List<AlignedSubstring> extensionCandiates = new List<AlignedSubstring>();

					foreach (Pair<int> max in maxima)
					{
						// read out the transition
						int iStart = max.Left;
						int jStart = max.Right;

						int len = 0;

						while (_Table[iStart, jStart].Score > 0)
						{
							Cell c = _Table[iStart, jStart];
							if (c.Op == Operation.Align)
							{
								// not a skip
								len++;
							}
							iStart = c.BackI;
							jStart = c.BackJ;
						}

						// aligned sequence is s1[iStart, globalMaxI[, s2[jStart, globalMaxJ[

						AlignedSubstring lsa
							= new AlignedSubstring(iStart, max.Left - iStart, jStart, max.Right - jStart, globalMax, len);

						if (len >= _MinLength)
							extensionCandiates.Add(lsa);
					}

					if (extensionCandiates.Count == 0)
					{
						// if we have maxima, but no extension candidates, they got dropped by the minLength
						//  requirement. To avoid infinite loops, we must stop further iteration and set the 
						//  break criterion.
						maxima.Clear();
					}
					else
					{
						AlignedSubstring winner = null;
						if (_Picker == null)
							// if we have no disambiguator, pick the first candidate
							// TODO other defaults may be better (longest, etc.)
							winner = extensionCandiates[0];
						else
							winner = _Picker.PickExtension(result, extensionCandiates);

						if (winner == null)
							break;
						else
						{
							if (blocked != null)
							{
								// mark the covered ranges as "taken" so that we don't get overlaps
								for (int iStart = 1; iStart <= uptoSource; ++iStart)
								{
									for (int jStart = 1; jStart <= uptoTarget; ++jStart)
									{
										if ((iStart > winner.Source.Start && iStart <= winner.Source.Start + winner.Source.Length)
											|| (jStart > winner.Target.Start && jStart <= winner.Target.Start + winner.Target.Length))
										{
											blocked[iStart, jStart] = true;
										}
									}
								}
							}
							result.Add(winner);
						}
					}
				}

			} while (maxima.Count > 0 && (_MaxItems == 0 || result.Count < _MaxItems));

			return result;
		}

	}
}
