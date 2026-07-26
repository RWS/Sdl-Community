using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.ExportAnalysisReports.Service;

namespace Sdl.Community.ExportAnalysisReports.Controls
{
	public partial class InformationMessage : Form
	{
		private readonly SettingsService _settingsService;
		public InformationMessage(SettingsService settingsService, List<string> projectPaths)
		{
			InitializeComponent();

			_settingsService = settingsService;
			
			this.TitleMessage_textbox.Text = PluginResources.InformationMessage_The_selected_projects_cannot_be_exported;
			this.DontShowThisMessageAgain_CheckBox.Text = PluginResources.InformationMessage_Don_t_show_this_again;
			this.Text = PluginResources.InformationMessage_Projects_without_analysis_reports;
			this.OK_Button.Text = PluginResources.InformationMessage_OK;
			this.Cancel_Button.Text = PluginResources.InformationMessage_Cancel;

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
			settings.DontShowInfoMessage = this.DontShowThisMessageAgain_CheckBox.Checked;
			_settingsService.SaveSettings(settings);
			this.Close();
		}
	}
}
