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
		private ProviderSettings _providerSettings;
		public IATETerminologyProvider(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
		public const string IATEUriTemplate = "iateglossary://";

		public override IDefinition Definition => new Definition(GetDescriptiveFields(), GetDefinitionLanguages());

		public override string Description => PluginResources.IATETerminologyProviderDescription;

		public override string Name => PluginResources.IATETerminologyProviderName;

		public override Uri Uri => new Uri((IATEUriTemplate + "https://iate.europa.eu/em-api/entries/_search").RemoveUriForbiddenCharacters());

		public override IEntry GetEntry(int id)
		{
			return null;

		}

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return null;

		}

		public override IList<ILanguage> GetLanguages()
		{
			return GetDefinitionLanguages().Cast<ILanguage>().ToList();
		}

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			var result = new List<ISearchResult>();

			var searchService = new TermSearchService(_providerSettings);
			searchService.GetTerms(text, source, destination, maxResultsCount);
			
			// To Do: add the results from the response in the result

			return result;
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
	}
}