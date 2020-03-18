using System;
using System.IO;

namespace Sdl.Community.MTCloud.Languages.Provider
{
	public class Constants
	{				
		public static string MTCloudFolderPath = 
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "SDLMachineTranslationCloud");
		public static string MTLanguageCodesFilePath = Path.Combine(MTCloudFolderPath, "MTLanguageCodes.xlsx");

		public static string ColumnNameLanguage = "Language";
		public static string ColumnNameRegion = "Region";
		public static string ColumnNameTradosCode = "Trados Code";
		public static string ColumnNameMTCode = "MT Code";
		public static string ColumnNameMTCodeLocale = "MT Code (locale)";
	}
}
