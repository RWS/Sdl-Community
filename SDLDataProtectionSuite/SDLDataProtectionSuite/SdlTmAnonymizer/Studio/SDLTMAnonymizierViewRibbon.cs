using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio
{
	[RibbonGroup("SDPSTmAnonymizerSettingsRibbonGroup", "Configuration")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDPSTmAnonymizerSettingsRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDPSTmAnonymizerTmRibbonGroupSettingsAction", typeof(SDLTMAnonymizerView), Name = " Settings ", Icon = "Settings", Description = "Settings ")]
	[ActionLayout(typeof(SDPSTmAnonymizerSettingsRibbonGroup), 7, DisplayType.Large)]
	public class SDPSTmAnonymizerTmRibbonGroupSettingsAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[RibbonGroup("SDPSTmAnonymizerTMRibbonGroup", "Translation Memories")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDPSTmAnonymizerTmRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDPSTmAnonymizerTmRibbonGroupAddFileBasedTmAction", typeof(SDLTMAnonymizerView), Name = "Add file-based TM", Icon = "TranslationMemory", Description = "Add file-based TM")]
	[ActionLayout(typeof(SDPSTmAnonymizerTmRibbonGroup), 6, DisplayType.Large)]
	public class SDPSTmAnonymizerTmRibbonGroupAddFileBasedTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTmRibbonGroupAddServerTmAction", typeof(SDLTMAnonymizerView), Name = "Add server TM", Icon = "ServerBasedTranslationMemory", Description = "Add server TM")]
	[ActionLayout(typeof(SDPSTmAnonymizerTmRibbonGroup), 5, DisplayType.Large)]
	public class SDPSTmAnonymizerTmRibbonGroupAddServerTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTMRibbonGroupOpenFolderAction", typeof(SDLTMAnonymizerView), Name = "Select Folder", Icon = "TranslationMemoriesFolder_Open", Description = "Add all file-based TMs in the selected folder")]
	[ActionLayout(typeof(SDPSTmAnonymizerTmRibbonGroup), 4, DisplayType.Normal)]
	public class SDPSTmAnonymizerTmRibbonGroupOpenFolderAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTmRibbonGroupRemoveTmAction", typeof(SDLTMAnonymizerView), Name = "Remove TM", Icon = "Remove", Description = "Remove TM")]
	[ActionLayout(typeof(SDPSTmAnonymizerTmRibbonGroup), 3, DisplayType.Normal)]
	public class SDPSTmAnonymizerTmRibbonGroupRemoveTmAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTmRibbonGroupRemoveTmCacheAction", typeof(SDLTMAnonymizerView), Name = "Clear TM Cache", Icon = "RemoveCache", Description = "Clear TM Cache")]
	[ActionLayout(typeof(SDPSTmAnonymizerTmRibbonGroup), 2, DisplayType.Normal)]
	public class SDPSTmAnonymizerTmRibbonGroupRemoveTmCacheAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[RibbonGroup("SDPSTmAnonymizerTasksRibbonGroup", "  Tasks ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDPSTmAnonymizerTasksRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDPSTmAnonymizerTasksRibbonGrouppProcessAction", typeof(SDLTMAnonymizerView), Name = "Preview Changes", Icon = "PreviewChanges", Description = "Preview Changes (Content Filtering Rules)")]
	[ActionLayout(typeof(SDPSTmAnonymizerTasksRibbonGroup), 4, DisplayType.Large)]
	public class SDPSTmAnonymizerTasksRibbonGroupProcessAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTasksRibbonGroupExportAction", typeof(SDLTMAnonymizerView), Name = "Export", Icon = "ExportTo", Description = "Export the selected Rules")]
	[ActionLayout(typeof(SDPSTmAnonymizerTasksRibbonGroup), 3, DisplayType.Normal)]
	public class SDPSTmAnonymizerTasksRibbonGroupExportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[Action("SDPSTmAnonymizerTasksRibbonGroupImportAction", typeof(SDLTMAnonymizerView), Name = "Import", Icon = "ImportFrom", Description = "Import Rules")]
	[ActionLayout(typeof(SDPSTmAnonymizerTasksRibbonGroup), 2, DisplayType.Normal)]
	public class SDPSTmAnonymizerTasksRibbonGroupImportAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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

	[Action("SDPSTmAnonymizerTasksRibbonGroupRemoveAction", typeof(SDLTMAnonymizerView), Name = "Remove ", Icon = "Delete", Description = "Remove the slected Rules")]
	[ActionLayout(typeof(SDPSTmAnonymizerTasksRibbonGroup), 1, DisplayType.Normal)]
	public class SDPSTmAnonymizerTasksRibbonGroupRemoveAction : AbstractViewControllerAction<SDLTMAnonymizerView>
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


	[RibbonGroup("SDPSTmAnonymizerHelpRibbonGroup", "    Help    ")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class SDPSTmAnonymizerHelpRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("SDPSTmAnonymizerHelpRibbonGroupHelpAction", typeof(SDLTMAnonymizerView), Name = "Online Help", Icon = "help", Description = "An wiki page will be opened in browser with user documentation")]
	[ActionLayout(typeof(SDPSTmAnonymizerHelpRibbonGroup), 1, DisplayType.Large)]
	public class SDPSTmAnonymizerHelpRibbonGroupHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdltmanonymizer");
		}
	}

	[Action("SDPSTmAnonymizerHelpRibbonGroupAboutAction", typeof(SDLTMAnonymizerView), Name = "About", Icon = "information", Description = "About")]
	[ActionLayout(typeof(SDPSTmAnonymizerHelpRibbonGroup), 0, DisplayType.Large)]
	public class SDPSTmAnonymizerHelpRibbonGroupAboutAction : AbstractAction
	{
		protected override void Execute()
		{
			var about = new AboutBox();
			about.ShowDialog();
		}
	}
}
