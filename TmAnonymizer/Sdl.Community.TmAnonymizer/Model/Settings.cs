using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class Settings
	{
		public Settings()
		{
			TmFiles = new List<TmFile>();
			Rules = new List<Rule>();
			Accepted = false;
			AlreadyAddedDefaultRules = false;
		}

		public List<TmFile> TmFiles { get; set; }

		public List<Rule> Rules { get; set; }

		public bool Accepted { get; set; }

		public bool AlreadyAddedDefaultRules { get; set; }
	}
}
