using System.IO;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class ExcelFileSniffer : INativeFileSniffer
	{
		//private LanguageMappingSettings _languageMappingSettings;

		//private void OverrideSettings(ISettingsGroup settingsGroup)
		//{
		//	if (settingsGroup == null)
		//	{
		//		return;
		//	}

		//	_languageMappingSettings = new LanguageMappingSettings();
		//	_languageMappingSettings.PopulateFromSettingsBundle(settingsGroup.SettingsBundle, FiletypeConstants.FileTypeDefinitionId);
		//}

		public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
			INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
		{
			//OverrideSettings(settingsGroup);

			var info = new SniffInfoWithNativePath(nativeFilePath);
			//Encoding suggestedEncoding = null;
			//if (suggestedCodepage != null)
			//	suggestedEncoding = suggestedCodepage.Encoding;

			//if (suggestedEncoding == null && suggestedSourceLanguage?.CultureInfo != null)
			//{
			//	var codepage = FileEncoding.GetDefaultAnsiCodepage(suggestedSourceLanguage.CultureInfo);
			//	if (codepage != 0)
			//		suggestedEncoding = Encoding.GetEncoding(codepage);
			//}

			//info.DetectedEncoding = FileEncoding.Detect(nativeFilePath, suggestedEncoding, out var lineBreakType, out var hasUTF8Bom);
			//info.SetMetaData("LineBreakType", lineBreakType);
			//info.SetMetaData("HasUTF8Bom", hasUTF8Bom.ToString());

			//var encoding = info.DetectedEncoding.First.Encoding ?? Encoding.UTF8;
			
			if (File.Exists(nativeFilePath))
			{
				info.IsSupported = IsFileSupported(nativeFilePath);
			}

			return info;
		}

		private bool IsFileSupported(string filePath)
		{
			
			return true;
		}
	}
}
