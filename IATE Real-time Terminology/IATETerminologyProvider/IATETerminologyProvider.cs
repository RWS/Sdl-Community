using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.IATETerminologyProvider.EventArgs;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.LanguagePlatform.Core;

using Sdl.TranslationStudioAutomation.IntegrationApi;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Definition;
using TradosStudio.API.TranslationResources.Terminology.Entries;
using TradosStudio.API.TranslationResources.Terminology.Search;

namespace Sdl.Community.IATETerminologyProvider
{
	public class IATETerminologyProvider : ITerminologyProvider
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private IList<EntryModel> _entryModels;
		private TermSearchService _searchService;
		private EditorController _editorController;
		private IStudioDocument _activeDocument;

		public event EventHandler<TermEntriesChangedEventArgs> TermEntriesChanged;

		public SettingsModel ProviderSettings { get; set; }

		public IATETerminologyProvider(SettingsModel providerSettings, 
			IConnectionProvider connectionProvider,
			IInventoriesProvider inventoriesProvider, 
			ICacheProvider cacheProvider)
		{
			ProviderSettings = providerSettings;
			_connectionProvider = connectionProvider;
			_inventoriesProvider = inventoriesProvider;
			_cacheProvider = cacheProvider;
			Id = Guid.NewGuid().ToString();
		}

		public bool Initialize()
		{
			if (IsInitialized)
			{
				return true;
			}

			if (!_inventoriesProvider.IsInitialized)
			{
				_ = Core.Globalization.LegacyAsyncHelpers.WrapAsyncCode(_inventoriesProvider.Initialize);
			}

			_entryModels = new List<EntryModel>();
			_searchService = new TermSearchService(_connectionProvider, _inventoriesProvider);

			InitializeEditorController();
			ActivateDocument(_editorController.ActiveDocument);
			IsInitialized = true;
			return true;
		}

		private IConnectionProvider _connectionProvider { get; }

		private IInventoriesProvider _inventoriesProvider { get; }

		private ICacheProvider _cacheProvider { get; }

