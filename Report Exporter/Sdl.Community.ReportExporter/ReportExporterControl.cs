using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
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
	public partial class ReportExporterControl : Form
	{
		private readonly string _projectXmlPath;
		private readonly List<ReportDetails> _selectedProjectsForReport;
		private readonly OptionalInformation _optionalInformation;
		public ReportExporterControl()
		{
			InitializeComponent();
			_projectXmlPath = Help.GetStudioProjectsPath();
			_selectedProjectsForReport = new List<ReportDetails>();
			LoadProjectsList();
			projListbox.SelectedIndex = 0;

			//set for language list box check box should be checked/unchecked when row is selected
			languagesListBox.CheckOnClick = true;
			IsClipboardEnabled();
			IsCsvBtnEnabled();
			includeHeaderCheck.Checked = true;
			_optionalInformation = new OptionalInformation
			{
				IncludeAdaptiveBaseline = adaptiveMT.Checked,
				IncludeAdaptiveLearnings = adaptiveLearnings.Checked,
				IncludeInternalFuzzies = internalFuzzies.Checked,
				IncludeContextMatch = contextMatch.Checked,
				IncludeCrossRep = crossRep.Checked,
				IncludeLocked = locked.Checked,
				IncludePerfectMatch = perfectMatch.Checked
			};

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
			IsCsvBtnEnabled();
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
			IsCsvBtnEnabled();
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
			try
			{
				if (selectedLanguage != null)
				{
					var report = new StudioAnalysisReport(selectedLanguage.PathToReport);
					Clipboard.SetText(report.ToCsv(includeHeaderCheck.Checked,_optionalInformation));
				}//check from  selected project if we have any language checked
				else
				{
					var selectedReport = GetSelectedReport();
					var language = selectedReport.LanguagesForPoject.FirstOrDefault(c => c.Value);
					var copyReport = new StudioAnalysisReport(language.Key.PathToReport);
					Clipboard.SetText(copyReport.ToCsv(includeHeaderCheck.Checked,_optionalInformation));
				}

				MessageBox.Show(this, @"Copy to clipboard successful.", @"Copy result", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
			catch( Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}

		}

		private void IsClipboardEnabled()
		{
			var selectedReport = GetSelectedReport();
			var selectedLanguagesCount = selectedReport.LanguagesForPoject.Count(c => c.Value);
			copyBtn.Enabled = selectedLanguagesCount == 1;
		}

		private void IsCsvBtnEnabled()
		{
			var selectedReport = GetSelectedReport();
			var selectedLanguagesCount = selectedReport.LanguagesForPoject.Count(c => c.Value);
			csvBtn.Enabled = selectedLanguagesCount >= 1;
		}

		private void includeHeaderCheck_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void exitBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void csvBtn_Click(object sender, EventArgs e)
		{
			try
			{
				foreach (var project in _selectedProjectsForReport)
				{
					// check which languages to export
					var checkedLanguages = project.LanguagesForPoject.Where(c => c.Value);
					foreach (var languageReport in checkedLanguages)
					{
						var csvFullReportPath = GetReportFolderPath(languageReport.Key.PathToReport);

						//write report to Reports folder
						using (var sw = new StreamWriter(csvFullReportPath + Path.DirectorySeparatorChar +
						                                 languageReport.Key.TargetLang.Name + ".csv"))
						{
							//var report = new StudioAnalysisReport(languageReport.Key.PathToReport);
							var report = new StudioAnalysisReport(@"C:\Users\aghisa\Desktop\enhanced_analysis.xml");
							sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
						}

					}
				}

				MessageBox.Show(this, @"Export successful.", @"Export result", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
	
		}

		private string GetReportFolderPath(string projectFolderPath)
		{
			var reportFolderPath =
				projectFolderPath.Substring(0, projectFolderPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
			return reportFolderPath;
		}

		private void targetBtn_Click(object sender, EventArgs e)
		{
			var selectedProject = (ProjectDetails)projListbox.SelectedItem;
			if (selectedProject != null)
			{
				var reportPath = Path.Combine(selectedProject.ProjectFolderPath, "Reports");
				if (Directory.Exists(reportPath))
				{
					Process.Start("explorer.exe", "\"" + reportPath + "\"");
				}
			}
		}

		private void adaptiveMT_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeAdaptiveBaseline = adaptiveMT.Checked;
		}

		private void fragmentMatches_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeAdaptiveLearnings = adaptiveLearnings.Checked;
		}

		private void internalFuzzies_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeInternalFuzzies = internalFuzzies.Checked;
		}

		private void locked_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeLocked = locked.Checked;
		}

		private void perfectMatch_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludePerfectMatch = perfectMatch.Checked;
		}

		private void contextMatch_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeContextMatch = contextMatch.Checked;
		}

		private void crossRep_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeCrossRep = crossRep.Checked;
		}
	}
}
