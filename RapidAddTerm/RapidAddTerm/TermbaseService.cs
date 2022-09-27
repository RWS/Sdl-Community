using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Application = Sdl.MultiTerm.TMO.Interop.Application;
using Termbase = Sdl.MultiTerm.TMO.Interop.Termbase;

namespace Sdl.Community.RapidAddTerm
{
	public class TermbaseService
	{
		private ProjectsController _projectsController = null;
		private EditorController _editorController = null;

		private ProjectsController ProjectsController => _projectsController ?? (_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>());
		private EditorController EditorController => _editorController ?? (_editorController = SdlTradosStudio.Application.GetController<EditorController>());

		private FileBasedProject GetProject() => ProjectsController.CurrentProject;
		private TermbaseConfiguration GetTermbaseConfiguration() => GetProject()?.GetTermbaseConfiguration();

		private IStudioDocument GetActiveDocument() => EditorController.ActiveDocument;
		public void AddNewTerm()
		{
			var activeDocument = GetActiveDocument();
			if (activeDocument == null) 
				return;

			var sourceSelection = activeDocument.Selection?.Source?.ToString().TrimStart().TrimEnd();
			var targetSelection = activeDocument.Selection?.Target?.ToString().TrimStart().TrimEnd();
			if (!string.IsNullOrEmpty(sourceSelection) && !string.IsNullOrEmpty(targetSelection))
			{
				var sourceLanguage = activeDocument.ActiveFile?.SourceFile.Language;
				var targetLanguage = activeDocument.ActiveFile?.Language;

				// Add concept to default termbase for source and target language of active file
				var termbasePath = GetTermbasePath();
				var languageIndexes = GetTermbaseConfiguration().LanguageIndexes;
				if (!string.IsNullOrEmpty(termbasePath))
				{
					var termbase = GetTermbase(termbasePath);
					var sourceIndexName = GetTermbaseIndex(termbase, languageIndexes, sourceLanguage);
					var sourceLanguageCode = GetLanguageCode(sourceIndexName, sourceLanguage);

					var targetIndexName = GetTermbaseIndex(termbase, languageIndexes, targetLanguage);
					var targetLanguageCode = GetLanguageCode(targetIndexName,targetLanguage);
						
					var sourceEntry = SearchEntries(termbasePath, sourceSelection, sourceIndexName);
					var targetEntries = SearchTargetEntries(termbasePath, targetSelection, targetIndexName);
					targetSelection = SecurityElement.Escape(targetSelection);
					sourceSelection = SecurityElement.Escape(sourceSelection);
					if (sourceEntry != null)
					{
						var targetAlreadyExists = IsTargetAlreadyAdded(targetEntries, sourceEntry.ID);
						if (targetAlreadyExists)
						{
							MessageBox.Show(@"The term you are trying to add already exists", @"Duplicate", MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
							return;
						}
						AddTermToExistingEntry(sourceEntry, targetSelection, targetLanguageCode);
					}
					else
					{
						var entries = GetTermbaseEntries(termbasePath);
						var entryText =
							$"<conceptGrp><languageGrp><language type=\"{sourceIndexName}\" lang=\"{sourceLanguageCode}\"></language><termGrp><term>{sourceSelection}</term></termGrp></languageGrp><languageGrp><language type=\"{targetIndexName}\" lang=\"{targetLanguageCode}\"></language><termGrp><term>{targetSelection}</term></termGrp></languageGrp></conceptGrp>";
						entries.New(entryText, false);
					}
				}
			}
			else
			{
				MessageBox.Show(@"Please select source and target text.", @"Empty selection", MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}
		}

		private static bool IsLanguage(Index index, Language language)
		{
			return index.Locale.ToLower() == language.CultureInfo.TwoLetterISOLanguageName.ToLower();
		}
		private  string GetTermbaseIndex(Termbase termbase, List<TermbaseLanguageIndex> projectTermbaseIndexes, Language currentLanguage)
		{
			// https://jira.sdl.com/browse/SDLCOM-3794 - always prefer language code from the termbase file, if possible
			var termbaseIndex = termbase.Definition.Indexes.OfType<Index>().FirstOrDefault(i => IsLanguage(i, currentLanguage));
			if (termbaseIndex != null)
				return termbaseIndex.Label;

			if (!projectTermbaseIndexes.Any()) return string.Empty;
			if (currentLanguage == null) return string.Empty;

			var projectTermbaseIndex = projectTermbaseIndexes.FirstOrDefault(t => t.ProjectLanguage.CultureInfo.Name.Equals(currentLanguage.CultureInfo.Name));
			return projectTermbaseIndex?.TermbaseIndex ?? string.Empty;
		}

		private bool IsSubLanguage(string languageName)
		{
			return languageName.IndexOf('(') >0;
		}

		private string GetLanguageCode(string termbaseIndex,Language language)
		{
			if (IsSubLanguage(termbaseIndex))
			{
				return language.CultureInfo.Name.ToUpper();
			}
			return language.CultureInfo.TwoLetterISOLanguageName.ToUpper();
		}


		private string GetTermbasePath()
		{
			var termbConfig = GetTermbaseConfiguration();
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

		/// <summary>
		/// Search a entru
		/// </summary>
		/// <param name="termbasePath">Local path of the default termbase</param>
		/// <param name="searchText">Searched text</param>
		/// <param name="languageCode">Language code for searched entry</param>
		/// <returns>First entry content or NULL if there is no entry which matches the criteria</returns>
		private Entry SearchEntries(string termbasePath,string searchText, string languageCode)
		{
			var termbase = GetTermbase(termbasePath);

			var entries = termbase.Entries;

			//set search parameters
			var search = termbase.Search;
			search.Direction = MtSearchDirection.mtSearchDown;
			search.MaximumHits = 10;
			search.FuzzySearch = false;
			search.SearchExpression = searchText;
			search.SourceIndex = languageCode; 
			var oHits = search.Execute();

			foreach (HitTerm oHit in oHits)
			{
				if (!string.IsNullOrEmpty(oHit.ParentEntryID))
				{
					var hitText = oHit.Text;
					if (hitText.Equals(searchText))
					{
						var entryId = Convert.ToInt32(oHit.ParentEntryID);
						var entry = entries.Item(entryId);
						return entry;
					}
				}
			}
			return null;
		}

		private List<Entry> SearchTargetEntries(string termbasePath, string searchText, string languageCode)
		{
			var termbase = GetTermbase(termbasePath);
			var entries = termbase.Entries;

			//set search parameters
			var search = termbase.Search;
			search.Direction = MtSearchDirection.mtSearchDown;
			search.MaximumHits = 10;
			search.FuzzySearch = false;
			search.SearchExpression = searchText;
			search.SourceIndex = languageCode;
			var oHits = search.Execute();

			var targetEntriesList= new List<Entry>();
			foreach (HitTerm oHit in oHits)
			{
				if (!string.IsNullOrEmpty(oHit.ParentEntryID))
				{
					var hitText = oHit.Text;
					if (hitText.Equals(searchText))
					{
						var entryId = Convert.ToInt32(oHit.ParentEntryID);
						var entry = entries.Item(entryId);
						var entryExists = targetEntriesList.Exists(e => e.ID.Equals(entryId));
						if (!entryExists)
						{
							targetEntriesList.Add(entry);
						}
					}
				}
			}
			return targetEntriesList;
		}

		private void AddTermToExistingEntry(Entry entry, string term, string languageCode)
		{
			var xml = new XmlDocument();
			xml.LoadXml(entry.Content.Content);
			var languageGrNodes = xml.SelectNodes("/conceptGrp/languageGrp");
			if (languageGrNodes != null)
			{
				foreach (XmlNode node in languageGrNodes)
				{
					foreach (XmlNode childNode in node.ChildNodes)
					{
						if (childNode.Name.Equals("language"))
						{
							var attributes = childNode.Attributes;
							if (attributes != null)
							{
								foreach (XmlAttribute attribute in attributes)
								{
									if (attribute.Name.Equals("lang"))
									{
										if (attribute.Value.Equals(languageCode))
										{
											var termGrNode = xml.CreateNode(XmlNodeType.Element, "termGrp", string.Empty);
											var termNode = xml.CreateNode(XmlNodeType.Element, "term", string.Empty);
											termNode.InnerText = term;
											termGrNode.AppendChild(termNode);
											node.AppendChild(termGrNode);
										}
									}
								}
							}
						}
					}
				}
			}
			var content = xml.InnerXml;
			entry.LockEntry(MtLockingState.mtLock);
			entry.Content.Content = content;
			entry.Save();
		}

		private bool IsTargetAlreadyAdded(List<Entry> targetEntries, int parentId)
		{
			var exists = targetEntries.Exists(e => e.ID.Equals(parentId));
			return exists;
		}

		private Entries GetTermbaseEntries(string termbasePath)
		{
			var termbase = GetTermbase(termbasePath);
			return termbase.Entries;
		}

		private Termbases GetTermbases()
		{
			var multiTermApplication = new Application();
			var localRep = multiTermApplication.LocalRepository;
			localRep.Connect("", "");

			var termbases = localRep.Termbases;
			return termbases;
		}

		private Termbase GetTermbase(string termbasePath)
		{
			var termbases = GetTermbases();
			termbases.Add(termbasePath, string.Empty, string.Empty);
			var termbase = termbases[termbasePath];
			return termbase;
		}
	}
}
