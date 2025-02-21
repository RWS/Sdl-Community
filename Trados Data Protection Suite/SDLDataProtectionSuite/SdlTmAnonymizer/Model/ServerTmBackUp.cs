using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class ServerTmBackUp
	{
		public ScheduledServerTranslationMemoryExport ScheduledExport { get; set; }
		public string FilePath { get; set; }
	}
}
