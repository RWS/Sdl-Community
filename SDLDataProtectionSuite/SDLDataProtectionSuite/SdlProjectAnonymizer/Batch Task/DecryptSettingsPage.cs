using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	public class DecryptSettingsPage : DefaultSettingsPage<DecryptSettingsControl, AnonymizerSettings>
	{
		private AnonymizerSettings _settings;
		private DecryptSettingsControl _control;

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();

			_control = base.GetControl() as DecryptSettingsControl;
			return _control;
		}

		public override bool ValidateInput()
		{
			var accepted = AgreementMethods.UserAgreed();

			if (!accepted)
			{
				MessageBox.Show(
					"You must agree to the terms and conditions of the SDL Data Protection Suite before using Project Anonymizer",
					"Agreement");
			}

			return accepted;
		}

		public override void Save()
		{
			base.Save();
			if (_settings.EncryptionKey != AnonymizeData.EncryptData(_control.EncryptionKey, Constants.Key) && !_settings.EncryptionState.HasFlag(State.Decrypted))
			{
				_settings.ShouldDeanonymize = false;
				return;
			}
			_settings.IsOldVersion = !_settings.EncryptionState.HasFlag(State.PatternsEncrypted) && _settings.EncryptionState.HasFlag(State.DataEncrypted);
			DecryptPatterns();

			_settings.ShouldDeanonymize = _settings.EncryptionState.HasFlag(State.DataEncrypted & State.Decrypted);
			_settings.EncryptionState = State.Decrypted;
			_settings.EncryptionKey = _control.EncryptionKey;
			_settings.HasBeenCheckedByControl = true;
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