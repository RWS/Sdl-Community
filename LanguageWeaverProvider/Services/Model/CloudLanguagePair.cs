using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudLanguagePair
	{
		private const string ActiveValue = "yes";

		public string Active { get; set; }

		public bool IsActive => !string.IsNullOrEmpty(Active) && Active.Equals(ActiveValue);

		public string Model { get; set; }

		public string SourceLanguageId { get; set; }

		public string TargetLanguageId { get; set; }
	}
}