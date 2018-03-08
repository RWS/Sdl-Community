using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	/**
	 * @author Oli
	 * 
	 * Ported from Genesis/java on 03-JAN-2006 (Oli)
	 * 
	 * Represents an FST state
	 */

	public class State
	{
		private bool _IsStart = false;
		private bool _IsFinal = false;
		private List<FSTTransition> _Transitions;
		private int _Id;

		private bool _TransitionsSorted;

		public State()
		{
			_Transitions = new List<FSTTransition>();
			_TransitionsSorted = true;
		}

		public List<FSTTransition> Transitions
		{
			get { return _Transitions; }
		}

		public bool IsInitial
		{
			get { return _IsStart; }
			set { _IsStart = value; }
		}

		public bool IsFinal
		{
			get { return _IsFinal; }
			set { _IsFinal = value; }
		}

		public int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		public int TransitionCount
		{
			get { return _Transitions.Count; }
		}

		public void AddTransition(int target, Label input, Label output)
		{
			if (!HasTransition(target, input, output))
			{
				FSTTransition t = new FSTTransition(_Id, target, input, output);
				_Transitions.Add(t);

				if (_TransitionsSorted && _Transitions.Count > 1)
				{
					// compare the transition we just added to the last one in the previous list
					int tc = _Transitions.Count;
					if (_Transitions[tc - 1].CompareTo(_Transitions[tc - 2]) <= 0)
						_TransitionsSorted = false;
				}
			}
		}

		public void RemoveTransition(int target, Label input, Label output)
		{
			int i = FindTransitionInternal(target, input, output);
			if (i >= 0)
				_Transitions.RemoveAt(i);
		}

		public void RemoveTransitionAt(int p)
		{
			_Transitions.RemoveAt(p);
			// Removing doesn't change the sort property
			// _TransitionsSorted = false;
		}

		public bool HasTransition(int target, Label input, Label output)
		{
			return FindTransition(target, input, output) != null;
		}

		public FSTTransition FindTransition(int target, Label input, Label output)
		{
			int p = FindTransitionInternal(target, input, output);
			return (p < 0) ? null : _Transitions[p];
		}

		private int FindTransitionInternal(int target, Label input, Label output)
		{
			// TODO make use of "TransitionsSorted" property

			for (int i = 0; i < _Transitions.Count; ++i)
			{
				// transitions may temporarily be null during DFA construction
				if (_Transitions[i] != null)
				{
					System.Diagnostics.Debug.Assert(_Transitions[i].Source == _Id);

					if (_Transitions[i].Target == target
						&& _Transitions[i].Input.Equals(input)
						&& _Transitions[i].Output.Equals(output))
						return i;
				}
			}
			return -1;
		}

		public void SortTransitions()
		{
			if (_Transitions.Count > 1)
			{
				_Transitions.Sort();
			}
			_TransitionsSorted = true;
		}

		public bool TransitionsSorted
		{
			get { return _TransitionsSorted; }
		}

		private bool EnsureSourceState()
		{
			for (int i = 0; i < _Transitions.Count; ++i)
				// transitions may temporarily be null during DFA construction
				if (_Transitions[i] != null && _Transitions[i].Source != _Id)
					return false;
			return true;
		}

		/// <summary>
		/// Determines whether the state is deterministic. It is not deterministic if it has any
		/// outgoing true eps transitions or two transitions have the same input/output label.
		/// </summary>
		public bool IsDeterministic()
		{
			// TODO cache this flag and only recompute if required

			if (_Transitions == null || _Transitions.Count == 0)
				return true;

			System.Diagnostics.Debug.Assert(EnsureSourceState());

			// TODO don't resort if transitions are already sorted
			SortTransitions();

			if (_Transitions[0].IsEpsilon)
				return false;

			for (int t = 1; t < _Transitions.Count; ++t)
			{
				if ((_Transitions[t].Input.Symbol == _Transitions[t - 1].Input.Symbol
					&& _Transitions[t].Output.Symbol == _Transitions[t - 1].Output.Symbol)
					|| _Transitions[t].IsEpsilon)
					return false;
			}

			return true;
		}

		public bool HasEpsilonTransitions()
		{
			System.Diagnostics.Debug.Assert(EnsureSourceState()); 
			return _Transitions.Any(t => t.IsEpsilon);
		}
	}
}
