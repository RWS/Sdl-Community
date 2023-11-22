using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UnitTests
{
	//TODO: Use IPackageService
	public class StarTransitConfiguration
	{
		private PackageService _packageService;
		public StarTransitConfiguration(PackageService packageService)
		{
			_packageService = packageService;
		}

		public async Task<PackageModel> GetPackageModel(string filePath, string fileName)
		{
			var tempPath = CreateTempPackageFolder();
			var packagePath = Path.Combine(filePath, fileName);
			var packageModel = await _packageService.OpenPackage(packagePath, tempPath);
			return packageModel;
		}

		private string CreateTempPackageFolder()
		{
			var tempFolder = $@"C:\Users\{Environment.UserName}\StarTransit";
			var pathToTempFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString());

			if (Directory.Exists(pathToTempFolder))
			{
				Directory.Delete(pathToTempFolder, true);
			}
			Directory.CreateDirectory(pathToTempFolder);
			return pathToTempFolder;
		}

		public ProjectTemplateInfo SetTemplateInfo(string filePath, string templateName)
		{
			var templatePath = Path.Combine(filePath, templateName);
			var projectTemplateInfo = new ProjectTemplateInfo
			{
				Description = "Default template",
				Name = "Default",
				Id = Guid.NewGuid(),
				Uri = new Uri(templatePath)
			};
			return projectTemplateInfo;
		}

		public ProjectInfo SetProjectInfo(PackageModel packageModel, string tempTestFolder)
		{
			return new ProjectInfo
			{
				Name = packageModel.Name,
				LocalProjectFolder = Path.Combine(tempTestFolder, packageModel.Name),
				SourceLanguage = LanguageRegistryApi.Instance.GetLanguage(packageModel.LanguagePairs[0].SourceLanguage.Name),
				TargetLanguages = GetTargetLanguages(packageModel.LanguagePairs),
				DueDate = packageModel.DueDate
			};
		}

		public MessageModel SetMessageModel(bool isProjectCreated, string message)
		{
			return new MessageModel
			{
				IsProjectCreated = isProjectCreated,
				Message = message,
			};
		}

		public Language[] GetTargetLanguages(List<LanguagePair> languagePairs)
		{
			var targetCultureInfoList = new List<CultureInfo>();
			foreach (var pair in languagePairs)
			{
				var targetLanguage = pair.TargetLanguage;
				targetCultureInfoList.Add(targetLanguage);
			}
			var targetLanguageList = new List<Language>();
			foreach (var target in targetCultureInfoList)
			{
				var language = LanguageRegistryApi.Instance.GetLanguage(target.Name);
				targetLanguageList.Add(language);
			}
			var targetArray = targetLanguageList.ToArray();
			return targetArray;
		}

		public StarTranslationMemoryMetadata GetTmsOrMtMetadataFromList(List<StarTranslationMemoryMetadata> currentList, bool tmFiles)
		{
			if (tmFiles)
			{
				return currentList.FirstOrDefault(n => !n.Name.Contains("MT"));
			}

			return currentList.FirstOrDefault(n => n.Name.Contains("MT"));
		}

		//Source files, Target files
		public (List<string>, List<string>) GetTmsForLanguagePair(List<LanguagePair> languagePairs,
			LanguagePair languagePair)
		{
			var sourceFilesPaths = new List<string>();
			var targetFilesPaths = new List<string>();

			var langPair = languagePairs.FirstOrDefault(lp => lp.TargetLanguage.Equals(languagePair.TargetLanguage));
			if (langPair != null)
			{
				foreach (var tm in langPair.StarTranslationMemoryMetadatas)
				{
					sourceFilesPaths.AddRange(tm.TransitTmsSourceFilesPath);
					targetFilesPaths.AddRange(tm.TransitTmsTargeteFilesPath);
				}
			}

			return (sourceFilesPaths, targetFilesPaths);
		}
	}
}