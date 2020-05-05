using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Core.Settings;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class ProviderSettings : SettingsGroup, IProviderSettings
	{
		private readonly List<DomainModel> _domanins = new List<DomainModel>();
		private readonly List<TermTypeModel> _termTypes = new List<TermTypeModel>();

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

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(Domains):
					return _domanins;
				case nameof(TermTypes):
					return _termTypes;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}