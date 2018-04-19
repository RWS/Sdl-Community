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
		///private static List<RegexPattern> _regexPatterns = GetDefaultRegexPatterns();
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
			//if (_regexPatterns.Count.Equals(0))
			//{
			//	_regexPatterns = RegexPatterns;
			//}
		//we add have an empty pattern
			var patterns = _regexPatterns.Where(p => p.Id!=null).ToList();
			var patternAlreadyExists = patterns.Exists(i => i.Id.Equals(pattern.Id));
			if (!patternAlreadyExists)
			{
				_regexPatterns.Add(pattern);
			}
		}


		//public AnonymizerSettings()
		//{
		//	RegexPatterns = new List<RegexPattern>
		//	{
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "email",
		//			Pattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A - Z]{ 2,}\b"
		//		},

		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "PCI",
		//			Pattern = @"\b(?:\d[ -]*?){13,16}\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "IP6 Address",
		//			Pattern = @"(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Social Security Numbers",
		//			Pattern=@"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Telephone Numbers",
		//			Pattern=@"\b\d{4}\s\d+-\d+\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Car Registrations",
		//			Pattern=@"\b\p{Lu}+\s\p{Lu}+\s\d+\b|\b\p{Lu}+\s\d+\s\p{Lu}+\b|\b\p{Lu}+\d+\s\p{Lu}+\b|\b\p{Lu}+\s\d+\p{Lu}+\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Passport Numbers",
		//			Pattern=@"\b\d{9}\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="National Insurance Number",
		//			Pattern=@"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Pattern = "Date of Birth",
		//			Description = @"\b\d{2}/\d{2}/\d{4}\b"
		//		},
		//		new RegexPattern
		//		{
		//			Pattern = "",
		//			Description = ""
		//		}
		//	};
		//}
		//public  static List<RegexPattern> GetDefaultRegexPatterns()
		//{
			
		//	_regexPatterns = new List<RegexPattern>
		//	{
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "email",
		//			Pattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A - Z]{ 2,}\b"
		//		},

		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "PCI",
		//			Pattern = @"\b(?:\d[ -]*?){13,16}\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description = "IP6 Address",
		//			Pattern = @"(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Social Security Numbers",
		//				Pattern=@"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Telephone Numbers",
		//				Pattern=@"\b\d{4}\s\d+-\d+\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Car Registrations",
		//				Pattern=@"\b\p{Lu}+\s\p{Lu}+\s\d+\b|\b\p{Lu}+\s\d+\s\p{Lu}+\b|\b\p{Lu}+\d+\s\p{Lu}+\b|\b\p{Lu}+\s\d+\p{Lu}+\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="Passport Numbers",
		//				Pattern=@"\b\d{9}\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Description ="National Insurance Number",
		//				Pattern=@"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b"
		//		},
		//		new RegexPattern
		//		{
		//			Id = Guid.NewGuid().ToString(),
		//			Pattern = "Date of Birth",
		//			Description ="blabla"
		//		},
		//		new RegexPattern
		//		{
		//			Pattern = "",
		//			Description = ""
		//		}
		//	};

		//	return _regexPatterns;
		//}

		public List<RegexPattern> GetRegexPatterns()
		{
			//RegexPatterns.AddRange(_regexPatterns);
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
