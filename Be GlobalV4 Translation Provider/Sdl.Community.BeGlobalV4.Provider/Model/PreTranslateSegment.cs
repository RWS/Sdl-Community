using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class PreTranslateSegment
	{
		public SearchSettings SearchSettings { get; set; }
		public TranslationUnit TranslationUnit { get; set; }
		public string Id { get; set; }
		public string Translation { get; set; }
	}
}
