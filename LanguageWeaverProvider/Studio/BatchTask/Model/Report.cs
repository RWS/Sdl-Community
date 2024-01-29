namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class Report
	{
		public Data Data { get; set; } = new();

		public ReportSummary Summary { get; set; } = new();
	}
}