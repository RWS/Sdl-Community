using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	public class LanguageMappingSettings : SettingsGroup, ILanguageMappingSettings
	{		
		private readonly List<LanguageMappingModel> _languageMappings = new List<LanguageMappingModel>();

		public List<LanguageMappingModel> LanguageMappings
		{
			get => GetSetting<List<LanguageMappingModel>>(nameof(LanguageMappings));
			set
			{
				var langMap = GetSetting<List<LanguageMappingModel>>(nameof(LanguageMappings));
				if (langMap != null)
				{
					langMap.Value = value;
				}
			}
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(LanguageMappings):
					return _languageMappings;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}
