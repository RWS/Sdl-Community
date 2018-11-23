using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio
{
	[RibbonGroup("SDLTMAnonymizerSettingsRibbonGroup", "Configuration")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDLTMAnonymizerSettingsRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDLTMAnonymizerRibbonGroupSettingsAction", typeof(SDLTMAnonymizerView), Name = " Settings ", Icon = "Settings", Description = "Settings ")]
	[ActionLayout(typeof(SDLTMAnonymizerSettingsRibbonGroup), 7, DisplayType.Large)]
	public class SDLTMAnonymizerRibbonGroupSettingsAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[RibbonGroup("SDLTMAnonymizerRibbonGroup", "Translation Memories")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDLTMAnonymizerRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDLTMAnonymizerRibbonGroupAddFileBasedTmAction", typeof(SDLTMAnonymizerView), Name = "Add file-based TM", Icon = "TranslationMemory", Description = "Add file-based TM")]
	[ActionLayout(typeof(SDLTMAnonymizerRibbonGroup), 6, DisplayType.Large)]
	public class SDLTMAnonymizerRibbonGroupAddFileBasedTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerRibbonGroupAddServerTmAction", typeof(SDLTMAnonymizerView), Name = "Add server TM", Icon = "ServerBasedTranslationMemory", Description = "Add server TM")]
	[ActionLayout(typeof(SDLTMAnonymizerRibbonGroup), 5, DisplayType.Large)]
	public class SDLTMAnonymizerRibbonGroupAddServerTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerRibbonGroupOpenFolderAction", typeof(SDLTMAnonymizerView), Name = "Select Folder", Icon = "TranslationMemoriesFolder_Open", Description = "Add all file-based TMs in the selected folder")]
	[ActionLayout(typeof(SDLTMAnonymizerRibbonGroup), 4, DisplayType.Normal)]
	public class SDLTMAnonymizerRibbonGroupOpenFolderAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerRibbonGroupRemoveTmAction", typeof(SDLTMAnonymizerView), Name = "Remove TM", Icon = "Remove", Description = "Remove TM")]
	[ActionLayout(typeof(SDLTMAnonymizerRibbonGroup), 3, DisplayType.Normal)]
	public class SDLTMAnonymizerRibbonGroupRemoveTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerRibbonGroupRemoveTmCacheAction", typeof(SDLTMAnonymizerView), Name = "Clear TM Cache", Icon = "RemoveCache", Description = "Clear TM Cache")]
	[ActionLayout(typeof(SDLTMAnonymizerRibbonGroup), 2, DisplayType.Normal)]
	public class SDLTMAnonymizerRibbonGroupRemoveTmCacheAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[RibbonGroup("SDLTMAnonymizerTasksRibbonGroup", "  Tasks ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDLTMAnonymizerTasksRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDLTMAnonymizerTasksRibbonGrouppProcessAction", typeof(SDLTMAnonymizerView), Name = "Preview Changes", Icon = "PreviewChanges", Description = "Preview Changes (Content Filtering Rules)")]
	[ActionLayout(typeof(SDLTMAnonymizerTasksRibbonGroup), 4, DisplayType.Large)]
	public class SDLTMAnonymizerTasksRibbonGroupProcessAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerTasksRibbonGroupExportAction", typeof(SDLTMAnonymizerView), Name = "Export", Icon = "ExportTo", Description = "Export the selected Rules")]
	[ActionLayout(typeof(SDLTMAnonymizerTasksRibbonGroup), 3, DisplayType.Normal)]
	public class SDLTMAnonymizerTasksRibbonGroupExportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[Action("SDLTMAnonymizerTasksRibbonGroupImportAction", typeof(SDLTMAnonymizerView), Name = "Import", Icon = "ImportFrom", Description = "Import Rules")]
	[ActionLayout(typeof(SDLTMAnonymizerTasksRibbonGroup), 2, DisplayType.Normal)]
	public class SDLTMAnonymizerTasksRibbonGroupImportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDLTMAnonymizerTasksRibbonGroupRemoveAction", typeof(SDLTMAnonymizerView), Name = "Remove ", Icon = "Delete", Description = "Remove the slected Rules")]
	[ActionLayout(typeof(SDLTMAnonymizerTasksRibbonGroup), 1, DisplayType.Normal)]
	public class SDLTMAnonymizerTasksRibbonGroupRemoveAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[RibbonGroup("SDLTMAnonymizerHelpRibbonGroup", "    Help    ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDLTMAnonymizerHelpRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDLTMAnonymizerHelpRibbonGroupHelpAction", typeof(SDLTMAnonymizerView), Name = "Online Help", Icon = "help", Description = "An wiki page will be opened in browser with user documentation")]
	[ActionLayout(typeof(SDLTMAnonymizerHelpRibbonGroup), 1, DisplayType.Large)]
	public class SDLTMAnonymizerHelpRibbonGroupHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdltmanonymizer");
		}
	}

	[Action("SDLTMAnonymizerHelpRibbonGroupAboutAction", typeof(SDLTMAnonymizerView), Name = "About", Icon = "information", Description = "About")]
	[ActionLayout(typeof(SDLTMAnonymizerHelpRibbonGroup), 0, DisplayType.Large)]
	public class SDLTMAnonymizerHelpRibbonGroupAboutAction : AbstractAction
	{
		protected override void Execute()
		{
			var about = new AboutBox();
			about.ShowDialog();
		}
	}
}
