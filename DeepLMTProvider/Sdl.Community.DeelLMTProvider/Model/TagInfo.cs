using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class TagInfo
	{
		public string TagId { get; set; }
		public int Index { get; set; }
		public TagType TagType { get; set; }
		public bool IsClosed { get; set; }
	}
}
