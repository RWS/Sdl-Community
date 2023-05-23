using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CustomViewExample.Model;
using CustomViewExample.Services;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace CustomViewExample.Ribbon.Actions
{
	[Action("CustomViewExample_RemoveUsefulTipsAction",
		Name = "CustomViewExample_RemoveUsefulTipsAction_Name",
		Description = "CustomViewExample_RemoveUsefulTipsAction_Description",
		ContextByType = typeof(CustomViewExampleController),
		Icon = "CustomViewExample_RemoveUsefulTipsAction_Icon"
	)]
	[ActionLayout(typeof(RibbonGroups.ActionsGroup), 8, DisplayType.Normal)]
	internal class RemoveUsefulTipsAction : AbstractAction
	{
		protected override void Execute()
		{
			var customViewPathInfo = new CustomViewPathInfo();
			var pathInfo = new PathInfo(PluginResources.Plugin_Name, "17");
			var tipsProvider = new TipsProvider(pathInfo);
			var usefulTipsService = new UsefulTipsService(tipsProvider, customViewPathInfo);

			var tipLanguages = usefulTipsService.GetPluginUsefulTips();
			if (tipLanguages.Count == 0)
			{
				MessageBox.Show(PluginResources.No_useful_tips_found, PluginResources.Plugin_Name);
			}

			AlignLanguageTipIds(tipLanguages, tipsProvider, usefulTipsService);

			var tipReferences = new List<TipReference>();
			foreach (var tipLanguage in tipLanguages)
			{
				tipReferences.Add(new TipReference
				{
					LanguageId = tipLanguage.LanguageId,
					Resources = tipLanguage.Resources.Select(a => a.RelativePath).ToList(),
					TipIds = tipLanguage.Tips.Where(a => !string.IsNullOrEmpty(a.Id)).Select(a => a.Id).ToList()
				});
			}

			var removeTips = new RemoveTips { TipReferences = tipReferences };

			var count = usefulTipsService.RemoveUsefulTips(removeTips);
			if (count > 0)
			{
				//Option (1)
				// Use this if you have a reference to "using Sdl.Desktop.Platform" and comment out Option (2)
				//var message = string.Format(PluginResources.Removed__0__useful_tips, count);
				//message += Environment.NewLine + Environment.NewLine;
				//message += PluginResources.Restart_Trados_Studio_to_apply_changes;

				//var dr = MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);
				//if (dr == MessageBoxResult.Yes)
				//{
				//	StudioPlatform.Studio.Restart();
				//}

				//Option (2)
				// Use this if you DO NOT have a reference to "using Sdl.Desktop.Platform" and comment out Option (1)
				var message = string.Format(PluginResources.Removed__0__useful_tips, count);
				message += Environment.NewLine + Environment.NewLine;
				message += PluginResources.Restart_Trados_Studio_before_changes_are_applied;

				MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			MessageBox.Show(PluginResources.No_useful_tips_removed, PluginResources.Plugin_Name);
		}

		private static void AlignLanguageTipIds(IEnumerable<TipLanguage> tipLanguages, TipsProvider tipsProvider, UsefulTipsService usefulTipsService)
		{
			var studioTips = tipsProvider.GetStudioTips();
			foreach (var tipLanguage in tipLanguages)
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
	}
}
