using System;
using System.IO;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.Community.Toolkit.LanguagePlatform.ExcelParser;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[RibbonGroup("SDLMTCloud", Name = "SDLMTCloud")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class BeGlobalExcelRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.BeGlobalV4.Provider.Studio", Name = "MT Cloud Codes", Icon = "add_langcode", Description = "Import MT Codes from MTCloud excel sheet")]
		[ActionLayout(typeof(BeGlobalExcelRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class BeGlobalExcelAction : AbstractAction
		{
			private Constants _constants = new Constants();		
			public static readonly Log Log = Log.Instance;

			protected override void Execute()
			{
				try
				{
					var mtCloudFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _constants.SDLCommunity, _constants.SDLMachineTranslationCloud);
					var excelFilePath = Path.Combine(mtCloudFolderPath, "MTLanguageCodes.xlsx");
					var excelParser = new ExcelParser();

					if (!Directory.Exists(mtCloudFolderPath))
					{
						Directory.CreateDirectory(mtCloudFolderPath);
					}

					WriteExcelLocally(excelFilePath, mtCloudFolderPath);
					var excelResults = excelParser.ReadExcel(excelFilePath, 0);

					var mtCodesWindow = new MTCodesWindow();
					var mtCodesViewModel = new MTCodesViewModel(excelResults);
					mtCodesWindow.DataContext = mtCodesViewModel;
					mtCodesWindow.ShowDialog();
				}

				catch (Exception ex)
				{
					Log.Logger.Error($"{_constants.ExcelExecuteAction} {ex.Message}\n {ex.StackTrace}");
					throw;
				}
			}

			private void WriteExcelLocally(string excelFilePath, string mtCloudFolderPath)
			{
				try
				{
					var resource = PluginResources.MTLanguageCodes;

					if (!File.Exists(excelFilePath))
					{
						File.WriteAllBytes(Path.Combine(mtCloudFolderPath, "MTLanguageCodes.xlsx"), resource);
					}
				}
				catch(Exception ex)
				{
					Log.Logger.Error($"{_constants.WriteExcelLocally} {ex.Message}\n {ex.StackTrace}");
					throw;
				}
			}
		}
	}
}
