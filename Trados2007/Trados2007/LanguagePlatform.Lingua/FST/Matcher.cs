using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class Matcher
	{
		public delegate bool MatchFoundCallback(MatchState ms);
		public delegate bool ContinueIterationCallback(int ticks);

		private FST _FST;

		public enum MatchMode
		{
			Analyse,
			Generate
		}

		public Matcher(FST fst)
		{
			_FST = fst;
		}

		public void Match(string s, bool matchWholeInput, MatchMode mode,
			bool ignoreCase,
			MatchFoundCallback matchFoundCallback,
			ContinueIterationCallback continueIterationCallback)
		{
			Match(s, matchWholeInput, mode, 0, ignoreCase, matchFoundCallback, continueIterationCallback);
		}

		/// <summary>
		/// Finds and collects all matches.
		/// </summary>
		public List<MatchState> Match(string input, bool matchWholeInput, MatchMode mode, int startOffset,
			bool ignoreCase)
		{
			List<MatchState> result = null;

			Match(input, matchWholeInput, mode, startOffset, ignoreCase,
				delegate(MatchState ms)
				{
					if (result == null)
						result = new List<MatchState>();
					result.Add(ms);
					return true;
				}, null);


			return result;
		}

		public void Match(string input, bool matchWholeInput, MatchMode mode, int startOffset,
			bool ignoreCase,
			MatchFoundCallback matchFoundCallback,
			ContinueIterationCallback continueIterationCallback)
		{
			if (startOffset < 0)
				throw new ArgumentOutOfRangeException("startOffset");
			if (matchFoundCallback == null)
				throw new ArgumentNullException("callback");

			int inputLength = String.IsNullOrEmpty(input) ? 0 : input.Length;
			if (startOffset > inputLength)
				return;

			int start = _FST.GetStartState();
			if (start < 0)
				return;

#if DEBUG
			bool dumpAutomaton = false;
			if (dumpAutomaton)
				_FST.Dump("D:/temp/fst.txt");
#endif

			// TODO make sure the FST is not modified during transition

			// NOTE the match callbacks throw each found match, not just the longest one. The
			//  caller needs to make sure that longer matches are scanned.

			List<MatchState> states = new List<MatchState>();
			bool continueMatchProcess = true;
			MatchState startState = new MatchState(start);

			startState.InputPosition = startOffset;
			states.Add(startState);

			if (_FST.IsFinal(startState.State))
			{
				if (!matchWholeInput || startState.InputPosition >= inputLength)
					continueMatchProcess = matchFoundCallback(startState);
			}

			// remember the number of attempted/tested transitions to send caller a keep-alive signal
			int ticks = 0;
			bool logTransitions = false;

			while (states.Count > 0 && continueMatchProcess)
			{
				/* we put the transition target states into a new list so that we don't mess
				 * around with the current one.
				 */

				List<MatchState> newStates = new List<MatchState>();

				for (int s = states.Count - 1; s >= 0; --s)
				{
					MatchState ms = states[s];
					State state = _FST.GetState(ms.State);

					foreach (FSTTransition trans in state.Transitions)
					{
						/*
						if (state.TransitionsSorted
							&& !ignoreCase
							&& (mode == MatchMode.Analyse)
							&& trans.Input.IsCharLabel
							&& ms.InputPosition < input.Length
							&& input[ms.InputPosition] > trans.Input.Symbol)
						{
							break;
						}
						 */

						++ticks;
						MatchState next;
						if ((next = ms.Traverse(input, trans, mode, ignoreCase)) != null)
						{
							if (logTransitions)
							{
								System.Diagnostics.Debug.WriteLine(String.Format("\tTraversing from ({0}, {1}/{2}) to ({3}, {4}/{5}) on {6}",
									ms.State, ms.InputPosition, ms.ConsumedSymbols,
									next.State, next.InputPosition, next.ConsumedSymbols,
									ms.InputPosition < input.Length ? input[ms.InputPosition] : '#'));
							}

							newStates.Add(next);
							if (_FST.IsFinal(next.State))
							{
								if (!matchWholeInput || next.InputPosition >= inputLength)
									continueMatchProcess = matchFoundCallback(next);
							}
						}
					}
				}

				states = null;
				states = newStates;

				if ((ticks % 1000) == 0 && continueMatchProcess && continueIterationCallback != null)
				{
					continueMatchProcess = continueIterationCallback(ticks);
				}
			}
		}

	}
}
