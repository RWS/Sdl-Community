using System;
using System.Windows.Interop;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[RibbonGroup("SDLMTCloud", Name = "SDLMTCloud")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class SdlMTCloudExcelRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.MTCloud.Provider.Studio", Name = "MT Cloud Codes", Icon = "add_langcode", Description = "Import MT Codes from MTCloud excel sheet")]
		[ActionLayout(typeof(SdlMTCloudExcelRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class BeGlobalExcelAction : AbstractAction
		{			
			protected override void Execute()
			{
				try
				{										
					var window = new MTCodesWindow();
					var interopHelper = new WindowInteropHelper(window)
					{
						Owner = StudioInstance.GetActiveForm().Handle
					};

					var languages = new Languages.Provider.Languages();
					var viewModel = new MTCodesViewModel(window, languages);
					window.DataContext = viewModel;
					window.ShowDialog();
				}

				catch (Exception ex)
				{
					Log.Logger.Error($"{Constants.ExcelExecuteAction} {ex.Message}\n {ex.StackTrace}");
					throw;
				}
			}
		}
	}
}
