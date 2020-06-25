using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public partial class ImportSettingsControl : UserControl, ISettingsAware<ImportSettings>
	{
		private ImportSettings _settings;

		public ImportSettingsControl()
		{
			InitializeComponent();
		}

		public ImportSettings Settings
		{
			get => _settings;
			set => _settings = value;
		}
	}
}
