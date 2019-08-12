using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelTerminology.Model;
using ExcelTerminology.Services;
using ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace ExcelTerminology
{
	public class TerminologyProviderExcel : AbstractTerminologyProvider
	{
		private readonly ITermSearchService _termSearchService;

		public const string ExcelUriTemplate = "excelglossary://";

        public ProviderSettings ProviderSettings { get; }
        public List<ExcelEntry> Terms { get; private set; }

		public override bool IsReadOnly => false;
		public override string Name => Path.GetFileName(ProviderSettings.TermFilePath);
		public override string Description => PluginResources.ExcelTerminologyProviderDescription;
		public override Uri Uri => new Uri((ExcelUriTemplate + Path.GetFileName(ProviderSettings.TermFilePath)).RemoveUriForbiddenCharacters());
		public override IDefinition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());
		
		public event Action<List<ExcelEntry>> TermsLoaded;

		public TerminologyProviderExcel(ProviderSettings providerSettings, ITermSearchService termSearchService)
		{
			ProviderSettings = providerSettings ?? throw new ArgumentNullException(nameof(providerSettings));
			Terms = new List<ExcelEntry>();
			_termSearchService = termSearchService ?? throw new ArgumentNullException(nameof(termSearchService));
		}

		public TerminologyProviderExcel(ProviderSettings providerSettings)
		{
			ProviderSettings = providerSettings;
		}

		public async Task LoadEntries()
		{
			try
			{
				var parser = new Parser(ProviderSettings);
				var transformerService = new EntryTransformerService(parser);
				var excelTermLoader = new ExcelTermLoaderService(ProviderSettings);
				var excelTermProviderService = new ExcelTermProviderService(excelTermLoader, transformerService);

				Terms = await excelTermProviderService.LoadEntries();

				TermsLoaded?.Invoke(Terms);
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
				PickListValues = new List<string> { "Approved", "Not Approved" },
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
				Locale = ProviderSettings.SourceLanguage,
				Name = ProviderSettings.SourceLanguage.EnglishName,

				TargetOnly = false
			};

			result.Add(sourceLanguage);

			var targetLanguage = new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = ProviderSettings.TargetLanguage,
				Name = ProviderSettings.TargetLanguage.EnglishName,
				TargetOnly = false
			};

			result.Add(targetLanguage);
			return result;
		}

		public override IEntry GetEntry(int id)
		{
			return Terms.FirstOrDefault(termEntry => termEntry.Id == id);
		}

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return Terms.FirstOrDefault(termEntry => termEntry.Id == id);
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
						.Search(text, Terms, maxResultsCount));
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return results;
		}
	}
}