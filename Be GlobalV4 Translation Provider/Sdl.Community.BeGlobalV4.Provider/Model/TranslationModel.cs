using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class TranslationModel : BaseViewModel
	{
		public TranslationModel()
		{
			LanguagesSupported = new Dictionary<string, string>();
		}

		public string DisplayName { get; set; }

		public Dictionary<string, string> LanguagesSupported { get; set; }

		[JsonIgnore]
		public string Model { get; set; }
	}
}