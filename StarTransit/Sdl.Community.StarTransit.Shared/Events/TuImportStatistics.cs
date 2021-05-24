using System.Globalization;

namespace Sdl.Community.StarTransit.Shared.Events
{
	public class TuImportStatistics
	{
		public int ReadTusCount { get; set; }
		public int AddedTusCount { get; set; }
		public int ErrorCount { get; set; }
		public int TotalImported { get; set; }
		public CultureInfo TargetLanguage { get; set; }
	}
}
