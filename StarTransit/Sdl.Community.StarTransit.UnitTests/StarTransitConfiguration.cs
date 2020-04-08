using System;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
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
	}
}