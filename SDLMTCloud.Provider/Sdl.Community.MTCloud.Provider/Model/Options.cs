using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.RateItControl.Implementation;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Options : BaseModel
	{
		private bool _sendFeedback;

		public Options()
		{
			LanguageMappings = new List<LanguageMappingModel>();
		}

		public bool AutoSendFeedback { get; set; }

		[JsonProperty("LanguagesSupported")]
		public List<LanguageMappingModel> LanguageMappings { get; set; } = new();
		public bool ResendDraft { get; set; }

		public bool SendFeedback
		{
			get => _sendFeedback;
			set
			{
				_sendFeedback = value;
				OnPropertyChanged(nameof(SendFeedback));
			}
		}
	}
}