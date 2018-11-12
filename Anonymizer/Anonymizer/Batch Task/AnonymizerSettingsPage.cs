using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Community.projectAnonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class AnonymizerSettingsPage : DefaultSettingsPage<AnonymizerSettingsControl, AnonymizerSettings>
	{
		private AnonymizerSettings _settings;
		private AnonymizerSettingsControl _control;

		private bool IsEncryptionEnabled
		{
			get
			{
				foreach (var pattern in _control.RegexPatterns)
				{
					if (pattern.ShouldEncrypt && pattern.ShouldEnable)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();
			_control = base.GetControl() as AnonymizerSettingsControl;
			_control.Settings = _settings;
			return _control;
		}

		public override void Save()
		{
			base.Save();
			_settings.ShouldAnonymize = _settings.EncryptionState.HasFlag(State.Decrypted);

			if (IsEncryptionEnabled && (_settings.ShouldAnonymize ?? true))
			{
				EncryptPatterns();
				_settings.EncryptionState = State.Encrypted;
				_settings.EncryptionKey = _control.EncryptionKey;
			}

			_settings.RegexPatterns = _control.RegexPatterns;
			_settings.SelectAll = _control.SelectAll;
			_settings.HasBeenCheckedByControl = false;
		}

		public override bool ValidateInput()
		{
			return AgreementMethods.UserAgreed();
		}

		private void EncryptPatterns()
		{
			foreach (var regexPattern in _control.RegexPatterns)
			{
				regexPattern.Pattern = AnonymizeData.EncryptData(regexPattern.Pattern, _control.EncryptionKey);
			}
		}
	}
}