using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using InterpretBank.Studio.Model;
using InterpretBank.TerminologyService;
using InterpretBank.TerminologyService.Interface;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	public class InterpretBankProvider : AbstractTerminologyProvider
	{
		public InterpretBankProvider(ITerminologyService termSearchService, ISettingsService settingsService)
		{
			TermSearchService = termSearchService;
			SettingsService = settingsService;
		}

		public override IDefinition Definition
		{
			get
			{
				//TODO: take name of these fields from Settings
				return new Definition(new[]{
					new DescriptiveField
				{
					Label = "Extra1",
					Level = FieldLevel.TermLevel,
					Type = FieldType.String
				}, new DescriptiveField
				{
					Label = "Extra2",
					Level = FieldLevel.TermLevel,
					Type = FieldType.String
				}}, GetLanguages().Cast<IDefinitionLanguage>());
			}
		}

		public override string Description => PluginResources.Plugin_Description;

		public override string Name
		{
			get
			{
				return "Interpret Bank";
			}
		}

		public ISettingsService SettingsService { get; }

		public override Uri Uri
		{
			get
			{
				return new Uri(Constants.InterpretBankUri);
			}
		}

		private HashSet<IEntry> Entries { get; } = new();

		private int TermIndex { get; set; }
		private ITerminologyService TermSearchService { get; }

		public override IEntry GetEntry(int id) => Entries.FirstOrDefault(e => e.Id == id);

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			throw new NotImplementedException();
		}

		public override IList<ILanguage> GetLanguages()
		{
			var languages = new List<ILanguage>();

			var interpretBankLanguages = TermSearchService.GetLanguages();

			//var currentProject = StudioContext.ProjectsController.CurrentProject;
			//if (currentProject == null) return null;

			//var projectInfo = currentProject.GetProjectInfo();

			//languages.Add(new DefinitionLanguage
			//{
			//	IsBidirectional = true,
			//	Locale = projectInfo.SourceLanguage.CultureInfo,
			//	Name = projectInfo.SourceLanguage.DisplayName,
			//	TargetOnly = false
			//});

			var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

			languages.AddRange(interpretBankLanguages.Select(lang => new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = cultures.FirstOrDefault(cult => cult.EnglishName == lang.Name),
				Name = lang.Name,
				TargetOnly = false
			}));

			return languages;
		}

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			var words = Regex.Split(text, "\\s+");

			var results = new List<ISearchResult>();
			foreach (var word in words)
			{
				var terms = mode switch
				{
					SearchMode.Fuzzy => TermSearchService.GetFuzzyTerms(word, source.Name, destination.Name),
					SearchMode.Normal => TermSearchService.GetExactTerms(word, source.Name, destination.Name),
					SearchMode.FullText => throw new NotImplementedException(),
				};

				var id = TermIndex++;
				//TODO: calculate score instead of hardcoding "100"

				if (terms is not { Count: > 0 })
					continue;

				results.Add(new SearchResult
				{
					Id = id,
					Score = 100,
					Text = word
				});
				Entries.Add(CreateEntry(id, terms, destination.Name));
			}

			return results;
		}

		private IEntry CreateEntry(int id, List<StudioTermEntry> targetTerms, string targetLanguage)
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

			var entry = new Entry { Id = id, };
			entry.Languages.Add(entryTargetLanguage);
			
			return entry;
		}
	}
}