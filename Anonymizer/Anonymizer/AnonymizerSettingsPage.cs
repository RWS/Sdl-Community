using Sdl.Community.Anonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Anonymizer
{
	public class AnonymizerSettingsPage : DefaultSettingsPage<AnonymizerSettingsControl, AnonymizerSettings>
	{
		private AnonymizerSettings Settings;
		private AnonymizerSettingsControl Control;


		public override object GetControl()
		{
			Settings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();
			Control = base.GetControl() as AnonymizerSettingsControl;
			return Control;
		}
		
	

		public override void Save()
		{
			//base.Save();
			Settings.EncryptionKey = Control.EncryptionKey;
			Settings.RegexPatterns = Control.RegexPatterns;
		}

		public override void OnActivate()
		{
			Control.EncryptionKey = Settings.EncryptionKey;
			Control.RegexPatterns = Settings.RegexPatterns;
		}
		//public override void OnDeactivate()
		//{
		//	Settings.EncryptionKey = Control.EncryptionKey;
		//	Settings.RegexPatterns = Control.RegexPatterns;
		//}

		//public override void Dispose()
		//{
		//	Control.Dispose();
		//}
	}
}
