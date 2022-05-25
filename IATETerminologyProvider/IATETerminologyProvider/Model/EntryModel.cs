﻿using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class EntryModel : IEntry
	{
		public int Id { get; set; }
		public string ItemId { get; set; }
		public string SearchText { get; set; }
		public IList<IEntryField> Fields { get; set; }
		public IList<IEntryTransaction> Transactions { get; set; }
		public IList<IEntryLanguage> Languages { get; set; }		
	}
}