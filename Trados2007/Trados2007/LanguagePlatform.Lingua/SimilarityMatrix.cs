using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	internal class SimilarityMatrix
	{
		public delegate double TokenSimilarityComputer(Core.Tokenization.Token a, Core.Tokenization.Token b);

		private IList<Core.Tokenization.Token> _SourceTokens;
		private IList<Core.Tokenization.Token> _TargetTokens;
		private double[,] _Sim;
		private bool _UseStringEditDistance;
		private Core.Tokenization.BuiltinRecognizers _DisabledAutoSubstitutions;

		private static readonly double _Uncomputed = -1000.0d;

		public SimilarityMatrix(IList<Core.Tokenization.Token> sourceTokens,
			IList<Core.Tokenization.Token> targetTokens, 
			bool useStringEditDistance, 
			Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions)
		{
			_SourceTokens = sourceTokens;
			_TargetTokens = targetTokens;
			_UseStringEditDistance = useStringEditDistance;
			_DisabledAutoSubstitutions = disabledAutoSubstitutions;

			_Sim = new double[_SourceTokens.Count, _TargetTokens.Count];
			for (int s = 0; s < _SourceTokens.Count; ++s)
			{
				for (int t = 0; t < _TargetTokens.Count; ++t)
				{
					_Sim[s, t] = _Uncomputed;
				}
			}
		}

		public IList<Core.Tokenization.Token> SourceTokens
		{
			get { return _SourceTokens; }
		}

		public IList<Core.Tokenization.Token> TargetTokens
		{
			get { return _TargetTokens; }
		}

		public bool IsAssigned(int s, int t)
		{
			return _Sim[s, t] != _Uncomputed;
		}

		public double this[int s, int t]
		{
			get
			{
				double result = _Sim[s, t];

				if (result == _Uncomputed)
				{
					result = SimilarityComputers.GetTokenSimilarity(_SourceTokens[s], _TargetTokens[t],
						_UseStringEditDistance, _DisabledAutoSubstitutions);
					_Sim[s, t] = result;
				}
				return result;
			}
			set
			{
				_Sim[s, t] = value;
			}
		}

		public int CountUncomputedValues()
		{
			int result = 0;

			for (int i = 0; i < _SourceTokens.Count; ++i)
				for (int j = 0; j < _TargetTokens.Count; ++j)
				{
					if (_Sim[i, j] == _Uncomputed)
						++result;
				}
			return result;
		}

		public void Compute(bool computeDiagonalOnly)
		{
			for (int i = 0; i < _SourceTokens.Count; ++i)
			{
				for (int j = 0; j < _TargetTokens.Count; ++j)
				{
					if (_Sim[i, j] != _Uncomputed)
						continue;

					if (computeDiagonalOnly)
					{
						if (i == j)
							_Sim[i, j] = SimilarityComputers.GetTokenSimilarity(_SourceTokens[i], 
								_TargetTokens[j], _UseStringEditDistance, _DisabledAutoSubstitutions);
						else
							_Sim[i, j] = -1.0d;
					}
					else
						_Sim[i, j] = SimilarityComputers.GetTokenSimilarity(_SourceTokens[i],
							_TargetTokens[j], _UseStringEditDistance, _DisabledAutoSubstitutions);
				}
			}
		}
	}
}
