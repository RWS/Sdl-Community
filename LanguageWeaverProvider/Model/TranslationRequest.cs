namespace LanguageWeaverProvider.Model
{
	public class TranslationRequest
	{
		public string SourceLanguageId { get; set; }
		public string TargetLanguageId { get; set; }
		public string Model { get; set; }
		public string SubmissionType { get; set; }
		public string InputFormat { get; set; }
		public string RequestId { get; set; }
	}
}