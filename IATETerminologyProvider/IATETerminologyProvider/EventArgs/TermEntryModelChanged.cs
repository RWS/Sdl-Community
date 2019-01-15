using System.Collections.Generic;
using IATETerminologyProvider.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.EventArgs
{
	public class TermEntriesChangedEventArgs: System.EventArgs
	{
		public IList<EntryModel> EntryModels { get; set; }

		public ILanguage SourceLanguage { get; set; }
	}
}
