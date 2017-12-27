using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Model
{
	public class EngineMappingDetails
	{
		public string SourceLang { get; set; }
		public string TargetLang { get; set; }
		public string Id { get; set; }
		public List<string> ResourcesIds { get; set; }
	}
}
