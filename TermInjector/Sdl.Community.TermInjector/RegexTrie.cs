using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.TermInjector
{
    public class RegexTrie<T>
    {
        /// <summary>
        /// Each node consists dictionaries of normal character transitions and complement character transitions
        /// (move to the target node on any chararcter not in dictionary). There are also range transitions (move
        /// to the target node on any character within the range, or any character outside the range if the
        /// complement boolean is switched on). There's also a list of epsilon transitions (move to the target
        /// without any input at all).
        /// </summary>
        /// 

        //The values of the character transitions are lists as there can be multiple group transitions
        //corresponding to the same character. The list position indicates the group number.
        public Dictionary<char, List<Transition<T>>> characterTransitions;
        public List<RangeTransition<T>> rangeTransitions;
        public List<RegexTrie<T>> epsilonTransitions;
        public List<ComplementTransition<T>> complementTransitions;
        public static int counter = 0;
        public int index;

        //Boolean for checking whether trie has been marked during group number propagation
        public Boolean groupsMarked;

        //List of results, if any
        public List<T> matches;

        public RegexTrie()
        {
            this.characterTransitions = new Dictionary<char, List<Transition<T>>>();
            this.epsilonTransitions = new List<RegexTrie<T>>();
            this.rangeTransitions = new List<RangeTransition<T>>();
            this.complementTransitions = new List<ComplementTransition<T>>();
            this.groupsMarked = false;
            this.index = counter;
            counter++;

            //Throw exception if the amount of created nodes is excessive. This way all the memory won't be used up
            if (RegexTrie<T>.counter > 2000000)
            {
                throw new Exception("TermInjector regexes could not be loaded because the resulting finite state machine would have been too large. "+
                "If the regex file is not huge, check the regex file for wave bracket expressions which have a high upper limit (these are computationally expensive).");
            }
        }

        public void AddEpsilonTransition(RegexTrie<T> newEpsilon)
        {
            this.epsilonTransitions.Add(newEpsilon);
        }

        //Adds a range transition, which is used for character ranges and complements
        public void AddRangeTransition(
            RegexTrie<T> dest,
            char rangeStart,
            char rangeEnd,
            byte groupNumber)
        {
            this.rangeTransitions.Add(new RangeTransition<T>(groupNumber, dest, rangeStart, rangeEnd));
        }

        //This is used to add a transition to the specified trie
        //The transitionDict determines whether the transition is added to the normal or complement transitions
        public void AddCharacterTransition(
            char key,
            byte groupNumber,
            RegexTrie<T> newTransition)
        {
            if (this.characterTransitions.ContainsKey(key))
            {
                Transition<T> newTrans = new Transition<T>(groupNumber, newTransition);
                this.characterTransitions[key].Add(newTrans);
            }
            else
            {
                Transition<T> newTrans = new Transition<T>(groupNumber, newTransition);
                List<Transition<T>> newTransList = new List<Transition<T>>();
                newTransList.Add(newTrans);
                this.characterTransitions.Add(key, newTransList);
            }
        }

        //This function gets or creates a transition in the trie
        public RegexTrie<T> GetOrAddChildNode(char c, byte groupNumber)
        {
            //Check if any transitions exists for a character
            if (this.characterTransitions.ContainsKey(c))
            {
                //Then check if a transitions exists for the correct group number
                foreach (Transition<T> tup in this.characterTransitions[c])
                {
                    if (tup.groupNumber == groupNumber)
                    {
                        return tup.destination;
                    }
                }

                //If this is reached, no transition for the group exists, so create it
                Transition<T> newTrans = new Transition<T>(groupNumber, new RegexTrie<T>());
                this.characterTransitions[c].Add(newTrans);
                return newTrans.destination;


            }
            //If no transitions exist for character, create a new transition list
            else
            {
                Transition<T> newTrans = new Transition<T>(groupNumber, new RegexTrie<T>());
                List<Transition<T>> newTransList = new List<Transition<T>>();
                newTransList.Add(newTrans);
                this.characterTransitions.Add(c, newTransList);
                return newTrans.destination;
            }
        }

        public List<RegexTrie<T>> GetEpsilonTransitions()
        {
            return (this.epsilonTransitions);
        }

        public RegexTrie<T> Clone()
        {
            return (RegexTrie<T>)this.MemberwiseClone();
        }
    }

    //Complement transition can contain range and character conditions. Character is rejected if it matches any of the
    //conditions
    public class ComplementTransition<T>
    {
        public byte groupNumber;
        public RegexTrie<T> destination;
        public List<char> characters;
        public List<KeyValuePair<char, char>> ranges;

        public ComplementTransition(byte group, RegexTrie<T> dest)
        {
            this.groupNumber = group;
            this.destination = dest;
            this.characters = new List<char>();
            this.ranges = new List<KeyValuePair<char, char>>();
        }

        public ComplementTransition(byte group, RegexTrie<T> dest, List<char> characters, List<KeyValuePair<char, char>> ranges)
        {
            this.groupNumber = group;
            this.destination = dest;
            this.characters = characters;
            this.ranges = ranges;
        }
    }

    public class RangeTransition<T>
    {
        public byte groupNumber;
        public RegexTrie<T> destination;
        public char rangeStart;
        public char rangeEnd;

        public RangeTransition(byte group, RegexTrie<T> dest, char rangeStart, char rangeEnd)
        {
            this.groupNumber = group;
            this.destination = dest;
            this.rangeStart = rangeStart;
            this.rangeEnd = rangeEnd;
        }
    }

    public class Transition<T>
    {
        public byte groupNumber;
        public RegexTrie<T> destination;

        public Transition(byte group, RegexTrie<T> dest)
        {
            this.groupNumber = group;
            this.destination = dest;
        }
    }

    public class TranslationAndReplacement
    {
        public string translation;
        public string replacement;
        public TranslationAndReplacement(string translation, string replacement)
        {
            this.translation = translation;
            this.replacement = replacement;
        }
    }
}