using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;
using Sdl.TranslationStudioAutomation.IntegrationApi;

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
				if (!string.IsNullOrEmpty(termbasePath))
				{
					var provider = GetTerminologyProvider(termbasePath);
					if (provider == null)
					{
						ShowTermbaseError();
						return;
					}

					// Do not dispose the provider: TerminologyProviderManager hands back a shared
					// instance that Studio's Termbase Viewer and terminology search reuse. Disposing it
					// closes the underlying termbase, so every later terminology access in Studio throws
					// ConnectionException (SQLiteFileBasedTerminologyProvider.ErrorIfNotInitialized).
					if (!(provider is IStudioTermbase termbase))
					{
						ShowTermbaseError();
						return;
					}

					var sourceTermbaseLanguage = ResolveLanguage(provider, sourceLanguage);
					var targetTermbaseLanguage = ResolveLanguage(provider, targetLanguage);
					if (sourceTermbaseLanguage?.Locale == null || targetTermbaseLanguage?.Locale == null)
						return;

					var entries = LoadAllEntries(termbase);
					var sourceEntry = entries.FirstOrDefault(e => ContainsTerm(e, sourceTermbaseLanguage, sourceSelection));
					var targetEntryIds = entries
						.Where(e => ContainsTerm(e, targetTermbaseLanguage, targetSelection))
						.Select(e => e.Id)
						.Distinct()
						.ToList();

					if (sourceEntry != null)
					{
						if (targetEntryIds.Contains(sourceEntry.Id))
						{
							MessageBox.Show(@"The term you are trying to add already exists", @"Duplicate", MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
							return;
						}
						AddTermToExistingEntry(termbase, sourceEntry, targetSelection, targetTermbaseLanguage);
					}
					else
					{
						AddNewConcept(termbase, sourceSelection, sourceTermbaseLanguage, targetSelection, targetTermbaseLanguage);
					}
				}
			}
			else
			{
				MessageBox.Show(@"Please select source and target text.", @"Empty selection", MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}
		}

		private static ITerminologyProvider GetTerminologyProvider(string termbasePath)
		{
			var termbaseUri = new Uri("ttb." + new Uri(termbasePath).AbsoluteUri);
			var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
			if (provider == null)
				return null;

			// The provider is owned by TerminologyProviderManager and shared across Studio
			// (Termbase Viewer, terminology search). Never dispose it here - disposing closes the
			// underlying termbase and makes every later Studio terminology access throw
			// ConnectionException. Initialize() is idempotent and safe to call on a shared instance.
			return provider.Initialize() ? provider : null;
		}

		private static TermbaseLanguage ResolveLanguage(ITerminologyProvider provider, Language language)
		{
			var cultureInfo = language?.CultureInfo;
			if (cultureInfo == null)
				return null;

			var definitionLanguages = provider.Definition?.Languages?.ToList();
			if (definitionLanguages == null || !definitionLanguages.Any())
				return null;

			var definitionLanguage =
				definitionLanguages.FirstOrDefault(l => IsSameCulture(l.Locale, cultureInfo.Name)) ??
				definitionLanguages.FirstOrDefault(l => IsSameCulture(l.Locale, cultureInfo.TwoLetterISOLanguageName));
			if (definitionLanguage == null)
				return null;

			return new TermbaseLanguage
			{
				Locale = definitionLanguage.Locale.Name,
				Name = definitionLanguage.Name
			};
		}

		private static bool IsSameCulture(CultureCode locale, string cultureName)
		{
			return string.Equals(locale?.Name, cultureName, StringComparison.OrdinalIgnoreCase);
		}

		private static void ShowTermbaseError()
		{
			MessageBox.Show(@"The default termbase could not be opened. Make sure the project's default termbase is a file-based (.ttb) termbase.",
				@"Termbase error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

		private static List<Entry> LoadAllEntries(IStudioTermbase termbase)
		{
			var entries = new List<Entry>();
			var lastSeenId = 0;
			const int pageSize = 200;

			while (true)
			{
				var page = termbase.GetEntries(lastSeenId, pageSize);
				if (page == null || page.Count == 0)
					break;

				entries.AddRange(page);
				lastSeenId = page[page.Count - 1].Id;

				if (page.Count < pageSize)
					break;
			}

			return entries;
		}

		private static bool ContainsTerm(Entry entry, TermbaseLanguage language, string termText)
		{
			if (entry?.Languages == null)
				return false;

			foreach (var entryLanguage in entry.Languages)
			{
				if (!IsSameLocale(entryLanguage, language) || entryLanguage.Terms == null)
					continue;

				if (entryLanguage.Terms.Any(term => string.Equals(term.Value, termText, StringComparison.Ordinal)))
					return true;
			}

			return false;
		}

		private static bool IsSameLocale(EntryLanguage entryLanguage, TermbaseLanguage language)
		{
			return string.Equals(entryLanguage?.Locale?.Name, language?.Locale, StringComparison.OrdinalIgnoreCase);
		}

		private static void AddTermToExistingEntry(IStudioTermbase termbase, Entry entry, string term, TermbaseLanguage language)
		{
			var entryLanguage = entry.Languages?.FirstOrDefault(l => IsSameLocale(l, language));
			if (entryLanguage == null)
			{
				entryLanguage = CreateEntryLanguage(language);
				if (entry.Languages == null)
					entry.Languages = new List<EntryLanguage>();
				entry.Languages.Add(entryLanguage);
			}

			if (entryLanguage.Terms == null)
				entryLanguage.Terms = new List<EntryTerm>();
			entryLanguage.Terms.Add(CreateTerm(term));

			if (entry.Transactions == null)
				entry.Transactions = new List<EntryTransaction>();
			entry.Transactions.Add(CreateTransaction(TransactionType.Modification));

			termbase.UpdateEntry(entry);
		}

		private static void AddNewConcept(IStudioTermbase termbase, string sourceTerm, TermbaseLanguage sourceLanguage, string targetTerm, TermbaseLanguage targetLanguage)
		{
			var entry = new Entry
			{
				Languages = new List<EntryLanguage>
				{
					CreateEntryLanguage(sourceLanguage, sourceTerm),
					CreateEntryLanguage(targetLanguage, targetTerm)
				},
				Transactions = new List<EntryTransaction>
				{
					CreateTransaction(TransactionType.Origination)
				}
			};

			termbase.AddEntry(entry);
		}

		private static EntryLanguage CreateEntryLanguage(TermbaseLanguage language, string term = null)
		{
			var entryLanguage = new EntryLanguage
			{
				Name = language.Name,
				Locale = language.Locale,
				Terms = new List<EntryTerm>()
			};

			if (term != null)
				entryLanguage.Terms.Add(CreateTerm(term));

			return entryLanguage;
		}

		private static EntryTerm CreateTerm(string value)
		{
			return new EntryTerm { Value = value };
		}

		private static EntryTransaction CreateTransaction(TransactionType type)
		{
			return new EntryTransaction { Type = type, Date = DateTime.Now };
		}

		private sealed class TermbaseLanguage
		{
			public string Locale { get; set; }

			public string Name { get; set; }
		}
	}
}
