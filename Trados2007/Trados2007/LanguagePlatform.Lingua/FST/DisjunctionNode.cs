using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
	internal class DisjunctionNode : Node
	{
		private List<Node> _Alternatives;

		public DisjunctionNode()
		{
			_Alternatives = new List<Node>();
		}

		public override string GetExpression()
		{
			System.Text.StringBuilder builder = new StringBuilder("(");

			for (int i = 0; i < _Alternatives.Count; ++i)
			{
				if (i > 0)
					builder.Append("|");
				builder.Append(_Alternatives[i].GetExpression());
			}

			builder.Append(")");
			return builder.ToString();
		}

		public void Add(Node n)
		{
			n.Owner = this;
			_Alternatives.Add(n);
		}

		public List<Node> Alternatives
		{
			get { return _Alternatives; }
		}

		/*
		 * Compute an FST which recognizes the disjunction of the sub-FSTs. 
		 * 
		 * The returned automaton will usually have multiple final states, but not have (pure) epsilon transitions.
		 *  
		 * @return The new FST representing the disjunction of the inner FSTs
		 * */
		public override FST GetFST()
		{
			if (_Alternatives.Count == 0)
				throw new Exception("Internal error");

			FST result = _Alternatives[0].GetFST();
			if (_Alternatives.Count > 1)
			{
				List<FST> alternatives = new List<FST>();
				for (int i = 1; i < _Alternatives.Count; ++i)
					alternatives.Add(_Alternatives[i].GetFST());
				result.Disjunct(alternatives);
			}

			return result;
		}

	}
}
