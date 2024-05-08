using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffCompare.TellMe.Actions
{
	public class SettingsAction : AbstractTellMeAction
	{
		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		public override void Execute()
		{
			ApplicationContext.CompareAction();
			ApplicationContext.Controller.Control.LoadSettingsDialog();
		}
	}
}