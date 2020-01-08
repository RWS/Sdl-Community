using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = Sdl.MultiTerm.TMO.Interop.Application;

namespace Sdl.Community.RapidAddTerm
{
	public class TermbaseService
	{
		public void AddNewTerm()
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
						var entries = GetTermbaseEntries(defaultTermbasePath);
						var sourceIndexName = GetTermbaseIndex(languageIndexes, activeDocument.ActiveFile?.SourceFile.Language);
						var targetIndexName = GetTermbaseIndex(languageIndexes, activeDocument.ActiveFile?.Language);
						var entryText =
							$"<conceptGrp><languageGrp><language type=\"{sourceIndexName}\" lang=\"{sourceLanguageCode}\"></language><termGrp><term>{sourceSelection}</term></termGrp></languageGrp><languageGrp><language type=\"{targetIndexName}\" lang=\"{targetLanguageCode}\"></language><termGrp><term>{targetSelection}</term></termGrp></languageGrp></conceptGrp>";
						entries.New(entryText, true);
					}
				}
				else
				{
					MessageBox.Show(@"Please select source and target text.", @"Empty selection", MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation);
				}
			}
		}
		private  string GetTermbaseIndex(List<TermbaseLanguageIndex> termbaseIndexes, Language currentLanguage)
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

		private Entries GetTermbaseEntries(string termbasePath)
		{
			var multiTermApplication = new Application();
			var localRep = multiTermApplication.LocalRepository;
			localRep.Connect("", "");

			var termbases = localRep.Termbases;
			var path = termbasePath;
			termbases.Add(path, "", "");
			var termbase = termbases[path];
			return termbase.Entries;
		}
	}
}
