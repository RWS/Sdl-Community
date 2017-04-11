using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexTrie
{
    class CharacterTransitionComparer : IEqualityComparer<KeyValuePair<char, byte>>
    {
        public bool Equals(KeyValuePair<char, byte> x, KeyValuePair<char, byte> y)
        {
            return x.Key == y.Key && x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair<char, byte> trans)
        {
            return (trans.Key.GetHashCode()) ^ (trans.Value.GetHashCode());
        }
    }

    class RangeTransitionComparer<T> : IEqualityComparer<RangeTransition<T>>
    {
        public bool Equals(RangeTransition<T> x, RangeTransition<T> y)
        {
            return x.groupNumber == y.groupNumber &&
                x.rangeEnd == y.rangeEnd &&
                x.rangeStart == y.rangeStart;
        }

        public int GetHashCode(RangeTransition<T> trans)
        {
            return (trans.groupNumber.GetHashCode()) ^
                (trans.rangeEnd.GetHashCode()) ^
                (trans.rangeStart.GetHashCode());
        }
    }

    class ComplementTransitionComparer<T> : IEqualityComparer<ComplementTransition<T>>
    {
        public bool Equals(ComplementTransition<T> x, ComplementTransition<T> y)
        {
            foreach (char c in x.characters)
            {
                if (!y.characters.Contains(c))
                {
                    return false;
                }
            }

            foreach (KeyValuePair<char, char> kv in x.ranges)
            {
                Boolean identicalRangeFound = false;
                foreach (KeyValuePair<char, char> kv2 in y.ranges)
                {
                    if ((kv.Key == kv2.Key) && (kv.Value == kv2.Value))
                    {
                        identicalRangeFound = true;
                    }
                }
                if (!identicalRangeFound)
                {
                    return false;
                }
            }

            return x.groupNumber == y.groupNumber;
        }

        public int GetHashCode(ComplementTransition<T> trans)
        {
            int hashCode = 0;
            foreach (char c in trans.characters)
            {
                hashCode = c.GetHashCode() ^ hashCode;
            }
            foreach (KeyValuePair<char, char> kv in trans.ranges)
            {
                hashCode = kv.Value.GetHashCode() ^ kv.Key.GetHashCode() ^ hashCode;
            }

            return trans.groupNumber.GetHashCode() ^ hashCode;
        }
    }

    class StateComparer<T> : IEqualityComparer<List<RegexTrie<T>>>
    {
        private int hashCompiler(List<RegexTrie<T>> state)
        {
            return state.Aggregate(new int(), (hash, next) => hash ^ next.GetHashCode());
        }

        public bool Equals(List<RegexTrie<T>> x, List<RegexTrie<T>> y)
        {
            //If the states have a different amount of nodes, they cannot be equal
            if (x.Count != y.Count)
            {
                return false;
            }

            //Turn the other list into hashset to speed up the processing
            HashSet<RegexTrie<T>> hashedY = new HashSet<RegexTrie<T>>(y);
            //Check for existence of every x node in y
            foreach (var node in x)
            {
                //If some node is not found, the states are not equal
                if (!hashedY.Contains(node))
                {
                    return false;
                }
            }

            //If this point is reached, the states must be equal (they are of the same size,
            //and all nodes that exist in x also exist in y)
            return true;
        }

        public int GetHashCode(List<RegexTrie<T>> state)
        {
            return hashCompiler(state);
        }
    }

    //Return value for the getClosureTransitions function. Contains all non-epsilon transitions from a list
    //of NFA states
    class ClosureTransitions<T,T2>
    {
        public Dictionary<KeyValuePair<char, byte>, T2> normalTransitions;
        public Dictionary<RangeTransition<T>, T2> rangeTransitions;
        public Dictionary<ComplementTransition<T>, T2> complementTransitions;

        public ClosureTransitions()
        {
            this.normalTransitions = new Dictionary<KeyValuePair<char, byte>, T2>();
            this.rangeTransitions = new Dictionary<RangeTransition<T>, T2>();
            this.complementTransitions = new Dictionary<ComplementTransition<T>, T2>();
        }

        public ClosureTransitions(
            Dictionary<KeyValuePair<char, byte>, T2> normalTrans,
            Dictionary<RangeTransition<T>, T2> rangeTrans,
            Dictionary<ComplementTransition<T>, T2> compTrans)
        {
            this.normalTransitions = normalTrans;
            this.rangeTransitions = rangeTrans;
            this.complementTransitions = compTrans;
        }
    }

    public class Determiniser<T>
    {
        public Determiniser()
        {
        }

        //Get's the epsilon closure of a state. Epsilon closure needs to be calculated
        //for multiple nodes during determinisation, so the argument is a list
        public List<RegexTrie<T>> getEpsilonClosure(List<RegexTrie<T>> epsilonClosure)
        {
            //Make a stack out of the input list
            Stack<RegexTrie<T>> NFAStates = new Stack<RegexTrie<T>>(epsilonClosure);
            RegexTrie<T> topState;
            while (true)
            {
                //Get the top state from the stack
                if (NFAStates.Count == 0)
                {
                    break;
                }
                else
                {
                    topState = NFAStates.Pop();
                }

                //Go through each epsilon transition in the node and add the
                //nodes reached from them to the stack and the epsilon closure 
                //(unless they are already in the epsilon closure)
                foreach (RegexTrie<T> eTrans in topState.GetEpsilonTransitions())
                {
                    if (!epsilonClosure.Contains(eTrans))
                    {
                        epsilonClosure.Add(eTrans);
                        NFAStates.Push(eTrans);
                    }
                }
            }
            return epsilonClosure;
        }

        //Collects character transitions from an epsilon closure
        private void collectCharacterTransitions(
            Dictionary<KeyValuePair<char, byte>,
            List<RegexTrie<T>>> epsilonClosureTransitions, Dictionary<char, List<Transition<T>>> stateTransitions)
        {
            foreach (char key in stateTransitions.Keys)
            {
                //Iterate over the tuples of trie and group number
                foreach (Transition<T> trans in stateTransitions[key])
                {
                    var newKey = new KeyValuePair<char, byte>(key, trans.groupNumber);
                    if (epsilonClosureTransitions.ContainsKey(newKey))
                    {
                        epsilonClosureTransitions[newKey].Add(trans.destination);
                    }
                    else
                    {
                        epsilonClosureTransitions.Add(newKey, new List<RegexTrie<T>>());
                        epsilonClosureTransitions[newKey].Add(trans.destination);
                    }
                }
            }
        }

        private void collectRangeTransitions(
            Dictionary<RangeTransition<T>, List<RegexTrie<T>>> rangeTransitions,
            List<RangeTransition<T>> stateRangeTransitions)
        {
            foreach (RangeTransition<T> rangeTrans in stateRangeTransitions)
            {
                if (rangeTransitions.ContainsKey(rangeTrans))
                {
                    rangeTransitions[rangeTrans].Add(rangeTrans.destination);
                }
                else
                {
                    rangeTransitions.Add(rangeTrans, new List<RegexTrie<T>>());
                    rangeTransitions[rangeTrans].Add(rangeTrans.destination);
                }
            }
        }

        private void collectComplementTransitions(
            Dictionary<ComplementTransition<T>, List<RegexTrie<T>>> complementTransitions,
            List<ComplementTransition<T>> stateComplementTransitions)
        {
            foreach (ComplementTransition<T> compTrans in stateComplementTransitions)
            {
                if (complementTransitions.ContainsKey(compTrans))
                {
                    complementTransitions[compTrans].Add(compTrans.destination);
                }
                else
                {
                    complementTransitions.Add(compTrans, new List<RegexTrie<T>>());
                    complementTransitions[compTrans].Add(compTrans.destination);
                }
            }
        }



        //Collects all non-epsilon transitions from a list of regexes that make up the epsilon closure
        //For the purposes of determinisation, tuple of char and group is used as key
        private ClosureTransitions<T, List<RegexTrie<T>>> getClosureTransitions(List<RegexTrie<T>> epsilonClosure)
        {
            //Character transition dictionaries use reference types as keys but identity depends on constituent values,
            //so we need a custom comparer
            CharacterTransitionComparer charTransComparer = new CharacterTransitionComparer();

            //Range transitions also need a custom comparer
            RangeTransitionComparer<T> rangeTransComparer = new RangeTransitionComparer<T>();

            //As do complement transitions
            ComplementTransitionComparer<T> compTransComparer = new ComplementTransitionComparer<T>();

            ClosureTransitions<T, List<RegexTrie<T>>> results = new ClosureTransitions<T, List<RegexTrie<T>>>();

            Dictionary<KeyValuePair<char, byte>, List<RegexTrie<T>>> normalTransitions =
                new Dictionary<KeyValuePair<char, byte>, List<RegexTrie<T>>>(charTransComparer);
            Dictionary<RangeTransition<T>, List<RegexTrie<T>>> rangeTransitions =
                new Dictionary<RangeTransition<T>, List<RegexTrie<T>>>(rangeTransComparer);
            Dictionary<ComplementTransition<T>, List<RegexTrie<T>>> complementTransitions =
                new Dictionary<ComplementTransition<T>, List<RegexTrie<T>>>(compTransComparer);

            foreach (RegexTrie<T> state in epsilonClosure)
            {
                collectCharacterTransitions(normalTransitions, state.characterTransitions);
                collectRangeTransitions(rangeTransitions, state.rangeTransitions);
                collectComplementTransitions(complementTransitions, state.complementTransitions);
            }
            return new ClosureTransitions<T, List<RegexTrie<T>>>(normalTransitions, rangeTransitions, complementTransitions);
        }

        public void addCharacterTransitionsToTransitionTable(
            Dictionary<KeyValuePair<char, byte>, List<RegexTrie<T>>> nfaTransitions,
            Dictionary<KeyValuePair<char, byte>, int> dfaTransitions,
            Dictionary<List<RegexTrie<T>>, int> stateExists,
            List<List<RegexTrie<T>>> unmarkedDStates)
        {
            List<RegexTrie<T>> epsilonClosure;
            int targetState;
            foreach (KeyValuePair<char, byte> key in nfaTransitions.Keys)
            {
                epsilonClosure = getEpsilonClosure(nfaTransitions[key]);
                //Check if the transition leads to an existing state
                if (stateExists.ContainsKey(epsilonClosure))
                {
                    targetState = stateExists[epsilonClosure];
                }
                else
                {
                    unmarkedDStates.Add(epsilonClosure);
                    targetState = unmarkedDStates.Count - 1;
                    //Map the epsilon closure to counter
                    stateExists.Add(epsilonClosure, targetState);
                }

                //Add the transition to the DTrans table
                dfaTransitions.Add(key, targetState);
            }
        }

        public void addRangeTransitionsToTransitionTable(
            Dictionary<RangeTransition<T>, List<RegexTrie<T>>> nfaTransitions,
            Dictionary<RangeTransition<T>, int> dfaTransitions,
            Dictionary<List<RegexTrie<T>>, int> stateExists,
            List<List<RegexTrie<T>>> unmarkedDStates)
        {
            List<RegexTrie<T>> epsilonClosure;
            int targetState;
            foreach (RangeTransition<T> key in nfaTransitions.Keys)
            {
                epsilonClosure = getEpsilonClosure(nfaTransitions[key]);
                //Check if the transition leads to an existing state
                if (stateExists.ContainsKey(epsilonClosure))
                {
                    targetState = stateExists[epsilonClosure];
                }
                else
                {
                    unmarkedDStates.Add(epsilonClosure);
                    targetState = unmarkedDStates.Count - 1;
                    //Map the epsilon closure to counter
                    stateExists.Add(epsilonClosure, targetState);
                }

                //Add the transition to the DTrans table
                dfaTransitions.Add(key, targetState);
            }
        }

        public void addComplementTransitionsToTransitionTable(
            Dictionary<ComplementTransition<T>, List<RegexTrie<T>>> nfaTransitions,
            Dictionary<ComplementTransition<T>, int> dfaTransitions,
            Dictionary<List<RegexTrie<T>>, int> stateExists,
            List<List<RegexTrie<T>>> unmarkedDStates)
        {
            List<RegexTrie<T>> epsilonClosure;
            int targetState;
            foreach (ComplementTransition<T> key in nfaTransitions.Keys)
            {
                epsilonClosure = getEpsilonClosure(nfaTransitions[key]);
                //Check if the transition leads to an existing state
                if (stateExists.ContainsKey(epsilonClosure))
                {
                    targetState = stateExists[epsilonClosure];
                }
                else
                {
                    unmarkedDStates.Add(epsilonClosure);
                    targetState = unmarkedDStates.Count - 1;
                    //Map the epsilon closure to counter
                    stateExists.Add(epsilonClosure, targetState);
                }

                //Add the transition to the DTrans table
                dfaTransitions.Add(key, targetState);
            }
        }

        public void addCharacterTransitionsToDFA(
            Dictionary<KeyValuePair<char, byte>, int> dfaTransitions,
            Dictionary<int, RegexTrie<T>> allDTries,
            RegexTrie<T> origin)
        {
            foreach (var trans in dfaTransitions)
            {
                if (!allDTries.ContainsKey(trans.Value))
                {
                    allDTries[trans.Value] = new RegexTrie<T>();
                }
                RegexTrie<T> target = allDTries[trans.Value];

                origin.AddCharacterTransition(trans.Key.Key, trans.Key.Value, target);
            }
        }

        public void addRangeTransitionsToDFA(
            Dictionary<RangeTransition<T>, int> dfaTransitions,
            Dictionary<int, RegexTrie<T>> allDTries,
            RegexTrie<T> origin)
        {
            foreach (var trans in dfaTransitions)
            {
                if (!allDTries.ContainsKey(trans.Value))
                {
                    allDTries[trans.Value] = new RegexTrie<T>();
                }
                RegexTrie<T> target = allDTries[trans.Value];

                origin.AddRangeTransition(
                    target, trans.Key.rangeStart, trans.Key.rangeEnd, trans.Key.groupNumber);
            }
        }

        public void addComplementTransitionsToDFA(
            Dictionary<ComplementTransition<T>, int> dfaTransitions,
            Dictionary<int, RegexTrie<T>> allDTries,
            RegexTrie<T> origin)
        {
            foreach (var trans in dfaTransitions)
            {
                if (!allDTries.ContainsKey(trans.Value))
                {
                    allDTries[trans.Value] = new RegexTrie<T>();
                }
                RegexTrie<T> target = allDTries[trans.Value];

                origin.complementTransitions.Add(new ComplementTransition<T>(
                    trans.Key.groupNumber, target, trans.Key.characters, trans.Key.ranges));

            }
        }

        public RegexTrie<T> determiniseNFA(RegexTrie<T> regTrie)
        {
            //If regTrie is empty, return empty trie
            if (regTrie.characterTransitions.Count == 0 && regTrie.epsilonTransitions.Count == 0)
            {
                return new RegexTrie<T>();
            }

            //Keeps track of the amount of DFA states added
            int stateCounter = 0;

            //Lists of DFA states.
            List<List<RegexTrie<T>>> unmarkedDStates =
                new List<List<RegexTrie<T>>>();
            List<List<RegexTrie<T>>> markedDStates =
                new List<List<RegexTrie<T>>>();

            //List of DFA transitions.
            List<ClosureTransitions<T, int>> dTrans = new List<ClosureTransitions<T, int>>();

            //This compares two states to see if they are equal (and provides hash codes)
            StateComparer<T> comparer = new StateComparer<T>();
            //This needs to be a dictionary so that the existence of a state can be quickly checked
            Dictionary<List<RegexTrie<T>>, int> stateExists =
                new Dictionary<List<RegexTrie<T>>, int>(comparer);
            //Get the first epsilonClosure
            List<RegexTrie<T>> epsilonClosure =
                getEpsilonClosure(new List<RegexTrie<T>>() { regTrie });
            //Add the first state to the unmarked states and all states
            unmarkedDStates.Add(epsilonClosure);

            //Map the epsilon closure to counter
            stateExists.Add(epsilonClosure, stateCounter);
            stateCounter++;
            int lowestKey = 0;

            //This will hold the transitions from an epsilon closure
            ClosureTransitions<T, List<RegexTrie<T>>> transitions;

            while (true)
            {
                //If the list does not contain lowest key, end the loop
                if (lowestKey > unmarkedDStates.Count - 1)
                {
                    break;
                }

                //Move the state from unmarked to marked and get its transitions
                //The lowest key is always the lowest key by definition, so the order will be preserved
                markedDStates.Add(unmarkedDStates[lowestKey]);
                transitions = getClosureTransitions(unmarkedDStates[lowestKey]);
                unmarkedDStates[lowestKey] = null;

                //Go through the transitions of the closure
                //Always add the dTrans entry to keep lists in sync
                dTrans.Add(new ClosureTransitions<T, int>());

                //Handle normal transitions
                addCharacterTransitionsToTransitionTable(
                    transitions.normalTransitions,
                    dTrans[lowestKey].normalTransitions,
                    stateExists,
                    unmarkedDStates);

                //Handle range transitions
                addRangeTransitionsToTransitionTable(
                    transitions.rangeTransitions,
                    dTrans[lowestKey].rangeTransitions,
                    stateExists,
                    unmarkedDStates);

                //Handle complement transitions
                addComplementTransitionsToTransitionTable(
                    transitions.complementTransitions,
                    dTrans[lowestKey].complementTransitions,
                    stateExists,
                    unmarkedDStates);

                //increment lowest key
                lowestKey++;
            }

            //Construct the new trie adding tries as necessary
            //This is a dictionary as the states are added non-consecutively
            Dictionary<int, RegexTrie<T>> allDTries = new Dictionary<int, RegexTrie<T>>(markedDStates.Count);

            for (int index = 0; index < markedDStates.Count; index++)
            {
                //Add the origin state to all tries, unless it already exists

                if (!allDTries.ContainsKey(index))
                {
                    allDTries[index] = new RegexTrie<T>();
                }

                //Add normal transitions to all tries
                addCharacterTransitionsToDFA(
                    dTrans[index].normalTransitions,
                    allDTries,
                    allDTries[index]);

                addRangeTransitionsToDFA(
                    dTrans[index].rangeTransitions,
                    allDTries,
                    allDTries[index]);

                addComplementTransitionsToDFA(
                    dTrans[index].complementTransitions,
                    allDTries,
                    allDTries[index]);

                //Add translations and replace fields to states that have epsilon closures containing translations
                foreach (RegexTrie<T> state in markedDStates[index])
                {
                    if (state.matches != null)
                    {
                        if (allDTries[index].matches == null)
                        {
                            allDTries[index].matches = new List<T>() { state.matches[0] };
                        }
                        else
                        {
                            allDTries[index].matches.Add(state.matches[0]);
                        }
                    }
                }
            }
            //Return a reference to the root node of the trie
            return allDTries[0];
        }
    }
}
