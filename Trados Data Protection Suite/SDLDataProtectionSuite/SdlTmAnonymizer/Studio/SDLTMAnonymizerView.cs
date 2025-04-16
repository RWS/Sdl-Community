using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.EventArgs;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using PathInfo = Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.PathInfo;
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
			SettingsService = ApplicationContext.SettingsService;

			Model = new MainViewModel(SettingsService, this, new GroupshareCredentialManager());
			Model.PropertyChanged += Model_PropertyChanged;
			Model.SelectedTabIndex = 0;

			_explorerControl = new TmAnonymizerExplorerControl(Model);
			_logViewController = new SDLTMAnonymizerLogViewPart(Model);

			if (SettingsService.GetSettings().Accepted)
			{
				Model.LogViewModel.IsEnabled = true;
				_control = new TmAnonymizerViewControl(Model);


                var majorStudioVersion = Versions.GetBuildVersion().Split('.').FirstOrDefault();

                var pathInfo = new RwsAppStore.UsefulTipsService.Model.PathInfo(PluginResources.Plugin_Name, majorStudioVersion);
				var tipsProvider = new TipsProvider(pathInfo);
				var usefulTipsService = new UsefulTipsService(tipsProvider, SettingsService.PathInfo);

				var pluginTips = usefulTipsService.GetPluginUsefulTips();

                var studioTips = TryGetStudioTips(tipsProvider);
				AlignLanguageTipIds(studioTips, pluginTips, usefulTipsService);

				var tipsInstalled = TipsInstalled(studioTips, "SDLTMAnonymizerView");
				if (tipsInstalled == 0)
				{
					var importTips = new ImportTips { TipLanguages = pluginTips };
					var countInstalled = usefulTipsService.AddUsefulTips(importTips, true);

					Trace.WriteLine(string.Format("Installed {0}", countInstalled));
				}


				//var tipReferences = new List<TipReference>();
				//foreach (var tipLanguage in tipLanguages)
				//{
				//	tipReferences.Add(new TipReference
				//	{
				//		LanguageId = tipLanguage.LanguageId,
				//		Resources = tipLanguage.Resources.Select(a => a.RelativePath).ToList(),
				//		TipIds = tipLanguage.Tips.Where(a => !string.IsNullOrEmpty(a.Id)).Select(a => a.Id).ToList()
				//	});
				//}

				//var removeTips = new RemoveTips { TipReferences = tipReferences };

				//var countRemoved = usefulTipsService.RemoveUsefulTips(removeTips);
				//Trace.WriteLine(string.Format("Removed {0}", countRemoved));

				// Debug
				//var count = usefulTipsService.RemoveUsefulTips(usefulTips, StringResources.SDLTM_Anonymizer_Name);
			}
			else
			{
				Model.LogViewModel.IsEnabled = false;
			}
		}

        private List<TipLanguage> TryGetStudioTips(TipsProvider tipsProvider)
        {
            try
            {
                var studioTips = tipsProvider.GetStudioTips();
                return studioTips;
            }
            catch
            {
                return [];
            }
        }

        private static void AlignLanguageTipIds(IReadOnlyCollection<TipLanguage> studioTips, List<TipLanguage> pluginTips, UsefulTipsService usefulTipsService)
		{
			if (pluginTips == null || studioTips == null)
			{
				return;
			}

			foreach (var tipLanguage in pluginTips)
			{
				var installedTipLanguages = studioTips.FirstOrDefault(a => a.LanguageId == tipLanguage.LanguageId);
				if (installedTipLanguages == null)
				{
					continue;
				}

				foreach (var languageTip in tipLanguage.Tips)
				{
					var existingTip = usefulTipsService.GetExistingTip(installedTipLanguages.Tips, languageTip);
					if (existingTip != null)
					{
						languageTip.Id = existingTip.Id;
					}
				}
			}
		}

		public int TipsInstalled(List<TipLanguage> studioTips, string context)
		{
			return studioTips.Sum(tipContext => tipContext.Tips.Count(a => a.Context == context));
		}

		protected override IUIControl GetContentControl()
		{
			return _control;
		}

		protected override IUIControl GetExplorerBarControl()
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
