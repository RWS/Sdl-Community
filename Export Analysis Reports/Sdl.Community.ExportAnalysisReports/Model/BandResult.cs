namespace Sdl.Community.ExportAnalysisReports.Model
{
	public class BandResult
	{
		public string BandName { get; }
		public int Segments { get; set; }
		public int Words { get; set; }
		public int Characters { get; set; }
		public int Placeables { get; set; }
		public int Tags { get; set; }
		public int Min { get; set; }
		public int Max { get; set; }
		public int FullRecallWords { get; set; }
		public int Locked { get; set; }
		public int PerfectMatch { get; set; }
		public int ContextMatch { get; set; }
		public int CrossRep { get; set; }

		public BandResult(string bandName)
		{
			BandName = bandName;
		}
	}
}