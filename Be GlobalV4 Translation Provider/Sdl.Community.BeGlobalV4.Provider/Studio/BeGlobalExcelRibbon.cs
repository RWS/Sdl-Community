using System;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
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
					var mtCodesWindow = new MTCodesWindow();
					var mtCodesViewModel = new MTCodesViewModel(AppInitializer.WriteMTCodesLocally());
					mtCodesWindow.DataContext = mtCodesViewModel;
					mtCodesWindow.ShowDialog();
				}

				catch (Exception ex)
				{
					Log.Logger.Error($"{_constants.ExcelExecuteAction} {ex.Message}\n {ex.StackTrace}");
					throw;
				}
			}
		}
	}
}
