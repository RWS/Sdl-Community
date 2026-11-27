using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MultiTermTestPlugin
{
	[Action("MultiTermTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var path = "";// termbase local path
			//Concept with terms for English and German
			AddNewConceptAction.GetTermbase(path)?.AddEntry(
				AddNewConceptAction.NewEntry("English", "en-US", "for", "German", "de-DE", "für"));
		}
	}
	[Action("Community: Add term", Name = "Community: Add Term to termbase", Icon = "")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class AddNewConceptAction : AbstractAction
	{
		// Add new concept in TB based on user selection
		protected override void Execute()
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocument = editorController?.ActiveDocument;
			
			if (activeDocument != null)
			{
				var sourceSelection = activeDocument.Selection?.Source?.ToString().TrimStart().TrimEnd();
				var targetSelection = activeDocument.Selection?.Target?.ToString().TrimStart().TrimEnd();
				if (!string.IsNullOrEmpty(sourceSelection) && !string.IsNullOrEmpty(targetSelection))
				{
					var sourceLanguageCode = activeDocument.ActiveFile?.SourceFile.Language.CultureInfo.Name;
					var targetLanguageCode = activeDocument.ActiveFile?.Language.CultureInfo.Name;
					// Add concept to default termbase for source and target language of active file
					var defaultTermbasePath = GetTermbasePath();
					var languageIndexes = GetDefaultTermbaseConfiguration().LanguageIndexes;
					if (!string.IsNullOrEmpty(defaultTermbasePath))
					{
						var sourceIndexName = GetTermbaseIndex(languageIndexes, activeDocument.ActiveFile?.SourceFile.Language);
						var targetIndexName = GetTermbaseIndex(languageIndexes, activeDocument.ActiveFile?.Language);
						GetTermbase(defaultTermbasePath)?.AddEntry(
							NewEntry(sourceIndexName, sourceLanguageCode, sourceSelection, targetIndexName, targetLanguageCode, targetSelection));
					}
				}
			}
		}

		private string GetTermbaseIndex(List<TermbaseLanguageIndex> termbaseIndexes,Language currentLanguage)
		{
			if (termbaseIndexes.Any())
			{
				var termbaseIndex =
					termbaseIndexes.FirstOrDefault(t => t.ProjectLanguage.CultureInfo.Name.Equals(currentLanguage.CultureInfo.Name));
				if (termbaseIndex != null)
				{
					return termbaseIndex.TermbaseIndex;
				}
			}
			return string.Empty;
		}

		private TermbaseConfiguration GetDefaultTermbaseConfiguration()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var activeProject = projectsController?.CurrentProject;

			return activeProject?.GetTermbaseConfiguration();
		}
		private string GetTermbasePath()
		{
			var termbConfig = GetDefaultTermbaseConfiguration();
			var termbaseSettingsXml = termbConfig?.Termbases.FirstOrDefault()?.SettingsXML;
			if (!string.IsNullOrEmpty(termbaseSettingsXml))
			{
				var xml = new XmlDocument();
				xml.LoadXml(termbaseSettingsXml);
				var xnList = xml.SelectNodes("/TermbaseSettings/Path");
				if (xnList?.Count > 0)
				{
					if (xnList[0].HasChildNodes)
					{
						return xnList[0].ChildNodes[0].Value;
					}
				}
			}
			return string.Empty;
		}

		internal static IStudioTermbase GetTermbase(string termbasePath)
		{
			var termbaseUri = new Uri("ttb." + new Uri(termbasePath).AbsoluteUri);
			var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
			if (provider == null || !provider.Initialize())
			{
				return null;
			}

			return provider as IStudioTermbase;
		}

		internal static Entry NewEntry(string sourceIndexName, string sourceLanguageCode, string sourceTerm,
			string targetIndexName, string targetLanguageCode, string targetTerm)
		{
			return new Entry
			{
				Languages = new List<EntryLanguage>
				{
					NewLanguage(sourceIndexName, sourceLanguageCode, sourceTerm),
					NewLanguage(targetIndexName, targetLanguageCode, targetTerm)
				}
			};
		}

		private static EntryLanguage NewLanguage(string indexName, string languageCode, string term)
		{
			return new EntryLanguage
			{
				Name = indexName,
				Locale = new CultureCode(languageCode),
				Terms = new List<EntryTerm> { new EntryTerm { Value = term } }
			};
		}
	}
}
