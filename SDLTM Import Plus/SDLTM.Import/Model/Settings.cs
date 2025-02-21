using Sdl.LanguagePlatform.TranslationMemory;

namespace SDLTM.Import.Model
{
	public class Settings
	{
		public bool ImportPlain { get; set; }
		public bool ExcludeVariantsForXliff { get; set; }
		public bool ExcludeVariantsForTmx { get; set; }
		public bool UseBilingualInfo { get; set; }
		public ImportSettings.TUUpdateMode TuUpdateMode { get; set; }
	}
}
