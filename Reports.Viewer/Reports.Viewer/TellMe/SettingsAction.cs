using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reports.Viewer.Plus.Actions;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.TellMe
{
	internal class SettingsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Settings;

		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} settings";
		}

		public override void Execute()
		{
			SdlTradosStudio.Application.GetAction<OpenSettingsAction>().Run();
		}
	}
}
