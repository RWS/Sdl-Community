using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.ReportExporter.Helpers;
using Sdl.Community.ReportExporter.Model;
using Sdl.Desktop.IntegrationApi;
using static System.String;
using Help = Sdl.Community.ReportExporter.Helpers.Help;

namespace Sdl.Community.ReportExporter
{
	public partial class ReportExporterControl : UserControl, ISettingsAware<ReportExporterSettings>
	{
		private readonly string _projectXmlPath;
		private List<ReportDetails> _selectedProjectsForReport;

		public ReportExporterControl()
		{
			InitializeComponent();
			_projectXmlPath = Help.GetStudioProjectsPath();
			_selectedProjectsForReport = new List<ReportDetails>();
			LoadProjectsList();
			projListbox.SelectedIndex = 0;

			languagesListBox.CheckOnClick = true;
			IsClipboardEnabled();
		}

		private void AddNewProject(List<LanguageDirection> languages)
		{
			var projectDetails = projListbox.SelectedItem as ProjectDetails;
			var selectedProjectName = Empty;

			if (projectDetails != null)
			{
				selectedProjectName = projectDetails.ProjectName;
			}

			var reportDetails = new ReportDetails
			{
				ProjectName = selectedProjectName,
				LanguagesForPoject = new Dictionary<LanguageDirection, bool>()
			};
			foreach (LanguageDirection language in languages)
			{
				reportDetails.LanguagesForPoject.Add(language, false);

			}
			//add to list where we'll keep information about checkboxes state
			_selectedProjectsForReport.Add(reportDetails);
		}

		private void FillLanguagesList()
		{
			var projectDetails = projListbox.SelectedItem as ProjectDetails;
			if (projectDetails != null)
			{
				var selectedProjectName = projectDetails.ProjectName;
				var existSelectedProjInList =
					_selectedProjectsForReport.FirstOrDefault(n => n.ProjectName.Equals(selectedProjectName));
				if (existSelectedProjInList != null)
				{
					var languageList = existSelectedProjInList
						.LanguagesForPoject;

					foreach (var language in languageList)
					{
						languagesListBox.Items.Add(language.Key, language.Value);
					}

				}

			}
		}

		/// <summary>
		/// Reads studio projects from project.xml
		/// Adds projects to listbox
		/// </summary>
		private void LoadProjectsList()
		{
			var projectXmlDocument = new XmlDocument();

			projectXmlDocument.Load(_projectXmlPath);

			var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
			if (projectsNodeList == null) return;
			foreach (var item in projectsNodeList)
			{
				var projectInfo = ((XmlNode) item).SelectSingleNode("./ProjectInfo");
				if (projectInfo?.Attributes != null && projectInfo.Attributes["IsInPlace"].Value != "true")
				{
					projListbox.Items.Add(CreateProjectDetails((XmlNode) item));
				}
			}

		}

		/// <summary>
		/// Creates project details for given project from xml file
		/// </summary>
		/// <param name="projNode"></param>
		/// <returns></returns>
		private ProjectDetails CreateProjectDetails(XmlNode projNode)
		{
			var projectDetails = new ProjectDetails();
			var projectFolderPath = Empty;

			var selectSingleNode = projNode.SelectSingleNode("ProjectInfo");
			if (selectSingleNode?.Attributes != null)
			{
				projectDetails.ProjectName = selectSingleNode.Attributes["Name"].Value;
			}
			if (projNode.Attributes != null)
			{
				projectFolderPath = projNode.Attributes["ProjectFilePath"].Value;
			}
			if (Path.IsPathRooted(projectFolderPath))
			{
				projectDetails.ProjectPath = projectFolderPath; //location outside standard project place
			}
			else
			{
				var projectsFolderPath = _projectXmlPath.Substring
					(0, _projectXmlPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
				projectDetails.ProjectPath = projectsFolderPath + projectFolderPath;
			}
			return projectDetails;
		}

		public ReportExporterSettings Settings { get; set; }
		

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{

			if (projListbox.SelectedItem == null) return;

			languagesListBox.Items.Clear();

			var selectedProject = projListbox.SelectedItem as ProjectDetails;
			var doc = new XmlDocument();
			if (selectedProject != null)
			{
				doc.Load(selectedProject.ProjectPath);

				var languages = Help.LoadLanguageDirections(doc);
				Help.LoadReports(doc, selectedProject.ProjectFolderPath, languages);
				var projectLanguages = languages.Values.Where(item => !IsNullOrEmpty(item.PathToReport)).ToList();
				//add resutls to the list view

				var selectedProjectExists = _selectedProjectsForReport.Any(n => n.ProjectName.Equals(selectedProject.ProjectName));

				if (!selectedProjectExists)
				{
					AddNewProject(projectLanguages);
				}
				FillLanguagesList();
			}
			IsClipboardEnabled();

		}

		private void languagesListBox_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			var projectDetails = (ProjectDetails) projListbox.SelectedItem;
			if (projectDetails != null)
			{
				var selectedLanguage = (LanguageDirection) languagesListBox.SelectedItem;
				var index = languagesListBox.SelectedIndex;
				var isChecked = languagesListBox.GetItemChecked(index);

				var selectedProject = GetSelectedReport();
				if (selectedProject.LanguagesForPoject.ContainsKey(selectedLanguage))
				{
					selectedProject.LanguagesForPoject[selectedLanguage] = isChecked;
				}
			}
			IsClipboardEnabled();
			Settings.ProjectsList = _selectedProjectsForReport;
		}

		private ReportDetails GetSelectedReport()
		{
			var projectDetails = (ProjectDetails) projListbox.SelectedItem;
			var selectedProjectName = projectDetails.ProjectName;
			var selectedProject = _selectedProjectsForReport.FirstOrDefault(n => n.ProjectName.Equals(selectedProjectName));

			return selectedProject;
		}

		private void copyBtn_Click(object sender, EventArgs e)
		{
			var selectedLanguage = (LanguageDirection) languagesListBox.SelectedItem;
		
			if (selectedLanguage != null)
			{
				var report = new StudioAnalysisReport(selectedLanguage.PathToReport);
				Clipboard.SetText(report.ToCsv(true));
			}//check from  selected project if we have any language checked
			else
			{
				var selectedReport = GetSelectedReport();
				var language = selectedReport.LanguagesForPoject.FirstOrDefault(c => c.Value);
				var copyReport = new StudioAnalysisReport(language.Key.PathToReport);
				Clipboard.SetText(copyReport.ToCsv(true));
			}
		

		}

		private void IsClipboardEnabled()
		{
			var selectedReport = GetSelectedReport();
			var selectedLanguagesCount = selectedReport.LanguagesForPoject.Count(c => c.Value);
			copyBtn.Enabled = selectedLanguagesCount == 1;
		}

	}
}
