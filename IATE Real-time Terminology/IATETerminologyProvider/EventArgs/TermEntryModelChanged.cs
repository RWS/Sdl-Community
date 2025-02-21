using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.IATETerminologyProvider.EventArgs
{
	public class TermEntriesChangedEventArgs : System.EventArgs
	{
		public IList<EntryModel> EntryModels { get; set; }

		public Language SourceLanguage { get; set; }

		public Language TargetLanguage { get; set; }
	}
}