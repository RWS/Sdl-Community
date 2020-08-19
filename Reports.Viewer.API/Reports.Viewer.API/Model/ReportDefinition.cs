using System.Reflection;

namespace Sdl.Reports.Viewer.API.Model
{
	public class ReportDefinition
	{		
		public string Id { get; set; }
	
		public byte[] Data { get; set; }

		public Assembly Assembly { get; set; }
	}
}
