namespace LanguageWeaverProvider.Services.Model
{
	public class EdgeTranslationStatus
	{
		public string TranslationId { get; set; }

		public string ErrorMessage { get; set; }

		public string State { get; set; }

		public string Substate { get; set; }

		public EdgeTranslationError Error { get; set; }
	}
}