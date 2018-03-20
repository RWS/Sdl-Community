using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class MatchState
	{
		private int _State;
		private List<Label> _Output;
		private int _InputPosition;
		private int _ConsumedSymbols;

		public MatchState(int s)
		{
			_State = s;
			_Output = new List<Label>();
			_InputPosition = 0;
			_ConsumedSymbols = 0;
		}

		public MatchState(MatchState other)
		{
			_State = other._State;
			_Output = new List<Label>(other._Output);
			_InputPosition = other._InputPosition;
			_ConsumedSymbols = other._ConsumedSymbols;
		}

		public int InputPosition
		{
			get { return _InputPosition; }
			set { _InputPosition = value; }
		}

		public List<Label> Output
		{
			get { return _Output; }
		}

		public int State
		{
			get { return _State; }
		}

		public string GetOutputAsString()
		{
			if (_Output == null || _Output.Count == 0)
				return null;
			System.Text.StringBuilder result = new StringBuilder();

			foreach (Label l in _Output)
			{
				if (l.IsCharLabel)
					result.Append((char)l.Symbol);
				else if (l.Symbol == Label.SpecialSymbolWhitespace)
					result.Append(' ');
				// TODO other cases
			}

			return result.ToString();
		}

		public int ConsumedSymbols
		{
			get { return _ConsumedSymbols; }
		}

		public void AppendOutput(Label l)
		{
			if (_Output != null && l.IsConsuming)
				_Output.Add(l);
		}

		/// <summary>
		/// Attempts to traverse the specified transition, given the current match state. 
		/// </summary>
		/// <param name="input">The string input</param>
		/// <param name="t">The transition to probe</param>
		/// <param name="mode">The match mode</param>
		/// <returns>The new match state or null if the transition cannot be traversed</returns>
		public MatchState Traverse(string input, FSTTransition t, Matcher.MatchMode mode, bool ignoreCase)
		{
#if false
			bool consumed;
			Label output;

			if (t.CanTraverse(mode, input, _InputPosition, ignoreCase, out output, out consumed))
			{
				MatchState result = new MatchState(this);
				result._State = t.Target;

				result.AppendOutput(output);

				if (consumed)
				{
					++result._ConsumedSymbols;
					++result._InputPosition;
				}

				return result;
			}
			else
				return null;

#else
			Label matchLabel;
			Label outputLabel;

			switch (mode)
			{
			case Matcher.MatchMode.Analyse:
				matchLabel = t.Input;
				outputLabel = t.Output;
				break;
			case Matcher.MatchMode.Generate:
				matchLabel = t.Output;
				outputLabel = t.Input;
				break;
			default:
				throw new Exception("Illegal case constant");
			}

			if (matchLabel.Matches(input, _InputPosition, ignoreCase))
			{
				MatchState result = new MatchState(this);
				result._State = t.Target;

				if (outputLabel.IsConsuming)
					result._Output.Add(outputLabel);

				if (matchLabel.IsConsuming)
				{
					++result._ConsumedSymbols;
					++result._InputPosition;
				}

				return result;
			}
			return null;
#endif
		}

	}
}
