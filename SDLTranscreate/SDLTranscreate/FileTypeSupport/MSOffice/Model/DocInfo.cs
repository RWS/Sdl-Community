using System;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
    public class DocInfo
    {
	    /// Trados Studio Project Id
	    public string ProjectId { get; set; }

	    /// Path to the binlingual SDLXLIFF documented used to create the XLIFF file
	    public string Source { get; set; }

	    public string SourceLanguage { get; set; }

	    public string TargetLanguage { get; set; }

	    /// Format: "yyyy-MM-ddTHH:mm:ss.fffZ"
	    public DateTime Created { get; set; }
	}
}
