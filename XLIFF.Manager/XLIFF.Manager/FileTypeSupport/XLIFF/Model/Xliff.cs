using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	
	public class Xliff
	{
		public Xliff()
		{
			DocInfo = new DocInfo();
			Files = new List<File>();
			Comments = new Dictionary<string, List<IComment>>();
			Version = "1.2";
		}

		public string Version { get; set; }

		public DocInfo DocInfo { get; set; }

		public List<File> Files { get; set; }

		public Dictionary<string, List<IComment>> Comments { get; set; }
	}
}
