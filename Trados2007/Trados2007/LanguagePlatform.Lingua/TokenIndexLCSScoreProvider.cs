using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// A score provider for computing the longest common substring or longest common subsequence
	/// of two token vectors. Use with <see cref="Sdl.LanguagePlatform.Core.Alignment.SequenceAlignmentComputer&lt;T&gt;"/>
	/// <para>The similarity scores are retrieved from a similarity matrix which must also be provided.</para>
	/// <para>To align two token sequences, you first need to construct position lists which map to the
	/// token position in the input vectors.</para>
	/// </summary>
	internal class TokenIndexLCSScoreProvider 
		: ISequenceAlignmentItemScoreProvider<int>
	{
		private SimilarityMatrix _SimMatrix;
		private double _Threshold;
		private bool _MaySkip;

		/// <summary>
		/// blablabla
		/// </summary>
		/// <param name="simMatrix"></param>
		/// <param name="threshold"></param>
		/// <param name="maySkip">If true, computes the longest common subsequence. Otherwise,
		/// computes the longest common substring.
		/// </param>
		public TokenIndexLCSScoreProvider(SimilarityMatrix simMatrix, 
			double threshold,
			bool maySkip)
		{
			_SimMatrix = simMatrix;
			_Threshold = threshold;
			_MaySkip = maySkip;
		}

		public int GetAlignScore(int a, int b)
		{
			int sp = a;
			int tp = b;

			Core.Tokenization.Token st = _SimMatrix.SourceTokens[sp];
			Core.Tokenization.Token tt = _SimMatrix.TargetTokens[tp];

			double v = _SimMatrix[a, b];
			return v >= _Threshold ? 1 : -100000;
		}

		public int GetSourceSkipScore(int a)
		{
			return _MaySkip ? -1 : -100000;
		}

		public int GetTargetSkipScore(int a)
		{
			return _MaySkip ? -1 : -100000;
		}

		public bool MaySkip
		{
			get { return _MaySkip; }
		}

	}

}
