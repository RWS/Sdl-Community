using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.Community.Toolkit.LanguagePlatform.ExcelParser;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	[ApplicationInitializer]
	public sealed class AppInitializer : IApplicationInitializer
	{
		private static Constants _constants = new Constants();
		public static List<MTCodeModel> MTCodes = new List<MTCodeModel>();

		public void Execute()
		{
			if (Application.Current == null)
			{
				new Application();
			}

			if (Application.Current != null)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}

			//Load the MTCodes from the local excel when initialize the app
			// The codes are used inside Language Mappings tab
			MTCodes = GetMTCodes();
		}
		
		public static List<ExcelSheet> WriteMTCodesLocally()
		{
			var mtCloudFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _constants.SDLCommunity, _constants.SDLMachineTranslationCloud);
			var excelFilePath = Path.Combine(mtCloudFolderPath, "MTLanguageCodes.xlsx");
			var excelParser = new ExcelParser();

			if (!Directory.Exists(mtCloudFolderPath))
			{
				Directory.CreateDirectory(mtCloudFolderPath);
			}
			WriteExcelLocally(excelFilePath, mtCloudFolderPath);
			return excelParser.ReadExcel(excelFilePath, 0);
		}

		private static void WriteExcelLocally(string excelFilePath, string mtCloudFolderPath)
		{
			try
			{
				var resource = PluginResources.MTLanguageCodes;

				if (!File.Exists(excelFilePath))
				{
					File.WriteAllBytes(Path.Combine(mtCloudFolderPath, "MTLanguageCodes.xlsx"), resource);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{_constants.WriteExcelLocally} {ex.Message}\n {ex.StackTrace}");
				throw;
			}
		}

		private List<MTCodeModel> GetMTCodes()
		{
			return new MTCodesViewModel(WriteMTCodesLocally())?.MTCodes?.ToList();
		}
	}
}