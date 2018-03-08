using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
	internal class RepetitiveNode : Node
	{
		private int _Lower;
		private int _Upper;
		private Node _Content;

		public const int INFINITY = -1;

		public RepetitiveNode(int lower, int upper)
		{
			System.Diagnostics.Debug.Assert(upper == INFINITY || upper >= lower);
			_Lower = lower;
			_Upper = upper;
		}

		public override string GetExpression()
		{
			char op;

			if (_Lower == 0 && _Upper == 1)
				op = '?';
			else if (_Lower == 0 && _Upper == INFINITY)
				op = '*';
			else if (_Lower == 1 && _Upper == INFINITY)
				op = '+';
			else
				op = '!';

			return String.Format("({0}){1}", _Content.GetExpression(), op);
		}

		public Node Content
		{
			get { return _Content; }
			set
			{
				value.Owner = this;
				_Content = value;
			}
		}

		public override FST GetFST()
		{
			FST sub = _Content.GetFST();

			// catch a couple of special cases
			if (_Lower == 0 && _Upper == 1 // optional (?)
			 || _Lower == 1 && _Upper == INFINITY // plus-closure (+)
			 || _Lower == 0 && _Upper == INFINITY)  // kleene-closure (*)
			{
				int subStart = sub.GetStartState();

				// the order of the next two tests matters 

				if (_Upper == INFINITY)
				{
					List<int> finalStates = sub.GetFinalStates();

					/* for each transition ending in a final state, copy that transition to
					 * point to the single start state
					 */
					foreach (int source in sub.GetStates())
					{
						IList<FSTTransition> transitions = sub.GetTransitions(source);
						for (int t = transitions.Count - 1; t >= 0; --t)
						{
							FSTTransition trans = transitions[t];
							if (finalStates.Contains(trans.Target))
							{
								sub.AddTransition(source, subStart, new Label(trans.Input), new Label(trans.Output));
							}
						}
					}
				}

				if (_Lower == 0)
				{
					// optionality or kleene star - simply make start state final
					sub.SetFinal(subStart, true);
				}
			}
			else
				throw new NotImplementedException();

			return sub;
		}
	}
}
