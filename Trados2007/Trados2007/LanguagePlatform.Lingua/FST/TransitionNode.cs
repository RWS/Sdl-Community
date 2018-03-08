using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
	internal class TransitionNode : Node
	{
		private Label _Input;
		private Label _Output;

		public TransitionNode(Label input, Label output)
		{
			_Input = input;
			_Output = output;
		}

		public override string GetExpression()
		{
			if (_Input.Equals(_Output))
				return _Input.ToString();
			else
				return String.Format("<{0}:{1}>", _Input.ToString(), _Output.ToString());
		}

		public override FST GetFST()
		{
			FST result = new FST();

			int start = result.AddState();
			result.SetInitial(start);

			if (_Input.IsEpsilon && _Output.IsEpsilon)
			{
				// this results in an automaton without any transitions, which accepts the empty langauge
				result.SetFinal(start, true);
			}
			else
			{
				int end = result.AddState();
				result.SetFinal(end, true);
				result.AddTransition(start, end, _Input, _Output);
			}

			return result;
		}
	}
}
