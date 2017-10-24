using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ReportExporter.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.ReportExporter
{
	public class ReportExporterSettings : SettingsGroup
	{
		public bool IncludeHeader {
			get => GetSetting<bool>(nameof(IncludeHeader));
			set => GetSetting<bool>(nameof(IncludeHeader)).Value = value;
		}

		public List<ReportDetails> ProjectsList
		{
			get => GetSetting<List<ReportDetails>>(nameof(ProjectsList));
			set => GetSetting<List<ReportDetails>>(nameof(ProjectsList)).Value = value;
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(IncludeHeader):
					return true;
			}
			return base.GetDefaultValue(settingId);
		}
	}
}
