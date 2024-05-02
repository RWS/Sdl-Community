using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services;
using Sdl.Community.TermExcelerator.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator
{
	public class TerminologyProviderExcel : ITerminologyProvider
	{
		private readonly ITermSearchService _termSearchService;

		public const string ExcelUriTemplate = "excelglossary://";

        public ProviderSettings ProviderSettings { get; set; }
        public List<ExcelEntry> Terms { get; private set; }
		public static readonly Log Log = Log.Instance;

		public TerminologyProviderType Type => TerminologyProviderType.Custom;
		public bool IsReadOnly => false;
		public bool SearchEnabled => true;

		public IList<FilterDefinition> GetFilters()
		{
			return new List<FilterDefinition>();
		}

		public bool Uninitialize()
		{
			IsInitialized = false;
			return true;
		}

		public string Name => $"{PluginResources.Plugin_Name}: {Uri.Host}";
		public string Description => PluginResources.ExcelTerminologyProviderDescription;
		public string Id { get; } = Guid.NewGuid().ToString();
		public Uri Uri => new Uri((ExcelUriTemplate + Path.GetFileName(ProviderSettings.TermFilePath)).RemoveUriForbiddenCharacters());
		public Definition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());
		public FilterDefinition ActiveFilter { get; set; }
		public bool IsInitialized { get; set; }

		public event Action<List<ExcelEntry>> TermsLoaded;

		public void Dispose()
		{
			Terms.Clear();
		}

		public TerminologyProviderExcel(ProviderSettings providerSettings, ITermSearchService termSearchService) : this(providerSettings)
		{
			Terms = new List<ExcelEntry>();
			_termSearchService = termSearchService ?? throw new ArgumentNullException(nameof(termSearchService));
		}

		public TerminologyProviderExcel(ProviderSettings providerSettings)
		{
			ProviderSettings = providerSettings;
			ApplicationContext.SettingsChangedFromTellMeAction -= ApplicationContext_SettingsChangedFromTellMeAction;
			ApplicationContext.SettingsChangedFromTellMeAction += ApplicationContext_SettingsChangedFromTellMeAction;

		}

		private void ApplicationContext_SettingsChangedFromTellMeAction()
		{
			ProviderSettings = ApplicationContext.PersistenceService.Load(Uri);
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
				Log.Logger.Error($"LoadEntries method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		public void SetDefault(bool value)
		{
		}

		public IList<ILanguage> GetLanguages()
		{
			return GetDefinitionLanguages().Cast<ILanguage>().ToList();
		}

		public IList<DescriptiveField> GetDescriptiveFields()
		{
			var result = new List<DescriptiveField>();

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
		public IList<DefinitionLanguage> GetDefinitionLanguages()
		{
			var result = new List<DefinitionLanguage>();
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

		public Entry GetEntry(int id)
		{
			return Terms?.FirstOrDefault(termEntry => termEntry.Id == id);
		}

		public Entry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return Terms?.FirstOrDefault(termEntry => termEntry.Id == id);
		}

		public IList<SearchResult> Search(string text, ILanguage source, ILanguage destination,
			int maxResultsCount, SearchMode mode,
			bool targetRequired)
		{
			var results = new List<SearchResult>();
			try
			{
				results.AddRange(_termSearchService.Search(text, Terms, maxResultsCount));
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadEntries Search: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return results.ToList();
		}

		public bool Initialize()
		{
			IsInitialized = true;
			return true;
		}

		public bool Initialize(TerminologyProviderCredential credential)
		{
			return Initialize();
		}

		public bool IsProviderUpToDate()
		{
			return true;
		}
	}
}