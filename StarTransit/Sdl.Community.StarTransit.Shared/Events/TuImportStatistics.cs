using System.Globalization;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Shared.Events
{
	public class TuImportStatistics:BaseViewModel
	{
		public int ReadTusCount { get; set; }
		public int AddedTusCount { get; set; }
		public int ErrorCount { get; set; }
		public int TotalImported { get; set; }
		public CultureInfo TargetLanguage { get; set; }
	}
}
