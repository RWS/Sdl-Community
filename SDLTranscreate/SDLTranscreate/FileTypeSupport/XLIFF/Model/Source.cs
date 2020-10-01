using System.Collections.Generic;

namespace Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Source : Segment
	{
		public Source()
		{
			Elements = new List<Element>();			
		}

		public string Id { get; set; }
	}
}
