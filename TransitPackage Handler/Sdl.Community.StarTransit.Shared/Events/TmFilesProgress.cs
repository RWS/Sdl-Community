using System.Globalization;

namespace Sdl.Community.StarTransit.Shared.Events
{
	public class TmFilesProgress
	{
		public int TotalFilesNumber { get; set; }
		public int ProcessingFileNumber { get; set; }
		public CultureInfo TargetLanguage { get; set; }
	}
}
