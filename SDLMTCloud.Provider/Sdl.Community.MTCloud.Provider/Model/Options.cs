using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Options
	{
		public Options()
		{			
			ResendDraft = true;
			LanguageMappings = new List<LanguageMappingModel>();
		}

		public bool ResendDraft { get; set; }

		public List<LanguageMappingModel> LanguageMappings { get; set; }
	}
}
