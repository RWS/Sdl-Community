﻿using System.ComponentModel;
using System.IO;
using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Interfaces;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services;
using Sdl.Core.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	public class AnonymizerSettings : SettingsGroup, IAnonymizerSettings
	{
		public bool? IsOldVersion
		{
			get => GetSetting<bool?>(nameof(IsOldVersion));
			set => GetSetting<bool?>(nameof(IsOldVersion)).Value = value;
		}

		public State EncryptionState
		{
			get => GetSetting<State>(nameof(EncryptionState));
			set => GetSetting<State>(nameof(EncryptionState)).Value = value;
		}

		public bool? ShouldAnonymize
		{
			get => GetSetting<bool?>(nameof(ShouldAnonymize));
			set => GetSetting<bool?>(nameof(ShouldAnonymize)).Value = value;
		}

		public bool? ShouldDeanonymize
		{
			get => GetSetting<bool?>(nameof(ShouldDeanonymize));
			set => GetSetting<bool?>(nameof(ShouldDeanonymize)).Value = value;
		}

		public BindingList<RegexPattern> RegexPatterns
		{
			get
			{
				if (!ContainsSetting(nameof(RegexPatterns)))
				{
					// here, we don't yet have it, create it as a default
					var regexPatterns = Constants.GetDefaultRegexPatterns();
					GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns)).Value = regexPatterns;
				}
				return GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns));
			}
			set => GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns)).Value = value;
		}

		public string EncryptionKey
		{
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = AnonymizeData.EncryptData(value, Constants.Key);
		}

		public bool DefaultListAlreadyAdded
		{
			get => GetSetting<bool>(nameof(DefaultListAlreadyAdded));
			set => GetSetting<bool>(nameof(DefaultListAlreadyAdded)).Value = value;
		}

		public bool SelectAll
		{
			get => GetSetting<bool>(nameof(SelectAll));
			set => GetSetting<bool>(nameof(SelectAll)).Value = value;
		}

		public bool EnableAll
		{
			get => GetSetting<bool>(nameof(EnableAll));
			set => GetSetting<bool>(nameof(EnableAll)).Value = value;
		}

		public bool EncryptAll
		{
			get => GetSetting<bool>(nameof(EncryptAll));
			set => GetSetting<bool>(nameof(EncryptAll)).Value = value;
		}

		public bool IgnoreEncrypted
		{
			get => GetSetting<bool>(nameof(IgnoreEncrypted));
			set => GetSetting<bool>(nameof(IgnoreEncrypted)).Value = value;
		}

		//Initialize settings with default regex list
		public void AddPattern(RegexPattern pattern)
		{
			RegexPatterns.Add(pattern);
		}

		public BindingList<RegexPattern> GetRegexPatterns() => RegexPatterns;

		public string GetEncryptionKey()
		{
			return EncryptionKey;
		}
		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(RegexPatterns):
					return new BindingList<RegexPattern>();

				case nameof(EncryptionKey):
					return "<dummy-encryption-key>";
			}
			return base.GetDefaultValue(settingId);
		}

		public bool? HasBeenCheckedByControl
		{
			get => GetSetting<bool?>(nameof(HasBeenCheckedByControl));
			set => GetSetting<bool?>(nameof(HasBeenCheckedByControl)).Value = value;
		}
	}
}