using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
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
			if (_control != null)
			{
				_control.Settings = _settings;			
			}

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

			_settings.RegexPatterns = GetListOfValidRegexPatterns();
			_settings.SelectAll = _control.SelectAll;
			_settings.HasBeenCheckedByControl = false;
		}

		private BindingList<RegexPattern> GetListOfValidRegexPatterns()
		{
			var  listOfValidPatterns = new BindingList<RegexPattern>();

			foreach (var pattern in _control.RegexPatterns)
			{
				if (!string.IsNullOrEmpty(pattern.Pattern))
				{
					listOfValidPatterns.Add(pattern);
				}
			}

			return listOfValidPatterns;
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

		private void EncryptPatterns()
		{
			foreach (var regexPattern in _control.RegexPatterns)
			{
				regexPattern.Pattern = AnonymizeData.EncryptData(regexPattern.Pattern, _control.EncryptionKey);
			}
		}
	}
}