using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Interfaces;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Core.Settings;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class AnonymizerSettings : SettingsGroup,IAnonymizerSettings
	{
		private BindingList<RegexPattern> _regexPatterns = new BindingList<RegexPattern>();
		private bool _defaultListAlreadyAdded = false;

		public BindingList<RegexPattern> RegexPatterns
		{
			get => GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns));
			
			set => GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns)).Value = value;
		}

		public string EncryptionKey
		{
			//get => GetSetting<string>(AnonymizeData.EncryptData(nameof(EncryptionKey), Constants.Key));
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = AnonymizeData.EncryptData(value,Constants.Key);
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
		//Initialize settings with default regex list
		public void AddPattern(RegexPattern pattern)
		{
			_regexPatterns.Add(pattern);
		}

		public BindingList<RegexPattern> GetRegexPatterns()
		{
			return RegexPatterns;
		}

		public string GetEncryptionKey()
		{
			return EncryptionKey;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(RegexPatterns):
					return _regexPatterns;
			}
			return base.GetDefaultValue(settingId);
		}


	}
}
