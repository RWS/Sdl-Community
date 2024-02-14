using System.Drawing;
using System.Windows;

namespace LanguageWeaverProvider.Studio.TellMe
{
	class SettingsAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "project", "settings" };
		private static readonly string _actionName = Constants.TellMe_Settings_Name;
		private static readonly Icon _icon = PluginResources.TellMeSettings;
		private static readonly bool _isAvailable = true;

		public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: DisplayMessage) { }

		private static void DisplayMessage()
		{
			MessageBox.Show("See documentation for guidance.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}