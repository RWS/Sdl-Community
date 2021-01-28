using System.Collections.Generic;

namespace Sdl.Community.StudioViews.Model
{
	public class ExportResult
	{
		public ExportResult()
		{
			Success = false;
			InputFiles = new List<string>();
			OutputFiles = new List<OutputFile>();
		}
		
		public bool Success { get; set; }
		
		public List<string> InputFiles { get; set; }

		public List<OutputFile> OutputFiles { get; set; }

		public string Message { get; set; }
	}
}
