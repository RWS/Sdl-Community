using System;
using System.Collections.Generic;
using Sdl.Community.Anonymizer.Models;
using Sdl.Core.Settings;

namespace Sdl.Community.Anonymizer
{
	public class AnonymizerSettings : SettingsGroup
	{
		//private List<RegexPattern> _regexPatterns;
		private string _anonymizeKey = "Andrea";

		public List<RegexPattern> RegexPatterns
		{
			get
			{
				var regexPatterns = new List<RegexPattern>();
				return regexPatterns;
			}

			set => GetSetting<List<RegexPattern>>(nameof(RegexPatterns)).Value = value;
		}

		public string AnonymizeKey
		{
			get => GetSetting<string>(nameof(AnonymizeKey));
			set => GetSetting<string>(nameof(AnonymizeKey)).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(AnonymizeKey):
					return _anonymizeKey;
			}
			return base.GetDefaultValue(settingId);
		}
	}
}
