using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Community.TermInjector;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RegexTrie;

namespace Sdl.Community.TermInjector
{
    public class TermInjectorTranslationProvider : ITranslationProvider
    {
        ///<summary>
        /// This string needs to be a unique value.
        /// This is the string that precedes the plug-in URI.
        ///</summary>    
        public static readonly string TermInjectorTranslationProviderScheme = "terminjector";
        
        //private static int instances = 0;
        //public int instancenumber;

        private Trie exactMatchTrieSource;
        private Trie exactMatchTrieReplaces;
        private RegexTrie<TranslationAndReplacement> regexTrieSource;
        private RegexTrie<TranslationAndReplacement> regexTrieReplaces;
        private FileBasedTranslationMemory fileTM;

        private TermInjectorTranslationProviderElementTermReplacementVisitor noMatchVisitor;
        private TermInjectorTranslationProviderElementTermExtractionVisitor fuzzyCurrentVisitor;
        private TermInjectorTranslationProviderElementTermExtractionVisitor fuzzyMatchVisitor;
        private TermInjectorTranslationProviderElementTermReplacementVisitor fuzzyReplaceVisitor;

        private RegexTrieFactory<TranslationAndReplacement> regexTrieFactory;
        private Determiniser<TranslationAndReplacement> determiniser;
        private TrieLoader trieLoader;        

        #region "TermInjectorTranslationOptions"
        public TermInjectorTranslationOptions Options
        {
            get;
            set;
        }

        private char delimiterToChar(string delimiter)
        {
            if (delimiter == "Tab")
            {
                return '\t';
            }
            else
            {
                return Convert.ToChar(delimiter);
            }
        }

        public void loadTries()
        {
            //Reset the node counter
            RegexTrie<TranslationAndReplacement>.counter = 0;
            //Load tries
            char delimiter = delimiterToChar(this.Options.Delimiter);
            //Load the exact match tries
            this.exactMatchTrieSource = new Trie();
            this.exactMatchTrieReplaces = new Trie();
            this.trieLoader.loadTrieFromFile(
                this.Options.GlossaryFileName,
                this.Options.MatchCase == "true" ? true : false,
                delimiter,
                this.exactMatchTrieSource,
                this.exactMatchTrieReplaces);
            
            //Load regex tries
            this.regexTrieSource = new RegexTrie<TranslationAndReplacement>();
            this.regexTrieReplaces = new RegexTrie<TranslationAndReplacement>();
            //Pass the tries by ref, as determinisation needs to return a new trie
            this.trieLoader.loadRegexTrieFromFile(
                this.Options.RegexFileName,
                delimiter,
                ref this.regexTrieSource,
                ref this.regexTrieReplaces);
        }

        /*public void checkExistenceOfFiles()
        {
            if (this.Options.RegexFileName !="" && !File.Exists(this.Options.RegexFileName) ||
                this.Options.TMFileName != "" && !File.Exists(this.Options.TMFileName) ||
                this.Options.GlossaryFileName != "" && !File.Exists(this.Options.GlossaryFileName))
            {
                MessageBox.Show("TermInjector settings refer to files that do not exist. Check that the glossary files (if defined) and the TM have correct paths.");
                TermInjectorTranslationProviderConfDialog dialog = new TermInjectorTranslationProviderConfDialog(this.Options, this);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Options = dialog.Options;
                }
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    throw new Exception();
                }
            }

        }
        */
        

        public TermInjectorTranslationProvider(TermInjectorTranslationOptions options)
        {
            this.Options = options;
            
            //Initialize regex building objects
            this.regexTrieFactory = new RegexTrieFactory<TranslationAndReplacement>();

            this.determiniser = new Determiniser<TranslationAndReplacement>();
            this.trieLoader = new TrieLoader(regexTrieFactory,determiniser);

            //Check that the files specified in the options exist
            //this.checkExistenceOfFiles();

            //Load tries and TM
            try
            {
                this.loadTries();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.loadTM();
            
            //Initialize the segment visitors
            this.initializeVisitors();
        }
        #endregion

        public void initializeVisitors()
        {
            //This replaces terms in no match segments
            this.noMatchVisitor = new TermInjectorTranslationProviderElementTermReplacementVisitor(
                this.Options,
                this.exactMatchTrieSource,
                this.regexTrieSource);
            //This collects terms from current segment source
            this.fuzzyCurrentVisitor = new TermInjectorTranslationProviderElementTermExtractionVisitor(
                this.Options,
                this.exactMatchTrieSource,
                this.regexTrieSource);
            //This collects terms from translation suggestion source
            this.fuzzyMatchVisitor = new TermInjectorTranslationProviderElementTermExtractionVisitor(
                this.Options,
                this.exactMatchTrieSource,
                this.regexTrieSource);
            //This replaces terms in fuzzy match segments
            this.fuzzyReplaceVisitor = new TermInjectorTranslationProviderElementTermReplacementVisitor(
                this.Options,
                this.exactMatchTrieReplaces,
                this.regexTrieReplaces);
        }

        #region "ITranslationProvider Members"
        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new TermInjectorTranslationProviderLanguageDirection(this, languageDirection);
        }

