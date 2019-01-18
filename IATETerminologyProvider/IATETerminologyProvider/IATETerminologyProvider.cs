using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IATETerminologyProvider.EventArgs;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Service;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace IATETerminologyProvider
{
	public class IATETerminologyProvider : AbstractTerminologyProvider
	{
		private IList<EntryModel> _entryModels;
		private ProviderSettings _providerSettings;
		private TermSearchService _searchService;
		private EditorController _editorController;		

		public event EventHandler<TermEntriesChangedEventArgs> TermEntriesChanged;

		public IATETerminologyProvider(ProviderSettings providerSettings)
		{
			UpdateSettings(providerSettings);
		}

		public const string IateUriTemplate = Constants.IATEUriTemplate;

		public override IDefinition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());

		public override string Description => PluginResources.IATETerminologyProviderDescription;

		public override string Name => PluginResources.IATETerminologyProviderName;

		public override Uri Uri => new Uri((IateUriTemplate + "https://iate.europa.eu/em-api/entries/_search").RemoveUriForbiddenCharacters());

		public override IEntry GetEntry(int id)
		{
			return _entryModels.FirstOrDefault(e => e.Id == id);
		}

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return _entryModels.FirstOrDefault(e => e.Id == id);
		}

		public override IList<ILanguage> GetLanguages()
		{
			return GetDefinitionLanguages().Cast<ILanguage>().ToList();
		}

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage target, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			_entryModels.Clear();

			var results = _searchService.GetTerms(text, source, target, maxResultsCount);
			if (results != null)
			{
				var termGroups = SortSearchResultsByPriority(text, GetTermResultGroups(results), source);

				results = RemoveDuplicateTerms(termGroups, source, target);
				results = MaxSearchResults(results, maxResultsCount);
				CreateEntryTerms(results, source, GetLanguages());
			}

			if (IsActiveSegmentText(text, source))
			{
				OnTermEntriesChanged(new TermEntriesChangedEventArgs
				{
					EntryModels = _entryModels,
					SourceLanguage = new Language(source.Locale.Name)
				});
			}

			return results;
		}

		public void UpdateSettings(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
			_entryModels = new List<EntryModel>();
			_searchService = new TermSearchService(_providerSettings);

			InitializeEditorController();
		}

		public IList<IDescriptiveField> GetDescriptiveFields()
		{
			var result = new List<IDescriptiveField>();

			var definitionField = new DescriptiveField
			{
				Label = "Definition",
				Level = FieldLevel.EntryLevel,
				Mandatory = true,
				Multiple = true,
				Type = FieldType.String
			};
			result.Add(definitionField);

			var domainField = new DescriptiveField
			{
				Label = "Domain",
				Level = FieldLevel.EntryLevel,
				Mandatory = true,
				Multiple = true,
				Type = FieldType.String
			};
			result.Add(domainField);

			var subdomainField = new DescriptiveField
			{
				Label = "Subdomain",
				Level = FieldLevel.LanguageLevel,
				Mandatory = true,
				Multiple = true,
				Type = FieldType.String
			};
			result.Add(subdomainField);

			return result;
		}

		public IList<IDefinitionLanguage> GetDefinitionLanguages()
		{
			var result = new List<IDefinitionLanguage>();
			var currentProject = GetProjectController().CurrentProject;
			var projTargetLanguage = currentProject.GetTargetLanguageFiles()[0].Language;
			var projSourceLanguage = currentProject.GetSourceLanguageFiles()[0].Language;

			var sourceLanguage = new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = projSourceLanguage.CultureInfo,
				Name = projSourceLanguage.DisplayName,
				TargetOnly = false
			};

			result.Add(sourceLanguage);

			var targetLanguage = new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = projTargetLanguage.CultureInfo,
				Name = projTargetLanguage.DisplayName,
				TargetOnly = false
			};

			result.Add(targetLanguage);
			return result;
		}

		public ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		public EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		public string GetStatusName(int value)
		{
			switch (value)
			{
				case 0: return "Deprecated";
				case 1: return "Obsolete";
				case 2: return ""; // TODO: confirm value
				case 3: return "Preferred";
				default: return ""; // TODO: confirm default value
			}
		}

		public override void Dispose()
		{
			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			}

			base.Dispose();
		}

		protected virtual void OnTermEntriesChanged(TermEntriesChangedEventArgs e)
		{
			TermEntriesChanged?.Invoke(this, e);
		}

		private bool IsActiveSegmentText(string text, ILanguage source)
		{
			var selectedSegmentPair = _editorController?.ActiveDocument?.GetActiveSegmentPair();
			if (selectedSegmentPair?.Source == null)
			{
				return true;
			}

			var regex = new Regex(@"[\s\t]+", RegexOptions.None);
			var searchText = regex.Replace(text, string.Empty);

			var segment = new Segment(source.Locale);
			var segmentVisitor = new SegmentVisitor(segment, true);
			segmentVisitor.VisitSegment(selectedSegmentPair.Source);
			var sourceText = regex.Replace(segmentVisitor.Segment.ToPlain(), string.Empty);

			return string.Compare(searchText, sourceText, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		private void InitializeEditorController()
		{
			if (_editorController == null)
			{
				_editorController = GetEditorController();
				if (_editorController != null)
				{
					_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
				}
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			if (e.Document == null)
			{
				OnTermEntriesChanged(null);
			}
		}

		private void CreateEntryTerms(IReadOnlyCollection<ISearchResult> termsResult, ILanguage sourceLanguage, IList<ILanguage> languages)
		{
			_entryModels.Clear();

			foreach (var searchResult in termsResult.Where(s => s.Language.Locale.TwoLetterISOLanguageName.Equals(sourceLanguage.Locale.TwoLetterISOLanguageName)))
			{
				var termResult = (SearchResultModel)searchResult;

				var entryModel = new EntryModel
				{
					SearchText = termResult.Text,
					Id = termResult.Id,
					ItemId = termResult.ItemId,
					Fields = SetEntryFields(termResult, 0),
					Transactions = new List<IEntryTransaction>(),
					Languages = SetEntryLanguages(termsResult, sourceLanguage, languages, termResult)
				};

				_entryModels.Add(entryModel);
			}
		}

		private IList<IEntryLanguage> SetEntryLanguages(IReadOnlyCollection<ISearchResult> termsResult, ILanguage sourceLanguage, IEnumerable<ILanguage> languages, SearchResultModel termResult)
		{
			var entryLanguages = new List<IEntryLanguage>();
			foreach (var language in languages)
			{
				var entryLanguage = new EntryLanguageModel
				{
					Fields = !language.Locale.TwoLetterISOLanguageName.Equals(sourceLanguage.Locale.TwoLetterISOLanguageName) ? SetEntryFields(termResult, 1) : new List<IEntryField>(),
					Locale = language.Locale,
					Name = language.Name,
					ParentEntry = null,
					Terms = CreateEntryTerms(termsResult, language, termResult.Id),
					IsSource = language.Locale.TwoLetterISOLanguageName.Equals(sourceLanguage.Locale.TwoLetterISOLanguageName)
				};
				entryLanguages.Add(entryLanguage);
			}

			return entryLanguages;
		}

		private IList<IEntryTerm> CreateEntryTerms(IEnumerable<ISearchResult> termsResult, ILanguage language, int id)
		{
			IList<IEntryTerm> entryTerms = new List<IEntryTerm>();
			var terms = termsResult.Where(t => t.Id == id && t.Language.Locale.TwoLetterISOLanguageName == language.Locale.TwoLetterISOLanguageName).ToList();

			foreach (var searchResult in terms)
			{
				var term = (SearchResultModel)searchResult;

				var entryTerm = new EntryTerm
				{
					Value = term.Text,
					Fields = SetEntryFields(term, 2)
				};

				entryTerms.Add(entryTerm);
			}

			return entryTerms;
		}

		private IList<IEntryField> SetEntryFields(SearchResultModel searchResultModel, int level)
		{
			var entryFields = new List<IEntryField>();

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

		private static List<ISearchResult> MaxSearchResults(List<ISearchResult> searchResults, int maxResultsCount)
		{
			var results = new List<ISearchResult>();
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

		private static List<TermResultGroup> SortSearchResultsByPriority(string text, List<TermResultGroup> termsResult, ILanguage source)
		{
			var index = new List<int>();
			var secondaryIndex = new List<int>();

			foreach (var termResult in termsResult)
			{
				var sourceTerms = termResult.Results.Where(a => a.Language.Locale.TwoLetterISOLanguageName == source.Locale.TwoLetterISOLanguageName).ToList();

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

			termsResult = termsResult.OrderBy(a => index.IndexOf(a.Id)).ToList();
			return termsResult;
		}

		private static List<ISearchResult> RemoveDuplicateTerms(IReadOnlyCollection<TermResultGroup> termGroups, ILanguage source, ILanguage target)
		{
			var results = new List<ISearchResult>();

			var indexes = new List<string>();
			foreach (var termGroup in termGroups)
			{
				var sourceTerms = termGroup.Results.Where(a => a.Language.Locale.TwoLetterISOLanguageName == source.Locale.TwoLetterISOLanguageName).ToList();
				var targetTerms = termGroup.Results.Where(a => a.Language.Locale.TwoLetterISOLanguageName == target.Locale.TwoLetterISOLanguageName).ToList();

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

			foreach (var resultGroup in termGroups)
			{
				if (resultGroup.Results.Count > 0)
				{
					results.AddRange(resultGroup.Results);
				}
			}

			return results;
		}

		private static List<TermResultGroup> GetTermResultGroups(IEnumerable<ISearchResult> termsResult)
		{
			var resultGroups = new List<TermResultGroup>();
			foreach (var searchResult in termsResult)
			{
				var resultGroup = resultGroups.FirstOrDefault(a => a.Id == searchResult.Id);
				if (resultGroup != null)
				{
					resultGroup.Results.Add(searchResult);
				}
				else
				{
					resultGroups.Add(new TermResultGroup { Id = searchResult.Id, Results = new List<ISearchResult> { searchResult } });
				}
			}

			return resultGroups;
		}
	}
}