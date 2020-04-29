using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class PreTranslateSegment
	{
		public SearchSettings SearchSettings { get; set; }
		public TranslationUnit TranslationUnit { get; set; }
		public string PlainTranslation { get; set; }
		public string SourceText { get; set; }
		public Segment TranslationSegment { get; set; }
	}
}