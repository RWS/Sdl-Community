using System;
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
					var sourceLanguage = activeDocument.ActiveFile?.SourceFile.Language;
					var targetLanguage = activeDocument.ActiveFile?.Language;

					// Add concept to default termbase for source and target language of active file
					var defaultTermbasePath = GetTermbasePath();
					var languageIndexes = GetDefaultTermbaseConfiguration().LanguageIndexes;
					if (!string.IsNullOrEmpty(defaultTermbasePath))
					{
						var sourceIndexName = GetTermbaseIndex(languageIndexes, sourceLanguage);
						var sourceLanguageCode = GetLanguageCode(sourceIndexName, sourceLanguage);

						var targetIndexName = GetTermbaseIndex(languageIndexes, targetLanguage);
						var targetLanguageCode = GetLanguageCode(targetIndexName,targetLanguage);
						
						var sourceEntry = SearchEntries(defaultTermbasePath, sourceSelection, sourceIndexName);
						var targetEntries = SearchTargetEntries(defaultTermbasePath, targetSelection, targetIndexName);
						if (sourceEntry != null)
						{
							var targetAlreadyExists = TargetAlreadyAdded(targetEntries, sourceEntry.ID);
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
							var entries = GetTermbaseEntries(defaultTermbasePath);
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
		}
		private  string GetTermbaseIndex(List<TermbaseLanguageIndex> termbaseIndexes, Language currentLanguage)
		{
			if (termbaseIndexes.Any())
			{
				if (currentLanguage != null)
				{
					var termbaseIndex =
						termbaseIndexes.FirstOrDefault(t => t.ProjectLanguage.CultureInfo.Name.Equals(currentLanguage.CultureInfo.Name));
					if (termbaseIndex != null)
					{
						return termbaseIndex.TermbaseIndex;
					}
				}
			}
			return string.Empty;
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

		/// <summary>
		/// Search a entru
		/// </summary>
		/// <param name="termbasePath">Local path of the default termbase</param>
		/// <param name="searchText">Searched text</param>
		/// <param name="languageCode">Language code for searched entry</param>
		/// <returns>First entry content or NULL if there is no entry which matches the criteria</returns>
		private Entry SearchEntries(string termbasePath,string searchText, string languageCode)
		{
			var termbases = GetTermbases();
			termbases.Add(termbasePath, "", "");
			var termbase = termbases[termbasePath];
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
			var termbases = GetTermbases();
			termbases.Add(termbasePath, "", "");
			var termbase = termbases[termbasePath];
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

		private bool TargetAlreadyAdded(List<Entry> targetEntries, int parentId)
		{
			var exists = targetEntries.Exists(e => e.ID.Equals(parentId));
			return exists;
		}

		private Entries GetTermbaseEntries(string termbasePath)
		{
			var termbases = GetTermbases();
			termbases.Add(termbasePath, "", "");
			var termbase = termbases[termbasePath];
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
	}
}
