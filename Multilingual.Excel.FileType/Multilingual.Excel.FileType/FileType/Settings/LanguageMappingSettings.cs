using System.Collections.Generic;
using System.Linq;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Newtonsoft.Json;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.Excel.FileType.FileType.Settings
{
	public class LanguageMappingSettings : AbstractSettingsClass
	{
		private const string LanguageMappingFirstRowIndexSetting = "LanguageMappingFirstRowIndex";
		private readonly int _languageMappingFirstRowIndexDefault;

		private const string LanguageMappingFirstRowIsHeadingSetting = "LanguageMappingFirstRowIsHeading";
		private readonly bool _languageMappingFirstRowIsHeadingDefault;

		private const string LanguageMappingReadAllWorkSheetsSetting = "LanguageMappingReadAllWorkSheets";
		private readonly bool _languageMappingReadAllWorkSheetsDefault;

		private const string LanguageMappingLanguagesSetting = "LanguageMappingLanguages";
		private readonly List<LanguageMapping> _languageMappingLanguagesDefault;

		private readonly List<Language> _allLanguages;
		private readonly ImageService _imageService;

		public LanguageMappingSettings()
		{
			_imageService = new ImageService();
			_allLanguages = LanguageRegistryApi.Instance.GetAllLanguages().ToList();

			_languageMappingFirstRowIndexDefault = 1;
			_languageMappingFirstRowIsHeadingDefault = true;
			_languageMappingReadAllWorkSheetsDefault = false;
			_languageMappingLanguagesDefault = new List<LanguageMapping>();
			
			ResetToDefaults();
		}

		public int LanguageMappingFirstRowIndex { get; set; }

		public bool LanguageMappingFirstRowIsHeading { get; set; }

		public bool LanguageMappingReadAllWorkSheets { get; set; }

		public List<LanguageMapping> LanguageMappingLanguages { get; set; }

		public override string SettingName => "MultilingualExcelLanguageMappingSettings";

		public override void Read(IValueGetter valueGetter)
		{
			LanguageMappingFirstRowIndex = valueGetter.GetValue(LanguageMappingFirstRowIndexSetting, _languageMappingFirstRowIndexDefault);
			LanguageMappingFirstRowIsHeading = valueGetter.GetValue(LanguageMappingFirstRowIsHeadingSetting, _languageMappingFirstRowIsHeadingDefault);
			LanguageMappingReadAllWorkSheets = valueGetter.GetValue(LanguageMappingReadAllWorkSheetsSetting, _languageMappingReadAllWorkSheetsDefault);
			LanguageMappingLanguages = JsonConvert.DeserializeObject<List<LanguageMapping>>(valueGetter.GetValue(LanguageMappingLanguagesSetting,
				JsonConvert.SerializeObject(_languageMappingLanguagesDefault)));

			foreach (var parserTargetLanguage in LanguageMappingLanguages)
			{
				var targetLanguage = _allLanguages.FirstOrDefault(a => a.CultureInfo.Name == parserTargetLanguage.LanguageId);
				parserTargetLanguage.DisplayName = targetLanguage?.DisplayName;
				parserTargetLanguage.Image = _imageService.GetImage(parserTargetLanguage.LanguageId);
				parserTargetLanguage.ContentColumn = parserTargetLanguage.ContentColumn.ToUpper();
				parserTargetLanguage.CharacterLimitationColumn = parserTargetLanguage.CharacterLimitationColumn?.ToUpper();
				parserTargetLanguage.LineLimitationColumn = parserTargetLanguage.LineLimitationColumn?.ToUpper();
				parserTargetLanguage.PixelLimitationColumn = parserTargetLanguage.PixelLimitationColumn?.ToUpper();
				parserTargetLanguage.PixelFontFamilyColumn = parserTargetLanguage.PixelFontFamilyColumn?.ToUpper();
				parserTargetLanguage.PixelFontSizeColumn = parserTargetLanguage.PixelFontSizeColumn?.ToUpper();
				parserTargetLanguage.ContextColumn = parserTargetLanguage.ContextColumn?.ToUpper();
				parserTargetLanguage.CommentColumn = parserTargetLanguage.CommentColumn?.ToUpper();
			}
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(LanguageMappingFirstRowIndexSetting, LanguageMappingFirstRowIndex, _languageMappingFirstRowIndexDefault);
			valueProcessor.Process(LanguageMappingFirstRowIsHeadingSetting, LanguageMappingFirstRowIsHeading, _languageMappingFirstRowIsHeadingDefault);
			valueProcessor.Process(LanguageMappingReadAllWorkSheetsSetting, LanguageMappingReadAllWorkSheets, _languageMappingReadAllWorkSheetsDefault);
			valueProcessor.Process(LanguageMappingLanguagesSetting, JsonConvert.SerializeObject(LanguageMappingLanguages),
				JsonConvert.SerializeObject(_languageMappingLanguagesDefault));
		}

		public override object Clone()
		{
			return new LanguageMappingSettings
			{
				LanguageMappingFirstRowIndex = LanguageMappingFirstRowIndex,
				LanguageMappingFirstRowIsHeading = LanguageMappingFirstRowIsHeading,
				LanguageMappingReadAllWorkSheets = LanguageMappingReadAllWorkSheets,
				LanguageMappingLanguages = LanguageMappingLanguages
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is LanguageMappingSettings otherSetting &&
				   otherSetting.LanguageMappingFirstRowIndex == LanguageMappingFirstRowIndex &&
				   otherSetting.LanguageMappingFirstRowIsHeading == LanguageMappingFirstRowIsHeading &&
				   otherSetting.LanguageMappingReadAllWorkSheets == LanguageMappingReadAllWorkSheets &&
				   otherSetting.LanguageMappingLanguages == LanguageMappingLanguages;
		}

		public sealed override void ResetToDefaults()
		{
			LanguageMappingFirstRowIndex = _languageMappingFirstRowIndexDefault;
			LanguageMappingFirstRowIsHeading = _languageMappingFirstRowIsHeadingDefault;
			LanguageMappingReadAllWorkSheets = _languageMappingReadAllWorkSheetsDefault;
			LanguageMappingLanguages = _languageMappingLanguagesDefault;
		}
	}
}
