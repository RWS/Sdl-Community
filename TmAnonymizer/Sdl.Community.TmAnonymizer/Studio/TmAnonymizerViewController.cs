using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	[View(
		Id = "SdLTmAnonymizer",
		Name = "SDLTm Anonymizer",
		Icon = "icon",
		Description = "Anonymize personal information from tm",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class TmAnonymizerViewController: AbstractViewController
	{
		private static readonly Lazy<TmAnonymizerUserControl> Control = new Lazy<TmAnonymizerUserControl>(() => new TmAnonymizerUserControl(_model));
		private static readonly Lazy<TmAnonymizerExplorerControl> ExplorerControl = new Lazy<TmAnonymizerExplorerControl>(() => new TmAnonymizerExplorerControl(_model));
		private static MainViewModel _model;
		private SettingsService _settingsService;

		protected override void Initialize(IViewContext context)
		{
			_settingsService = new SettingsService(new PathInfo());
			

			_model = new MainViewModel(_settingsService);
		}

		protected override Control GetContentControl()
		{
			return Control.Value;
		}

		protected override Control GetExplorerBarControl()
		{
			return ExplorerControl.Value;
		}

		[RibbonGroup("TmRibbonGroup", "SDLTm Anonymizer user guide")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class TmAnonymizerRibbonGroup : AbstractRibbonGroup
		{
		}

		[Action("Sdl.Community.SDLTmAnonymizer", typeof(TmAnonymizerViewController), Name = "Help", Icon = "wiki", Description = "An wiki page will be opened in browser with user documentation")]
		[ActionLayout(typeof(TmAnonymizerRibbonGroup), 250, DisplayType.Large)]
		public class TmAnonymizerHelpAction : AbstractAction
		{
			protected override void Execute()
			{
				System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdltmanonymizer");
			}
		}
	}
}
