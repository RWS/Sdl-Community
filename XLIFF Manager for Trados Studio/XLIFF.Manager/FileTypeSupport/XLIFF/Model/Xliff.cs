using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	
	public class Xliff
	{
		public Xliff()
		{
			DocInfo = new DocInfo();
			Files = new List<File>();			
			Version = "1.2";
		}

		public string Version { get; set; }

		public DocInfo DocInfo { get; set; }

		public string SpecificVersion { get; set; }

		public Enumerators.XLIFFSupport Support { get; set; }

		public List<File> Files { get; set; }		
	}
}
