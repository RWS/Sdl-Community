using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RegexTrie;
using System.IO;

namespace Sdl.Community.TermInjector
{
    /// <summary>
    /// This contains search methods for tries and regex tries. The RegexTrie library contains another search method class,
    /// this is the original search class, the RegexTrie version was adapted from this. It would be best to change this
    /// to use the RegexTrie class, but this works for now.
    /// </summary>
    class TrieProcessing
    {
        //This removes duplicate trieAndGroup objects.
        public List<RegexPathWithStartCharacterPosition> removeDuplicateTrieAndGroups(
            List<RegexPathWithStartCharacterPosition> paths)
        {
            Dictionary<RegexTrie<TranslationAndReplacement>,RegexPathWithStartCharacterPosition> newPaths =
                new Dictionary<RegexTrie<TranslationAndReplacement>,RegexPathWithStartCharacterPosition>();
            foreach (RegexPathWithStartCharacterPosition path in paths)
            {
                
                //Check if the trie has been added
                if (!newPaths.ContainsKey(path.trie))
                {
                    //Add trie to new paths
                    newPaths.Add(path.trie, path);
                }
                else
                {
                    //If trie has been added, compare start character positions
                    if (newPaths[path.trie].startCharacterPosition > path.startCharacterPosition)
                    {
                        //The old path is shorter, so replace it
                        newPaths[path.trie] = path;
                    }
                    if (newPaths[path.trie].startCharacterPosition == path.startCharacterPosition)
                    {
                        //The paths are the same length, so choose according to group length
                        for (var i = 0; i < path.groups.Count;i++)
                        {
                            try
                            {
                                //Whichever has the longest first group is selected
                                if (path.groups[i].Length > newPaths[path.trie].groups[i].Length)
                                {
                                    newPaths[path.trie] = path;
                                }
                            }
                            //If groups are of same length, the one that has more groups wins
                            catch
                            {
                                newPaths[path.trie] = path;
                            }
                        }
                    }
                }
            }
            
            return newPaths.Values.ToList();
        }

        private static List<char> regexOperators = new List<char> { '\\','(', ')', '[', ']', '.', '-', '^', '*' };

        private string allNumbers()
        {
            return "0123456789";
        }

