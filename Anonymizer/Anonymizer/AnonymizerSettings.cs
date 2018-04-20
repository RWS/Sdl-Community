using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.Anonymizer.Interfaces;
using Sdl.Community.Anonymizer.Models;
using Sdl.Core.Settings;

namespace Sdl.Community.Anonymizer
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
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = value;
		}

		public bool DefaultListAlreadyAdded
		{
			get => GetSetting<bool>(nameof(DefaultListAlreadyAdded));
			set => GetSetting<bool>(nameof(DefaultListAlreadyAdded)).Value = value;
		}

		public void AddPattern(RegexPattern pattern)
		{
			//we add have an empty pattern last one to display an empty row in grid
			var patterns = _regexPatterns.Where(p => p.Id!=null).ToList();
			var patternAlreadyExists = patterns.Exists(i => i.Id.Equals(pattern.Id));
			if (!patternAlreadyExists)
			{
				_regexPatterns.Add(pattern);
			}
		}

		public void EditPattern(RegexPattern pattern)
		{
			var patternToEdit = _regexPatterns.FirstOrDefault(p => p.Id.Equals(pattern.Id));
			if (patternToEdit != null)
			{
				patternToEdit.Description = pattern.Description;
				patternToEdit.Pattern = pattern.Pattern;
				patternToEdit.ShouldEncrypt = pattern.ShouldEncrypt;
			}
			else
			{
				_regexPatterns.Insert(_regexPatterns.Count-1,pattern);
			}
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
