using System.Collections.Generic;
using IATETerminologyProvider.Interface;
using Sdl.Core.Settings;

namespace IATETerminologyProvider.Model
{
	public class ProviderSettings : SettingsGroup, IProviderSettings
	{
		private readonly List<DomainModel> _domanins = new List<DomainModel>();
		private readonly List<TermTypeModel> _termTypes = new List<TermTypeModel>();
		private readonly bool _searchInSubdomains = false;

		public List<DomainModel> Domains
		{
			get => GetSetting<List<DomainModel>>(nameof(Domains));
			set
			{
				var domains = GetSetting<List<DomainModel>>(nameof(Domains));
				if (domains != null)
				{
					domains.Value = value;
				}
			}
		}

		public List<TermTypeModel> TermTypes
		{
			get => GetSetting<List<TermTypeModel>>(nameof(TermTypes));
			set
			{
				var termTypes = GetSetting<List<TermTypeModel>>(nameof(TermTypes));
				if (termTypes != null)
				{
					termTypes.Value = value;
				}
			}
		}

		public bool SearchInSubdomains
		{
			get => GetSetting<bool>(nameof(SearchInSubdomains));
			set
			{
				var searchInSubdomain = GetSetting<bool>(nameof(SearchInSubdomains));
				searchInSubdomain.Value = value;
			}
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(Domains):
					return _domanins;
				case nameof(TermTypes):
					return _termTypes;
				case nameof(SearchInSubdomains):
					return _searchInSubdomains;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}