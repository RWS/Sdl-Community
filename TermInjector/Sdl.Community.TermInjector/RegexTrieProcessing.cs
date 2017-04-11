using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Sdl.Community.TermInjector
{
    class MinMaxResult
    {
        public int minOccurs;
        public int maxOccurs;
        public Boolean starred;
        public Boolean zeroInclusive;

        public MinMaxResult()
        {

        }

        public MinMaxResult(int min, int max, Boolean starred, Boolean zeroed)
        {
            this.minOccurs = min;
            this.maxOccurs = max;
            this.starred = starred;
            this.zeroInclusive = zeroed;
        }

    }

    class TrieAndGroups<T>
    {
        public RegexTrie<T> trie;
        public Dictionary<int, StringBuilder> groups;

        public TrieAndGroups(RegexTrie<T> trie, Dictionary<int, StringBuilder> groups)
        {
            this.trie = trie;
            this.groups = groups;
        }
    }      

    public class RegexTrieFactory<T>
    {
        private StringBuilder handleEscaped(StringBuilder set, char currentChar)
        {
            if (currentChar == 't')
            {
                set.Append("\t");
            }
            else
            {
                set.Append(currentChar);
            }
            return set;
        }

        public RegexTrieFactory()
        {
        }

        public List<KeyValuePair<string, string>> regexValidationErrors = new List<KeyValuePair<string, string>>();

        //Add transitions from a square bracket group between the tries given as arguments.
        //Returns the amount of characters consumed from the input string (so the string index can be incremented).
        private int handleSquareBracketGroup(RegexTrie<T> sourceTrie, RegexTrie<T> targetTrie, string sourceTail)
        {
            Boolean isEscaped = false;
            Boolean isComplement = false;
            //The complement transition to build
            ComplementTransition<T> complement = new ComplementTransition<T>(0, targetTrie);
            //Check whether this is a complement set
            if (sourceTail[0] == '^')
            {
                //Add complement transition to trie
                sourceTrie.complementTransitions.Add(complement);

                isComplement = true;
                sourceTail = sourceTail.Substring(1);
            }

            StringBuilder set = new StringBuilder();
            char currentChar;

            //First build a string out of the expression, then add every char in it or its complement to the trie
            //as transitions
            for (int stringIndex = 0; stringIndex < sourceTail.Length; stringIndex++)
            {
                currentChar = sourceTail[stringIndex];

                if (isEscaped)
                {
                    set = handleEscaped(set,currentChar);
                    isEscaped = false;
                    continue;
                }

                switch (currentChar)
                {

                    case '\\':
                        {
                            isEscaped = true;
                            break;
                        }
                    case '-':
                        {
                            char start = sourceTail[stringIndex - 1];
                            char end = sourceTail[stringIndex + 1];
                            //Add a range transition, which can be a complement
                            if (isComplement)
                            {
                                complement.ranges.Add(new KeyValuePair<char, char>(start, end));
                            }
                            else
                            {
                                sourceTrie.AddRangeTransition(
                                    targetTrie,
                                    start,
                                    end,
                                    0);
                            }
                            //Remove the range start from set
                            set.Remove(set.Length - 1, 1);

                            //skip the range end
                            stringIndex++;
                            break;
                        }
                    //Add the set or its complement as transitions
                    case ']':
                        {
                            string finalSet = set.ToString();
                            if (isComplement)
                            {
                                complement.characters.AddRange(finalSet);
                                //One character was consumed by the complement special character, so increment by 2
                                return stringIndex + 2;
                            }
                            else
                            {
                                foreach (char c in finalSet)
                                {
                                    sourceTrie.AddCharacterTransition(c, 0, targetTrie);
                                }
                                return stringIndex + 1;
                            }
                        }
                    default:
                        {
                            set.Append(currentChar);
                            break;
                        }
                }
            }
            //This should never be reached, as the regex is validated to always contain closing square bracket
            return -1;
        }

        //Adds a control character transition
        private RegexTrie<T> addSpecialTransition(int controlCode, RegexTrie<T> endNodeOfPreviousComponent)
        {
            RegexTrie<T> startNodeOfCurrentComponent = new RegexTrie<T>();
            //Connect start node to previous end node
            endNodeOfPreviousComponent.AddEpsilonTransition(startNodeOfCurrentComponent);
            //Use STX control character
            return startNodeOfCurrentComponent.GetOrAddChildNode((char)controlCode, 0);
        }

        //Version of the method without stack arguments (so they don't need to be initialized in the calling code)
        public int AddToRegexTrie(RegexTrie<T> trie, string source, T matchobject)
        {
            AddToRegexTrie(
                trie,
                source,
                matchobject,
                new Stack<RegexTrie<T>>(),
                new Stack<RegexTrie<T>>(),
                new Stack<RegexTrie<T>>());
            return 0;
        }

        //This has to keep track of four nodes: end of the previous component, start of the current component,
        //end of the current component and the node that is connected to the end of the current component with
        //an epsilon transition (which will become the end of the previous component for the next component).
        public void AddToRegexTrie
            (RegexTrie<T> trie,
            string source,
            T matchobject,
            Stack<RegexTrie<T>> endStack,
            Stack<RegexTrie<T>> startStack,
            Stack<RegexTrie<T>> commonDestination)
        {
            RegexTrie<T> newTrie = new RegexTrie<T>();

            //Current trie being added to
            RegexTrie<T> endNodeOfCurrentComponent = trie;

            //End of previous trie, to which the current trie is joined
            RegexTrie<T> endNodeOfPreviousComponent = trie;

            //End of previous trie, to which the current trie is joined
            RegexTrie<T> startNodeOfCurrentComponent = trie;

            //Push trie on the start stack
            startStack.Push(trie);

            //Push a null on the commonStart and commonDestination stacks
            commonDestination.Push(null);

            //Matching group counter.
            byte groupCount = 1;

            //True if previous character was an escape char
            Boolean escapedCharacter = false;

            //Variable for the character at the loop index
            char currentChar;

            //Holds the result of the quantifier check
            KeyValuePair<int, RegexTrie<T>> quantifierCheckResult;

            for (int stringIndex = 0; stringIndex < source.Length; stringIndex++)
            {
                currentChar = source[stringIndex];


                //Check for escape character
                if (currentChar == '\\')
                {
                    escapedCharacter = true;
                    continue;
                }

                //If the character is escaped, just make it into a trie
                if (escapedCharacter)
                {
                    //Special char handling
                    if (currentChar == 't')
                    {
                        currentChar = '\t';
                    }
                    startNodeOfCurrentComponent = new RegexTrie<T>();
                    endNodeOfCurrentComponent = startNodeOfCurrentComponent.GetOrAddChildNode(currentChar, 0);
                    quantifierCheckResult = checkForQuantifier(
                            source.Substring(stringIndex + 1),
                            endNodeOfCurrentComponent,
                            startNodeOfCurrentComponent,
                            endNodeOfPreviousComponent,
                            commonDestination);
                    stringIndex += quantifierCheckResult.Key;
                    endNodeOfPreviousComponent = quantifierCheckResult.Value;
                    escapedCharacter = false;
                    continue;
                }
                //Else check character for special meaning
                else
                {
                    switch (currentChar)
                    {
                        //Open a new group
                        case '(':
                            {
                                //Push the previous trie to the end stack
                                endStack.Push(endNodeOfPreviousComponent);
                                //Create a new trie
                                endNodeOfPreviousComponent = new RegexTrie<T>();
                                //Push the newly created trie on the start stack
                                startStack.Push(endNodeOfPreviousComponent);
                                //Push a null trie on the common destination and start stacks (to be defined, if pipes are found)
                                commonDestination.Push(null);
                                break;
                            }
                        //Close a group
                        case ')':
                            {
                                //If common destination exists, add an epsilon transition to it
                                if (commonDestination.Peek() != null)
                                {
                                    //Connect the end node and common destination as part of the same epsilon closure
                                    commonDestination.Peek().AddEpsilonTransition(endNodeOfPreviousComponent);
                                    endNodeOfPreviousComponent.AddEpsilonTransition(commonDestination.Peek());
                                    //Move the current trie to common destination, as that's where the building will continue
                                    //Pop the common destination, it won't be needed anymore
                                    endNodeOfCurrentComponent = commonDestination.Pop();

                                }
                                else
                                {
                                    endNodeOfCurrentComponent = endNodeOfPreviousComponent;
                                    //Pop the null destination
                                    commonDestination.Pop();
                                }

                                startNodeOfCurrentComponent = startStack.Pop();
                                endNodeOfPreviousComponent = endStack.Pop();

                                quantifierCheckResult = checkForQuantifier(
                                    source.Substring(stringIndex + 1),
                                    endNodeOfCurrentComponent,
                                    startNodeOfCurrentComponent,
                                    endNodeOfPreviousComponent,
                                    commonDestination);
                                stringIndex += quantifierCheckResult.Key;
                                endNodeOfPreviousComponent = quantifierCheckResult.Value;
                                try
                                {
                                    endStack.Peek();
                                }
                                //If we're at the top, add group number to each node of the trie
                                catch
                                {
                                    addGroupNumbers(startNodeOfCurrentComponent, groupCount);
                                    groupCount++;
                                }
                                break;
                            }

                        //Handle square bracket set
                        case '[':
                            {
                                startNodeOfCurrentComponent = new RegexTrie<T>();
                                endNodeOfCurrentComponent = new RegexTrie<T>();
                                //This skips over the closing square bracket, so there's no need for closing square bracket handling
                                stringIndex += handleSquareBracketGroup(startNodeOfCurrentComponent, endNodeOfCurrentComponent, source.Substring(stringIndex + 1));
                                quantifierCheckResult = checkForQuantifier(
                                    source.Substring(stringIndex + 1),
                                    endNodeOfCurrentComponent,
                                    startNodeOfCurrentComponent,
                                    endNodeOfPreviousComponent,
                                    commonDestination);
                                stringIndex += quantifierCheckResult.Key;
                                endNodeOfPreviousComponent = quantifierCheckResult.Value;
                                break;
                            }
                        //Caret at the start: add a transition with a control character that won't exist in text.
                        //Feed the control character when finding matches.
                        case '^':
                            {
                                endNodeOfPreviousComponent = addSpecialTransition(2, endNodeOfPreviousComponent);
                                break;
                            }
                        //Dollar at end: add a transition with a control character that won't exist in text.
                        //Feed the control character when finding matches.
                        case '$':
                            {
                                endNodeOfPreviousComponent = addSpecialTransition(3, endNodeOfPreviousComponent);
                                break;
                            }

                        //Period handling
                        case '.':
                            {
                                startNodeOfCurrentComponent = new RegexTrie<T>();
                                endNodeOfCurrentComponent = new RegexTrie<T>();

                                //Add complement of null character
                                startNodeOfCurrentComponent.complementTransitions.Add(
                                    new ComplementTransition<T>(
                                        0,
                                        endNodeOfCurrentComponent,
                                        new List<char>(),
                                        new List<KeyValuePair<char, char>>() { new KeyValuePair<char, char>((char)0, (char)0) }));

                                quantifierCheckResult = checkForQuantifier(
                                    source.Substring(stringIndex + 1),
                                    endNodeOfCurrentComponent,
                                    startNodeOfCurrentComponent,
                                    endNodeOfPreviousComponent,
                                    commonDestination);
                                stringIndex += quantifierCheckResult.Key;
                                endNodeOfPreviousComponent = quantifierCheckResult.Value;
                                break;
                            }
                        //Change previous end to common destination and move end node to common start
                        case '|':
                            {
                                if (commonDestination.Peek() == null)
                                {
                                    commonDestination.Pop();
                                    commonDestination.Push(endNodeOfPreviousComponent);
                                }
                                else
                                {
                                    //Connect the end node and common destination as part of the same epsilon closure
                                    commonDestination.Peek().AddEpsilonTransition(endNodeOfPreviousComponent);
                                    endNodeOfPreviousComponent.AddEpsilonTransition(commonDestination.Peek());
                                }
                                endNodeOfPreviousComponent = startStack.Peek();
                                break;
                            }
                        default:
                            {
                                startNodeOfCurrentComponent = new RegexTrie<T>();
                                endNodeOfCurrentComponent = startNodeOfCurrentComponent.GetOrAddChildNode(currentChar, 0);
                                quantifierCheckResult = checkForQuantifier(
                                    source.Substring(stringIndex + 1),
                                    endNodeOfCurrentComponent,
                                    startNodeOfCurrentComponent,
                                    endNodeOfPreviousComponent,
                                    commonDestination);
                                stringIndex += quantifierCheckResult.Key;
                                endNodeOfPreviousComponent = quantifierCheckResult.Value;
                                break;
                            }
                    }
                }
            }
            //Link end node to common destination and start node to common start if they exist
            if (commonDestination.Peek() != null)
            {
                endNodeOfPreviousComponent.AddEpsilonTransition(commonDestination.Peek());
                endNodeOfPreviousComponent = commonDestination.Pop();
            }

            //Add translation and replace fields to the current trie
            endNodeOfPreviousComponent.matches = new List<T> {
                matchobject  };
        }

        //This checks for a quantifier immediately after the character, and joins the current trie to the end of the
        //previous trie. Returns a pair of int (to increment string index) and the node to which further tries will be connected
        private KeyValuePair<int, RegexTrie<T>> checkForQuantifier(
            string sourceTail,
            RegexTrie<T> endNodeOfCurrentComponent,
            RegexTrie<T> startNodeOfCurrentComponent,
            RegexTrie<T> endNodeOfPreviousComponent,
            Stack<RegexTrie<T>> commonDestination)
        {
            char nextChar;
            try
            {
                nextChar = sourceTail[0];
            }
            catch
            {
                //assign a non-special character to trigger the default case
                nextChar = 'n';
            }

            switch (nextChar)
            {
                case '*':
                    {
                        return new KeyValuePair<int, RegexTrie<T>>(1, joinQuantifiedTrie(
                            new MinMaxResult(1, 1, true, true),
                            startNodeOfCurrentComponent,
                            endNodeOfCurrentComponent,
                            endNodeOfPreviousComponent));
                    }
                case '+':
                    {
                        return new KeyValuePair<int, RegexTrie<T>>(1, joinQuantifiedTrie(
                            new MinMaxResult(1, 1, true, false),
                            startNodeOfCurrentComponent,
                            endNodeOfCurrentComponent,
                            endNodeOfPreviousComponent));
                    }
                case '?':
                    {
                        return new KeyValuePair<int, RegexTrie<T>>(1, joinQuantifiedTrie(
                            new MinMaxResult(1, 1, false, true),
                            startNodeOfCurrentComponent,
                            endNodeOfCurrentComponent,
                            endNodeOfPreviousComponent));
                    }
                case '{':
                    {
                        int closeBraceIndex = sourceTail.IndexOf('}');
                        string quantString = sourceTail.Substring(1, closeBraceIndex - 1);
                        MinMaxResult minMax = findMinMax(quantString);
                        return new KeyValuePair<int, RegexTrie<T>>(closeBraceIndex + 1, joinQuantifiedTrie(
                            minMax,
                            startNodeOfCurrentComponent,
                            endNodeOfCurrentComponent,
                            endNodeOfPreviousComponent));
                    }

                default:
                    {
                        endNodeOfPreviousComponent.AddEpsilonTransition(startNodeOfCurrentComponent);
                        return new KeyValuePair<int, RegexTrie<T>>(0, endNodeOfCurrentComponent);
                    }
            }

        }

        //This adds the transitions required to make a group trie correspond to the correct quantified regex
        private RegexTrie<T> joinQuantifiedTrie(MinMaxResult result,
            RegexTrie<T> startNodeOfCurrentComponent,
            RegexTrie<T> endNodeOfCurrentComponent,
            RegexTrie<T> endNodeOfPreviousComponent)
        {
            //This is the end node which will be reached by epsilon transition in case of zeroed quantifiers
            //or quantifiers with a number range of more than one, start node is used for zeroing
            RegexTrie<T> epsilonEndNode = new RegexTrie<T>();
            RegexTrie<T> epsilonStartNode = new RegexTrie<T>();

            //This holds the end node used during the loop
            RegexTrie<T> loopEndNode = endNodeOfCurrentComponent;

            //Loop from 1 (one occurrence already exists) to max occurrences. If loop index is greater than min,
            //add epsilon from iteration trie end to final trie end.

            if (result.minOccurs > 1 || result.maxOccurs > 1)
            {
                //This is used as a model for copying, only needs to be calculated if there's a set number of occurrences
                KeyValuePair<RegexTrie<T>, RegexTrie<T>> trieModel = copyTrie(startNodeOfCurrentComponent, endNodeOfCurrentComponent);
                //Result of trie copy
                KeyValuePair<RegexTrie<T>, RegexTrie<T>> trieCopy;

                for (int index = 2; index <= result.maxOccurs; index++)
                {
                    if (index <= result.maxOccurs)
                    {
                        //Copy the trie and link it to the end of trie
                        trieCopy = copyTrie(trieModel);
                        loopEndNode.AddEpsilonTransition(trieCopy.Key);
                        //If the index is equal or greater than minoccurs, add epsilon to end node
                        if (result.minOccurs <= index)
                        {
                            trieCopy.Value.AddEpsilonTransition(epsilonEndNode);
                        }
                        //Assign the end node of the copy as the end node of the component
                        loopEndNode = trieCopy.Value;
                    }
                }
            }

            //If there's less than two occurrences, connect the end of the first span of the trie to the epsilon end trie
            //Do this after the copy loop in order to not disturb the copying.
            if (result.minOccurs < 2)
            {
                endNodeOfCurrentComponent.AddEpsilonTransition(epsilonEndNode);
            }

            //If the quantifier is starred, add epsilon from end to start
            if (result.starred)
            {
                endNodeOfCurrentComponent.AddEpsilonTransition(startNodeOfCurrentComponent);
            }

            //If the quantifier is zeroed, add an epsilon from epsilon start node
            //newCurrentTrie
            if (result.zeroInclusive)
            {
                epsilonStartNode.AddEpsilonTransition(startNodeOfCurrentComponent);
                epsilonStartNode.AddEpsilonTransition(epsilonEndNode);
                startNodeOfCurrentComponent = epsilonStartNode;
            }

            //Connect trie to previous trie
            endNodeOfPreviousComponent.AddEpsilonTransition(startNodeOfCurrentComponent);

            return epsilonEndNode;

        }

        //Makes an exact copy of a trie, returns the start and end nodes. Wrapper for pair argument 
        private KeyValuePair<RegexTrie<T>, RegexTrie<T>> copyTrie(KeyValuePair<RegexTrie<T>, RegexTrie<T>> pair)
        {
            return copyTrie(pair.Key, pair.Value);
        }

        //Makes an exact copy of a trie, returns the start and end nodes
        private KeyValuePair<RegexTrie<T>, RegexTrie<T>> copyTrie(RegexTrie<T> start, RegexTrie<T> end)
        {
            //The start node of the copied trie
            RegexTrie<T> trieCopy = new RegexTrie<T>();

            //The source node being copied
            RegexTrie<T> sourceNode;

            //The target node being copied
            RegexTrie<T> targetNode;

            //Dictionary specifying which source node corresponds to which target node
            Dictionary<RegexTrie<T>, RegexTrie<T>> nodeCorrespondences = new Dictionary<RegexTrie<T>, RegexTrie<T>>();

            //The first correspondence is that of start and trieCopy
            nodeCorrespondences.Add(start, trieCopy);

            //List of visited nodes, which won't be added to the stack when encountered
            List<RegexTrie<T>> visitedNodes = new List<RegexTrie<T>>();

            //Node stack used to imitate recursion
            Stack<KeyValuePair<RegexTrie<T>, RegexTrie<T>>> nodeStack = new Stack<KeyValuePair<RegexTrie<T>, RegexTrie<T>>>();

            //Push the start of the trie and the copy to the stack
            nodeStack.Push(new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(start, trieCopy));

            while (nodeStack.Count > 0)
            {
                KeyValuePair<RegexTrie<T>, RegexTrie<T>> currentPair = nodeStack.Pop();
                sourceNode = currentPair.Key;
                targetNode = currentPair.Value;
                visitedNodes.Add(sourceNode);

                //Copy epsilon transitions
                foreach (var node in sourceNode.epsilonTransitions)
                {
                    //Add the epsilon transition to the target node
                    if (!nodeCorrespondences.ContainsKey(node))
                    {
                        RegexTrie<T> newCorrespondingNode = new RegexTrie<T>();
                        nodeCorrespondences.Add(node, newCorrespondingNode);
                    }

                    targetNode.AddEpsilonTransition(nodeCorrespondences[node]);

                    //If the node has not been visited, push it to the stack
                    if (!visitedNodes.Contains(node))
                    {
                        nodeStack.Push(new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(node, nodeCorrespondences[node]));
                    }

                }

                //Copy character transitions
                foreach (var key in sourceNode.characterTransitions.Keys)
                {
                    foreach (var transition in sourceNode.characterTransitions[key])
                    {
                        //Add the transition to the target node
                        if (!nodeCorrespondences.ContainsKey(transition.destination))
                        {
                            RegexTrie<T> newCorrespondingNode = new RegexTrie<T>();
                            nodeCorrespondences.Add(transition.destination, newCorrespondingNode);
                        }

                        if (targetNode.characterTransitions.ContainsKey(key))
                        {
                            targetNode.characterTransitions[key].Add(
                                new Transition<T>(transition.groupNumber, nodeCorrespondences[transition.destination]));
                        }
                        else
                        {
                            targetNode.characterTransitions.Add(key, new List<Transition<T>>());
                            targetNode.characterTransitions[key].Add(
                                new Transition<T>(transition.groupNumber, nodeCorrespondences[transition.destination]));
                        }

                        //If the node has not been visited, push it to the stack
                        if (!visitedNodes.Contains(transition.destination))
                        {
                            nodeStack.Push(new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(
                                transition.destination, nodeCorrespondences[transition.destination]));
                        }
                    }
                }

                //Copy range transitions
                foreach (var rangeTrans in sourceNode.rangeTransitions)
                {
                    if (!nodeCorrespondences.ContainsKey(rangeTrans.destination))
                    {
                        RegexTrie<T> newCorrespondingNode = new RegexTrie<T>();
                        nodeCorrespondences.Add(rangeTrans.destination, newCorrespondingNode);
                    }

                    targetNode.AddRangeTransition(
                        nodeCorrespondences[rangeTrans.destination],
                        rangeTrans.rangeStart,
                        rangeTrans.rangeEnd,
                        rangeTrans.groupNumber);

                    //If the node has not been visited, push it to the stack
                    if (!visitedNodes.Contains(rangeTrans.destination))
                    {
                        nodeStack.Push(
                            new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(
                                rangeTrans.destination, nodeCorrespondences[rangeTrans.destination]));
                    }
                }

                //Copy complement transitions
                foreach (var compTrans in sourceNode.complementTransitions)
                {
                    if (!nodeCorrespondences.ContainsKey(compTrans.destination))
                    {
                        RegexTrie<T> newCorrespondingNode = new RegexTrie<T>();
                        nodeCorrespondences.Add(compTrans.destination, newCorrespondingNode);
                    }

                    targetNode.complementTransitions.Add(new ComplementTransition<T>(
                        compTrans.groupNumber,
                        nodeCorrespondences[compTrans.destination],
                        compTrans.characters,
                        compTrans.ranges));

                    //If the node has not been visited, push it to the stack
                    if (!visitedNodes.Contains(compTrans.destination))
                    {
                        nodeStack.Push(
                            new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(
                                compTrans.destination, nodeCorrespondences[compTrans.destination]));
                    }
                }
            }

            //Return the node corresponding to the end node
            return new KeyValuePair<RegexTrie<T>, RegexTrie<T>>(nodeCorrespondences[start], nodeCorrespondences[end]);
        }


        //This goes through the trie adding the specified group number to transitions of each node
        private void addGroupNumbers(RegexTrie<T> trie, byte groupNumber)
        {
            //Make sure the same trie is not handled twice
            if (trie.groupsMarked)
            {
                return;
            }
            trie.groupsMarked = true;

            //Iterate over epsilon transitions
            foreach (RegexTrie<T> epsilon in trie.epsilonTransitions)
            {
                addGroupNumbers(epsilon, groupNumber);
            }

            //Iterate over the transitions, change the group number and call addGroupNumbers for transition destinations
            foreach (List<Transition<T>> transList in trie.characterTransitions.Values)
            {
                foreach (Transition<T> trans in transList)
                {
                    trans.groupNumber = groupNumber;
                    addGroupNumbers(trans.destination, groupNumber);
                }
            }

            //Do the same for range transitions
            foreach (RangeTransition<T> trans in trie.rangeTransitions)
            {
                trans.groupNumber = groupNumber;
                addGroupNumbers(trans.destination, groupNumber);
            }

            //Do the same for complement transitions
            foreach (ComplementTransition<T> trans in trie.complementTransitions)
            {
                trans.groupNumber = groupNumber;
                addGroupNumbers(trans.destination, groupNumber);
            }
            return;
        }

        private static List<char> regexOperatorsForNormalizing = new List<char> { '+', '(', ')', '?', '{' };

        private static List<char> regexOperators = new List<char> { '(', ')', '[', ']', '.', '-', '^', '*', '\\' };

        
        private StringBuilder generateQuantifierEquivalent(int min, int max, Boolean starred, string expression)
        {
            StringBuilder repeatBuilder = new StringBuilder();
            StringBuilder equivalent = new StringBuilder();
            for (int i = (min == 0 ? 1 : min); i <= max; i++)
            {
                repeatBuilder.Length = 0;
                for (int r = 0; r < i; r++)
                {
                    repeatBuilder.Append(expression);
                }
                equivalent.Append(repeatBuilder.ToString() + '|');
            }
            if (starred)
            {
                equivalent.Append(expression + "*");
            }
            else
            {
                //Remove the extra pipe
                equivalent.Remove(equivalent.Length - 1, 1);
            }            
            return equivalent;
        }

        private MinMaxResult findMinMax(string quantString)
        {
            Boolean zeroed = false;
            Boolean starred = false;
            int min;
            int max;
            int commaIndex = quantString.IndexOf(',');
            if (commaIndex == -1)
            {
                int minAndMax = Convert.ToInt32(quantString);
                return new MinMaxResult(minAndMax, minAndMax, false, false);
            }
            else
            {
                try
                {
                    min = Convert.ToInt32(quantString.Substring(0, commaIndex));
                    if (min == 0)
                    {
                        zeroed = true;
                    }
                }
                catch
                {
                    min = 1;
                    zeroed = true;
                }
                try
                {
                    max = Convert.ToInt32(quantString.Substring(commaIndex + 1));
                }
                catch
                {
                    max = min;
                    starred = true;
                }
            }

            return new MinMaxResult(min, max, starred, zeroed);
        }


        private static string bigAsciiChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string smallAsciiChars = bigAsciiChars.ToLower();

        private Boolean isValidAsciiRange(char start, char end)
        {
            return (
                start<end);
        }

        private Boolean isValidAsciiRange(char start, char end, string range)
        {
            int startPos = range.IndexOf(start);
            int endPos = range.IndexOf(end);

            if (startPos == -1 || endPos == -1)
            {
                return false;
            }

            if (endPos > startPos)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Error messages, first is empty because 0 indicates all fine
        public List<string> errorMessages = new List<string>(){
            "",
            "Wave bracket quantifier contains several commas. Only one comma is allowed in a wave bracket quantifier.",
            "Wave bracket quantifier contains a character that is not a comma or a number. Wave bracket quantifiers can contain only numbers and a single comma",
            "Square bracket set is missing the closing square bracket.",
            "Invalid range in a square bracket set. Start of range has to be before end of range.",
            "The amounts of opening and closing parentheses don't match.",
            "Wave bracket quantifier is missing the closing wave bracket.",
            "Wave bracket quantifier is missing the opening wave bracket.",
            "Square bracket set is missing the opening square bracket.",
            "Consecutive quantifiers are not allowed. Characters |,+,? and * and wave bracket expressions (e.g. {1,2}) are considered quantifiers.",
            "String start character ^ has to be at the start of the regular expression",
            "String end character $ has to be at the end of the regular expression",
            "Unescaped pipe not allowed at the start or end of the regular expression"
        };

        //Validates a regex, returns an error code if validation fails. Error messages above

        public int validateRegex(string regex)
        {
            //Counter for parentheses, opening parenthesis increments, closing decremenents
            //Must be zero at end of string
            int parenthesisCount = 0;
            //Booleans for being within special blocks
            Boolean withinSquareBracketSet = false;
            Boolean withinWaveBracketQuantifier = false;
            Boolean startingWaveBracket = false;
            Boolean waveBracketCommaFound = false;
            Boolean isEscaped = false;

            char currentChar;
            char nextChar;
            for (int stringIndex = 0; stringIndex < regex.Length; stringIndex++)
            {
                if (isEscaped)
                {
                    isEscaped = false;
                    continue;
                }

                currentChar = regex[stringIndex];
                try
                {
                    nextChar = regex[stringIndex + 1];
                }
                catch
                {
                    nextChar = 'n';
                }

                //Check for consecutive quantifiers
                if ("*?+|}".Contains(currentChar) && "*?{+".Contains(nextChar))
                {
                    return 9;
                }

                //Special case for consecutive pipes (pipe is allowed after other quantifier characters
                if (currentChar == '|' && nextChar == '|')
                {
                    return 9;
                }

                switch (currentChar)
                {
                    case '|':
                        {
                            if (stringIndex == 0 || stringIndex == regex.Length - 1)
                            {
                                return 12;
                            }
                            break;
                        }
                    case '\\':
                        {
                            isEscaped = true;
                            break;
                        }
                    //Caret is allowed at the start of the string and inside square bracket expressions
                    //I've disabled the caret and dollar errors, as they will occasionally be used within the string
                    //I ought the replace this later with a proper error detection
                    case '^':
                        {
                            if (!withinSquareBracketSet)
                            {
                                if (stringIndex != 0)
                                {
                                    //return 10;
                                }
                            }
                            break;

                        }
                    case '$':
                        {
                            if (stringIndex != regex.Length - 1)
                            {
                                //return 11;
                            }
                            break;
                        }
                    case '(':
                        {
                            parenthesisCount += 1;
                            break;
                        }
                    case ')':
                        {
                            parenthesisCount -= 1;
                            break;
                        }
                    case '[':
                        {
                            withinSquareBracketSet = true;
                            break;
                        }
                    case ']':
                        {
                            if (!withinSquareBracketSet)
                            {
                                return 8;
                            }
                            withinSquareBracketSet = false;
                            break;
                        }
                    case '{':
                        {
                            withinWaveBracketQuantifier = true;
                            startingWaveBracket = true;
                            break;
                        }
                    case '}':
                        {
                            if (!withinWaveBracketQuantifier)
                            {
                                return 7;
                            }
                            //Reset wave bracket booleans
                            withinWaveBracketQuantifier = false;
                            waveBracketCommaFound = false;
                            break;
                        }
                }

                //Error conditions

                if (withinWaveBracketQuantifier)
                {

                    if (startingWaveBracket)
                    {
                        startingWaveBracket = false;
                        continue;
                    }

                    if (currentChar == ',')
                    {
                        //Several commas or non-numeric characters in a wave bracket quantifier
                        if (waveBracketCommaFound)
                        {
                            return 1;
                        }
                        else
                        {
                            waveBracketCommaFound = true;
                        }
                    }

                    //Non-numeric characters in a wave bracket quantifier
                    if (!Char.IsNumber(currentChar) && currentChar != ',')
                    {
                        return 2;
                    }
                }

                if (withinSquareBracketSet)
                {

                    //Non-numeric or non-alphabetic range
                    if (currentChar == '-')
                    {
                        //Might be the last character, so next char may not exist
                        char previousChar = regex[stringIndex - 1];
                        try
                        {
                            nextChar = regex[stringIndex + 1];
                        }
                        catch
                        {
                            return 3;
                        }

                        if (Char.IsNumber(previousChar))
                        {
                            if (!Char.IsNumber(nextChar))
                            {
                                return 4;
                            }
                            continue;
                        }

                        if (!isValidAsciiRange(previousChar, nextChar))
                        {
                            return 4;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            //Final error conditions
            if (parenthesisCount != 0)
            {
                return 5;
            }

            if (withinSquareBracketSet)
            {
                return 3;
            }

            if (withinWaveBracketQuantifier)
            {
                return 6;
            }
            //All fine
            return 0;
        }
    }
}