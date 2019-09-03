using System;
using System.IO;
using System.Reflection;
using SdlCommunityUnitTest.Services;
using Xunit;

namespace SdlCommunityUnitTest
{
	public class PublicApiAssembliesVersions
	{
		private readonly string _studioFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "SDL", "SDL Trados Studio");
		private readonly PublicApiVersion _publicApiService = new PublicApiVersion();

		[Theory]
		[InlineData("Studio15")]
		public void CheckAssembliesVersions(string studioVersion)
		{
			var studioVersionFolderPath = Path.Combine(_studioFolderPath,studioVersion);

			var pluginConfigFile = _publicApiService.LoadConfigFile(studioVersionFolderPath);
			var publicAssembliesNodeList = _publicApiService.GetPublicAssembliesNodeList(pluginConfigFile);
			var pluginCongigApiDetails = _publicApiService.GetPublicApiDetails(publicAssembliesNodeList);

			foreach (var pluginConfigApi in pluginCongigApiDetails)
			{
				var publicApiPath = Path.Combine(studioVersionFolderPath, pluginConfigApi.ApiName);
				if (File.Exists(publicApiPath))
				{
					var publicApiAssembly = Assembly.LoadFile(publicApiPath);
					var assemblyVersion = publicApiAssembly.GetName().Version;
					var majorAndMinorVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}";

					var areTheSame = pluginConfigApi.Version.Equals(majorAndMinorVersion);
					Assert.True(areTheSame, $"Version of {pluginConfigApi.ApiName} is different: config version : {pluginConfigApi.Version}, actual dll version: {majorAndMinorVersion}");
				}
			}
		}

		[Theory]
		[InlineData("Studio15")]
		public void AllApiExists(string studioVersion)
		{
			var studioVersionFolderPath = Path.Combine(_studioFolderPath, studioVersion);

			var pluginConfigFile = _publicApiService.LoadConfigFile(studioVersionFolderPath);
			var publicAssembliesNodeList = _publicApiService.GetPublicAssembliesNodeList(pluginConfigFile);
			var pluginCongigApiDetails = _publicApiService.GetPublicApiDetails(publicAssembliesNodeList);

			foreach (var pluginConfigApi in pluginCongigApiDetails)
			{
				var publicApiPath = Path.Combine(studioVersionFolderPath, pluginConfigApi.ApiName);
				Assert.True(File.Exists(publicApiPath), $"{pluginConfigApi.ApiName} does not exist in Studio {studioVersion} folder");
			}
		}
	}
}

