using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.Community.TuToTm.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TuToTm.Helpers
{
	public class TmHelper
	{
		private readonly string _tmsConfigPath;

		public TmHelper()
		{
			var studioService = new StudioVersionService();
			var publicVersion = studioService.GetStudioVersion().ExecutableVersion.Major;
			var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			_tmsConfigPath = Path.Combine(localAppDataPath, $@"SDL\SDL Trados Studio\{publicVersion}.0.0.0\TranslationMemoryRepository.xml");
		}

		public List<TmDetails> LoadLocalUserTms()
		{
			var localTms = new List<TmDetails>();
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(_tmsConfigPath);
			if (xmlDocument.DocumentElement == null)
			{
				return null;
			}
			var tmNodes = xmlDocument.SelectNodes("/TranslationMemoryRepository/TranslationMemories/TranslationMemory");
			if (tmNodes != null)
			{
				foreach (XmlElement tmNode in tmNodes)
				{
					var tmPath = tmNode.GetAttribute("path");
					if (!string.IsNullOrEmpty(tmPath))
					{
						var tm = new FileBasedTranslationMemory(tmPath);
						var tmDetails = new TmDetails
						{
							TmPath = tmPath,
							Name = tm.Name,
							SourceFlag = new Language(tm.LanguageDirection.SourceLanguage.Name).GetFlagImage(),
							TargetFlag = new Language(tm.LanguageDirection.TargetLanguage.Name).GetFlagImage(),
							FileBasedTranslationMemory = tm
						};
						localTms.Add(tmDetails);
					}
				}
			}
			return localTms;
		}

		public void AddTu(TmDetails tmDetails,string sourceText,string targetText)
		{
			var tu = new TranslationUnit
			{
				SourceSegment = new Segment(tmDetails.FileBasedTranslationMemory.LanguageDirection.SourceLanguage),
				TargetSegment = new Segment(tmDetails.FileBasedTranslationMemory.LanguageDirection.TargetLanguage)
			};

			tu.SourceSegment.Add(sourceText);
			tu.TargetSegment.Add(targetText);

			tmDetails.FileBasedTranslationMemory.LanguageDirection.AddTranslationUnit(tu, GetImportSettings());
			tmDetails.FileBasedTranslationMemory.Save();
		}

		private ImportSettings GetImportSettings()
		{
			var settings = new ImportSettings
			{
				CheckMatchingSublanguages = true,
				ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge
			};

			return settings;
		}

	}
}
