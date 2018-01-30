using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Model
{
	public class TranslateRequest
	{
		public Content Content { get; set; }
		public Definition Definition { get; set; }
		public LanguagePair LanguagePair { get; set; }
	}
}
