using System.Collections.Generic;
using Sdl.Community.RateItControl.Implementation;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Options : BaseModel
	{
		private bool sendFeedback;

		public bool AutoSendFeedback { get; set; }
		public List<LanguageMappingModel> LanguageMappings { get; set; } = new();
		public bool ResendDraft { get; set; }
		public bool SendFeedback
		{
			get => sendFeedback; 
			set
			{
				sendFeedback = value;
				OnPropertyChanged(nameof(SendFeedback));
			}
		}
	}
}