using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// A simple string alignment cost computer which uses string lengths and an 
	/// empirically determined expansion factor for the score computation.
	/// </summary>
	public class StringLengthAlignmentCostComputer : IAlignmentCostComputer<string>
	{
		private LengthAlignmentCostComputer _CostComputer;

		/// <summary>
		/// Initializes a new instance with the provided expansion factor, which 
		/// is the average length of a target string given a source string of length 1.0.
		/// </summary>
		/// <param name="expansionFactor">The expansion factor</param>
		public StringLengthAlignmentCostComputer(double expansionFactor)
		{
			_CostComputer = new LengthAlignmentCostComputer(expansionFactor);
		}

		/// <summary>
		/// Computes the substitution (alignment) costs for the two strings.
		/// </summary>
		/// <param name="s">The source string</param>
		/// <param name="t">The target string</param>
		/// <returns>The substitution costs</returns>
		public int GetSubstitutionCosts(string s, string t)
		{
			return _CostComputer.GetSubstitutionCosts(s.Length, t.Length);
		}

		/// <summary>
		/// Computes the deletion costs for the provided source string.
		/// </summary>
		/// <param name="s">The string</param>
		/// <returns>The deletion costs</returns>
		public int GetDeletionCosts(string s)
		{
			return _CostComputer.GetDeletionCosts(s.Length);
		}

		/// <summary>
		/// Computes the insertion costs for the provided target string.
		/// </summary>
		/// <param name="t">The string</param>
		/// <returns>The insertion costs</returns>
		public int GetInsertionCosts(string t)
		{
			return _CostComputer.GetInsertionCosts(t.Length);
		}

		/// <summary>
		/// Computes the contraction costs for mapping two source strings to a single target string.
		/// </summary>
		/// <param name="s1">The first source string</param>
		/// <param name="s2">The second source string</param>
		/// <param name="t">The target string</param>
		/// <returns>The contraction costs</returns>
		public int GetContractionCosts(string s1, string s2, string t)
		{
			return _CostComputer.GetContractionCosts(s1.Length, s2.Length, t.Length);
		}

		/// <summary>
		/// Computes the expansion costs for mapping a single source string to two target strings.
		/// </summary>
		/// <param name="s">The source string</param>
		/// <param name="t1">The first target string</param>
		/// <param name="t2">The second target string</param>
		/// <returns>The expansion costs</returns>
		public int GetExpansionCosts(string s, string t1, string t2)
		{
			return _CostComputer.GetExpansionCosts(s.Length, t1.Length, t2.Length);
		}

		/// <summary>
		/// Computes the melding costs for mapping two source strings to two target strings.
		/// </summary>
		/// <param name="s1">The first source string</param>
		/// <param name="s2">The second source string</param>
		/// <param name="t1">The first target string</param>
		/// <param name="t2">The second target string</param>
		/// <returns>The melding costs</returns>
		public int GetMeldingCosts(string s1, string s2, string t1, string t2)
		{
			return _CostComputer.GetMeldingCosts(s1.Length, s2.Length, t1.Length, t2.Length);
		}

	}
}
