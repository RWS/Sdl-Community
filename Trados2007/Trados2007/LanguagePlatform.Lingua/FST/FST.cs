using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class FST
	{
		private Dictionary<int, State> _States;
		private int _StartState;
		private int _MaxState = 0;

		private delegate bool TransitionProperty(FSTTransition t);

		public static readonly string ReservedCharacters = "<>()[]{}<>*+?\\^$|#:";

		private static char[] _SpecialCharsArray;

		static FST()
		{
			_SpecialCharsArray = ReservedCharacters.ToCharArray();
		}

		public static bool IsSpecial(char c)
		{
			return ReservedCharacters.IndexOf(c) >= 0;
		}

		public static string EscapeSpecial(char c)
		{
			if (_SpecialCharsArray.Contains(c))
				return "\\" + c;
			else
				return String.Empty + c;
		}

		public static string EscapeSpecial(string s)
		{
			if (String.IsNullOrEmpty(s))
				return s;
			if (s.IndexOfAny(_SpecialCharsArray) < 0)
			{
				return s;
			}

			// TODO can probably be optimized by using IndexOfAny and appending blocks of ok chars

			System.Text.StringBuilder sb = new StringBuilder();
			for (int p = 0; p < s.Length; ++p)
			{
				char c = s[p];
				if (_SpecialCharsArray.Contains(c))
				{
					sb.Append("\\");
				}
				sb.Append(c);
			}

			return sb.ToString();
		}


		public static FST Create(string expression)
		{
			Parser p = new Parser();
			FST result = p.Parse(expression);
			return result;
		}

		public static FST Create(byte[] data)
		{
			FST fst = new FST();
			fst.FromBinary(data);
			return fst;
		}

		public FST()
		{
			_States = new Dictionary<int, State>();
			_StartState = -1;
		}

		internal State GetState(int s)
		{
			State r;
			if (!_States.TryGetValue(s, out r))
				return null;
			return r;
		}

		internal IList<int> GetStates()
		{
			return _States.Select(kvp => kvp.Key).ToList();
		}

		public int GetStateCount()
		{
			return _States.Count;
		}

		public IList<FSTTransition> GetTransitions(int state)
		{
			if (!StateExists(state))
				throw new Exception("State doesn't exist");
			return _States[state].Transitions;
		}

		public int AddState()
		{
			State s = new State();
			s.Id = _MaxState;
			_MaxState++;
			_States.Add(s.Id, s);
			return s.Id;
		}

		public bool StateExists(int s)
		{
			return _States.ContainsKey(s);
		}

		public List<int> GetFirstSet(bool forOutput)
		{
			List<int> result = new List<int>();

			ComputeStateClosure(_StartState, true,
				delegate(FSTTransition t)
				{
					Label l = forOutput ? t.Output : t.Input;
					bool r = l.IsConsuming;

					if (r)
					{
						int idx = result.BinarySearch(l.Symbol);
						if (idx < 0)
							result.Insert(~idx, l.Symbol);
					}
					// compute closure only for non-consuming symbols
					return !r;
				});

			return result;
		}

		public bool RemoveState(int s)
		{
			if (!StateExists(s))
				throw new Exception("State doesn't exist");
			if (s == _StartState)
				_StartState = -1;
			RemoveTransitionsInto(s);
			return _States.Remove(s);
		}

		public void Concatenate(FST rhs)
		{
			List<int> previouslyFinalStates = GetFinalStates();
			if (previouslyFinalStates.Count == 0)
				throw new Exception("Automaton does not have any final states");

			if (!StateExists(_StartState))
				throw new Exception("Automaton does not have a start state");

			if (!rhs.StateExists(rhs._StartState))
				throw new Exception("RHS Automaton does not have a start state");

			bool lhsStartWasFinal = IsFinal(_StartState);
			bool rhsStartWasFinal = rhs.IsFinal(rhs._StartState);

			// the mapping from the state IDs of the RHS automaton. Key is the 
			//  original RHS state ID, Value is the new state ID in the source automaton
			Dictionary<int, int> stateMapping = new Dictionary<int, int>();

			// all previously final states become non-final unless the RHS start state is final
			if (!rhsStartWasFinal)
			{
				foreach (int pf in previouslyFinalStates)
					SetFinal(pf, false);
			}

			// copy over the states
			foreach (KeyValuePair<int, State> rhsState in rhs._States)
			{
				int newState = AddState();
				stateMapping.Add(rhsState.Value.Id, newState);
				// final states in the RHS automaton remain final
				if (rhsState.Value.IsFinal)
					SetFinal(newState, true);
			}

			// now that all states exist, copy over the RHS transitions
			foreach (KeyValuePair<int, State> rhsState in rhs._States)
			{
				foreach (FSTTransition t in rhsState.Value.Transitions)
				{
					AddTransition(stateMapping[t.Source], stateMapping[t.Target],
						new Label(t.Input), new Label(t.Output));
				}
			}

			// next, link the two automatons. 

			// first, each transition which ends in a previously final state is 
			//  copied into a transition to the previous start state of RHS
			int newRhsStart = stateMapping[rhs._StartState];
			foreach (KeyValuePair<int, State> lhsState in _States)
			{
				State testState = lhsState.Value;

				List<FSTTransition> transitions = testState.Transitions;
				// Take care with this iteration as the loop adds to the state's transitions
				for (int transition = transitions.Count - 1; transition >= 0; --transition)
				{
					FSTTransition link = transitions[transition];
					if (previouslyFinalStates.Contains(link.Target))
					{
						testState.AddTransition(newRhsStart, new Label(link.Input), new Label(link.Output));
					}
				}
			}

			if (lhsStartWasFinal)
			{
				// need to introduce an eps transition to the rhs start state
				AddTransition(_StartState, newRhsStart,
					new Label(Label.SpecialSymbolEpsilon),
					new Label(Label.SpecialSymbolEpsilon));
			}

			Clean();
		}

		/// <summary>
		/// Builds the disjunction of the current automaton with the rhs ones.
		/// </summary>
		/// <param name="alternatives">The other alternatives</param>
		public void Disjunct(IList<FST> alternatives)
		{
			if (alternatives == null || alternatives.Count == 0)
				return;

			// TODO check other preconditions (defined start state, # finals, etc)

			// enforce single new start state

            //Dump("d:/temp/disj-input.txt");
            //    for (int a = 0; a < alternatives.Count; ++a)
            //        alternatives[a].Dump(String.Format("d:/temp/disj-alt-{0}.txt", a));
			
			int oldStart = _StartState;
			int newStart = AddState();
			SetInitial(newStart);

			AddTransition(newStart, oldStart, new Label(Label.SpecialSymbolEpsilon), new Label(Label.SpecialSymbolEpsilon));

			for (int alt = 0; alt < alternatives.Count; ++alt)
			{
				// copy over states

				Dictionary<int, int> stateMapping = new Dictionary<int, int>();
				int rhsStart = alternatives[alt]._StartState;

				foreach (KeyValuePair<int, State> rhsState in alternatives[alt]._States)
				{
					int newState = AddState();
					System.Diagnostics.Debug.Assert(rhsState.Key == rhsState.Value.Id);

					stateMapping.Add(rhsState.Key, newState);

					// final states in the RHS automaton remain final
					if (rhsState.Value.IsFinal)
						SetFinal(newState, true);
					// (initial states don't)

					if (rhsState.Key == rhsStart)
						AddTransition(newStart, newState, new Label(Label.SpecialSymbolEpsilon), new Label(Label.SpecialSymbolEpsilon));
				}

				// now that all states exist, copy over the RHS transitions
				foreach (KeyValuePair<int, State> rhsState in alternatives[alt]._States)
				{
					foreach (FSTTransition t in rhsState.Value.Transitions)
					{
						AddTransition(stateMapping[t.Source], stateMapping[t.Target],
							new Label(t.Input), new Label(t.Output));
					}
				}
			}

			MergeSimpleFinalStates();

			EliminateEpsilonTransitions();
			Clean();

			if (false)
			{
				Dump("d:/temp/disj-output.txt");
			}
		}

		/// <summary>
		/// Reachable states are states which can be reached by the transitive closure of any 
		/// transition, starting at the start state.
		/// </summary>
		/// <returns></returns>
		public List<int> ComputeReachableStates()
		{
			if (!StateExists(_StartState))
				// if the FST has no start state, no state can be reached
				return new List<int>();

			// any transition is good for the "is reachable" property
			List<int> result = ComputeStateClosure(_StartState, true,
				delegate(FSTTransition t) { return true; });

			return result;
		}

		/// <summary>
		/// true iff any transition from any start-reachable state points "back" (i.e. there
		/// are loops). This includes state-local loops (a transition from state x to itself).
		/// </summary>
		public bool IsCyclic()
		{
			List<int> visited = new List<int>();
			List<int> statesToCheck = new List<int>();

			statesToCheck.Add(_StartState);

			while (statesToCheck.Count > 0)
			{
				int p;

				int s = statesToCheck[0];
				statesToCheck.RemoveAt(0);

				p = visited.BinarySearch(s);
				System.Diagnostics.Debug.Assert(p < 0);
				visited.Insert(~p, s);

				foreach (FSTTransition t in _States[s].Transitions)
				{
					int targetState = t.Target;
					p = visited.BinarySearch(targetState);

					if (p >= 0)
					{
						// we already visited the target state of the transition - the
						//  automaton is cyclic

						return true;
					}

					// enqueue the target state for later checking
					p = statesToCheck.BinarySearch(targetState);
					if (p < 0)
						statesToCheck.Insert(~p, targetState);
				}
			}

			return false;
		}

		/// <summary>
		/// Merge all final states with only incoming transitions into one state
		/// </summary>
		private void MergeSimpleFinalStates()
		{
			List<int> finalStates = GetFinalStates();
			if (finalStates == null || finalStates.Count < 2)
				return;

			List<int> victims = new List<int>();
			int mergedState = -1;
			bool isInitial = false;

			foreach (int f in finalStates)
			{
				State finalState = _States[f];
				if (_States[f].Transitions.Count == 0)
				{
					if (f == _StartState)
						isInitial = true;

					if (mergedState < 0)
						mergedState = f;
					else
						victims.Add(f);
				}
			}

			if (victims.Count == 0)
				return;

			victims.Sort();

			if (isInitial)
			{
				System.Diagnostics.Debug.Assert(false, "Check!");
				SetInitial(mergedState);
			}

			// iterate through all transitions of all states and if it ends in one of the victim
			//  states, change it to end in the winning state
			foreach (KeyValuePair<int, State> kvp in _States)
			{
				foreach (FSTTransition t in kvp.Value.Transitions)
				{
					if (victims.BinarySearch(t.Target) >= 0)
						t.Target = mergedState;
				}
			}

			// no more transitions should point into any victim state, so unless it's a start state, 
			//  it can be safely deleted.
			foreach (int s in victims)
				_States.Remove(s);
		}

		/// <summary>
		/// Eliminate "true" epsilon transitions (where both input and output are eps). Note that 
		/// partial eps transitions (either input or output is eps) will remain.
		/// </summary>
		public int EliminateEpsilonTransitions()
		{
			// TODO this is a very naive implementation which is slow

			int result = 0;

			Dictionary<int, List<int>> epsClosure = new Dictionary<int, List<int>>();

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				if (HasEpsilonTransitions(kvp.Key))
				{
					List<int> closure = ComputeStateClosure(kvp.Key, false,
						delegate(FSTTransition t) { return t.IsEpsilon; });
					epsClosure.Add(kvp.Key, closure);

					if (!kvp.Value.IsFinal)
					{
						// if a final state is in the eps closure, then the current state is final
						kvp.Value.IsFinal = closure.Any(c => IsFinal(c));
					}
				}
			}

			if (epsClosure.Count > 0)
			{
				// if the start state has an eps closure, it requries special attention
				if (epsClosure.ContainsKey(_StartState))
				{
					List<int> closure = epsClosure[_StartState];
					State startState = _States[_StartState];

					// we need to "jump" all eps transitions which means that we need to duplicate
					//  all non-eps transitions starting from any state in the start state's eps closure
					//  to start at the start state as well.

					foreach (int c in epsClosure[_StartState])
					{
						foreach (FSTTransition t in _States[c].Transitions)
						{
							if (!t.IsEpsilon)
								startState.AddTransition(t.Target, new Label(t.Input), new Label(t.Output));
						}
					}
				}

				// for each non-eps transition which ends in a state which has an eps closure, 
				//  copy that transition to point to each state in the closure

				foreach (KeyValuePair<int, State> kvp in _States)
				{
					List<FSTTransition> transitions = kvp.Value.Transitions;

					int currentTransitionCount = transitions.Count;

					for (int p = currentTransitionCount - 1; p >= 0; --p)
					{
						FSTTransition t = transitions[p];

						if (t.IsEpsilon)
							continue;

						List<int> closure;
						if (epsClosure.TryGetValue(t.Target, out closure))
						{
							System.Diagnostics.Debug.Assert(kvp.Key == t.Source && kvp.Value.Id == kvp.Key);

							foreach (int trg in closure)
								kvp.Value.AddTransition(trg, new Label(t.Input), new Label(t.Output));
						}
					}

					// delete pure eps transitions

					for (int p = currentTransitionCount - 1; p >= 0; --p)
					{
						if (transitions[p].IsEpsilon)
						{
							transitions.RemoveAt(p);
							++result;
						}
					}
				}

				// above elimination may leave non-reachable states
				DeleteNonreachableStates();
			}

			System.Diagnostics.Debug.Assert(!HasEpsilonTransitions());

			return result;
		}

		private void SortTransitions()
		{
			foreach (KeyValuePair<int, State> kvp in _States)
				kvp.Value.SortTransitions();
		}

		/// <summary>
		/// Attempts to make the automaton deterministic. Note that it's never fully deterministic on 
		/// a single band, since one symbol may always be eps.
		/// </summary>
		public void MakeDeterministic()
		{
			if (!StateExists(_StartState))
				throw new Exception("No start state");

			// the test is cheaper than going through the prep steps
			if (IsDeterministic())
				return;

			// TODO test other required features, such as reachability, productivity of states and 
			//  existance of start/final states

			// TODO eps-free prop should be memoized on FST level to avoid multiple runs of eps elim
			EliminateEpsilonTransitions();
			SortTransitions();

			// starting with the start state, create a new joint state for each state cluster which
			//  can be reached by multiple transitions with the same label. Copy the outgoing 
			//  transitions from each prior target state to outgoing transitions from the new cluster
			//  state.

			// schedule each state for processing, but start with the start state
			List<int> processedStates = new List<int>();
			processedStates.Add(_StartState);
			processedStates.AddRange(_States.Where(kvp => kvp.Key != _StartState).Select(s => s.Key));
			int current = 0;


			// need to remember the cluster states and which original states they represent
			Trie<int, int> clusterStates = new Trie<int, int>();

			while (current < processedStates.Count)
			{
				int currentState = processedStates[current];
				++current;

				State state = _States[currentState];

				if (state.Transitions.Count < 2)
					continue;

				bool wasModified = false;

				int firstEqualTransition = 0;
				int previousTransitionCount = state.TransitionCount;
				while (firstEqualTransition < previousTransitionCount)
				{
					int lastEqualTransition = firstEqualTransition + 1;
					// NOTE that the transitions are sorted by src state, input, output, trg state
					while (lastEqualTransition < previousTransitionCount
						&& state.Transitions[lastEqualTransition].Input.Equals(state.Transitions[firstEqualTransition].Input)
						&& state.Transitions[lastEqualTransition].Output.Equals(state.Transitions[firstEqualTransition].Output))
					{
						++lastEqualTransition;
					}

					if (lastEqualTransition > firstEqualTransition + 1)
					{

						// found a cluster of equal (i.e. non-deterministic) labels 
						//  for transitions [first..last[


						// build the cluster state label
						List<int> label = new List<int>();
						for (int p = firstEqualTransition; p < lastEqualTransition; ++p)
						{
							// usually multiple same target states are not possible, but
							//  we need to be sure:
							System.Diagnostics.Debug.Assert(!label.Contains(state.Transitions[p].Target));
							label.Add(state.Transitions[p].Target);
						}
						label.Sort();

						// search whether the cluster state alread exists:

						int clusterStateNumber = -1;
						int cluster;
						if (!clusterStates.Contains(label, out cluster))
						{
							// cluster state not found. Add new state to automaton 
							clusterStateNumber = AddState();
							State clusterState = _States[clusterStateNumber];

							// need to check the new state's transitions later
							processedStates.Add(clusterStateNumber);

							clusterStates.Add(label, clusterStateNumber);

							// it's final if any of its clustered states is final
							// NOTE we don't need to care about going back to the initial state
							//  as the only initial state in the DFA will be the original initial 
							//  state
							clusterState.IsFinal = label.Any(s => IsFinal(s));

							// copy all outgoing transitions of all the clustered states to the new state
							foreach (int cs in label)
							{
								State cState = _States[cs];
								foreach (FSTTransition t in cState.Transitions)
									clusterState.AddTransition(t.Target, new Label(t.Input), new Label(t.Output));
							}

							// for later once we determinize the new cluster state
							clusterState.SortTransitions();
						}
						else
						{
							clusterStateNumber = cluster;
						}

						// now create a new single transition with the ambiguous label and 
						//  let it point to the new cluster state
						state.AddTransition(clusterStateNumber, new Label(state.Transitions[firstEqualTransition].Input),
							new Label(state.Transitions[firstEqualTransition].Output));

						// and invalidate all prior transitions (set to NULL, we cleanup later)
						for (int p = firstEqualTransition; p < lastEqualTransition; ++p)
						{
							state.Transitions[p] = null;
						}

						wasModified = true;
					}

					firstEqualTransition = lastEqualTransition;
				}

				if (wasModified)
				{
					// transition list will contain NULL elements which we have to remove
					state.Transitions.RemoveAll(t => t == null);
				}
			}

			// the above may leave unproductive and/or unreachable states:
			Clean();
			System.Diagnostics.Debug.Assert(IsDeterministic());
		}

		public bool IsDeterministic()
		{
			return _States.All(kvp => kvp.Value.IsDeterministic());
		}

		public bool HasEpsilonTransitions()
		{
			return _States.Any(kvp => kvp.Value.HasEpsilonTransitions());
		}

		public bool HasEpsilonTransitions(int state)
		{
			State s;
			if (!_States.TryGetValue(state, out s))
				throw new Exception("State doesn't exist");
			return s.HasEpsilonTransitions();
		}

		/// <summary>
		/// Productive states are states from which, by following any sequence of transitions, a 
		/// final state can be reached. Note that productive states are not necessarily reachable.
		/// </summary>
		/// <returns>The list of productive states of the automaton</returns>
		public List<int> ComputeProductiveStates()
		{
			List<int> result = GetFinalStates();

			bool newProductiveStateFound;

			do
			{
				newProductiveStateFound = false;

				foreach (KeyValuePair<int, State> kvp in _States)
				{
					int p = result.BinarySearch(kvp.Key);
					if (p >= 0)
						continue;

					foreach (FSTTransition t in kvp.Value.Transitions)
					{
						if (result.BinarySearch(t.Target) >= 0)
						{
							// t ends in a productive state so kvp.Key is productive
							result.Insert(~p, kvp.Key);
							newProductiveStateFound = true;
							break;
						}
					}
				}
			} while (newProductiveStateFound);

			return result;
		}

		/// <summary>
		/// Deletes unproductive and then non-reachable states.
		/// </summary>
		public void Clean()
		{
			// NOTE order of these two calls matters
			DeleteUnproductiveStates();
			DeleteNonreachableStates();
		}

		public void DeleteUnproductiveStates()
		{
			List<int> productiveStates = ComputeProductiveStates();
			if (productiveStates.Count != _States.Count)
			{
				List<int> zombies = new List<int>();

				foreach (KeyValuePair<int, State> kvp in _States)
				{
					// need to copy the states to be deleted first, otherwise iteration will fail
					if (productiveStates.BinarySearch(kvp.Key) < 0)
						zombies.Add(kvp.Key);
				}

				System.Diagnostics.Debug.Assert(zombies.Count == _States.Count - productiveStates.Count);

				foreach (int z in zombies)
					RemoveState(z);
			}
		}

		public void DeleteNonreachableStates()
		{
			List<int> reachableStates = ComputeReachableStates();
			if (reachableStates.Count != _States.Count)
			{
				List<int> zombies = new List<int>();

				foreach (KeyValuePair<int, State> kvp in _States)
				{
					// need to copy the states to be deleted first, otherwise iteration will fail
					if (reachableStates.BinarySearch(kvp.Key) < 0)
						zombies.Add(kvp.Key);
				}

				System.Diagnostics.Debug.Assert(zombies.Count == _States.Count - reachableStates.Count);

				foreach (int z in zombies)
					RemoveState(z);
			}
		}

		public void SetStartState(int s)
		{
			if (s == _StartState)
				return;

			State oldStartState = null;
			if (_StartState >= 0)
			{
				oldStartState = GetState(_StartState);
				if (oldStartState == null)
					throw new Exception("State doesn't exist");
			}

			State newStartState = GetState(s);
			if (newStartState == null)
				throw new Exception("State doesn't exist");

			if (oldStartState != null)
				oldStartState.IsInitial = false;

			newStartState.IsInitial = true;

			_StartState = s;
		}

		public int GetStartState()
		{
			State state = GetState(_StartState);
			if (state == null)
				return -1;
			System.Diagnostics.Debug.Assert(state.IsInitial);
			return _StartState;
		}

		public bool IsInitial(int s)
		{
			State state;
			if (!_States.TryGetValue(s, out state))
				throw new Exception("State doesn't exist");
			return state.IsInitial;
		}

		public bool IsFinal(int s)
		{
			State state;
			if (!_States.TryGetValue(s, out state))
				throw new Exception("State doesn't exist");
			return state.IsFinal;
		}

		public void SetInitial(int s)
		{
			SetStartState(s);
		}

		public void SetFinal(int s, bool flag)
		{
			State state;
			if (!_States.TryGetValue(s, out state))
				throw new Exception("State doesn't exist");
			state.IsFinal = flag;
		}

		public List<int> GetFinalStates()
		{
			List<int> result = new List<int>();

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				System.Diagnostics.Debug.Assert(kvp.Key == kvp.Value.Id);
				if (kvp.Value.IsFinal)
					result.Add(kvp.Value.Id);
			}

			result.Sort();

			return result;
		}

		public void RemoveTransitionsInto(int s)
		{
			if (!StateExists(s))
				throw new Exception("State doesn't exist");

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				State state = kvp.Value;
				for (int t = state.Transitions.Count - 1; t >= 0; --t)
				{
					if (state.Transitions[t].Target == s)
						state.RemoveTransitionAt(t);
				}
			}
		}

		public bool AddTransition(int start, int target, Label input, Label output)
		{
			State startState;
			State targetState;

			if (!_States.TryGetValue(start, out startState))
				throw new Exception("State doesn't exist");

			if (!_States.TryGetValue(target, out targetState))
				throw new Exception("State doesn't exist");

			if (startState.HasTransition(target, input, output))
				return false;

			startState.AddTransition(target, input, output);
			return true;
		}

		public int IncomingTransitionCount(int s)
		{
			if (!StateExists(s))
				throw new Exception("State doesn't exist");
			return GetIncomingTransitions(s).Count;
		}

		public List<FSTTransition> GetIncomingTransitions(int targetState)
		{
			List<FSTTransition> result = new List<FSTTransition>();
			foreach (KeyValuePair<int, State> kvp in _States)
			{
				State state = kvp.Value;
				for (int t = 0; t < state.Transitions.Count; ++t)
				{
					FSTTransition trans = state.Transitions[t];
					if (trans.Target == targetState)
						result.Add(trans);
				}
			}
			return result;
		}

		public bool IsConsistent()
		{
			/*
			 * The following conditions are checked:
			 *   - the automaton has at least one state
			 *   - all transition target states are "contained" in the automaton
			 * returns Whether the automaton is consistent
			 */

			int errors = 0;

			if (_States.Count == 0)
				++errors;

			int startStates = 0;
			int finalStates = 0;

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				State state = kvp.Value;

				if (state.IsInitial)
					++startStates;

				if (state.IsFinal)
					++finalStates;

				for (int t = 0; t < state.Transitions.Count; ++t)
				{
					FSTTransition trans = state.Transitions[t];
					if (!StateExists(trans.Target))
						++errors;
				}
			}

			if (startStates != 1)
				++errors;

			// check whether we have at least one final state
			if (finalStates == 0)
				++errors;

			return errors == 0;
		}

		/// <summary>
		/// Compute the transitive closure (i.e. the list of states) which can be reached from the 
		/// start state by following transitions with the given property. 
		/// </summary>
		/// <param name="startState">The state at which to start the computation.</param>
		/// <param name="includeStartState">Whether or not the start state is included in the result.</param>
		/// <param name="prop">The property a transition must fulfill to be followed further.</param>
		/// <returns>A list of state IDs</returns>
		private List<int> ComputeStateClosure(int startState,
			bool includeStartState,
			TransitionProperty prop)
		{
			List<int> result = new List<int>();
			List<int> statesToCheck = new List<int>();

			statesToCheck.Add(startState);

			while (statesToCheck.Count > 0)
			{
				int p;

				int s = statesToCheck[0];
				statesToCheck.RemoveAt(0);

				p = result.BinarySearch(s);
				System.Diagnostics.Debug.Assert(p < 0);
				if (s != _StartState || includeStartState)
					result.Insert(~p, s);

				foreach (FSTTransition t in _States[s].Transitions)
				{
					if (prop(t))
					{
						int targetState = t.Target;

						// targetState is reachable through a transition which fulfills the property - add 
						//   to list of states to visit
						if (result.BinarySearch(targetState) < 0 && ((p = statesToCheck.BinarySearch(targetState)) < 0))
							statesToCheck.Insert(~p, targetState);
					}
				}
			}

			return result;
		}

		public void Dump(string fileName)
		{
			using (System.IO.StreamWriter wtr = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.UTF8))
			{
				Dump(wtr);
			}
		}

		// magic number - date of creation plus two digits for version
		private static readonly int _Magic = 2007110601;

		private void FromBinary(byte[] data)
		{
			_States = new Dictionary<int, State>();
			_StartState = -1;
			_MaxState = -1;

			using (System.IO.MemoryStream dataStream = new System.IO.MemoryStream(data))
			{
				using (System.IO.Compression.GZipStream zipper = new System.IO.Compression.GZipStream(dataStream, System.IO.Compression.CompressionMode.Decompress))
				{
					using (System.IO.BinaryReader rdr = new System.IO.BinaryReader(zipper))
					{
						int val = rdr.ReadInt32();
						if (val != _Magic)
							throw new Exception("Inconsistent FST data - version number mismatch");

						int nrStates = rdr.ReadInt32();
						if (nrStates < 0)
							throw new Exception("Inconsistent FST data - invalid state count");

						_StartState = rdr.ReadInt32();
						_MaxState = rdr.ReadInt32();

						for (int i = 0; i < nrStates; ++i)
						{
							State s = new State();
							s.Id = rdr.ReadInt32();
							_States.Add(s.Id, s);
							if (s.Id == _StartState)
								s.IsInitial = true;
						}

						int finalStateCount = rdr.ReadInt32();
						if (finalStateCount < 0)
							throw new Exception("Inconsistent FST data - invalid number of final states");

						for (int i = 0; i < finalStateCount; ++i)
						{
							int f = rdr.ReadInt32();
							SetFinal(f, true);
						}

						int totalTransitionCount = rdr.ReadInt32();

						// read the transitions until EOF
						while (totalTransitionCount > 0)
						{
							int start = rdr.ReadInt32();
							int target = rdr.ReadInt32();
							int input = rdr.ReadInt32();
							int output = rdr.ReadInt32();

							AddTransition(start, target, new Label(input), new Label(output));

							--totalTransitionCount;
						}
					}
				}
			}
		}

		/// <summary>
		/// Get a binary representation of the automaton, useful for storing it in a file.
		/// </summary>
		/// <returns>A byte array which represents a compiled, binary form of the FST</returns>
		public byte[] GetBinary()
		{
			using (System.IO.MemoryStream data = new System.IO.MemoryStream(4096))
			{
				using (System.IO.Compression.GZipStream zipper = new System.IO.Compression.GZipStream(data, System.IO.Compression.CompressionMode.Compress))
				{
					using (System.IO.BinaryWriter wtr = new System.IO.BinaryWriter(zipper))
					{
						wtr.Write(_Magic);

						wtr.Write(_States.Count);
						wtr.Write(_StartState);
						wtr.Write(_MaxState);

						int totalTransitionCount = 0;

						foreach (KeyValuePair<int, State> kvp in _States)
						{
							System.Diagnostics.Debug.Assert(kvp.Key == kvp.Value.Id);
							wtr.Write(kvp.Key);
							totalTransitionCount += kvp.Value.TransitionCount;
						}

						List<int> finalStates = GetFinalStates();
						wtr.Write(finalStates.Count);
						foreach (int f in finalStates)
							wtr.Write(f);

						wtr.Write(totalTransitionCount);

						foreach (KeyValuePair<int, State> kvp in _States)
						{
							foreach (FSTTransition t in kvp.Value.Transitions)
							{
								wtr.Write(t.Source);
								wtr.Write(t.Target);
								wtr.Write(t.Input.Symbol);
								wtr.Write(t.Output.Symbol);
							}
						}
					}
				}

				return data.ToArray();
			}
		}

		/// <summary>
		/// Detects whether the two FSTs are identical. Identity is stronger than equality in that
		/// the state numbering and all other information must be the same.
		/// </summary>
		/// <param name="other">The FST to compare to.</param>
		/// <returns>true if both FSTs are identical, false otherwise</returns>
		public bool IsIdentical(FST other)
		{
			if (other == null)
				throw new ArgumentNullException();

			if (_MaxState != other._MaxState
				|| _StartState != other._StartState
				|| _States.Count != other._States.Count)
				return false;

			SortTransitions();
			other.SortTransitions();

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				State otherState;
				if (!other._States.TryGetValue(kvp.Key, out otherState))
					return false;

				if (kvp.Value.IsFinal != otherState.IsFinal
					|| kvp.Value.IsInitial != otherState.IsInitial
					|| kvp.Value.TransitionCount != otherState.TransitionCount)
					return false;

				for (int t = 0; t < kvp.Value.TransitionCount; ++t)
				{
					FSTTransition tt = kvp.Value.Transitions[t];
					FSTTransition ot = otherState.Transitions[t];
					if (tt.Source != ot.Source
						|| tt.Target != ot.Target
						|| tt.Input.Symbol != ot.Input.Symbol
						|| tt.Output.Symbol != ot.Output.Symbol)
						return false;
				}
			}

			return true;
		}

		public void Dump(System.IO.TextWriter wtr)
		{
			wtr.WriteLine("================================================");
			wtr.WriteLine();
			wtr.WriteLine("{0} States, start state is {1}", _States.Count, _StartState);
			wtr.WriteLine();

			foreach (KeyValuePair<int, State> kvp in _States)
			{
				State s = kvp.Value;

				wtr.WriteLine("State {0} ({1}{2} - {3} transitions)",
					s.Id,
					s.IsInitial ? " start " : String.Empty,
					s.IsFinal ? " final " : String.Empty,
					s.Transitions == null ? 0 : s.TransitionCount);

				if (s.Transitions != null)
				{
					foreach (FSTTransition t in s.Transitions)
					{
						wtr.WriteLine("\t{0,-5} -> {1,-5} on ({2}:{3})",
							t.Source, t.Target, t.Input, t.Output);
					}
				}
			}

			wtr.WriteLine("================================================");
		}

	}
}
