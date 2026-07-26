namespace Sdl.Community.ExportAnalysisReports.Model
{
	public class OptionalInformation
	{
		public bool IncludeAdaptiveBaseline { get; set; }
		public bool IncludeAdaptiveLearnings { get; set; }
		public bool IncludeInternalFuzzies { get; set; }
		public bool IncludeLocked { get; set; }
		public bool IncludePerfectMatch { get; set; }
		public bool IncludeContextMatch{ get; set; }
		public bool IncludeCrossRep { get; set; }
	}
}