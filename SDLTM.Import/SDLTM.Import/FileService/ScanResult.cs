using System.Collections.Generic;
using System.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;

namespace SDLTM.Import.FileService
{
	public class ScanResult
	{
		public Dictionary<CultureInfo, int> EncounteredLanguages = new Dictionary<CultureInfo, int>();
		public Dictionary<LanguagePair, int> LanguageDirections = new Dictionary<LanguagePair, int>();
		public FieldDefinitions FieldDefinitions = new FieldDefinitions();
		public TMXStartOfInputEvent TmxHeader;
		public int RawTUs;
		public int TotalTUs;
	}
}
