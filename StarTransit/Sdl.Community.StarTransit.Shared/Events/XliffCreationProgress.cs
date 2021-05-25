using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Community.StarTransit.Shared.Events
{
	public class XliffCreationProgress
	{
		public List<string> BatchTaskIds { get; set; }
		public int Progress { get; set; }
		public CultureInfo TargetLanguage { get; set; }
		public int TotalFileNumber { get; set; }	
	}
}