        //This inserts groups into the variables in a regex and returns a string
        private string insertGroupsIntoRegex(string regexString,Dictionary<int, StringBuilder> groups, Boolean escape)
        {
            StringBuilder finalString = new StringBuilder();
            StringBuilder numberString = new StringBuilder();
            Dictionary<int, StringBuilder> escapedGroups = new Dictionary<int, StringBuilder>();

            //If groups are used in replace fields, all special characters need to be escaped.
            //Otherwise special characters in the text found in the source will mess up the
            //regex building when the replace field string is compiled into a regex

            if (escape)
            {
                for (int groupIndex = 0; groupIndex  < groups.Count; groupIndex++)
                {
                    //Deep copy all existing groups to temporary groups, which can be escaped
                    try
                    {
                        var escapedGroup = new StringBuilder(groups[groupIndex].ToString());
                        foreach (char c in regexOperators)
                        {
                            escapedGroup.Replace(c.ToString(), "\\" + c.ToString());
                        }
                        escapedGroups.Add(groupIndex, escapedGroup);
                    }
                    catch
                    {
                    }
                }
            }

            //Go through the regex 
            for (int charIndex = 0; charIndex < regexString.Length; charIndex++)
            {
                //Check for escape character 
                if (regexString[charIndex] == '\\')
                {
                    //If the escape character is escaped, skip both escape characters
                    if (regexString[charIndex + 1] == '\\')
                    {
                        charIndex += 1;
                        continue;
                    }
                    //Group zero is not allowed
                    if (regexString[charIndex + 1] == '0')
                    {
                        finalString.Append(regexString[charIndex]);
                        continue;
                    }
                    //If escape character is encountered, check for number next to it 
                    for (int numberIndex = charIndex + 1; numberIndex < regexString.Length; numberIndex++)
                    {
                        if (allNumbers().Contains(regexString[numberIndex]))
                        {
                            numberString.Append(regexString[numberIndex]);
                            charIndex = numberIndex;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (numberString.Length > 0)
                    {
                        int groupNumber = Convert.ToInt32(numberString.ToString());
                        //Check if the group string exists. If it does, append it to the final string
                        //Else append nothing (this effectively removes group tokens which have no string recorded)
                        try
                        {
                            if (escape) {
                                finalString.Append(escapedGroups[groupNumber].ToString());
                            } else {
                                finalString.Append(groups[groupNumber].ToString());
                            }
                        }
                        catch
                        {

                        }
                        numberString.Length = 0;
                        continue;
                    }

                }
                finalString.Append(regexString[charIndex]);
            }
            return finalString.ToString();
            
        }

        //Adds the a translation at the node to the results
        private void addTranslation(
            RegexPathWithStartCharacterPosition triePath,
            List<PositionAndTranslation> positionAndTranslationOfTerms,
            int endCharacter)
        {
            try
            {
                int termStartCharacter = triePath.startCharacterPosition;
                string termTranslation;
                string termReplaces;
                //If there are groups, insert their contents into the regex translation
                termTranslation = insertGroupsIntoRegex(triePath.trie.matches[0].translation, triePath.groups, false);
                termReplaces = insertGroupsIntoRegex(triePath.trie.matches[0].replacement, triePath.groups, true);

                //The end character position is character index minus one, as the current character is a boundary character
                positionAndTranslationOfTerms.Add(new PositionAndTranslation(
                    termStartCharacter, endCharacter, termTranslation, termReplaces, true));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                MessageBox.Show(e.Message);
            }
            
        }

        //Checks whether a trie path continues with the current character
        private void checkWhetherPathContinues(
            RegexPathWithStartCharacterPosition triePath,
            List<RegexPathWithStartCharacterPosition> newTriePathBeingTraversed,
            char currentChar)
        {
            //Get the groups in this path
            Dictionary<int, StringBuilder> currentGroups = triePath.groups;
            //Get the list of transitions for the current character from this path
            if (triePath.trie.characterTransitions.ContainsKey(currentChar))
            {
                //Iterate over the possible paths for the char (possible groups)                            
                foreach (Transition<TranslationAndReplacement> trans in triePath.trie.characterTransitions[currentChar])
                {
                    //Check the group number. If the number is greater than zero, record the transition
                    Dictionary<int, StringBuilder> newGroups = recordGroups(currentGroups, trans.groupNumber, currentChar);

                    //Add trie to newTries
                    newTriePathBeingTraversed.Add(new RegexPathWithStartCharacterPosition(
                        triePath.startCharacterPosition, trans.destination, newGroups));
                }

            }

            //Check normal range transitions
            foreach (var rangeTrans in triePath.trie.rangeTransitions)
            {
                //First check normal range transitions
                if (currentChar >= rangeTrans.rangeStart && currentChar <= rangeTrans.rangeEnd)
                {
                    //Check the group number. If the number is greater than zero, record the transition
                    //First make sure a string builder exists for the group number
                    Dictionary<int, StringBuilder> newGroups = recordGroups(currentGroups, rangeTrans.groupNumber, currentChar);
                    newTriePathBeingTraversed.Add(new RegexPathWithStartCharacterPosition(
                        triePath.startCharacterPosition, rangeTrans.destination, newGroups));
                }
                
            }

            //Check complement transitions
            foreach (var compTrans in triePath.trie.complementTransitions)
            {
                Boolean follow = true;
                if (compTrans.characters.Contains(currentChar))
                {
                    follow = false;
                }
                foreach (var range in compTrans.ranges)
                {
                    if (currentChar >= range.Key && currentChar <= range.Value)
                    {
                        follow = false;
                    }
                }

                if (follow)
                {
                    Dictionary<int, StringBuilder> newGroups = recordGroups(currentGroups, compTrans.groupNumber, currentChar);
                    newTriePathBeingTraversed.Add(new RegexPathWithStartCharacterPosition(
                            triePath.startCharacterPosition, compTrans.destination, newGroups));

                }
            }
            
        }

        public Dictionary<int, StringBuilder> recordGroups(Dictionary<int, StringBuilder> currentGroups,
            byte groupNumber,
            char currentChar)
        {
            if (groupNumber > 0)
            {
                if (!currentGroups.ContainsKey(groupNumber))
                {
                    currentGroups.Add(groupNumber, new StringBuilder());
                }

                //If there are several paths, the stringbuilder needs to be copied into each extra path
                //(Using them by reference obviously messes everything up)
                Dictionary<int, StringBuilder> newGroups = new Dictionary<int, StringBuilder>();
                StringBuilder SB;
                foreach (KeyValuePair<int,StringBuilder> kv in currentGroups) 
                {
                    SB = new StringBuilder(currentGroups[kv.Key].ToString());
                    newGroups.Add(kv.Key, SB);
                }
                
                //Add the char to the string builder
                newGroups[groupNumber].Append(currentChar);

                return newGroups;
            }
            return currentGroups;
        }

        //This will return all regexmatches found from the search string
        public List<PositionAndTranslation> FindRegexMatches
            (RegexTrie<TranslationAndReplacement> regexTrie, string inputString, string tokenBoundaryCharacterString,
            Boolean useBoundaryCharacters)
        {
            //triePathsBeingTraversed field holds the paths within the regexTrie that are currently being followed
            //Each value of the dictionary also contains the character position where following of the path was started
            //and string builder for the groups within the regex
            List<RegexPathWithStartCharacterPosition> triePathsBeingTraversed = new List<RegexPathWithStartCharacterPosition>();

            //This list will be built during iteration and used as the new list in the next loop cycle
            List<RegexPathWithStartCharacterPosition> newTriePathsBeingTraversed = new List<RegexPathWithStartCharacterPosition>();

            //positionAndTranslationOfTerms field holds the translations of the terms that have been discovered.
            //Each value of the dictionary also contains the start and end positions of the source term,
            //so that the translations can be inserted later
            List<PositionAndTranslation> positionAndTranslationOfTerms = new List<PositionAndTranslation>();

            //Define the set of characters used to tokenize the searchString (I call these boundary characters)
            HashSet<Char> setOfTokenBoundaryCharacters = new HashSet<char>();
            
            foreach (char tokenBoundaryChar in tokenBoundaryCharacterString)
            {
                setOfTokenBoundaryCharacters.Add(tokenBoundaryChar);
            }


            //Boolean for holding the result of checking whether character is a boundary character
            bool isTokenBoundaryCharacter;

            //Add the initial path to newTriePathsBeingTraversed at index 0
            int pathIndex = 0;
            newTriePathsBeingTraversed.Add(new RegexPathWithStartCharacterPosition(0, regexTrie));
            pathIndex++;

            //Initialize the current character variable
            char currentChar;

            //Feed the start control character to reach the portion of the trie with the
            //string start relative regexes
            if (regexTrie.characterTransitions.ContainsKey((char)2))
            {
                //The start character is never a part of group, so just select the zero group transition
                newTriePathsBeingTraversed.Add(
                    new RegexPathWithStartCharacterPosition(
                        0, regexTrie.characterTransitions[(char)2][0].destination));
            }

            //Iterate over the inputString.
            for (int charIndex = 0; charIndex < inputString.Length; charIndex++)
            {
                //Remove duplicate paths (with same trieAndGroups) from the paths

                triePathsBeingTraversed = removeDuplicateTrieAndGroups(newTriePathsBeingTraversed);
                newTriePathsBeingTraversed = new List<RegexPathWithStartCharacterPosition>();
                    
                currentChar = inputString[charIndex];

                //Check if char currentChar is a boundary character, or if the boundary characters are not used
                isTokenBoundaryCharacter = setOfTokenBoundaryCharacters.Contains(currentChar) || !useBoundaryCharacters;

                //Iterate over the triePathsBeingTraversed and check if they continue with the current char
                foreach (RegexPathWithStartCharacterPosition triePath in triePathsBeingTraversed)
                {
                    
                    //If the char is a boundary character, check for translation
                    if (isTokenBoundaryCharacter)
                    {
                        if (triePath.trie.matches != null)
                        {
                            addTranslation(triePath, positionAndTranslationOfTerms, charIndex - 1);
                        }
                    }

                    //Check whether path continues with the current char or range transitions,
                    //and add continuing paths
                    checkWhetherPathContinues(triePath, newTriePathsBeingTraversed, currentChar);
                    
                }

                //If char is a boundarycharacter, add a new path
                if (isTokenBoundaryCharacter)
                {
                    //The term will actually begin at the next charIndex (next loop), so add +1 to key
                    newTriePathsBeingTraversed.Add(new RegexPathWithStartCharacterPosition(charIndex + 1, regexTrie));
                }

            }

            //Check if any of the tries has a translation at the last position
            foreach (RegexPathWithStartCharacterPosition triePath in newTriePathsBeingTraversed)
            {
                //File.AppendAllText(@"C:\Users\Anonyymi\Desktop\log.txt", String.Join(",",triePath.trie.matches.Select(x => x.translation).ToArray()));
                //Feed the end control character to reach the portion of the trie with the
                //string end relative regexes
                if (triePath.trie.characterTransitions.ContainsKey((char)3))
                {
                    RegexPathWithStartCharacterPosition endCharConsumed = new RegexPathWithStartCharacterPosition(
                        triePath.startCharacterPosition,
                        triePath.trie.characterTransitions[(char)3][0].destination,
                        triePath.groups);
                    //There's always an translation behind the end character
                    addTranslation(endCharConsumed, positionAndTranslationOfTerms, inputString.Length-1);
                }
                //The end position needs to be the length of the input string - 1 , as spans are start to end character
                if (triePath.trie.matches != null)
                {
                    addTranslation(triePath, positionAndTranslationOfTerms, inputString.Length-1);
                }
            }
            return positionAndTranslationOfTerms;
            
        }

        private static int CompareMatches(PositionAndTranslation x, PositionAndTranslation y)
        {
            if (x.StartCharacterPosition < y.StartCharacterPosition)
            {
                return -1;
            }

            if (x.StartCharacterPosition > y.StartCharacterPosition)
            {
                return 1;
            }

            //x and y start positions are equal
            return 0;
        }

        //Removes overlaps from two match dictionaries
        public List<PositionAndTranslation> RemoveOverLaps
            (List<PositionAndTranslation> allMatches)
        {
            //Check if input list is empty
            if (allMatches.Count == 0)
            {
                return new List<PositionAndTranslation>();
            }

            //This will be the list of matches without overlaps
            List<PositionAndTranslation> matchesWithoutOverlaps = new List<PositionAndTranslation>();
            //Sort the match list by starting position
            allMatches.Sort(CompareMatches);
            
            //30042014: For some reason the largest match is used for translation in fuzzy replacement. DOES IT MATTER?
            //Smaller matches will in any case be removed, but would be nice to know what's happening.
            //ANSWER: It's picked up the largest match from the source segment, now it applies the regex to fuzzy
            //and uses the largest (i.e. only) match as the translation, works as intended. 
            
            //File.AppendAllText(@"C:\Users\Anonyymi\Desktop\log.txt",allMatches.Aggregate("", (log, next) =>
            //    log + String.Format("{0},{1},{2}\r\n", next.StartCharacterPosition, next.EndCharacterPosition, next.Translation)));

            //Initial values for the comparison loop
            int currentIndex = 0;
            int comparisonIndex = 1;

            PositionAndTranslation currentMatch;
            PositionAndTranslation comparisonMatch;

            //The list is sorted, so we can work through it one by one, only comparing
            //each match to the next match in the list
            do
            {
                currentMatch = allMatches[currentIndex];

                //Break the loop when the end of the list has been reached
                try
                {
                    comparisonMatch = allMatches[comparisonIndex];
                }
                catch
                {
                    matchesWithoutOverlaps.Add(currentMatch);
                    break;
                }

                //Check if matches overlap
                if (currentMatch.EndCharacterPosition >= comparisonMatch.StartCharacterPosition)
                {
                    //Discard the shorter match either by changing the comparison match or by
                    //assigning the comparison match as the new current match, and incrementing
                    //the comparison match

                    int currentMatchLength = currentMatch.EndCharacterPosition - currentMatch.StartCharacterPosition;
                    int comparisonMatchLength = comparisonMatch.EndCharacterPosition - comparisonMatch.StartCharacterPosition;

                    if (currentMatchLength > comparisonMatchLength)
                    {
                        comparisonIndex++;
                    }
                    else if (currentMatchLength < comparisonMatchLength)
                    {
                        currentIndex = comparisonIndex;
                        comparisonIndex++;
                    }
                    //If matches are the same length, use the first match
                    else if (currentMatchLength == comparisonMatchLength)
                    {
                        comparisonIndex++;
                    }
                }
                else
                {
                    //add the match to the final list
                    matchesWithoutOverlaps.Add(currentMatch);
                    currentIndex = comparisonIndex;
                    comparisonIndex++;
                }

            } while (true);

            return matchesWithoutOverlaps;
        }

        //Returns a data structure which contains the translations of the glossary terms and their positions in the search string
        public List<PositionAndTranslation> FindMatches
            (Trie glossaryTrie, string inputString, string tokenBoundaryCharacterString, Boolean matchCase, Boolean useBoundaryCharacters)
        {
            string searchString = inputString;

            //If Match case option is false, convert the search string to lower case.
            //The Match case option is also checked when loading the trie, as the
            //trie needs to be loaded in lower case if Match case if false. The loading
            //is done in the static loadTrieFromFile method of
            //TermInjectorTranslationProviderLanguageDirection class
            
            if (!matchCase)
            {
                searchString = searchString.ToLower();
            }

            //triePathsBeingTraversed field holds the paths within the glossaryTrie that are currently being followed
            //Each value of the dictionary also contains the character position where following of the path was started
            Dictionary<int, PathWithStartCharacterPosition> triePathsBeingTraversed = new Dictionary<int, PathWithStartCharacterPosition>();
            
            //positionAndTranslationOfTerms field holds the translations of the terms that have been discovered.
            //Each value of the dictionary also contains the start and end positions of the source term,
            //so that the translations can be inserted later
            List<PositionAndTranslation> positionAndTranslationOfTerms = new List<PositionAndTranslation>();

            //Define the set of characters used to tokenize the searchString (I call these boundary characters)
            HashSet<Char> setOfTokenBoundaryCharacters = new HashSet<char>();
            foreach (char tokenBoundaryChar in tokenBoundaryCharacterString)
            {
                setOfTokenBoundaryCharacters.Add(tokenBoundaryChar);
            }

            //Boolean for holding the result of checking whether character is a boundary character
            bool isTokenBoundaryCharacter = new Boolean();

            //Add the initial path to triePathsBeingTraversed at index 0
            int pathIndex = 0;
            triePathsBeingTraversed.Add(pathIndex, new PathWithStartCharacterPosition(0, glossaryTrie.Clone()));
            pathIndex++;

            //Initialize the index of positionAndTranslationOfTerms dictionary
            int termIndex = 0;

            //Initialize the current character variable
            char currentChar;

            //Iterate over the searchString
            for (int charIndex = 0; charIndex < searchString.Length;charIndex++)
            {
                currentChar = searchString[charIndex];
                //Check if char currentChar is a boundary character
                isTokenBoundaryCharacter = setOfTokenBoundaryCharacters.Contains(currentChar) || !useBoundaryCharacters;
                
                //Iterate over the triePathsBeingTraversed and check if they continue with the current char
                for (int i = 0; i <= pathIndex; i++)
                {

                    //Check whether path exists, if not move to next index
                    if (!triePathsBeingTraversed.ContainsKey(i))
                    {
                        continue;
                    }

                    //If the char is a boundary character and there's a translation at the glossaryTrie node,
                    //add translation to positionAndTranslationOfTerms dictionary with position information

                    if (isTokenBoundaryCharacter && triePathsBeingTraversed[i].Path.GetTranslation() != "")
                    {
                        int termStartCharacter = triePathsBeingTraversed[i].StartCharacterPosition;
                        string termTranslation = triePathsBeingTraversed[i].Path.GetTranslation();
                        string termReplaces = triePathsBeingTraversed[i].Path.GetReplaces();
                        //The end character position is character index minus one, as the current character is a boundary character
                        positionAndTranslationOfTerms.Add(new PositionAndTranslation(
                            termStartCharacter, charIndex - 1, termTranslation, termReplaces,false));
                        termIndex++;
                    }

                    //Check whether path continues with the current char
                    if (triePathsBeingTraversed[i].Path.GetChildNode(currentChar) != null)
                    {
                        //Assign the current char child of the path as a new path
                        triePathsBeingTraversed[i].Path = triePathsBeingTraversed[i].Path.GetChildNode(currentChar).Clone();
                    }
                    //If path does not continue, remove the path
                    else
                    {
                        triePathsBeingTraversed.Remove(i);
                    }

                }
                //If char is a boundarycharacter, add a new path and add the token
                //to token array if the length of the token is >0
                if (isTokenBoundaryCharacter)
                {
                    //The term will actually begin at the next charIndex (next loop), so add +1 to key
                    triePathsBeingTraversed.Add(pathIndex, new PathWithStartCharacterPosition(charIndex+1, glossaryTrie.Clone()));
                    pathIndex += 1;

                }

            }

            //Iterate over the triePathsBeingTraversed one last time, to catch the possible last term
            for (int i = 0; i <= pathIndex; i++)
            {

                //Check whether path exists
                if (!triePathsBeingTraversed.ContainsKey(i))
                {
                    continue;
                }

                if (triePathsBeingTraversed[i].Path.GetTranslation() != "")
                {
                    int termStartCharacter = triePathsBeingTraversed[i].StartCharacterPosition;
                    string termTranslation = triePathsBeingTraversed[i].Path.GetTranslation();
                    string termReplaces = triePathsBeingTraversed[i].Path.GetReplaces();
                    positionAndTranslationOfTerms.Add(new PositionAndTranslation(
                        termStartCharacter, (searchString.Length - 1), termTranslation, termReplaces,false));
                    termIndex++;
                }

            }


            

            //Return the positionAndTranslationOfTerms
            return positionAndTranslationOfTerms;
        }

        //This method replaces the glossary terms in the searchstring (returned by FindMatches) with their translations
        public string InjectMatches(string searchString, List<PositionAndTranslation> positionAndTranslationOfTerms)
        {
            try
            {
                //Build the return string from the input string and the dictionary of terms.
                //This is done with a foreach loop, which goes through each term and appends the unchanged source text
                //between terms and the term translation to the stringbuilder object.

                int spanLength;
                System.Text.StringBuilder newtext = new System.Text.StringBuilder();
                //This is used to track the unchanged text between terms, needs to be incremented by one, so start at -1 to get zero
                int endPositionOfPreviousTerm = -1;
                foreach (var term in positionAndTranslationOfTerms)
                {
                    int startCharacterPositionOfThisTerm = term.StartCharacterPosition;
                    //Append the unchanged text
                    spanLength = (startCharacterPositionOfThisTerm - (endPositionOfPreviousTerm + 1));
                    if (spanLength > 0)
                    {
                        newtext.Append(searchString, endPositionOfPreviousTerm + 1, spanLength);
                    }
                    //Append the term translation
                    newtext.Append(term.Translation);
                    //Update the endPositionOfPreviousTerm variable
                    endPositionOfPreviousTerm = term.EndCharacterPosition;
                }
                //Add the unchanged text after the last term, if such text exists
                if (endPositionOfPreviousTerm != searchString.Length - 1)
                {
                    spanLength = searchString.Length - (endPositionOfPreviousTerm + 1);
                    if (spanLength > 0)
                    {
                        newtext.Append(searchString, endPositionOfPreviousTerm + 1, spanLength);
                    }
                }

                //Return the changed text
                return newtext.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                return "";
            }
        }
    }

    //Data structure for trie processing
    class PathWithStartCharacterPosition
    {
        public Trie Path
        {
            get;
            set;
        }
        public int StartCharacterPosition
        {
            get;
            set;
        }

        public PathWithStartCharacterPosition(int newStartCharacterPosition, Trie newPath)
        {
            this.StartCharacterPosition = newStartCharacterPosition;
            this.Path = newPath;
        }

    }


    //Data structure for trie processing.
    class RegexPathWithStartCharacterPosition
    {
        public RegexTrie<TranslationAndReplacement> trie;
        public Dictionary<int, StringBuilder> groups;
        public int startCharacterPosition;

        public RegexPathWithStartCharacterPosition(int startCharacterPosition, RegexTrie<TranslationAndReplacement> trie)
        {
            //Record the position where following of the regex trie was started
            this.startCharacterPosition = startCharacterPosition;

            //Initialize the list of trieAndGroups objects
            this.trie = trie;

            this.groups = new Dictionary<int, StringBuilder>();

        }

        public RegexPathWithStartCharacterPosition(
            int startCharacterPosition,
            RegexTrie<TranslationAndReplacement> trie,
            Dictionary<int,StringBuilder> groups)
        {
            //Record the position where following of the regex trie was started
            this.startCharacterPosition = startCharacterPosition;

            //Initialize the list of trieAndGroups objects
            this.trie = trie;

            //String builder does not need to be added at this point, as string builders are only added
            //when group number greater than one is encountered in a transition
            this.groups = groups;
        }

    }

    //Data structure for trie processing
    public class PositionAndTranslation
    {

        public Boolean Regex
        {
            get;
            set;
        }

        public string Translation
        {
            get;
            set;
        }

        public string Replaces
        {
            get;
            set;
        }

        public int StartCharacterPosition
        {
            get;
            set;
        }

        public int EndCharacterPosition
        {
            get;
            set;
        }

        public PositionAndTranslation(int newStartCharacterPosition, int newEndCharacterPosition, string newTranslation, string replaces, Boolean isRegex)
        {
            this.StartCharacterPosition = newStartCharacterPosition;
            this.EndCharacterPosition = newEndCharacterPosition;
            this.Translation = newTranslation;
            this.Replaces = replaces;
            this.Regex = isRegex;
        }
    }
}