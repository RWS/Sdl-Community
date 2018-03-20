using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Provides methods to compute the longest common subsequence between two 
	/// item sequences.
	/// </summary>
	/// <typeparam name="T">The type of the elements of the sequences.</typeparam>
	public class LongestCommonSubsequenceComputer<T>
	{
		/// <summary>
		/// Computes the longest common subsequence of two input sequences of type T. To compare
		/// the items, .Equal() is called on the objects. Note that the longest common subsequence 
		/// does not have to be contiguous (it's not the longest common substring).
		/// </summary>
		/// <param name="sequenceA">The first (source) sequence</param>
		/// <param name="sequenceB">The second (target) sequence</param>
		/// <returns>A list of index pairs {(a, b)} where a is  a position in <paramref name="sequenceA"/>
		/// and b is a position in <paramref name="sequenceB"/>. It's guaranteed that A[a].Equals(B[b]). 
		/// The list represents the longest common subsequence.
		/// </returns>
		public static List<Pair<int>> ComputeLongestCommonSubsequence(IList<T> sequenceA,
			IList<T> sequenceB)
		{
			if (sequenceA == null || sequenceA.Count == 0
				|| sequenceB == null || sequenceB.Count == 0)
				return null;

			int lA = sequenceA.Count;
			int lB = sequenceB.Count;

			int[,] table = new int[lA, lB];

			for (int i = 0; i < lA; i++)
			{
				for (int j = 0; j < lB; j++)
				{
					if (sequenceA[i].Equals(sequenceB[j]))
					{
						if (i == 0 || j == 0)
							table[i, j] = 1;
						else
							table[i, j] = 1 + table[i - 1, j - 1];
					}
					else
					{
						if (i == 0 && j == 0)
							table[i, j] = 0;
						else if (i == 0)
							table[i, j] = Math.Max(0, table[i, j - 1]);
						else if (j == 0)
							table[i, j] = Math.Max(table[i - 1, j], 0);
						else
							table[i, j] = Math.Max(table[i - 1, j], table[i, j - 1]);
					}
				}
			}

			int lcsLength = table[lA - 1, lB - 1];

			// readout the actual common subsequence

			if (lcsLength == 0)
				return null;

			List<Pair<int>> result = new List<Pair<int>>();

			int pA = lA - 1;
			int pB = lB - 1;

			while (pA >= 0 && pB >= 0)
			{
				if (sequenceA[pA].Equals(sequenceB[pB]))
				{
					result.Insert(0, new Pair<int>(pA, pB));
					--pA;
					--pB;
				}
				else if (pA == 0)
					--pB;
				else if (pB == 0)
					--pA;
				else if (table[pA, pB - 1] >= table[pA - 1, pB])
					--pB;
				else
					--pA;
			}

			System.Diagnostics.Debug.Assert(result.Count == lcsLength);

			return result;
		}

		/// <summary>
		/// Computes the longest common substring of two input sequences of a common element type <typeparamref name="T"/>. To compare
		/// the items, .Equal() is called on the objects. A longest common substring is always 
		/// contiguous (unlike the longest common subsequence). Note
		/// that if more than one longest common substring exists, all are returned.
		/// </summary>
		/// <param name="sequenceA">The first (source) sequence</param>
		/// <param name="sequenceB">The second (target) sequence</param>
		/// <returns>A list of substring objects, which can be null (null/empty argument), empty (no common
		/// substring), or contain the longest common substrings of the two input sequences (which may not be 
		/// unique and may overlap).
		/// </returns>
		public static List<AlignedSubstring> ComputeLongestCommonSubstring(IList<T> sequenceA, 
			IList<T> sequenceB)
		{
			// http://en.wikipedia.org/wiki/Longest_common_substring_problem

			if (sequenceA == null || sequenceA.Count == 0
				|| sequenceB == null || sequenceB.Count == 0)
				return null;

			List<AlignedSubstring> result = new List<AlignedSubstring>();

			int lA = sequenceA.Count;
			int lB = sequenceB.Count;

			int[,] table = new int[lA, lB];

			int longest = 0;

			for (int i = 0; i < lA; i++)
			{
				for (int j = 0; j < lB; j++)
				{
					if (sequenceA[i].Equals(sequenceB[j]))
					{
						if (i == 0 || j == 0)
							table[i, j] = 1;
						else
							table[i, j] = 1 + table[i - 1, j - 1];

						if (table[i, j] > longest)
						{
							longest = table[i, j];
							result.Clear();
						}

						if (table[i, j] == longest)
						{
							result.Add(new AlignedSubstring(i - longest + 1, longest, j - longest + 1, longest));
						}
					}
				}
			}

			if (result.Count > 0)
			{
				result.Sort((x, y) => y.Source.Length - x.Source.Length);
			}

#if DEBUG
			if (result.Count > 0)
			{
				foreach (AlignedSubstring s in result)
				{
					System.Diagnostics.Debug.Assert(s.Source.Length == s.Target.Length);

					System.Diagnostics.Debug.Assert(s.Source.Start >= 0 && s.Source.Start < sequenceA.Count);
					System.Diagnostics.Debug.Assert(s.Target.Start >= 0 && s.Target.Start < sequenceB.Count);
					for (int p = 0; p < s.Source.Length; ++p)
					{
						System.Diagnostics.Debug.Assert(sequenceA[s.Source.Start + p].Equals(sequenceB[s.Target.Start + p]));
					}
				}
			}
#endif

			return result;
		}

		/// <summary>
		/// Computes the longest common substrings of two input sequences of a common element type <typeparamref name="T"/>. To compare
		/// the items, .Equal() is called on the objects. A longest common substring is always 
		/// contiguous (unlike the longest common subsequence, which is not necessarily continguous). This method 
		/// will compute the broadest coverage for the input sequences. This means that first the longest substring
		/// is found. Unlike <see cref="ComputeLongestCommonSubstring"/>, the search is then continued and more (possibly shorter)
		/// longest substrings are identified in the remainder (uncovered portion) of the string. Note that the
		/// coverage is not necessarily optimal - if more than one longest common substring exists during any 
		/// iteration, the first one found is picked which may lead to a suboptimal overall coverage. 
		/// </summary>
		/// <param name="sequenceA">The first (source) sequence</param>
		/// <param name="sequenceB">The second (target) sequence</param>
		/// <returns>A list of substring objects, which can be null (null/empty argument), empty (no common
		/// substring), or contain a good coverage of the input sequences. 
		/// </returns>
		public static List<AlignedSubstring> ComputeLongestCommonSubstringCoverage(IList<T> sequenceA,
			IList<T> sequenceB)
		{
			// http://en.wikipedia.org/wiki/Longest_common_substring_problem

			if (sequenceA == null || sequenceA.Count == 0
				|| sequenceB == null || sequenceB.Count == 0)
				return null;

			List<AlignedSubstring> result = SequenceAlignmentComputer<T>.ComputeCoverage(sequenceA,
				sequenceB, new SimpleLCSScoreProvider<T>(), null);

#if DEBUG
			if (result != null && result.Count > 0)
			{
				foreach (AlignedSubstring s in result)
				{
					System.Diagnostics.Debug.Assert(s.Source.Length == s.Target.Length);
					System.Diagnostics.Debug.Assert(s.Source.Start >= 0 && s.Source.Start < sequenceA.Count);
					System.Diagnostics.Debug.Assert(s.Target.Start >= 0 && s.Target.Start < sequenceB.Count);
					for (int p = 0; p < s.Source.Length; ++p)
					{
						System.Diagnostics.Debug.Assert(sequenceA[s.Source.Start + p].Equals(sequenceB[s.Target.Start + p]));
					}
				}
			}
#endif

			return result;
		}


	}
}
