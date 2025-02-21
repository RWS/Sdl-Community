using System.Collections.Generic;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudTranslationResponse
	{
		public string Model { get; set; }
		
		public string SourceLanguageId { get; set; }
		
		public string TargetLanguageId { get; set; }
		
		public List<string> Translation { get; set; }
	}
}