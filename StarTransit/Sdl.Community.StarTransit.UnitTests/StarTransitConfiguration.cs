using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UnitTests
{
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
				SourceLanguage = new Language(packageModel.LanguagePairs[0].SourceLanguage),
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
				var language = new Language(target);
				targetLanguageList.Add(language);
			}
			var targetArray = targetLanguageList.ToArray();
			return targetArray;
		}
	}
}