using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class SourceCopyRecord
    {
		public string Id { get; set; }

		public string Context { get; set; }

		public string Source { get; set; }

		public string Translation { get; set; }

		public string BackTranslation { get; set; }

		public List<Comment> Comments { get; set; }		
    }
}
