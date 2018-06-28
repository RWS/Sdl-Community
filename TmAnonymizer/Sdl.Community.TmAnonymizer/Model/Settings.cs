using System.Collections.ObjectModel;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class Settings
	{
		public bool Accepted { get; set; }
		public bool AlreadyAddedDefaultRules { get; set; }
		public ObservableCollection<Rule> Rules { get; set; }
	}
}
