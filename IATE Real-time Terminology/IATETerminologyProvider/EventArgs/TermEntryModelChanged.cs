using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Model;
using TradosStudio.API.TranslationResources.Terminology;

namespace Sdl.Community.IATETerminologyProvider.EventArgs
{
	public class TermEntriesChangedEventArgs : System.EventArgs
	{
		public IList<EntryModel> EntryModels { get; set; }

		public ILanguage SourceLanguage { get; set; }

		public ILanguage TargetLanguage { get; set; }
	}
}