        public void loadTM()
        {
            fileTM = new FileBasedTranslationMemory(this.Options.TMFileName);
        }

        public FileBasedTranslationMemory FileTM
        {
            get { return this.fileTM; }
        }

        public Determiniser<TranslationAndReplacement> Determiniser
        {
            get { return this.determiniser; }
        }

        public RegexTrieFactory<TranslationAndReplacement> RegexTrieFactory
        {
            get { return this.regexTrieFactory; }
        }

        public Trie ExactMatchTrieSource
        {
            get { return this.exactMatchTrieSource; }
        }

        public Trie ExactMatchTrieReplaces
        {
            get { return this.exactMatchTrieReplaces; }
        }

        public RegexTrie<TranslationAndReplacement> RegexTrieSource
        {
            get { return this.regexTrieSource; }
        }

        public RegexTrie<TranslationAndReplacement> RegexTrieReplaces
        {
            get { return this.regexTrieReplaces; }
        }

        public TermInjectorTranslationProviderElementTermReplacementVisitor NoMatchVisitor
        {
            get { return this.noMatchVisitor; }
        }
        public TermInjectorTranslationProviderElementTermExtractionVisitor FuzzyCurrentVisitor
        {
            get { return this.fuzzyCurrentVisitor; }
        }
        public TermInjectorTranslationProviderElementTermExtractionVisitor FuzzyMatchVisitor
        {
            get { return this.fuzzyMatchVisitor; }
        }
        public TermInjectorTranslationProviderElementTermReplacementVisitor FuzzyReplaceVisitor
        {
            get { return this.fuzzyReplaceVisitor; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void LoadState(string translationProviderState)
        {
        }

        public string Name
        {
            get { return PluginResources.Plugin_NiceName; }
        }

        public void RefreshStatusInfo()
        {

        }

        public string SerializeState()
        {
            // Save settings
            return null;
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return new ProviderStatusInfo(true, PluginResources.Plugin_NiceName); }
        }

        #region "SupportsConcordanceSearch"
        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }
        #endregion

        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        public bool SupportsFilters
        {
            get { return true; }
        }

        #region "SupportsFuzzySearch"
        public bool SupportsFuzzySearch
        {
            get { return true; }
        }
        #endregion

        #region "SupportsLanguageDirection"
        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return true;
        }
        #endregion


        #region "SupportsMultipleResults"
        public bool SupportsMultipleResults
        {
            get { return true; }
        }
        #endregion

        #region "SupportsPenalties"
        public bool SupportsPenalties
        {
            get { return true; }
        }
        #endregion

