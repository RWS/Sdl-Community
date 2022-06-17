namespace Trados.Transcreate.Model
{
	public class AnalysisTaskSettingsInfo
	{
		public AnalysisTaskSettingsInfo()
		{
			ReportCrossFileRepetitions = true;
			UnknownSegmentsMaximumMatchValue = 74;
			FrequentSegmentsNoOfOccurrences = 5;
		}

		public bool ReportCrossFileRepetitions { get; set; }

		public bool ReportInternalFuzzyMatchLeverage { get; set; }

		public bool ExcludeLockedSegments { get; set; }

		public bool ReportLockedSegmentsSeparately { get; set; }

		public bool ExportUnknownSegments { get; set; }

		public bool ExportFrequentSegments { get; set; }

		public int UnknownSegmentsMaximumMatchValue { get; set; }

		public int FrequentSegmentsNoOfOccurrences { get; set; }

	}
}
