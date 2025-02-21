using System.Collections.Generic;
using System.Linq;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services;
using Newtonsoft.Json;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.FileType.Settings
{
	public class LanguageMappingSettings : AbstractSettingsClass
	{
		private const string LanguageMappingLanguagesXPathSetting = "LanguageMappingLanguagesXPath";
		private readonly string _languageMappingLanguagesXPathDefault;

		private const string LanguageMappingCommentsXPathSetting = "LanguageMappingCommentsXPath";
		private readonly string _languageMappingCommentsXPathDefault;

		private const string LanguageMappingMonoLanguageSetting = "LanguageMappingMonoLanguage";
		private readonly bool _languageMappingMonoLanguageDefault;

		private const string LanguageMappingLanguagesSetting = "LanguageMappingLanguages";
		private readonly List<LanguageMapping> _languageMappingLanguagesDefault;

		private readonly List<Language> _allLanguages;
		private readonly ImageService _imageService;

		public LanguageMappingSettings()
		{
			_imageService = new ImageService();
			_allLanguages = LanguageRegistryApi.Instance.GetAllLanguages().ToList();

			_languageMappingLanguagesXPathDefault = "/";
			_languageMappingCommentsXPathDefault = "";
			_languageMappingLanguagesDefault = new List<LanguageMapping>();

			_languageMappingMonoLanguageDefault = false;

			ResetToDefaults();
		}

		public string LanguageMappingLanguagesXPath { get; set; }

		public string LanguageMappingCommentsXPath { get; set; }

		public List<LanguageMapping> LanguageMappingLanguages { get; set; }

		public bool LanguageMappingMonoLanguage { get; set; }

		public override string SettingName => "MultilingualXmlLanguageMappingSettings";

		public override void Read(IValueGetter valueGetter)
		{
			LanguageMappingLanguagesXPath = valueGetter.GetValue(LanguageMappingLanguagesXPathSetting, _languageMappingLanguagesXPathDefault);
			LanguageMappingCommentsXPath = valueGetter.GetValue(LanguageMappingCommentsXPathSetting, _languageMappingCommentsXPathDefault);
			LanguageMappingLanguages = JsonConvert.DeserializeObject<List<LanguageMapping>>(valueGetter.GetValue(LanguageMappingLanguagesSetting,
				JsonConvert.SerializeObject(_languageMappingLanguagesDefault)));

			LanguageMappingMonoLanguage = valueGetter.GetValue(LanguageMappingMonoLanguageSetting, _languageMappingMonoLanguageDefault);

			foreach (var parserTargetLanguage in LanguageMappingLanguages)
			{
				var targetLanguage = _allLanguages
					.FirstOrDefault(a => a.CultureInfo.Name == parserTargetLanguage.LanguageId);
				parserTargetLanguage.DisplayName = targetLanguage?.DisplayName;
				parserTargetLanguage.Image = _imageService.GetImage(parserTargetLanguage.LanguageId);
			}
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(LanguageMappingLanguagesXPathSetting, LanguageMappingLanguagesXPath, _languageMappingLanguagesXPathDefault);
			valueProcessor.Process(LanguageMappingCommentsXPathSetting, LanguageMappingCommentsXPath, _languageMappingCommentsXPathDefault);
			valueProcessor.Process(LanguageMappingLanguagesSetting, JsonConvert.SerializeObject(LanguageMappingLanguages),
				JsonConvert.SerializeObject(_languageMappingLanguagesDefault));

			valueProcessor.Process(LanguageMappingMonoLanguageSetting, LanguageMappingMonoLanguage, _languageMappingMonoLanguageDefault);
		}

		public override object Clone()
		{
			return new LanguageMappingSettings
			{
				LanguageMappingLanguagesXPath = LanguageMappingLanguagesXPath,
				LanguageMappingCommentsXPath = LanguageMappingCommentsXPath,
				LanguageMappingLanguages = LanguageMappingLanguages,
				LanguageMappingMonoLanguage = LanguageMappingMonoLanguage
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is LanguageMappingSettings otherSetting &&
				   otherSetting.LanguageMappingLanguagesXPath == LanguageMappingLanguagesXPath &&
				   otherSetting.LanguageMappingCommentsXPath == LanguageMappingCommentsXPath &&
				   otherSetting.LanguageMappingLanguages == LanguageMappingLanguages &&
				   otherSetting.LanguageMappingMonoLanguage == LanguageMappingMonoLanguage;
		}

		public sealed override void ResetToDefaults()
		{
			LanguageMappingLanguagesXPath = _languageMappingLanguagesXPathDefault;
			LanguageMappingCommentsXPath = _languageMappingCommentsXPathDefault;
			LanguageMappingLanguages = _languageMappingLanguagesDefault;
			LanguageMappingMonoLanguage = _languageMappingMonoLanguageDefault;
		}
	}
}
