using System;
using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	public class DocInfo
	{
		public DocInfo()
		{
			Comments = new Dictionary<string, List<IComment>>();
		}

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