		public Definition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());

		public string Description => PluginResources.IATETerminologyProviderDescription;

		public string Name => PluginResources.IATETerminologyProviderName;

		public Uri Uri => new Uri(Constants.IATEUriTemplate);

		public string Id { get; }


		public bool IsReadOnly => false;

		public bool SearchEnabled => true;

		public FilterDefinition ActiveFilter { get; set; }

		public bool IsInitialized { get; private set; }

		public void SetDefault(bool value)
		{
			//Nothing to do here
		}

		

		public bool IsProviderUpToDate()
		{
			return true;
		}

		public IList<FilterDefinition> GetFilters()
		{
			var filterDefinitions = new List<FilterDefinition>();

			return filterDefinitions;
		}

		public bool Uninitialize()
		{
			IsInitialized = false;
			return true;
		}

		public Entry GetEntry(int id)
		{
			var entry = _entryModels.ToList().FirstOrDefault(e => e.Id == id);
			return entry;
		}

		public Entry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return _entryModels.ToList().FirstOrDefault(e => e.Id == id);
		}

		public IList<ILanguage> GetLanguages()
		{
			return GetDefinitionLanguages().Cast<ILanguage>().ToList();
		}

		public IList<SearchResult> Search(string text, ILanguage source, ILanguage target, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			// Prevent the empty query
			if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)) return null;
			if (text == "\" \"" || text == "") return null;
			// Limit to EU languages
			if (!EUProvider.IsEULanguages(source, target))
			{
				return null;
			}

			//_logger.Info("--> Try searching for segment");

			var jsonBody = GetApiRequestBodyValues(source, target, text);
			var queryString = JsonConvert.SerializeObject(jsonBody);
			var canConnect = _cacheProvider?.Connect(IATEApplication.ProjectsController?.CurrentProject);

			if (canConnect != null && (bool)canConnect)
			{
				//_logger.Info("--> Try to get cache results");

				var cachedResults = _cacheProvider.GetCachedResults(text, CultureInfo.GetCultureInfo(target.LanguageIsoCode).Name, queryString);
				if (cachedResults != null && cachedResults.Count > 0)
				{
					var entryModels = CreateEntryTerms(cachedResults.ToList(), source, GetLanguages());

					//_logger.Info("--> Cache results found");
					OnTermEntriesChanged(entryModels, text, source, target);
					UpdateEntryModelsList(text, entryModels);

					return cachedResults?.Cast<SearchResult>().ToList();
				}
			}

			var config = IATEApplication.ProjectsController?.CurrentProject?.GetTermbaseConfiguration();
			var results = _searchService.GetTerms(queryString, config?.TermRecognitionOptions?.SearchDepth ?? 20);
			if (results != null)
			{
				var termGroups = SortSearchResultsByPriority(text, GetTermResultGroups(results), source);

				results = RemoveDuplicateTerms(termGroups, source, target);
				results = MaxSearchResults(results, maxResultsCount);


				// add search to cache db
				var searchCache = new SearchCache
				{
					SourceText = text,
					TargetLanguage = CultureInfo.GetCultureInfo(target.LanguageIsoCode).Name,
					QueryString = queryString
				};

				if (_cacheProvider != null)
				{
					//_logger.Info("--> Try to add results in db");
					_cacheProvider.AddSearchResults(searchCache, results);
				}

				var entryModels = CreateEntryTerms(results, source, GetLanguages());
				OnTermEntriesChanged(entryModels, text, source, target);
				UpdateEntryModelsList(text, entryModels);
			}

			return results?.Cast<SearchResult>().ToList();
		}

		public IList<DescriptiveField> GetDescriptiveFields()
		{
			var result = new List<DescriptiveField>
			{ 
				new DescriptiveField
				{
					Label = "Definition",
					Level = FieldLevel.EntryLevel,
					Mandatory = true,
					Multiple = true,
					Type = FieldType.String
				},
				new DescriptiveField
				{
					Label = "Domain",
					Level = FieldLevel.EntryLevel,
					Mandatory = true,
					Multiple = true,
					Type = FieldType.String
				},new DescriptiveField
				{
					Label = "Subdomain",
					Level = FieldLevel.LanguageLevel,
					Mandatory = true,
					Multiple = true,
					Type = FieldType.String
				},new DescriptiveField
				{
					Label = "Status",
					Level = FieldLevel.TermLevel,
					Mandatory = false,
					Multiple = true,
					Type = FieldType.PickList,
					PickListValues = new[] { "Deprecated", "Obsolete", "Admitted", "Preferred", "Proposed" }
				}

			};

			return result;
		}

		public IList<DefinitionLanguage> GetDefinitionLanguages()
		{
			var result = new List<DefinitionLanguage>();

			var currentProject = IATEApplication.ProjectsController?.CurrentProject;
			if (currentProject == null)
			{
				return result;
			}

			var projectInfo = currentProject.GetProjectInfo();

			var sourceLanguage = new DefinitionLanguage
			{
				IsBidirectional = true,
				LanguageIsoCode = projectInfo.SourceLanguage.CultureInfo.Name,
				Name = projectInfo.SourceLanguage.DisplayName,
				TargetOnly = false
			};
			result.Add(sourceLanguage);

			result.AddRange(projectInfo.TargetLanguages.Select(language => new DefinitionLanguage
			{
				IsBidirectional = true,
				LanguageIsoCode = language.CultureInfo.Name,
				Name = language.DisplayName,
				TargetOnly = false
			}));

			return result;
		}

		public string GetStatusName(int value)
		{
			switch (value)
			{
				case 0: return "Deprecated";
				case 1: return "Obsolete";
				case 2: return "Admitted";
				case 3: return "Preferred";
				case 4: return "Proposed";
				default: return ""; //
			}
		}

		private void UpdateEntryModelsList(string text, IEnumerable<EntryModel> entryModels)
		{
			foreach (var entryModel in entryModels)
			{
				var existingEntryModel = _entryModels.FirstOrDefault(a => a.Id == entryModel.Id && a.SearchText == text);
				if (existingEntryModel == null)
				{
					_entryModels.Add(entryModel);
				}
			}
		}

		private object GetApiRequestBodyValues(ILanguage source, ILanguage destination, string text)
		{
			var targetLanguages = new List<string>();
			var filteredDomains = new List<string>();
			var filteredTermTypes = new List<int>();
			var filteredCollections = new List<string>();
			var filteredInstitutions = new List<string>();

			targetLanguages.Add(CultureInfo.GetCultureInfo(destination.LanguageIsoCode).TwoLetterISOLanguageName);
			var primarities = new List<int>();
			var sourceReliabilities = new List<int>();
			var targetReliabilities = new List<int>();
			var searchInSubdomains = false;
			if (ProviderSettings != null)
			{
				var domains = ProviderSettings.Domains.Where(d => d.IsSelected).Select(d => d.Code).ToList();
				filteredDomains.AddRange(domains);

				var termTypes = ProviderSettings.TermTypes.Where(t => t.IsSelected).Select(t => t.Code).ToList();
				filteredTermTypes.AddRange(termTypes);

				var collections = ProviderSettings.Collections.Select(c => c.Code).ToList();
				filteredCollections.AddRange(collections);

				var institutions = ProviderSettings.Institutions.Select(i => i.Code).ToList();
				filteredInstitutions.AddRange(institutions);

				primarities = ProviderSettings.Primarities.GetPrimarities();

				sourceReliabilities = ProviderSettings.SourceReliabilities.GetReliabilityCodes();
				targetReliabilities = ProviderSettings.TargetReliabilities.GetReliabilityCodes();

				searchInSubdomains = ProviderSettings.SearchInSubdomains;
			}

			dynamic bodyModel = new ExpandoObject();

			bodyModel.query = text;
			bodyModel.source = CultureInfo.GetCultureInfo(source.LanguageIsoCode).TwoLetterISOLanguageName;
			bodyModel.targets = targetLanguages;
			bodyModel.cascade_domains = searchInSubdomains;
			bodyModel.query_operator = 18;
			bodyModel.search_in_fields = 0;

			if (filteredDomains.Count > 0)
				bodyModel.filter_by_domains = filteredDomains;

			if (filteredTermTypes.Count > 0)
				bodyModel.search_in_term_types = filteredTermTypes;

			if (filteredCollections.Count > 0)
				bodyModel.filter_by_entry_collection = filteredCollections;

			if (filteredInstitutions.Count > 0)
				bodyModel.filter_by_entry_institution_owner = filteredInstitutions;

			if (primarities.Count > 0)
				bodyModel.filter_by_entry_primarity = primarities;

			if (sourceReliabilities.Count > 0)
				bodyModel.filter_by_source_term_reliability = sourceReliabilities;

			if (targetReliabilities.Count > 0)
				bodyModel.filter_by_target_term_reliability = targetReliabilities;

			return bodyModel;
		}

		private void ClearEntries()
		{
			_entryModels ??= new List<EntryModel>();
			_entryModels.Clear();
		}

		private void OnTermEntriesChanged(IList<EntryModel> entryModels, string text, ILanguage source, ILanguage target)
		{
			if (IsActiveSegmentText(text, source))
			{
				OnTermEntriesChanged(new TermEntriesChangedEventArgs
				{
					EntryModels = entryModels,
					SourceLanguage = source,
					TargetLanguage = target
				});
			}
		}

		protected virtual void OnTermEntriesChanged(TermEntriesChangedEventArgs e)
		{
			TermEntriesChanged?.Invoke(this, e);
		}

		private bool IsActiveSegmentText(string text, ILanguage source)
		{
			if (text == null || source == null)
			{
				return false;
			}

			var selectedSegmentPair = _editorController?.ActiveDocument?.GetActiveSegmentPair();
			if (selectedSegmentPair?.Source == null)
			{
				return true;
			}

			var regex = new Regex(@"[\s\t]+", RegexOptions.None);
			var searchText = regex.Replace(text, string.Empty);

			var segment = new Segment(CultureInfo.GetCultureInfo(source.LanguageIsoCode));
			var segmentVisitor = new SegmentVisitor(segment, true);
			segmentVisitor.VisitSegment(selectedSegmentPair.Source);
			var sourceText = regex.Replace(segmentVisitor.Segment.ToPlain(), string.Empty);

			return string.Compare(searchText, sourceText, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		private void InitializeEditorController()
		{
			if (_editorController != null)
			{
				return;
			}

			_editorController = SdlTradosStudio.Application?.GetController<EditorController>();

			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
				_editorController.Opened += EditorControllerOnOpened;
			}
		}

		private void EditorControllerOnOpened(object sender, DocumentEventArgs e)
		{
			Application.DoEvents();
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			ActivateDocument(e.Document);
		}

		private void ActivateDocument(IStudioDocument document)
		{
			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			}

			_activeDocument = document;

			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
			}

			ClearEntries();
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, System.EventArgs e)
		{
			ClearEntries();
		}

		private IList<EntryModel> CreateEntryTerms(IReadOnlyCollection<SearchResultModel> searchResults, ILanguage sourceLanguage, IList<ILanguage> languages)
		{
			var entryModels = new List<EntryModel>();
			var searchResultsByLanguage = searchResults.Where(s =>
				CultureInfo.GetCultureInfo(s.Language.LanguageIsoCode).TwoLetterISOLanguageName.Equals(CultureInfo.GetCultureInfo(sourceLanguage.LanguageIsoCode).TwoLetterISOLanguageName));

			foreach (var searchResultByLanguage in searchResultsByLanguage)
			{
				var searchResult = searchResultByLanguage;
				var entryModel = new EntryModel
				{
					SearchText = searchResult.Text,
					Id = searchResult.Id,
					ItemId = searchResult.ItemId,
					Fields = SetEntryFields(searchResult, 0),
					Transactions = new List<EntryTransaction>(),
					Languages = SetEntryLanguages(searchResults, sourceLanguage, languages, searchResult)
				};

				entryModels.Add(entryModel);
			}

			return entryModels;
		}

		private IList<EntryLanguage> SetEntryLanguages(IReadOnlyCollection<SearchResultModel> searchResults, ILanguage sourceLanguage, IEnumerable<ILanguage> languages, SearchResultModel termResult)
		{
			var entryLanguages = new List<EntryLanguage>();
            var sourceCultureInfo = CultureInfo.GetCultureInfo(sourceLanguage.LanguageIsoCode);

            foreach (var language in languages)
			{
                var languageCultureInfo = CultureInfo.GetCultureInfo(language.LanguageIsoCode);

                var entryLanguage = new EntryLanguageModel
				{
					Fields = !languageCultureInfo.TwoLetterISOLanguageName.Equals(sourceCultureInfo.TwoLetterISOLanguageName) ? SetEntryFields(termResult, 1) : new List<EntryField>(),
					LanguageIsoCode = languageCultureInfo.Name,
					Name = language.Name,
					ParentEntry = null,
					Terms = CreateEntryTerms(searchResults, languageCultureInfo, termResult.Id),
					IsSource = languageCultureInfo.TwoLetterISOLanguageName.Equals(sourceCultureInfo.TwoLetterISOLanguageName)
				};
				entryLanguages.Add(entryLanguage);
			}

			return entryLanguages;
		}

		private IList<EntryTerm> CreateEntryTerms(IEnumerable<SearchResultModel> searchResults, CultureInfo cultureInfo, int id)
		{
			IList<EntryTerm> entryTerms = new List<EntryTerm>();
			var searchResultsByLanguage = searchResults.Where(t =>
				t.Id == id &&
                CultureInfo.GetCultureInfo(t.Language.LanguageIsoCode).TwoLetterISOLanguageName == cultureInfo.TwoLetterISOLanguageName).ToList();

			foreach (var searchResultByLanguage in searchResultsByLanguage)
			{
				var searchResult = searchResultByLanguage;
				var entryTerm = new EntryTerm
				{
					Value = searchResult.Text,
					Fields = SetEntryFields(searchResult, 2)
				};

				entryTerms.Add(entryTerm);
			}

			return entryTerms;
		}

		private IList<EntryField> SetEntryFields(SearchResultModel searchResultModel, int level)
		{
			var entryFields = new List<EntryField>();

			if (!string.IsNullOrEmpty(searchResultModel.Definition))
			{
				var definitionEntryField = new EntryField
				{
					Name = "Definition",
					Value = searchResultModel.Definition
				};
				entryFields.Add(definitionEntryField);
			}

			if (!string.IsNullOrEmpty(searchResultModel.Domain))
			{
				var domainEntryField = new EntryField
				{
					Name = "Domain",
					Value = searchResultModel.Domain
				};
				entryFields.Add(domainEntryField);
			}

			if (!string.IsNullOrEmpty(searchResultModel.Subdomain))
			{
				var domainEntryField = new EntryField
				{
					Name = "Subdomain",
					Value = searchResultModel.Subdomain
				};
				entryFields.Add(domainEntryField);
			}

			if (level == 2) // term
			{
				if (!string.IsNullOrEmpty(searchResultModel.TermType))
				{
					var termTypeEntryField = new EntryField
					{
						Name = "Type",
						Value = searchResultModel.TermType
					};
					entryFields.Add(termTypeEntryField);
				}

				if (searchResultModel.Evaluation > -1)
				{
					var evaluationEntryField = new EntryField
					{
						Name = "Status",
						Value = GetStatusName(searchResultModel.Evaluation)
					};
					entryFields.Add(evaluationEntryField);
				}
			}

			return entryFields;
		}

		private static List<SearchResultModel> MaxSearchResults(List<SearchResultModel> searchResults, int maxResultsCount)
		{
			var results = new List<SearchResultModel>();
			if (searchResults.Count > maxResultsCount)
			{
				for (var i = 0; i < maxResultsCount; i++)
				{
					results.Add(searchResults[i]);
				}
			}
			else
			{
				results = searchResults;
			}

			return results;
		}

		private static List<TermResultGroup> SortSearchResultsByPriority(string text, List<TermResultGroup> termResultGroups, ILanguage source)
		{
			var index = new List<int>();
			var secondaryIndex = new List<int>();

			foreach (var termResultGroup in termResultGroups)
			{
				var sourceTerms = termResultGroup.Results
					.Where(a => CultureInfo.GetCultureInfo(a.Language.LanguageIsoCode).TwoLetterISOLanguageName == CultureInfo.GetCultureInfo(source.LanguageIsoCode).TwoLetterISOLanguageName).ToList();

				foreach (var sourceTerm in sourceTerms)
				{
					if (text.IndexOf(sourceTerm.Text, StringComparison.InvariantCultureIgnoreCase) > -1)
					{
						if (!index.Contains(sourceTerm.Id))
						{
							index.Add(sourceTerm.Id);
							if (secondaryIndex.Contains(sourceTerm.Id))
							{
								secondaryIndex.Remove(sourceTerm.Id);
							}
						}
					}
					else
					{
						if (!secondaryIndex.Contains(sourceTerm.Id) && !index.Contains(sourceTerm.Id))
						{
							secondaryIndex.Add(sourceTerm.Id);
						}
					}
				}
			}

			index.AddRange(secondaryIndex);

			termResultGroups = termResultGroups.OrderBy(a => index.IndexOf(a.Id)).ToList();
			return termResultGroups;
		}

		private static List<SearchResultModel> RemoveDuplicateTerms(IReadOnlyCollection<TermResultGroup> termResultGroups, ILanguage source, ILanguage target)
		{
			var results = new List<SearchResultModel>();

			var indexes = new List<string>();
			foreach (var termGroup in termResultGroups)
			{
				var sourceTerms = termGroup.Results.Where(a => CultureInfo.GetCultureInfo(a.Language.LanguageIsoCode).TwoLetterISOLanguageName == CultureInfo.GetCultureInfo(source.LanguageIsoCode).TwoLetterISOLanguageName).ToList();
				var targetTerms = termGroup.Results.Where(a => CultureInfo.GetCultureInfo(a.Language.LanguageIsoCode).TwoLetterISOLanguageName == CultureInfo.GetCultureInfo(target.LanguageIsoCode).TwoLetterISOLanguageName).ToList();

				foreach (var sourceTerm in sourceTerms)
				{
					for (var j = targetTerms.Count - 1; j >= 0; j--)
					{
						var index = $"Source: {sourceTerm.Text};Target: {targetTerms[j].Text}";
						if (!indexes.Contains(index))
						{
							indexes.Add(index);
						}
						else
						{
							targetTerms.RemoveAt(j);
						}
					}
				}

				if (targetTerms.Count == 0)
				{
					termGroup.Results.Clear();
				}
			}

			foreach (var resultGroup in termResultGroups)
			{
				if (resultGroup.Results.Count > 0)
				{
					results.AddRange(resultGroup.Results);
				}
			}

			return results;
		}

		private static List<TermResultGroup> GetTermResultGroups(IEnumerable<SearchResultModel> searchResults)
		{
			var termResultGroups = new List<TermResultGroup>();
			foreach (var searchResult in searchResults)
			{
				var resultGroup = termResultGroups.FirstOrDefault(a => a.Id == searchResult.Id);
				if (resultGroup != null)
				{
					resultGroup.Results.Add(searchResult);
				}
				else
				{
					termResultGroups.Add(new TermResultGroup
					{
						Id = searchResult.Id,
						Results = new List<SearchResultModel> { searchResult }
					});
				}
			}

			return termResultGroups;
		}

		public void Dispose()
		{
			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
				_editorController.Opened -= EditorControllerOnOpened;
			}

			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			}
		}
    }
}