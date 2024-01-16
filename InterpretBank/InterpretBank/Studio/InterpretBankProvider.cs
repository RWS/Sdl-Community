using InterpretBank.SettingsService;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InterpretBank.Studio
{
    public class InterpretBankProvider(ITerminologyService termSearchService) : ITerminologyProvider
    {
        private Settings _settings;

        public event Action ProviderSettingsChanged;

        public FilterDefinition ActiveFilter { get; set; }

        public Definition Definition =>
            //TODO: take name of these fields from Settings
            new(
                new[]
                {
                    new DescriptiveField { Label = "Extra1", Level = FieldLevel.TermLevel, Type = FieldType.String },
                    new DescriptiveField { Label = "Extra2", Level = FieldLevel.TermLevel, Type = FieldType.String }
                }, GetLanguages().Cast<DefinitionLanguage>());

        public string Description => PluginResources.Plugin_Description;

        public string Id  => "Interpret Bank";

        public bool IsInitialized => true;

        public bool IsReadOnly => false;

        public string Name => "Interpret Bank";

        public bool SearchEnabled => true;

        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                if (_settings.Tags?.Count > 0)
                    _settings.Glossaries.AddRange(TermSearchService.GetTaggedGlossaries(_settings.Tags));
            }
        }

        public ITerminologyService TermSearchService { get; } = termSearchService;
        public TerminologyProviderType Type => TerminologyProviderType.Custom;
        public Uri Uri => new($"{Constants.InterpretBankUri}/{Settings.SettingsId}.json://");
        private HashSet<Entry> Entries { get; } = new();
        private int TermIndex { get; set; }

        public void Dispose() => TermSearchService.Dispose();

        public Entry GetEntry(int id) => Entries.FirstOrDefault(e => e.Id == id);

        public Entry GetEntry(int id, IEnumerable<ILanguage> languages)
        {
            throw new NotImplementedException();
        }

        public IList<FilterDefinition> GetFilters()
        {
            var filterDefinitions = new List<FilterDefinition>();

            return filterDefinitions;
        }

        public IList<ILanguage> GetLanguages()
        {
            if (Settings.Glossaries == null || !Settings.Glossaries.Any()) return new List<ILanguage>();

            var languages = new List<ILanguage>();

            var cultures = LanguageRegistryApi.Instance.CultureMetadataManager.GetLanguagesAsync().Result.ToList();
            foreach (var glossary in Settings.Glossaries)
            {
                var glossaryLanguages = TermSearchService.GetGlossaryLanguages(glossary);
                glossaryLanguages.RemoveAll(l => l == null);

                languages.AddRange(glossaryLanguages.Select(lang => new DefinitionLanguage
                {
                    IsBidirectional = true,
                    Locale = LanguageRegistryApi
                        .Instance
                        .GetLanguage(cultures.FirstOrDefault(cult => cult.EnglishName == lang.Name)?.LanguageCode)
                        .CultureInfo,
                    Name = lang.Name,
                    TargetOnly = false
                }));
            }

            return languages
                .GroupBy(l => l.Name)
                .Select(gr => gr.First())
                .ToList();
        }

        public bool Initialize()
        {
            return true;
        }

        public bool Initialize(TerminologyProviderCredential credential)
        {
            return true;
        }

        public bool IsProviderUpToDate()
        {
            return true;
        }

        public void RaiseProviderSettingsChanged() => ProviderSettingsChanged?.Invoke();

        public IList<SearchResult> Search(string text, ILanguage source, ILanguage destination,
            int maxResultsCount, SearchMode mode, bool targetRequired)
        {
            var words = Regex.Split(text, "\\s+");

            List<SearchResult> results = null;
            Entries.Clear();
            foreach (var word in words)
            {
                results = mode switch
                {
                    SearchMode.Fuzzy => GetFuzzyTerms(source, destination, word),
                    SearchMode.Normal => GetExactTerms(source, destination, word),
                    SearchMode.FullText => throw new NotImplementedException()
                };
            }

            return results;
        }

        public void SetDefault(bool value)
        {
            
        }

        public void Setup(Settings settings)
        {
            TermSearchService.Setup(settings.DatabaseFilepath);
            Settings = settings;
        }

        public bool Uninitialize()
        {
            throw new NotImplementedException();
        }

        //TODO: simplify this; creates confusion
        /// <summary>
        /// Works differently for Fuzzy and Exact searches.
        /// When the search is exact, we always add the same search text for each of the found terms, since there's only one (search text).
        /// When the search is fuzzy, the search was done against the word itself and other similar words. In this case we add a search text for each of them.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="results"></param>
        /// <param name="terms"></param>
        /// <param name="score"></param>
        private void AddResultToList(ILanguage destination, List<SearchResult> results, List<StudioTermEntry> terms, int score)
        {
            if (!terms.Any()) return;
            var id = GetIndex();
            results.Add(new SearchResult { Id = (int)terms[0].Id, Score = score, Text = terms[0].SearchText });
            Entries.Add(CreateEntry((int)terms[0].Id, terms, destination.Name));
        }

        private Entry CreateEntry(int id, List<StudioTermEntry> targetTerms, string targetLanguage)
        {
            var entryTargetLanguage = new EntryLanguage { Name = targetLanguage };
            foreach (var targetTerm in targetTerms)
            {
                var entryTerm = new EntryTerm { Value = targetTerm.Text };

                var entryField1 = new EntryField { Name = "Extra1", Value = targetTerm.Extra1 };
                var entryField2 = new EntryField { Name = "Extra2", Value = targetTerm.Extra2 };

                entryTerm.Fields.Add(entryField1);
                entryTerm.Fields.Add(entryField2);

                entryTargetLanguage.Terms.Add(entryTerm);
            }

            var entry = new Entry { Id = id };
            entry.Languages.Add(entryTargetLanguage);

            return entry;
        }

        private List<SearchResult> GetExactTerms(ILanguage source, ILanguage destination, string word)
        {
            var terms = TermSearchService.GetExactTerms(word, source.Name, destination.Name, Settings.Glossaries);

            var results = new List<SearchResult>();
            AddResultToList(destination, results, terms, 100);

            return results;
        }

        private List<SearchResult> GetFuzzyTerms(ILanguage source, ILanguage destination, string word)
        {
            var terms = TermSearchService.GetFuzzyTerms(word, source.Name, destination.Name, Settings.Glossaries);

            var results = new List<SearchResult>();
            foreach (var term in terms)
            {
                AddResultToList(destination, results, new List<StudioTermEntry> { term }, term.Score);
            }

            return results;
        }

        private int GetIndex() => TermIndex++;
    }
}