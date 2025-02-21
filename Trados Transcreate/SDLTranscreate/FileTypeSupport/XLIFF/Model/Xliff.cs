using System.Collections.Generic;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	
	public class Xliff
	{
		public Xliff()
		{
			DocInfo = new DocInfo();
			Files = new List<File>();			
			Version = "1.2";
			TagPairIds = new List<string>();
			PlaceholderIds = new List<string>();
		}

		public string Version { get; set; }

		public DocInfo DocInfo { get; set; }

		public string SpecificVersion { get; set; }

		public Enumerators.XLIFFSupport Support { get; set; }

		public List<File> Files { get; set; }

		public List<string> TagPairIds { get; set; }

		public List<string> PlaceholderIds { get; set; }
	}
}
