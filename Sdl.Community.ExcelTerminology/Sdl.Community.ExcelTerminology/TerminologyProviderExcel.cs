using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Sdl.Community.ExcelTerminology.Insights;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    public class TerminologyProviderExcel: AbstractTerminologyProvider
    {
        public const string ExcelUriTemplate = "excelglossary://";

        private List<ExcelEntry> _termEntries;

        private readonly ProviderSettings _providerSettings;

        public ProviderSettings ProviderSettings => _providerSettings;

        private readonly ITermSearchService _termSearchService;

        public List<ExcelEntry> Terms => _termEntries;

        public event Action<List<ExcelEntry>> TermsLoaded;
        public override string Name =>
            Path.GetFileName(_providerSettings.TermFilePath);
        public override string Description => 
            PluginResources.ExcelTerminologyProviderDescription;

        public override Uri Uri =>
            new Uri((ExcelUriTemplate +
                     Path.GetFileName(_providerSettings.TermFilePath))
                .RemoveUriForbiddenCharacters());
                

        public override IDefinition Definition => 
            new Definition(GetDescriptiveFields(), GetDefinitionLanguages());

        public TerminologyProviderExcel(ProviderSettings providerSettings, ITermSearchService termSearchService)
        {
            if (providerSettings == null) throw new ArgumentNullException(nameof(providerSettings));
            if (termSearchService == null) throw new ArgumentNullException(nameof(termSearchService));


            _providerSettings = providerSettings; 
  
            _termSearchService = termSearchService;

            _termEntries = new List<ExcelEntry>();
        }

        public TerminologyProviderExcel(ProviderSettings providerSettings)
        {
            _providerSettings = providerSettings;
        }
        public async Task LoadEntries()
        {
            try
            {
                var parser = new Parser(_providerSettings);
                var transformerService = new EntryTransformerService(parser);
                var excelTermLoader = new ExcelTermLoaderService(_providerSettings);
                var excelTermProviderService = new ExcelTermProviderService(excelTermLoader, transformerService);

                _termEntries = await excelTermProviderService.LoadEntries();
              
                TermsLoaded?.Invoke(_termEntries);
            }
            catch (Exception ex)
            {
              
                throw ex;
            }
        }

        public override IList<ILanguage> GetLanguages()
        {
            return GetDefinitionLanguages().Cast<ILanguage>().ToList();
        }

        public IList<IDescriptiveField> GetDescriptiveFields()
        {
            var result = new List<IDescriptiveField>();

            var approvedField = new DescriptiveField
            {
                Label = "Approved",
                Level = FieldLevel.TermLevel,
                Mandatory = false,
                Multiple = true,
                PickListValues = new List<string> {"Approved", "Not Approved"},
                Type = FieldType.String
            };
            result.Add(approvedField);

            return result;
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

        public override IEntry GetEntry(int id)
        {
            return _termEntries.FirstOrDefault(termEntry => termEntry.Id == id);
        }

        public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
        {
            return _termEntries.FirstOrDefault(termEntry => termEntry.Id == id);

        }

        public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination,
            int maxResultsCount, SearchMode mode,
            bool targetRequired)
        {

            var results = new List<ISearchResult>();
            try
            {
                results.AddRange(
                    _termSearchService
                        .Search(text, _termEntries, maxResultsCount));
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            return results;
        }

    }

}
