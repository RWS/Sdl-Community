using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
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