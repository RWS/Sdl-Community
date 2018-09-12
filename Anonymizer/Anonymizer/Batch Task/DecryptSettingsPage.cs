using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Community.projectAnonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class DecryptSettingsPage: DefaultSettingsPage<DecryptSettingsControl,DecryptSettings>
	{
		private DecryptSettings _settings;
		private DecryptSettingsControl _control;
		private AnonymizerSettings _encryptionSettings;
		public override object GetControl()
		{
			 _encryptionSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<DecryptSettings>();
			_control = base.GetControl() as DecryptSettingsControl;
			return _control;
		}
		public override bool ValidateInput()
		{
			return AgreementMethods.UserAgreed();
		}
		public override void Save()
		{
			if (_encryptionSettings.ArePatternsEncrypted ?? false)
			{
				DecryptPatterns();
			}
			_encryptionSettings.IsProjectEncrypted = false;
			_encryptionSettings.IsOldVersion = false;
			_settings.EncryptionKey = _control.EncryptionKey;
			_settings.IgnoreEncrypted = _control.IgnoreEncrypted;
		}

		public void DecryptPatterns()
		{
			var key = AnonymizeData.DecryptData(_encryptionSettings.EncryptionKey, Constants.Key);
			var decryptedPatterns = new BindingList<RegexPattern>();
			foreach (var regexPattern in _encryptionSettings.RegexPatterns)
			{
				decryptedPatterns.Add(new RegexPattern()
				{
					Pattern = AnonymizeData.DecryptData(regexPattern.Pattern, key),
					Description = regexPattern.Description,
					ShouldEncrypt = regexPattern.ShouldEncrypt,
					ShouldEnable = regexPattern.ShouldEnable,
					IsDefaultPath = regexPattern.IsDefaultPath,
					Id = regexPattern.Id
				});
			}

			_encryptionSettings.RegexPatterns = decryptedPatterns;
			_encryptionSettings.ArePatternsEncrypted = false;
		}
	}
}
