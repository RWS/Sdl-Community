using System;
using System.Windows.Interop;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[RibbonGroup("SDLMTCloud", Name = "SDLMTCloud_RibbonGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class SdlMTCloudAddInsRibbon : AbstractRibbonGroup
	{
		[Action("SDLMTCloudLanguageMappingShowAction", Name = "SDLMTCloud_Action_LanguageMapping_Name", Icon = "add_langcode", Description = "SDLMTCloud_Action_LanguageMapping_Description")]
		[ActionLayout(typeof(SdlMTCloudAddInsRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class SDLMTCloudLanguageMappingShowAction : AbstractAction
		{
			private readonly Logger _logger = LogManager.GetCurrentClassLogger();

			protected override void Execute()
			{
				try
				{
					var window = new MTCodesWindow();
					var activeForm = StudioInstance.GetActiveForm();
					if (activeForm != null)
					{
						var interopHelper = new WindowInteropHelper(window)
						{
							Owner = activeForm.Handle
						};
					}

					var languages = new LanguageProvider();
					var viewModel = new MTCodesViewModel(window, languages);
					window.DataContext = viewModel;
					window.ShowDialog();
				}

				catch (Exception ex)
				{
					_logger.Error($"{Constants.ExcelExecuteAction} {ex.Message}\n {ex.StackTrace}");
					throw;
				}
			}
		}
	}
}
