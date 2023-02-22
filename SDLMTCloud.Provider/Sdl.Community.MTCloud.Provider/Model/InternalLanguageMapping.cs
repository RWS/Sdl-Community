using System.Collections.Generic;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Model
{
	internal class InternalLanguageMapping
	{
		public string Name { get; set; }
		public MappedLanguage SourceLanguageCode { get; set; }
		public MappedLanguage TargetLanguageCode { get; set; }
		public MTCloudLanguage SelectedSourceLanguageMapping { get; set; }
		public List<MTCloudLanguage> SourceLanguageMappings { get; set; }
		public MTCloudLanguage SelectedTargetLanguageMapping { get; set; }
		public List<MTCloudLanguage> TargetLanguageMappings { get; set; }
		public LanguageMappingModel SavedLanguageMappingModel { get; set; }
		public List<TranslationModel> EngineModels { get; set; }
		public List<LinguisticOptions> LinguisticOptions { get; set; }
		public string LinguisticOption { get; set; }
	}
}