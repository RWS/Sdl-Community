using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Service;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace IATETerminologyProvider
{
	public class IATETerminologyProvider : AbstractTerminologyProvider
	{
		#region Private Fields
		private ProviderSettings _providerSettings;
		private IList<ISearchResult> _termsResult = new List<ISearchResult>();
		private IList<EntryModel> _entryModels = new List<EntryModel>();
		#endregion

		#region Constructors
		public IATETerminologyProvider(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
		#endregion

		#region Public Properties
		public const string IATEUriTemplate = Constants.IATEUriTemplate;
		public override IDefinition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());
		public override string Description => PluginResources.IATETerminologyProviderDescription;
		public override string Name => PluginResources.IATETerminologyProviderName;
		public override Uri Uri => new Uri((IATEUriTemplate + "https://iate.europa.eu/em-api/entries/_search").RemoveUriForbiddenCharacters());
		#endregion

		#region Public Methods
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

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			var searchService = new TermSearchService(_providerSettings);
			var textResults = text.Split(' ').ToList();
			_termsResult.Clear();

			//search terms for each word in text (active segment)
			Parallel.ForEach(textResults, (textResult) =>
			{
				var termResults = searchService.GetTerms(textResult, source, destination, maxResultsCount);
				((List<ISearchResult>)_termsResult).AddRange(termResults);
			});

			if (_termsResult.Count > 0)
			{
				CreateEntryTerms(source, destination);				
			}
			return _termsResult;
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
		#endregion

		#region Private Methods
		/// <summary>
		/// Create entry models (used to return the text in the Termbase Search panel)
		/// </summary>
		/// <param name="sourceLanguage">source language</param>
		/// <param name="targetLanguage">target language</param>
		private void CreateEntryTerms(ILanguage sourceLanguage, ILanguage targetLanguage)
		{
			var languages = GetLanguages();
			_entryModels.Clear();

			foreach (SearchResultModel termResult in _termsResult.Where(s=>s.Language.Name.Equals(sourceLanguage.Locale.Parent.DisplayName)))
			{
				var entryModel = new EntryModel
				{
					SearchText = termResult.Text,
					Id = termResult.Id,
					Fields = SetEntryFields(termResult),
					Transactions = new List<IEntryTransaction>(),
					Languages = SetEntryLanguages(languages, sourceLanguage, termResult)
				};
				_entryModels.Add(entryModel);
			}
		}

		/// <summary>
		/// Set entry languages for the entry models
		/// </summary>
		/// <param name="languages">source and target languages</param>
		/// <param name="sourceLanguage">source language</param>
		/// <param name="termResult">term result</param>
		/// <returns>entryLanguages</returns>
		private IList<IEntryLanguage> SetEntryLanguages(IList<ILanguage> languages, ILanguage sourceLanguage, SearchResultModel termResult)
		{
			IList<IEntryLanguage> entryLanguages = new List<IEntryLanguage>();
			foreach (var language in languages)
			{
				var entryLanguage = new EntryLanguageModel
				{
					Fields = !language.Name.Equals(sourceLanguage.Name) ? SetEntryFields(termResult) : new List<IEntryField>(),
					Locale = language.Locale,
					Name = language.Name,
					ParentEntry = null,
					Terms = CreateEntryTerms(language, sourceLanguage, termResult.Id),
					IsSource = language.Name.Equals(sourceLanguage.Name) ? true : false
				};
				entryLanguages.Add(entryLanguage);
			}
			return entryLanguages;
		}

		/// <summary>
		/// Create Entry terms for the entry languages
		/// </summary>
		/// <param name="language">document language</param>
		/// <param name="sourceLanguage">term source language</param>
		/// <param name="id">term id</param>
		/// <returns>entryTerms</returns>
		private IList<IEntryTerm> CreateEntryTerms(ILanguage language, ILanguage sourceLanguage, int id)
		{
			IList<IEntryTerm> entryTerms = new List<IEntryTerm>();
			var terms = _termsResult.Where(t => t.Id == id).ToList();

			// if language is Source language, create entryTerm with value from source language text
			// otherwise create entry terms for the target language search results
			if (language.Name.Equals(sourceLanguage.Name))
			{
				var sourceLangTerm = terms.FirstOrDefault(t => t.Language.Name.Equals(sourceLanguage.Locale.Parent.DisplayName));
				if (sourceLangTerm != null)
				{
					var entryTerm = new EntryTerm
					{
						Value = sourceLangTerm.Text,
						Fields = SetEntryFields((SearchResultModel)sourceLangTerm)
					};
					entryTerms.Add(entryTerm);
				}
			}
			else
			{
				// add IEntryTerm only for the current ISearchResult term(otherwise it will duplicate all the term for each ISearchResult term)				
				foreach (SearchResultModel term in terms)
				{
					// add terms for the target
					if (!term.Language.Name.Equals(sourceLanguage.Locale.Parent.DisplayName))
					{
						var entryTerm = new EntryTerm
						{
							Value = term.Text,
							Fields = SetEntryFields(term)
						};
						entryTerms.Add(entryTerm);
					}
				}
			}
			return entryTerms;
		}

		/// <summary>
		/// Set the glossary descriptive fields based on the needed values from the search result.
		/// Entry fields are used in the Hitlist Settings and also to display information in the Termbase Viewer
		/// </summary>
		/// <param name="searchResultModel">the search result model with values retrieved from API search result</param>
		/// <returns>entryFields</returns>
		private IList<IEntryField> SetEntryFields(SearchResultModel searchResultModel)
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

			if (!string.IsNullOrEmpty(searchResultModel.TermType))
			{
				var termTypeEntryField = new EntryField
				{
					Name = "TermType",
					Value = searchResultModel.TermType
				};
				entryFields.Add(termTypeEntryField);
			}
			return entryFields;
		}
		#endregion
		}
	}