        public bool SupportsPlaceables
        {
            get { return true; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        #region "SupportsSearchForTranslationUnits"
        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }
        #endregion

        #region "SupportsSourceTargetConcordanceSearch"
        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return true; }
        }
        #endregion

        public bool SupportsStructureContext
        {
            get { return true; }
        }

        #region "SupportsTaggedInput"
        public bool SupportsTaggedInput
        {
            get { return true; }
        }
        #endregion


        public bool SupportsTranslation
        {
            get { return true; }
        }

        #region "SupportsUpdate"
        public bool SupportsUpdate
        {
            get { return true; }
        }
        #endregion

        public bool SupportsWordCounts
        {
            get { return true; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return TermInjectorTranslationOptions.ProviderTranslationMethod; }
        }

        #region "Uri"
        public Uri Uri
        {
            get { return this.Options.Uri; }
        }
        #endregion

        #endregion

        public void addStringToTries(string ruleString)
        {
            if (ruleString.Contains(this.Options.TermAdditionSeparator))
            {
                string[] splitTerm = ruleString.Split(this.Options.TermAdditionSeparator.ToCharArray());

                List<string> newTerm = splitTerm.ToList();
                //If there's only two fields, add an empty field
                if (newTerm.Count == 2)
                {
                    newTerm.Add("");
                }

                //Check that either first or the third field of the new term are non-empty
                if ((newTerm[0].Length > 0) || (newTerm[2].Length > 0))
                {
                    char fileDelimiter = delimiterToChar(this.Options.Delimiter);
                    //Regex for converting unicode escape sequences to characters
                    Regex rx = new Regex(@"\[uU]([0-9a-fA-F]{4})");

                    //Check whether this is a regex term or a normal term
                    if (ruleString[0] == 'r' && ruleString[1] == '\\')
                    {
                        //Remove the regex marker
                        newTerm[0] = newTerm[0].Substring(2);

                        //Convert the unicode escape sequences in the new term
                        List<string> unicodeParsedNewTerm = new List<string>();
                        foreach (string field in newTerm)
                        {
                            unicodeParsedNewTerm.Add(
                                rx.Replace(field, delegate (Match match) { return ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString(); }));
                        }

                        //Validate source and replaces fields
                        foreach (var num in new List<int> { 0, 2 })
                        {
                            try
                            {
                                int validationResult = this.regexTrieFactory.validateRegex(unicodeParsedNewTerm[num]);
                                if (validationResult != 0)
                                {
                                    List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
                                    results.Add(
                                        new KeyValuePair<string, string>(unicodeParsedNewTerm[num],
                                            this.regexTrieFactory.errorMessages[validationResult]));
                                    ValidationErrorForm errorForm = new ValidationErrorForm(results);
                                    return;
                                }
                            }
                            catch
                            {
                                //The field does not exist, no need to validate
                            }
                        }

                        if (File.Exists(this.Options.RegexFileName))
                        {
                            TextWriter tw = new StreamWriter(this.Options.RegexFileName, true);
                            tw.WriteLine();
                            tw.Write(newTerm[0] + fileDelimiter + newTerm[1] + fileDelimiter + newTerm[2]);
                            tw.Close();
                        }
                        else if (this.Options.RegexFileName != "" || this.Options.RegexFileName == null)
                        {
                            MessageBox.Show("Regular expression rule file does not exist", "TermInjector");
                            this.Options.RegexFileName = "";
                        }


                        if (this.trieLoader.addFieldsToRegexTrie(unicodeParsedNewTerm, this.regexTrieSource, this.regexTrieReplaces) == true)
                        {
                            this.regexTrieSource =
                                this.determiniser.determiniseNFA(
                                this.regexTrieSource);
                        }
                        else if (this.trieLoader.addFieldsToRegexTrie(unicodeParsedNewTerm, this.regexTrieSource, this.regexTrieReplaces) == false)
                        {
                            this.regexTrieReplaces =
                                this.determiniser.determiniseNFA(
                                this.regexTrieReplaces);
                        }

                    }
                    else
                    {
                        if (File.Exists(this.Options.GlossaryFileName))
                        {
                            TextWriter tw = new StreamWriter(this.Options.GlossaryFileName, true);
                            tw.WriteLine();
                            tw.Write(newTerm[0] + fileDelimiter + newTerm[1] + fileDelimiter + newTerm[2]);
                            tw.Close();
                        }
                        else if (this.Options.GlossaryFileName != "" || this.Options.GlossaryFileName == null)
                        {
                            MessageBox.Show("Exact match rule file does not exist", "TermInjector");
                        }
                        if (!matchCaseToBool(this.Options.MatchCase))
                        {
                            newTerm[0] = newTerm[0].ToLower();
                            newTerm[2] = newTerm[2].ToLower();
                        }

                        //Convert the unicode escape sequences in the new term
                        List<string> unicodeParsedNewTerm = new List<string>();
                        foreach (string field in newTerm)
                        {
                            unicodeParsedNewTerm.Add(rx.Replace(field, delegate (Match match) { return ((char)Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString(); }));
                        }

                        //Add term to normal or fuzzy trie
                        this.trieLoader.addFieldsToTrie(unicodeParsedNewTerm, this.exactMatchTrieSource, this.exactMatchTrieReplaces);
                    }
                }

                //Update the possible new tries to visitors
                initializeVisitors();
            }
            return;
        }

        private Boolean matchCaseToBool(string matchCase)
        {
            return matchCase == "true" ? true : false;
        }
    }
}
