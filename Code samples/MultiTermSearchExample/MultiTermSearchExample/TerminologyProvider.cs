using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiTermSearchExample.Model;
using EntryField = MultiTermSearchExample.Model.EntryField;
using Sdl.Terminology.TerminologyProvider.Core;

namespace MultiTermSearchExample
{
	public class TerminologyProvider
	{
		public ITerminologyProvider Termbase { get; }

		public TerminologyProvider(string termbasePath)
		{
			if (!File.Exists(termbasePath))
			{
				return;
			}

			var termbaseUri = new Uri("ttb." + new Uri(termbasePath).AbsoluteUri);
			var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
			if (provider != null && provider.Initialize())
			{
				Termbase = provider;
			}
		}

		public List<TermHit> SearchTerm(string text, bool fuzzySearch, int maximumHits, string source, string target)
		{
			var sourceLanguage = GetLanguage(source);
			var targetLanguage = GetLanguage(target);
			if (sourceLanguage == null || targetLanguage == null)
			{
				return null;
			}

			var searchMode = fuzzySearch ? SearchMode.Fuzzy : SearchMode.Normal;
			var searchResults = Termbase.Search(text, sourceLanguage, targetLanguage, maximumHits, searchMode, true);

			return searchResults.Select(result => new TermHit
			{
				Language = result.Language?.Name,
				SearchScore = result.Score,
				Text = result.Text,
				Termbase = Termbase.Name,
				ParentEntryId = result.Id.ToString(),
				TermEntries = GetTermEntries(result.Id)
			}).ToList();
		}

		private ILanguage GetLanguage(string name)
		{
			return Termbase?.GetLanguages()
				.FirstOrDefault(language => string.Equals(language.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		private List<TermEntry> GetTermEntries(int entryId)
		{
			var termEntries = new List<TermEntry>();
			var entry = Termbase.GetEntry(entryId);
			if (entry?.Languages == null)
			{
				return termEntries;
			}

			foreach (var entryLanguage in entry.Languages)
			{
				if (entryLanguage.Terms == null)
				{
					continue;
				}

				foreach (var term in entryLanguage.Terms)
				{
					termEntries.Add(new TermEntry
					{
						Term = term.Value,
						Language = entryLanguage.Name,
						ReadOnly = Termbase.IsReadOnly,
						IndexFields = GetEntryFields(entryLanguage.Fields),
						TermFields = GetEntryFields(term.Fields)
					});
				}
			}

			return termEntries;
		}

		private static List<EntryField> GetEntryFields(IEnumerable<Sdl.Terminology.TerminologyProvider.Core.EntryField> entryFields)
		{
			return entryFields?.Select(field => new EntryField
			{
				Name = field.Name,
				Value = field.Value
			}).ToList() ?? new List<EntryField>();
		}
	}
}