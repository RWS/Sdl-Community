using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio
{
	[RibbonGroup("TmAnonymizerSettingsRibbonGroup", "Configuration")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TmAnonymizerSettingsRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TmAnonymizerTmRibbonGroupSettingsAction", typeof(SDLTMAnonymizerView), Name = " Settings ", Icon = "Settings", Description = "Settings ")]
	[ActionLayout(typeof(TmAnonymizerSettingsRibbonGroup), 7, DisplayType.Large)]
	public class TmAnonymizerTmRibbonGroupSettingsAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			var settingsWindow = new SettingsView();
			var settingsViewModel = new SettingsViewModel(settingsWindow, Controller.SettingsService);
			settingsWindow.DataContext = settingsViewModel;

			settingsWindow.ShowDialog();
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}


	[RibbonGroup("TmAnonymizerTMRibbonGroup", "Translation Memories")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TmAnonymizerTmRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TmAnonymizerTmRibbonGroupAddFileBasedTmAction", typeof(SDLTMAnonymizerView), Name = "Add file-based TM", Icon = "TranslationMemory", Description = "Add file-based TM")]
	[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 6, DisplayType.Large)]
	public class TmAnonymizerTmRibbonGroupAddFileBasedTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			Controller.Model?.TranslationMemoryViewModel.AddFileBasedTm();
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}

	[Action("TmAnonymizerTmRibbonGroupAddServerTmAction", typeof(SDLTMAnonymizerView), Name = "Add server TM", Icon = "ServerBasedTranslationMemory", Description = "Add server TM")]
	[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 5, DisplayType.Large)]
	public class TmAnonymizerTmRibbonGroupAddServerTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			Controller.Model?.TranslationMemoryViewModel.AddServerTm();
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}

	[Action("TmAnonymizerTMRibbonGroupOpenFolderAction", typeof(SDLTMAnonymizerView), Name = "Select Folder", Icon = "TranslationMemoriesFolder_Open", Description = "Add all file-based TMs in the selected folder")]
	[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 4, DisplayType.Normal)]
	public class TmAnonymizerTmRibbonGroupOpenFolderAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			Controller.Model?.TranslationMemoryViewModel.SelectFolder();
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}

	[Action("TmAnonymizerTmRibbonGroupRemoveTmAction", typeof(SDLTMAnonymizerView), Name = "Remove TM", Icon = "Remove", Description = "Remove TM")]
	[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 3, DisplayType.Normal)]
	public class TmAnonymizerTmRibbonGroupRemoveTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			Controller.Model?.TranslationMemoryViewModel.RemoveTmCommand.Execute(null);
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}

	[Action("TmAnonymizerTmRibbonGroupRemoveTmCacheAction", typeof(SDLTMAnonymizerView), Name = "Clear TM Cache", Icon = "RemoveCache", Description = "Clear TM Cache")]
	[ActionLayout(typeof(TmAnonymizerTmRibbonGroup), 2, DisplayType.Normal)]
	public class TmAnonymizerTmRibbonGroupRemoveTmCacheAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			Controller.Model?.TranslationMemoryViewModel.ClearTmCacheCommand.Execute(null);
		}

		public override void Initialize()
		{
			Enabled = Controller.SettingsService.GetSettings().Accepted;
		}
	}

	[RibbonGroup("TmAnonymizerTasksRibbonGroup", "  Tasks ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TmAnonymizerTasksRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TmAnonymizerTasksRibbonGrouppProcessAction", typeof(SDLTMAnonymizerView), Name = "Preview Changes", Icon = "PreviewChanges", Description = "Preview Changes (Content Filtering Rules)")]
	[ActionLayout(typeof(TmAnonymizerTasksRibbonGroup), 4, DisplayType.Large)]
	public class TmAnonymizerTasksRibbonGroupProcessAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			switch (Controller.Model.SelectedTabIndex)
			{
				case 1:
					{
						Controller.Model.SystemFieldsViewModel.ApplyChangesCommand.Execute(null);
						break;
					}
				case 2:
					{
						Controller.Model.CustomFieldsViewModel.ApplyChangesCommand.Execute(null);
						break;
					}
				default:
					{
						Controller.Model.ContentFilteringRulesViewModel.PreviewCommand.Execute(null);
						break;
					}
			}
		}

		public override void Initialize()
		{
			Controller.SelectedTabIndexArgs += Controller_selectedTabIndexArgs;
			Enabled = Controller.SettingsService.GetSettings().Accepted;
			base.Initialize();
		}

		private void Controller_selectedTabIndexArgs(object sender, EventArgs.SelectedTabIndexArgs e)
		{
			switch (e.SelectedIndex)
			{
				case 1:
					{
						Text = "Apply Changes";
						ToolTipText = "Apply Changes (System-Fields)";
						Icon = PluginResources.ApplyChanges;
						break;
					}
				case 2:
					{
						Text = "Apply Changes";
						ToolTipText = "Apply Changes (Custom-Fields)";
						Icon = PluginResources.ApplyChanges;
						break;
					}
				default:
					{
						Text = "Preview Changes";
						ToolTipText = "Preview Changes (Content Filtering Rules)";
						Icon = PluginResources.PreviewChanges;
						break;
					}
			}
		}
	}

	[Action("TmAnonymizerTasksRibbonGroupExportAction", typeof(SDLTMAnonymizerView), Name = "Export", Icon = "ExportTo", Description = "Export the selected Rules")]
	[ActionLayout(typeof(TmAnonymizerTasksRibbonGroup), 3, DisplayType.Normal)]
	public class TmAnonymizerTasksRibbonGroupExportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			switch (Controller.Model.SelectedTabIndex)
			{
				case 1:
					{
						Controller.Model.SystemFieldsViewModel.ExportCommand.Execute(null);
						break;
					}
				case 2:
					{
						Controller.Model.CustomFieldsViewModel.ExportCommand.Execute(null);
						break;
					}
				default:
					{
						Controller.Model.ContentFilteringRulesViewModel.ExportCommand.Execute(null);
						break;
					}
			}
		}


		public override void Initialize()
		{
			Controller.SelectedTabIndexArgs += Controller_selectedTabIndexArgs;
			Enabled = Controller.SettingsService.GetSettings().Accepted;
			base.Initialize();
		}

		private void Controller_selectedTabIndexArgs(object sender, EventArgs.SelectedTabIndexArgs e)
		{
			switch (e.SelectedIndex)
			{
				case 1:
					{
						Text = "Export";
						ToolTipText = "Export the selected System-Fields";
						break;
					}
				case 2:
					{
						Text = "Export";
						ToolTipText = "Export the selected Custom-Fields";
						break;
					}
				default:
					{
						Text = "Export";
						ToolTipText = "Export the selected Rules";
						break;
					}
			}
		}
	}


	[Action("TmAnonymizerTasksRibbonGroupImportAction", typeof(SDLTMAnonymizerView), Name = "Import", Icon = "ImportFrom", Description = "Import Rules")]
	[ActionLayout(typeof(TmAnonymizerTasksRibbonGroup), 2, DisplayType.Normal)]
	public class TmAnonymizerTasksRibbonGroupImportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			switch (Controller.Model.SelectedTabIndex)
			{
				case 1:
					{
						Controller.Model.SystemFieldsViewModel.ImportCommand.Execute(null);
						break;
					}
				case 2:
					{
						Controller.Model.CustomFieldsViewModel.ImportCommand.Execute(null);
						break;
					}
				default:
					{
						Controller.Model.ContentFilteringRulesViewModel.ImportCommand.Execute(null);
						break;
					}
			}
		}

		public override void Initialize()
		{
			Controller.SelectedTabIndexArgs += Controller_selectedTabIndexArgs;
			Enabled = Controller.SettingsService.GetSettings().Accepted;
			base.Initialize();
		}

		private void Controller_selectedTabIndexArgs(object sender, EventArgs.SelectedTabIndexArgs e)
		{
			switch (e.SelectedIndex)
			{
				case 1:
					{
						Text = "Import";
						ToolTipText = "Import System-Fields";
						break;
					}
				case 2:
					{
						Text = "Import";
						ToolTipText = "Import Custom-Fields";
						break;
					}
				default:
					{
						Text = "Import";
						ToolTipText = "Import Rules";
						break;
					}
			}
		}
	}

	[Action("TmAnonymizerTasksRibbonGroupRemoveAction", typeof(SDLTMAnonymizerView), Name = "Remove ", Icon = "Delete", Description = "Remove the slected Rules")]
	[ActionLayout(typeof(TmAnonymizerTasksRibbonGroup), 1, DisplayType.Normal)]
	public class TmAnonymizerTasksRibbonGroupRemoveAction : AbstractViewControllerAction<SDLTMAnonymizerView>
	{
		protected override void Execute()
		{
			if (Controller.Model.SelectedTabIndex == 0)
			{
				Controller.Model.ContentFilteringRulesViewModel.RemoveRuleCommand.Execute(null);
			}
		}

		public override void Initialize()
		{
			Controller.SelectedTabIndexArgs += Controller_selectedTabIndexArgs;
			Enabled = Controller.SettingsService.GetSettings().Accepted;
			base.Initialize();
		}
		
		private void Controller_selectedTabIndexArgs(object sender, EventArgs.SelectedTabIndexArgs e)
		{
			if (e.SelectedIndex == 0)
			{
				ToolTipText = "Remove the slected Rules";
				Enabled = true;
			}
			else
			{
				ToolTipText = string.Empty;
				Enabled = false;
			}
		}
	}


	[RibbonGroup("TmAnonymizerHelpRibbonGroup", "    Help    ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TmAnonymizerHelpRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TmAnonymizerHelpRibbonGroupHelpAction", typeof(SDLTMAnonymizerView), Name = "Online Help", Icon = "help", Description = "An wiki page will be opened in browser with user documentation")]
	[ActionLayout(typeof(TmAnonymizerHelpRibbonGroup), 1, DisplayType.Large)]
	public class TmAnonymizerHelpRibbonGroupHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdltmanonymizer");
		}
	}

	[Action("TmAnonymizerHelpRibbonGroupAboutAction", typeof(SDLTMAnonymizerView), Name = "About", Icon = "information", Description = "About")]
	[ActionLayout(typeof(TmAnonymizerHelpRibbonGroup), 0, DisplayType.Large)]
	public class TmAnonymizerHelpRibbonGroupAboutAction : AbstractAction
	{
		protected override void Execute()
		{
			var about = new AboutBox();
			about.ShowDialog();
		}
	}
}
