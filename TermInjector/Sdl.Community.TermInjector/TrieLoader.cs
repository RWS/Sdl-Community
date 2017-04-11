using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Sdl.Community.TermInjector
{
    class TrieLoader
    {
        private RegexTrieFactory<TranslationAndReplacement> regexTrieFactory;
        private Determiniser<TranslationAndReplacement> determiniser;
        private List<KeyValuePair<string, string>> validationErrors;

        public TrieLoader(RegexTrieFactory<TranslationAndReplacement> regexTrieFactory, Determiniser<TranslationAndReplacement> determiniser)
        {
            this.regexTrieFactory = regexTrieFactory;
            this.determiniser = determiniser;
            this.validationErrors = new List<KeyValuePair<string, string>>();
        }

        //This adds a list of three fields to the normal term trie or the fuzzy term trie
        //Returns true if fields were added to normal term trie
        public Boolean addFieldsToTrie(List<string> fields,Trie exactMatchTrieSource, Trie exactMatchTrieReplaces)
        {
            //If first field has content, add fields to normal trie
            if (fields[0].Length > 0)
            {
                //add the pair to the trie
                exactMatchTrieSource.AddToTrie(fields[0], fields[1], fields[2]);
                return true;
            }
            //Otherwise add fields to fuzzy trie
            else
            {
                exactMatchTrieReplaces.AddToTrie(fields[2], fields[1], "");
                return false;
            }
        }

        private Boolean checkValidationErrors(string regex, List<KeyValuePair<string, string>> validationErrors)
        {

            int validationResult = this.regexTrieFactory.validateRegex(regex);

            if (validationResult != 0)
            {

                validationErrors.Add(
                    new KeyValuePair<string, string>(regex,
                        this.regexTrieFactory.errorMessages[validationResult]));

                return true;
            }
            return false;
        }

        //This adds a list of three fields to the normal regex trie or the fuzzy regex trie
        //Returns true if fields were added to normal term trie, false if added to fuzzy trie,
        //and null if validation fails
        public Boolean? addFieldsToRegexTrie(
            List<string> fields,
            RegexTrie<TranslationAndReplacement> regexTrieSource,
            RegexTrie<TranslationAndReplacement> regexTrieReplaces)
        {
         
            //If first field has content, add fields to normal trie
            if (fields[0].Length > 0)
            {
                if (checkValidationErrors(fields[0], this.validationErrors))
                {
                    return null;
                }
                //If there's a third field, also validate that
                if (fields.Count() > 2)
                {
                    if (checkValidationErrors(fields[2], this.validationErrors))
                    {
                        return null;
                    }
                }

                //add the fields to the trie
                this.regexTrieFactory.AddToRegexTrie(
                    regexTrieSource,
                    fields[0],
                    new TranslationAndReplacement(
                        fields[1],
                        fields[2]));
                return true;
            }
            //Otherwise add fields to fuzzy trie
            else
            {
                if (checkValidationErrors(fields[2], this.validationErrors))
                {
                    return null;
                }
                this.regexTrieFactory.AddToRegexTrie(
                    regexTrieReplaces,
                    fields[2],
                    new TranslationAndReplacement(
                        fields[1],
                        ""));
                return false;
            }
        }

        //This loads the regex tries from a file to the two regextries given as parameters
        public void loadRegexTrieFromFile(
            string fileName,
            char delimiter,
            ref RegexTrie<TranslationAndReplacement> regexTrieSource,
            ref RegexTrie<TranslationAndReplacement> regexTrieReplaces)
        {
            //Regextrie is used for visiting source segment, fuzzy regex trie for visiting
            //the fuzzy match target segment
            
            //Check if file exists, exit method and show a message if it doesn't
            if (!File.Exists(fileName))
            {
                //If the file name is not empty, display alert
                if (fileName != "")
                {
                    MessageBox.Show("Regular expression rule file does not exist", "TermInjector");
                }
                return;
            }

            //Regex for converting unicode escape sequences to characters
            Regex rx = new Regex(@"\\[uU]([0-9a-fA-F]{4})");

            //Counter for restricting glossary size
            int stringMemoryUsage = 0;

            //Counters for checking whether terms are being added
            int lineCount = 0;
            int termCount = 0;
            Boolean addedNormalRegexes = false;
            Boolean addedFuzzyRegexes = false;
            using (StreamReader sourceFile = File.OpenText(fileName))
            {

                while (!sourceFile.EndOfStream)
                {
                    //Check if memory usage is within bounds
                    if (stringMemoryUsage > 2500000)
                    {
                        MessageBox.Show("Regular expression rule file loading stopped due to excessive size: Only part of the regular expression rule file has been loaded.", "TermInjector");
                        break;
                    }

                    //Split the line before Unicode conversion (so as not to accidentally add separators)
                    List<string> unicodeEscapedSplitTerm = sourceFile.ReadLine().Split(delimiter).ToList();
                    
                    //Convert the unicode escape sequences in the fields
                    List<string> splitTerm = new List<string>();
                    foreach (var field in unicodeEscapedSplitTerm)
                    {
                        splitTerm.Add(
                                rx.Replace(field, delegate(Match match) { return ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString(); }));
                    }
                    
                    //Check whether the line was valid (at least two fields)
                    if (splitTerm.Count < 2)
                    {
                        lineCount++;
                        continue;
                    }
                    List<string> newTerm = splitTerm.ToList();

                    //If both first and third fields are empty
                    //, skip to next iteration
                    if (newTerm[0].Length == 0 && newTerm[1].Length == 0)
                    {
                        lineCount++;
                        continue;
                    }
                    //If length of list is two, add empty field
                    if (newTerm.Count == 2)
                    {
                        newTerm.Add("");
                    }

                    //Tally the proxy for memory usage, depending on whether source or replaces
                    //field was used as path
                    if (this.addFieldsToRegexTrie(newTerm,regexTrieSource,regexTrieReplaces)==true)
                    {
                        stringMemoryUsage += newTerm[0].Length;
                        addedNormalRegexes = true;
                        termCount++;
                        lineCount += 1;
                    }
                    else if (this.addFieldsToRegexTrie(newTerm, regexTrieSource, regexTrieReplaces) == false)
                    {
                        stringMemoryUsage += newTerm[2].Length;
                        addedFuzzyRegexes = true;
                        termCount++;
                        lineCount += 1;
                    }
                    
                }
                sourceFile.Close();

                //Determinise the regex tries
                if (addedNormalRegexes)
                {
                    //Here's the problem, determiniser breaks the reference: use ref keywords
                    regexTrieSource =
                        this.determiniser.determiniseNFA(
                        regexTrieSource);
                }
                if (addedFuzzyRegexes)
                {
                    regexTrieReplaces =
                        this.determiniser.determiniseNFA(
                        regexTrieReplaces);
                }


                //If the proportion of terms stored and lines read is skewed, the wrong delimiter may have been used.
                //Don't check very small glossaries, as otherwise an empty line or two could trigger the message
                if (lineCount - termCount > (lineCount / 2))
                {
                    string delimiterUsed = "";
                    if (delimiter == '\t')
                    {
                        delimiterUsed = "Tab";
                    }
                    else
                    {
                        delimiterUsed = delimiter.ToString();
                    }
                    MessageBox.Show((string.Format("The amount of regular expression rules stored is small compared to the amount of lines read: {0} lines read, but only {1} regular expression rules found. Are you sure the delimiter character {2} is correct?"
                        , lineCount.ToString(), termCount.ToString(), delimiterUsed)), "TermInjector");
                }
            }

            if (this.validationErrors.Count > 0)
            {
                ValidationErrorForm errorForm = new ValidationErrorForm(this.validationErrors);
            }
            
            this.validationErrors.Clear();
            
        }

        //A static method to load the tries from a file (this needs to be called from WinFormsUI if the settings are updated,
        //so I've made it static). Two tries are built: exactMatchTrieSource, which uses source term as a path, and fuzzyexactMatchTrieSource,
        //which uses target term as path. A line is added to fuzzyexactMatchTrieSource only in case the first field (source) is empty.
        public void loadTrieFromFile(string fileName, bool matchCase, char delimiter, Trie exactMatchTrieSource, Trie exactMatchTrieReplaces)
        {
            //Check if file exists, exit method and show a message if it doesn't
            if (!File.Exists(fileName))
            {
                //If the file name is not empty, display alert
                if (fileName != "")
                {
                    MessageBox.Show("Exact match rule file does not exist", "TermInjector");
                }
                return;
            }

            //Counter for restricting glossary size
            int stringMemoryUsage = 0;

            //Counters for checking whether terms are being added
            int lineCount = 0;
            int termCount = 0;

            using (StreamReader sourceFile = File.OpenText(fileName))
            {

                while (!sourceFile.EndOfStream)
                {
                    //Check if memory usage is within bounds
                    if (stringMemoryUsage > 2500000)
                    {
                        MessageBox.Show("Exact rule file loading stopped due to excessive size: Only part of the exact rule file has been loaded.", "TermInjector");
                        break;
                    }
                    string[] splitTerm = sourceFile.ReadLine().Split(delimiter);
                    //Check whether the line was valid (at least two fields)
                    if (splitTerm.Length < 2)
                    {
                        lineCount++;
                        continue;
                    }
                    List<string> newTerm = splitTerm.ToList();

                    //If both first and third fields are empty
                    //, skip to next iteration
                    if (newTerm[0].Length == 0 && newTerm[1].Length == 0)
                    {
                        lineCount++;
                        continue;
                    }
                    //If length of list is two, add empty field
                    if (newTerm.Count == 2)
                    {
                        newTerm.Add("");
                    }

                    //If case is not matched, convert both source and replaces fields to lower case
                    if (!matchCase)
                    {
                        newTerm[0] = newTerm[0].ToLower();
                        newTerm[2] = newTerm[2].ToLower();
                    }

                    //Tally the proxy for memory usage, depending on whether source or replaces
                    //field was used as path
                    if (this.addFieldsToTrie(newTerm,exactMatchTrieSource,exactMatchTrieReplaces))
                    {
                        stringMemoryUsage += newTerm[0].Length;
                        termCount++;
                    }
                    else
                    {
                        stringMemoryUsage += newTerm[2].Length;
                        termCount++;
                    }

                    lineCount += 1;
                }
                sourceFile.Close();

                //If the proportion of terms stored and lines read is skewed, the wrong delimiter may have been used.
                //Don't check very small glossaries, as otherwise an empty line or two could trigger the message
                if (lineCount - termCount > (lineCount / 2))
                {
                    string delimiterUsed = "";
                    if (delimiter == '\t')
                    {
                        delimiterUsed = "Tab";
                    }
                    else
                    {
                        delimiterUsed = delimiter.ToString();
                    }
                    MessageBox.Show((string.Format("The amount of exact match rules stored is small compared to the amount of lines read: {0} lines read, but only {1} exact match rules found. Are you sure the delimiter character {2} is correct?"
                        , lineCount.ToString(), termCount.ToString(), delimiterUsed)), "TermInjector");
                }
            }
        }

        
    }
}
