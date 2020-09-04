using System.Collections.Generic;

namespace Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Target : Segment
	{
		public Target()
		{
			Elements = new List<Element>();
		}

		public string Id { get; set; }
	}
}
