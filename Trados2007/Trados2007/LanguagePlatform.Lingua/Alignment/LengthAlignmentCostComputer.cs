using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Provides an <see cref="IAlignmentCostComputer&lt;T&gt;"/> which can be used to sequences
	/// of integer numbers (which may be indices into underlying sequences).
	/// </summary>
	public class LengthAlignmentCostComputer : IAlignmentCostComputer<int>
	{
		private double _ExpansionFactor = 1.0d;

		// empirically estimated variance for the lengths
		private static double _Variance = 6.8d;

		// "empirically estimated" probabilities for the basic alignment operations
		private static double _SubstitutionProbability = 0.89d;
		private static double _InsertionOrDeletionProbability = 0.01d;
		private static double _ExpansionOrContractionProbability = 0.09d;
		private static double _MeldingProbability = 0.01d;

		/// <summary>
		/// Initializes a new instance with the provided expansion factor.
		/// </summary>
		/// <param name="expansionFactor">An empirically estimated expansion factor
		/// which estimates the average target sequence length.</param>
		public LengthAlignmentCostComputer(double expansionFactor)
		{
			_ExpansionFactor = expansionFactor;
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetSubstitutionCosts"/>
		/// </summary>
		public virtual int GetSubstitutionCosts(int s, int t)
		{
			return (int)(-100.0d * GetScore(s, t));
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetDeletionCosts"/>
		/// </summary>
		public virtual int GetDeletionCosts(int s)
		{
			int penalty = (int)(-100.0d * Math.Log(_InsertionOrDeletionProbability / _SubstitutionProbability));
			return penalty + (int)(-100.0d * GetScore(s, 0));
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetInsertionCosts"/>
		/// </summary>
		public virtual int GetInsertionCosts(int t)
		{
			int penalty = (int)(-100.0d * Math.Log(_InsertionOrDeletionProbability / _SubstitutionProbability));
			return penalty + (int)(-100.0d * GetScore(0, t));
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetContractionCosts"/>
		/// </summary>
		public virtual int GetContractionCosts(int s1, int s2, int t)
		{
			int penalty = (int)(-100.0d * Math.Log(_ExpansionOrContractionProbability / _SubstitutionProbability));
			return penalty + (int)(-100.0d * GetScore(s1 + s2, t));
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetExpansionCosts"/>
		/// </summary>
		public virtual int GetExpansionCosts(int s, int t1, int t2)
		{
			int penalty = (int)(-100.0d * Math.Log(_ExpansionOrContractionProbability / _SubstitutionProbability));
			return penalty + (int)(-100.0d * GetScore(s, t1 + t2));
		}

		/// <summary>
		/// <see cref="IAlignmentCostComputer&lt;T&gt;.GetMeldingCosts"/>
		/// </summary>
		public virtual int GetMeldingCosts(int s1, int s2, int t1, int t2)
		{
			int penalty = (int)(-100.0d * Math.Log(_MeldingProbability / _SubstitutionProbability));
			return penalty + (int)(-100.0d * GetScore(s1 + s2, t1 + t2));
		}

		private double GetScore(int srcLen, int trgLen)
		{
			// computes P(delta|match)

			double p = 1.0d;
			if (srcLen != 0 || trgLen != 0)
			{
				double lenDiff = ((double)srcLen * _ExpansionFactor) - (double)trgLen;
				double mean = (srcLen + trgLen / _ExpansionFactor) / 2.0d;
				p = Math.Abs(lenDiff / Math.Sqrt(mean * _Variance));
			}
			return 2.0d * (1.0d - pnorm(p));
		}

		/// <summary>
		/// Returns an approximation of the area under the standardized normal distribution 
		/// between -inf and z standard deviations. A Standardized Normal Distribution is
		/// a normal distribution with mean 0 and variance 1.
		/// </summary>
		public static double pnorm(double z)
		{
			// cf. Abramowitz/Stegun 1964, 26.2.17, p.932
			double t, pd;
			t = 1.0d / (1.0d + 0.2316419d * z);
			pd = 1.0d - 0.3989423d *
				Math.Exp(-z * z / 2.0d) *
				((((1.330274429d * t - 1.821255978d) * t
				+ 1.781477937d) * t - 0.356563782d) * t + 0.319381530d) * t;
			return pd;
		}

	}
}
