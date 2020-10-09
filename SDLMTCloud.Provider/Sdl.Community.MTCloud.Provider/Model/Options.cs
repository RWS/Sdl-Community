using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Options
	{
		public bool AutoSendFeedback { get; set; }
		public List<LanguageMappingModel> LanguageMappings { get; set; }
		public bool ResendDraft { get; set; }
		public bool SendFeedback { get; set; }
	}
}