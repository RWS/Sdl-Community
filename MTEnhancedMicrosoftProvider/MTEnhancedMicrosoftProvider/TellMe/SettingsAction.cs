using System;
using System.Drawing;
using Sdl.TellMe.ProviderApi;
/*using System.Linq;
using System.Windows;
using MTEnhancedMicrosoftProvider.Service;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using MTEnhancedMicrosoftProvider.View;
using MTEnhancedMicrosoftProvider.ViewModel;
using Sdl.Community.MtEnhancedProvider.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;*/

namespace MTEnhancedMicrosoftProvider.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = "MT Enhanced Provider options";
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Settings;

		public override void Execute()
		{
			throw new NotImplementedException();
		}
	}
}