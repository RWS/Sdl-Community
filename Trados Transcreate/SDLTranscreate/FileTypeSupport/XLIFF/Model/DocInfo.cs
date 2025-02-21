using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class DocInfo
	{
		public DocInfo()
		{
			Comments = new Dictionary<string, List<IComment>>();
		}

		// SDLXLIFF document id
		public string DocumentId { get; set; }

		/// Trados Studio Project Id
		public string ProjectId { get; set; }

		/// Path to the binlingual SDLXLIFF documented used to create the XLIFF file
		public string Source { get; set; }

		public string SourceLanguage { get; set; }

		public string TargetLanguage { get; set; }

		/// Format: "yyyy-MM-ddTHH:mm:ss.fffZ"
		public DateTime Created { get; set; }

		public Dictionary<string, List<IComment>> Comments { get; set; }
	}
}
