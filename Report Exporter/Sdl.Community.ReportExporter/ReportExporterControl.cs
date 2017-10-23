using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ReportExporter.Helpers;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.ReportExporter
{
	public partial class ReportExporterControl : UserControl, ISettingsAware<ReportExporterSettings>
	{
		public ReportExporterControl()
		{
			InitializeComponent();
		}

		public ReportExporterSettings Settings { get; set; }

		private void browseBtn_Click(object sender, EventArgs e)
		{
			var folderDialog = new FolderSelectDialog();
			if (folderDialog.ShowDialog())
			{
				outputPathField.Text = folderDialog.FileName;
			}
		}
	}
}
