using System.Collections.ObjectModel;
using Sdl.Community.BeGlobalV4.Provider.Interfaces;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class LanguageMappingSettings : SettingsGroup, ILanguageMappingSettings
	{
		private readonly ObservableCollection<LanguageMappingModel> _languageMappings = new ObservableCollection<LanguageMappingModel>();

		public ObservableCollection<LanguageMappingModel> LanguageMappings
		{
			get => GetSetting<ObservableCollection<LanguageMappingModel>>(nameof(LanguageMappings));
			set
			{
				var langMap = GetSetting<ObservableCollection<LanguageMappingModel>>(nameof(LanguageMappings));
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
