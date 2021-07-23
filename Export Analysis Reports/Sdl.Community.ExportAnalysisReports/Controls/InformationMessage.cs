using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ExportAnalysisReports.Service;

namespace Sdl.Community.ExportAnalysisReports.Controls
{
	public partial class InformationMessage : Form
	{
		private readonly SettingsService _settingsService;
		public InformationMessage(SettingsService settingsService, List<string> projectPaths)
		{
			_settingsService = settingsService;
			InitializeComponent();
			this.DontShowThisMessageAgain_CheckBox.Checked = false;
			var projectPathsText = string.Empty;
			foreach (var projectPath in projectPaths)
			{
				projectPathsText += (string.IsNullOrEmpty(projectPathsText) ? string.Empty : "\r\n") + projectPath;
			}
			this.ListOfProjects_TextBox.Text = projectPathsText;
		}

		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			var settings = _settingsService.GetSettings();
			settings.DontShowProjectNotAvailabeInfoMessage = this.DontShowThisMessageAgain_CheckBox.Checked;
			_settingsService.SaveSettings(settings);
			this.Close();
		}
	}
}
