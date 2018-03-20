using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Model
{
	public class TranslateResponse
	{
		public string ByteCount { get; set; }
		public string CharCount { get; set; }
		public string OutputByteCount{ get; set; }
		public string OutputCharCount { get; set; }
		public string OutputWordCount { get; set; }
		public bool PartialTranslation { get; set; }
		public string To { get; set; }
		public string Translation { get; set; }
		public string WordCount { get; set; }
	}
}
