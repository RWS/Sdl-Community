using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleTerminology
{
    internal class MyTerminologyProvider : ITerminologyProvider
    {
        public readonly string _fileName;
        private List<Entry> _entry = new List<Entry>();

        public Definition Definition
        {
            get
            {
                return new Definition(GetDescriptiveFields(), GetLanguages().Cast<DefinitionLanguage>());
            }
        }

        public string Description => PluginResources.My_Terminology_Provider_Description;

        public string Name => PluginResources.My_Terminology_Provider_Name;

        public Uri Uri => new Uri(this._fileName);

        public string Id => "id";

        public TerminologyProviderType Type => TerminologyProviderType.Custom;

        public bool IsReadOnly => false;

        public bool SearchEnabled => true;

        public FilterDefinition ActiveFilter 
        { 
            get => null;
            set => value = new FilterDefinition(); 
        }

        public bool IsInitialized => true;

        public MyTerminologyProvider(string providerSettings)
        {
            _fileName = providerSettings;
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Entry GetEntry(int id)
        {
            return _entry.FirstOrDefault(_entry => _entry.Id == id);
        }

        public Entry GetEntry(int id, IEnumerable<ILanguage> languages)
        {
            return _entry.FirstOrDefault(_entry => _entry.Id == id);
        }

        public IList<FilterDefinition> GetFilters() => new List<FilterDefinition>();    

        public IList<ILanguage> GetLanguages()
        {
            StreamReader _inFile = new StreamReader(_fileName.Replace("file:///", ""));
            string[] languages = _inFile.ReadLine().Split(';');
            string srgLanguage = languages[0], trgLanguage = languages[1];
            string srcLabel = srgLanguage.Split(',')[0], srcLocale = srgLanguage.Split(',')[1];
            string trgLabel = trgLanguage.Split(',')[0], trgLocale = trgLanguage.Split(',')[1];
            _inFile.Close();

            var result = new List<DefinitionLanguage>();

            var sourceLanguage = new Language(srcLocale);
            var tbSrcLanguage = new DefinitionLanguage
            {
                IsBidirectional = true,
                Locale = sourceLanguage.CultureInfo,
                Name = sourceLanguage.DisplayName,
                TargetOnly = false
            };

            var targetLanguage = new Language(trgLocale);
            var tbTrgLanguage = new DefinitionLanguage
            {
                IsBidirectional = true,
                Locale = targetLanguage.CultureInfo,
                Name = targetLanguage.DisplayName,
                TargetOnly = false
            };


            result.Add(tbSrcLanguage);
            result.Add(tbTrgLanguage);

            return result.Cast<ILanguage>().ToList();
        }

        public bool Initialize() => true;

        public bool Initialize(TerminologyProviderCredential credential) => true;

        public bool IsProviderUpToDate() => true;

        public IList<SearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
        {
            string[] chunks;
            List<string> hits = new List<string>();

            // open the glossary text file
            using (StreamReader glossary = new StreamReader(_fileName.Replace("file:///", "")))
            {
                // skip the first line, as it contains only the language settings
                glossary.ReadLine();

                while (!glossary.EndOfStream)
                {
                    string thisLine = glossary.ReadLine();
                    if (thisLine.Trim() == "")
                        continue;
                    chunks = thisLine.Split(';');
                    string sourceTerm = chunks[1].ToLower();

                    // normal search (triggered from the Termbase Search window)
                    if (mode.ToString() == "Normal" && sourceTerm.StartsWith(text.ToLower()))
                        hits.Add(thisLine);

                    // fuzzy search (corresponds to the Terminology Eecognition)
                    if (mode.ToString() == "Fuzzy" && text.ToLower().Contains(sourceTerm))
                        hits.Add(thisLine);
                }
            }

            // Create search results object (hitlist)
            var results = new List<SearchResult>();

            for (int i = 0; i < hits.Count; i++)
            {
                chunks = hits[i].Split(';');
                // We create the search result object based on the source term
                // found in the glossary, we assign the id, which associates the search
                // result to the correspoinding entry, and we assume that the search score 
                // is always 100%.
                SearchResult result = new SearchResult
                {
                    Text = chunks[1], // source term
                    Score = 100,
                    Id = Convert.ToInt32(chunks[0]) // entry id
                };

                // Construct the entry object for the current search result
                _entry.Add(CreateEntry(chunks[0], chunks[1], chunks[2], chunks[3], destination.Name));

                results.Add(result);
            }

            return results;
        }

        public void SetDefault(bool value)
        {
        }

        public bool Uninitialize()
        {
            throw new NotImplementedException();
        }

        public IList<DescriptiveField> GetDescriptiveFields()
        {
            var result = new List<DescriptiveField>();

            var definitionField = new DescriptiveField
            {
                Label = "Definition",
                Level = FieldLevel.EntryLevel,
                Type = FieldType.String
            };
            result.Add(definitionField);

            return result;
        }

        private Entry CreateEntry(string id, string sourceTerm, string targetTerm, string definitionText, string targetLanguage)
        {
            // Assign the entry id
            Entry thisEntry = new Entry
            {
                Id = Convert.ToInt32(id)
            };

            // Add the target language
            EntryLanguage trgLanguage = new EntryLanguage
            {
                Name = targetLanguage
            };

            // Create the target term
            EntryTerm _term = new EntryTerm
            {
                Value = targetTerm
            };
            trgLanguage.Terms.Add(_term);
            thisEntry.Languages.Add(trgLanguage);

            // Also add the definition (if available)
            if (definitionText != "")
            {
                EntryField _definition = new EntryField
                {
                    Name = "Definition",
                    Value = definitionText
                };
                thisEntry.Fields.Add(_definition);
            }

            return thisEntry;
        }

        private Entry CreateEntry(SearchResult searchResult, ILanguage sourceLanguage, IList<ILanguage> languages)
        {

            throw new NotImplementedException();
        }
    }
}
