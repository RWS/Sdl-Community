using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Settings;

namespace Sdl.Community.ReportExporter
{
	public class ReportExporterSettings : SettingsGroup
	{
		public ObservableCollection<string> ProjectsList = new ObservableCollection<string>();
		public List<string> Test
		{
			get => GetSetting<List<string>>(nameof(Test));
			set => GetSetting<List<string>>(nameof(Test)).Value = value;
		}
		
	}
}
