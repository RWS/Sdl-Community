using System;
using System.Collections.Generic;
using System.IO;
using MultiTermSearchExample.Model;

namespace MultiTermSearchExample
{
	public class TerminologyProvider
	{
		public Sdl.MultiTerm.TMO.Interop.Termbase MultitermTermbase { get; set; }

		public TerminologyProvider(string termbasePath, string userId, string password)
		{
			if (!File.Exists(termbasePath))
			{
				return;
			}

			var multitermApplication = new Sdl.MultiTerm.TMO.Interop.Application();
			multitermApplication.LocalRepository.Connect(userId, password);

			multitermApplication.LocalRepository.Termbases.Add(termbasePath, Path.GetFileName(termbasePath), string.Empty);
			MultitermTermbase = multitermApplication.LocalRepository.Termbases[0];
		}

		public List<TermHit> SearchTerm(string text, bool fuzzySearch, int maximumHits, string source, string target)
		{
			var search = MultitermTermbase.Search;
			search.Direction = Sdl.MultiTerm.TMO.Interop.MtSearchDirection.mtSearchDown;
			search.MaximumHits = maximumHits;
			search.FuzzySearch = fuzzySearch;
			search.SearchExpression = text;
			search.SourceIndex = source;
			search.TargetIndex = target;
			search.SearchExistTarget = true;

			var termHits = GetTermHits(search.Execute());

			return termHits;
		}

		private List<TermHit> GetTermHits(Sdl.MultiTerm.TMO.Interop.IHitTerms oHits)
		{
			var termHits = new List<TermHit>();
			foreach (Sdl.MultiTerm.TMO.Interop.IHitTerm oHit in oHits)
			{
				var termHit = new TermHit
				{
					Language = oHit.Index,
					SearchScore = oHit.SearchScore,
					Text = oHit.Text,
					Termbase = oHit.Termbase,
					ParentEntryId = oHit.ParentEntryID
				};

				termHits.Add(termHit);
				termHit.TermEntries = GetTermEntries(oHit);
			}

			return termHits;
		}

		private List<TermEntry> GetTermEntries(Sdl.MultiTerm.TMO.Interop.IHitTerm oHit)
		{
			var termEntries = new List<TermEntry>();

			// Get the TermEntries
			var parentId = Convert.ToInt32(oHit.ParentEntryID);
			var parentEntry = MultitermTermbase.Entries.Item(parentId);
			if (parentEntry == null)
			{
				return termEntries;
			}

			foreach (var index in parentEntry.Content.EntryIndexes)
			{
				var entryIndex = index as Sdl.MultiTerm.TMO.Interop.IEntryIndex;
				if (entryIndex == null)
				{
					continue;
				}

				termEntries.Add(new TermEntry
				{
					Term = entryIndex.Term,
					Language = entryIndex.IndexName,
					IndexFields = GetEntryFields(entryIndex.IndexFields),
					TermFields = GetEntryFields(entryIndex.TermFields)
				});
			}

			return termEntries;
		}

		private List<EntryField> GetEntryFields(Sdl.MultiTerm.TMO.Interop.IEntryFields entryFields)
		{
			var indexFields = new List<EntryField>();
			if (entryFields == null)
			{
				return indexFields;
			}
			
			foreach (Sdl.MultiTerm.TMO.Interop.IEntryField field in entryFields)
			{
				indexFields.Add(new EntryField
				{
					Name = field.Name,
					Value = field.Value
				});
			}

			return indexFields;
		}
	}
}
