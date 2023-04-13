﻿namespace LanguageMappingProvider
{
	public class Constants
	{
		public static string MTCloudFolderPath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RWS AppStore", "Language Weaver");
		public static string MTLanguageCodesFilePath = Path.Combine(MTCloudFolderPath, "LanguageWeaverCodes.xlsx");

		public static string ColumnNameLanguage = "Language";
		public static string ColumnNameRegion = "Region";
		public static string ColumnNameTradosCode = "Trados Code";
		public static string ColumnNameMTCode = "LW Code";
		public static string ColumnNameMTCodeLocale = "LW Code (locale)";
	}
}