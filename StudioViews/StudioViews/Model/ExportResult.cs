using System.Collections.Generic;

namespace Sdl.Community.StudioViews.Model
{
	public class ExportResult
	{
		public bool Success { get; set; }
		
		public List<string> InputFiles { get; set; }

		public List<string> OutputFiles { get; set; }

		public string Message { get; set; }
	}
}
