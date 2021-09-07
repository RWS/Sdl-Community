using System;
using RwsAppStore.UsefulTipsService;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.EventArgs;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using UserControl = System.Windows.Forms.UserControl;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio
{
	[View(
		Id = "SDLTMAnonymizerView",
		Name = "TradosTM Anonymizer",
		Icon = "SDLTMAnonymizer",
		Description = "Anonymize personal information in Translation Memories",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
		AllowViewParts = true)]
	public class SDLTMAnonymizerView : AbstractViewController
	{
		private TmAnonymizerViewControl _control;
		private TmAnonymizerExplorerControl _explorerControl;
		private SDLTMAnonymizerLogViewPart _logViewController;

		internal MainViewModel Model;
		internal SettingsService SettingsService;
		internal event EventHandler<SelectedTabIndexArgs> SelectedTabIndexArgs;

		public UserControl ContentControl => _control;

		protected override void Initialize(IViewContext context)
		{
			SettingsService = new SettingsService(new PathInfo());

			Model = new MainViewModel(SettingsService, this);
			Model.PropertyChanged += Model_PropertyChanged;
			Model.SelectedTabIndex = 0;

			_explorerControl = new TmAnonymizerExplorerControl(Model);
			_logViewController = new SDLTMAnonymizerLogViewPart(Model);

			if (SettingsService.GetSettings().Accepted)
			{
				Model.LogViewModel.IsEnabled = true;
				_control = new TmAnonymizerViewControl(Model);

				var tipsProvider = new TipsProvider(new RwsAppStore.UsefulTipsService.Model.PathInfo("15"));
				var usefulTipsService = new UsefulTipsService(tipsProvider, SettingsService);
				var usefulTips = usefulTipsService.GetPluginUsefulTips();

				var tipsInstalled = usefulTipsService.TipsInstalled("SDLTMAnonymizerView");
				if (tipsInstalled == 0)
				{
					usefulTipsService.AddUsefulTips(usefulTips, StringResources.SDLTM_Anonymizer_Name);
				}

				// Debug
				//var count = usefulTipsService.RemoveUsefulTips(usefulTips, StringResources.SDLTM_Anonymizer_Name);
			}
			else
			{
				Model.LogViewModel.IsEnabled = false;
			}
		}

		protected override System.Windows.Forms.Control GetContentControl()
		{
			return _control;
		}

		protected override System.Windows.Forms.Control GetExplorerBarControl()
		{
			return _explorerControl;
		}

		public override void Dispose()
		{
			Model.PropertyChanged -= Model_PropertyChanged;
			base.Dispose();
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.EndsWith(nameof(MainViewModel.SelectedTabIndex)))
			{
				SelectedTabIndexArgs?.Invoke(this, new SelectedTabIndexArgs { SelectedIndex = Model.SelectedTabIndex });
			}
		}
	}
}
