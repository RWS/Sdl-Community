using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    public class TerminologyProviderExcel:ITerminologyProvider
    {

        //private ExcelTermProviderService _excelTermProviderService;
        private List<Entry> _termEntries;

        private ProviderSettings _providerSettings;
        public const string FixedUri = "excel://terminologyproviderfactoryexcel/";
        public string Name { get; }
        public string Description { get; }
        public string Id { get { return Uri.AbsoluteUri; } }
        public Uri Uri { get; }
        public TerminologyProviderType Type { get { return TerminologyProviderType.Custom; } }

        public bool IsReadOnly => false;

        public bool SearchEnabled { get; }
        public IDefinition Definition => new Definition(new List<IDescriptiveField>(), GetDefinitionLanguages());

        public TerminologyProviderExcel(ProviderSettings providerSettings)
        {
            _providerSettings = providerSettings; 
            var parser = new Parser(_providerSettings);
            var excelTermLoader = new ExcelTermLoaderService(_providerSettings);
            var excelTermProviderService = new ExcelTermProviderService(excelTermLoader, parser);

            _termEntries = excelTermProviderService.LoadEntries();

            Name = "TerminologyTermbaseExcel";
            Description= "TerminologyTermbaseExcel";
            Uri = new Uri(FixedUri);
            SearchEnabled = true;
        }
        public void SetDefault(bool value)
        {
        }

        public IList<ILanguage> GetLanguages()
        {
            return GetDefinitionLanguages().Cast<ILanguage>().ToList();
        }

        public IList<IDefinitionLanguage> GetDefinitionLanguages()
        {
            var result = new List<IDefinitionLanguage>();

            var sourceLanguage = new DefinitionLanguage
            {
                IsBidirectional = true,
                Locale = _providerSettings.SourceLanguage,
                Name = _providerSettings.SourceLanguage.EnglishName,
                TargetOnly = false
            };

            result.Add(sourceLanguage);

            var targetLanguage = new DefinitionLanguage
            {
                IsBidirectional = true,
                Locale = _providerSettings.TargetLanguage,
                Name = _providerSettings.TargetLanguage.EnglishName,
                TargetOnly = false
            };

            result.Add(targetLanguage);
            return result;
        }

        public IEntry GetEntry(int id)
        {
            return _termEntries.FirstOrDefault(termEntry => termEntry.Id == id);
        }

        public IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
        {
            return _termEntries.FirstOrDefault(termEntry => termEntry.Id == id);

        }

        public IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode,
            bool targetRequired)
        {
            var firstTerm = _termEntries.FirstOrDefault();
            var searchResult = new SearchResult
            {
                Id = firstTerm.Id,
                Language = firstTerm.Languages[0],
                Text = "Getting",
                Score = 100
            };
            return new List<ISearchResult> {searchResult};
        }

        public void Dispose()
        {
        }
    }
}
