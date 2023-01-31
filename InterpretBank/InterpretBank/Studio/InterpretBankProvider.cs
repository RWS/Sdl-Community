using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InterpretBank.TermSearch;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	public class InterpretBankProvider : AbstractTerminologyProvider
	{
		public InterpretBankProvider(ITermSearchService termSearchService)
		{
			TermSearchService = termSearchService;
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

		public override Uri Uri
		{
			get
			{
				return new Uri(Constants.InterpretBankUri);
			}
		}

		private List<IEntry> Entries { get; } = new();

		private int TermIndex { get; set; }
		private ITermSearchService TermSearchService { get; }

		public override IEntry GetEntry(int id) => Entries.FirstOrDefault(e => e.Id == id);

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			throw new NotImplementedException();
		}

		public override IList<ILanguage> GetLanguages()
		{
			var languages = new List<ILanguage>();

			var currentProject = Common.ProjectsController.CurrentProject;
			if (currentProject == null) return null;

			var projectInfo = currentProject.GetProjectInfo();

			languages.Add(new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = projectInfo.SourceLanguage.CultureInfo,
				Name = projectInfo.SourceLanguage.DisplayName,
				TargetOnly = false
			});

			languages.AddRange(projectInfo.TargetLanguages.Select(targetLanguage => new DefinitionLanguage
			{
				IsBidirectional = true,
				Locale = targetLanguage.CultureInfo,
				Name = targetLanguage.DisplayName,
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
				var term = mode switch
				{
					SearchMode.Fuzzy => TermSearchService.GetFuzzyTerms(word, source.Name, destination.Name),
					SearchMode.Normal => TermSearchService.GetExactTerms(word, source.Name, destination.Name),
					SearchMode.FullText => throw new NotImplementedException(),
				};

				var id = TermIndex++;
				//TODO: calculate score instead of hardcoding "100"
				results.Add(new SearchResult
				{
					Id = id,
					Score = 100,
					Text = word
				});

				Entries.Add(CreateEntry(id, term[0], destination.Name));

				//results.Add(new SearchResult
				//{
				//	Id = TermIndex++,
				//	Language = destination,
				//	Score = 100,
				//	Text = word
				//});
			}

			return results;
		}

		private IEntry CreateEntry(int id,string target, string targetLanguage)
		{
			var entryTerm = new EntryTerm { Value = target };

			var entryField1 = new EntryField { Name = "Extra1", Value = "Abreviere" };
			var entryField2 = new EntryField { Name = "Extra2", Value = "Abreviere2" };
			
			entryTerm.Fields.Add(entryField1);
			entryTerm.Fields.Add(entryField2);

			var entry = new Entry { Id = id, };

			var entryTargetLanguage = new EntryLanguage { Name = targetLanguage };
			entryTargetLanguage.Terms.Add(entryTerm);

			entry.Languages.Add(entryTargetLanguage);

			return entry;
		}
	}
}