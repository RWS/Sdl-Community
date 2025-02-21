

using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
    public class SourceCopyDocument
    {
	    public string Version { get; set; }

	    public DocInfo DocInfo { get; set; }		

		public List<SourceCopyRecord> Records { get; set; }
	}
}
