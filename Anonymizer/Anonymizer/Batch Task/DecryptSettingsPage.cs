using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Community.projectAnonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class DecryptSettingsPage: DefaultSettingsPage<DecryptSettingsControl,AnonymizerSettings>
	{
		private AnonymizerSettings _settings;
		private DecryptSettingsControl _control;
		public override object GetControl()
		{
			//_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<DecryptSettings>();
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();

			_control = base.GetControl() as DecryptSettingsControl;
			return _control;
		}
		public override bool ValidateInput()
		{
			return AgreementMethods.UserAgreed();
		}
		public override void Save()
		{
			if (_settings.EncryptionKey != AnonymizeData.EncryptData(_control.EncryptionKey, Constants.Key))
			{
				_settings.ShouldDeanonymize = false;
				return;
			}

			_settings.IsOldVersion = ((_settings.EncryptionState & State.PatternsEncrypted) == 0) && ((_settings.EncryptionState & State.DataEncrypted) != 0);
			DecryptPatterns();

			_settings.ShouldDeanonymize = (_settings.EncryptionState & State.DataEncrypted) != 0;
			_settings.EncryptionState = State.Decrypted;
			_settings.EncryptionKey = _control.EncryptionKey;
			_settings.IgnoreEncrypted = _control.IgnoreEncrypted;
		}

		public void DecryptPatterns()
		{
			var patternsEncrypted = (_settings.EncryptionState & State.PatternsEncrypted) != 0;

			if (!patternsEncrypted)
				return;

			var key = AnonymizeData.DecryptData(_settings.EncryptionKey, Constants.Key);
			var decryptedPatterns = new BindingList<RegexPattern>();
			foreach (var regexPattern in _settings.RegexPatterns)
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
			_settings.RegexPatterns = decryptedPatterns;
			_settings.EncryptionState &= ~State.PatternsEncrypted;
		}
	}
}
