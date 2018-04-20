using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.Anonymizer.Interfaces;
using Sdl.Community.Anonymizer.Models;
using Sdl.Core.Settings;

namespace Sdl.Community.Anonymizer
{
	public class AnonymizerSettings : SettingsGroup,IAnonymizerSettings
	{
		private List<RegexPattern> _regexPatterns = new List<RegexPattern>();

		public List<RegexPattern> RegexPatterns
		{
			get => GetSetting<List<RegexPattern>>(nameof(RegexPatterns));
			
			set => GetSetting<List<RegexPattern>>(nameof(RegexPatterns)).Value = value;
		}

		public string EncryptionKey
		{
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = value;
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

		public List<RegexPattern> GetRegexPatterns()
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
