using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Model
{
	public class FeedbackRequest
	{
		public LanguagePair LanguagePair { get; set; }
		public string OriginalOutput { get; set; }
		public string PostEdited { get; set; }
		public Definition Definition { get; set; }
		public string Source { get; set; }
	}
}
