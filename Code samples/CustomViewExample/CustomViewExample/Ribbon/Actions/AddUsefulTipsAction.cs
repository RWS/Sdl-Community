using System;
using System.Windows;
using CustomViewExample.Model;
using CustomViewExample.Services;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace CustomViewExample.Ribbon.Actions
{
	[Action("CustomViewExample_AddUsefulTipsAction",
		Name = "CustomViewExample_AddUsefulTipsAction_Name",
		Description = "CustomViewExample_AddUsefulTipsAction_Description",
		ContextByType = typeof(CustomViewExampleController),
		Icon = "CustomViewExample_AddUsefulTipsAction_Icon"
	)]
	[ActionLayout(typeof(RibbonGroups.ActionsGroup), 9, DisplayType.Normal)]
	internal class AddUsefulTipsAction : AbstractAction
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

			var importTips = new ImportTips { TipLanguages = tipLanguages };
			
			var count = usefulTipsService.AddUsefulTips(importTips, true);
			if (count > 0)
			{
				//Option (1)
				// Use this if you have a reference to "using Sdl.Desktop.Platform" and comment out Option (2)
				//var message = string.Format(PluginResources.Added__0__useful_tips, count);
				//message += Environment.NewLine + Environment.NewLine;
				//message += PluginResources.Restart_Trados_Studio_to_apply_changes;

				//var dr = MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);
				//if (dr == MessageBoxResult.Yes)
				//{
				//	StudioPlatform.Studio.Restart();
				//}

				//Option (2)
				// Use this if you DO NOT have a reference to "using Sdl.Desktop.Platform" and comment out Option (1)
				var message = string.Format(PluginResources.Added__0__useful_tips, count);
				message += Environment.NewLine + Environment.NewLine;
				message += PluginResources.Restart_Trados_Studio_before_changes_are_applied;

				MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			MessageBox.Show(PluginResources.No_new_useful_tips_installed, PluginResources.Plugin_Name);
		}
	}
}
