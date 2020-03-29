using System;
using System.Windows.Forms;
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
			public static readonly Log Log = Log.Instance;

			protected override void Execute()
			{
				try
				{
					var allForms = Application.OpenForms;
					var activeForm = allForms[allForms.Count - 1];
					foreach (Form form in allForms)
					{
						if (form.GetType().Name == "StudioWindowForm")
						{
							activeForm = form;
							break;
						}
					}
					
					var window = new MTCodesWindow();
					var helper = new WindowInteropHelper(window)
					{
						Owner = activeForm.Handle
					};


					var languages = new Languages.Provider.Languages();

					var mtCodesViewModel = new MTCodesViewModel(languages);
					window.DataContext = mtCodesViewModel;
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
