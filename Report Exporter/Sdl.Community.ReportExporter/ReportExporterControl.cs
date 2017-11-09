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
		private  string _projectXmlPath;
		private  List<ReportDetails> _selectedProjectsForReport;
		private  OptionalInformation _optionalInformation;
		private List<ProjectDetails> _allStudioProjectsDetails;
		private readonly List<ProjectDetails> _selectedProjectsFromStudioView = new List<ProjectDetails>();

		public ReportExporterControl()
		{
			InitializeComponent();
			InitializeSettings();
		}


		private void InitializeSettings()
		{
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
			targetBtn.Enabled = false;
			includeHeaderCheck.Checked = true;
			_projectXmlPath = Help.GetStudioProjectsPath();
			_selectedProjectsForReport = new List<ReportDetails>();
			_allStudioProjectsDetails = new List<ProjectDetails>();
			projectStatusComboBox.SelectedIndex = 0;
			LoadProjectsList();

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
		public ReportExporterControl(List<string> studioProjectsPath)
		{
			InitializeComponent();
			InitializeSettings();

			//convert projects from list into list model
			var projectList = projListbox.Items.Cast<ProjectDetails>().ToList();

			var selectedIndexList = new List<int>();

			foreach (var path in studioProjectsPath)
			{
				var item = projectList.FirstOrDefault(p => p.ProjectPath.Equals(path));
				var index = projectList.FindIndex(n => n.ProjectPath.Equals(path));
				selectedIndexList.Add(index);
				_selectedProjectsFromStudioView.Add(item);

			}
			//select in listbox projects selected in Studio Projects view
			foreach (var index in selectedIndexList)
			{
				projListbox.SetSelected(index, true);
				projListbox.SetItemCheckState(index,CheckState.Checked);
			}
		}

		private void AddNewProject(ProjectDetails project,List<LanguageDirection> languages,bool isProjectSelectedFromView)
		{
			//var projectDetails = projListbox.SelectedItem as ProjectDetails;
			var selectedProjectName = Empty;

			if (project != null)
			{
				selectedProjectName = project.ProjectName;
			}

			var reportDetails = new ReportDetails
			{
				ProjectName = selectedProjectName,
				LanguagesForPoject = new Dictionary<LanguageDirection, bool>()
			};
			foreach (LanguageDirection language in languages)
			{
				reportDetails.LanguagesForPoject.Add(language, isProjectSelectedFromView);

			}
			//add to list where we'll keep information about checkboxes state
			_selectedProjectsForReport.Add(reportDetails);
		}

		private void FillLanguagesList(ProjectDetails projectDetails)
		{
			//var projectDetails = projListbox.SelectedItem as ProjectDetails;
			if (projectDetails != null)
			{
				languagesListBox.Items.Clear();
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
					var projectDetails = CreateProjectDetails((XmlNode) item);
					projListbox.Items.Add(projectDetails);
					_allStudioProjectsDetails.Add(projectDetails);
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
			var projectStatus = ProjectInformation.GetProjectStatus(projectDetails.ProjectPath);
			projectDetails.Status = projectStatus;
			return projectDetails;
		}


		

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{

			if (projListbox.SelectedItem == null) return;

			languagesListBox.Items.Clear();
			var doc = new XmlDocument();
			ProjectDetails selectedProject;

			if (_selectedProjectsFromStudioView.Count > 0)
			{
				foreach (var project in _selectedProjectsFromStudioView)
				{
					selectedProject = project;

					doc.Load(selectedProject.ProjectPath);
					var languages = Help.LoadLanguageDirections(doc);
					Help.LoadReports(doc, selectedProject.ProjectFolderPath, languages);

					var projectLanguages = languages.Values.Where(item => !IsNullOrEmpty(item.PathToReport)).ToList();

					//add resutls to the list view
					var selectedProjectExists = _selectedProjectsForReport.Any(n => n.ProjectName.Equals(selectedProject.ProjectName));

					if (!selectedProjectExists)
					{
						AddNewProject(project,projectLanguages,true);
					}

					FillLanguagesList(project);
				}
			}
			else
			{
				selectedProject = projListbox.SelectedItem as ProjectDetails;

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
						AddNewProject((ProjectDetails)projListbox.SelectedItem, projectLanguages,false);
					}
					FillLanguagesList((ProjectDetails)projListbox.SelectedItem);

					var project = GetSelectedProject();
					if (project.ReportPath != null)
					{
						reportOutputPath.Text = project.ReportPath;
					}
					else
					{
						reportOutputPath.Text = Empty;
					}
					if (languages.Count.Equals(1))
					{
						//check the language
						languagesListBox.SelectedIndex = 0;
						languagesListBox.SetItemChecked(0, true);

						var selectedLanguage = (LanguageDirection)languagesListBox.SelectedItem;
						if (project.LanguagesForPoject.ContainsKey(selectedLanguage))
						{
							project.LanguagesForPoject[selectedLanguage] = true;
						}
					}
				}
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
				if (selectedLanguage != null)
				{
					var index = languagesListBox.SelectedIndex;
					var isChecked = languagesListBox.GetItemChecked(index);

					var selectedProject = GetSelectedProject();
					if (selectedProject.LanguagesForPoject.ContainsKey(selectedLanguage))
					{
						selectedProject.LanguagesForPoject[selectedLanguage] = isChecked;
					}
				}
			
			}
			IsClipboardEnabled();
			IsCsvBtnEnabled();
		}

		private ReportDetails GetSelectedProject()
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
					var selectedReport = GetSelectedProject();
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
			var selectedReport = GetSelectedProject();
			var selectedLanguagesCount = selectedReport.LanguagesForPoject.Count(c => c.Value);
			copyBtn.Enabled = selectedLanguagesCount == 1;
		}

		private void IsCsvBtnEnabled()
		{
			var selectedReport = GetSelectedProject();
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

						if (project.ReportPath == null)
						{
							project.ReportPath = reportOutputPath.Text;
						}

						//write report to Reports folder
						using (var sw = new StreamWriter(project.ReportPath + Path.DirectorySeparatorChar +project.ProjectName+"_"+
						                                 languageReport.Key.TargetLang.Name + ".csv"))
						{
							var report = new StudioAnalysisReport(languageReport.Key.PathToReport);
							//var report = new StudioAnalysisReport(@"C:\Users\aghisa\Desktop\enhanced_analysis.xml");
							sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
						}
					}
				}

				//Clear all lists
				_selectedProjectsForReport.Clear();
				_selectedProjectsFromStudioView.Clear();
				projListbox.SelectedItems.Clear();
				UncheckAllProjects();
				languagesListBox.Items.Clear();
				

				MessageBox.Show(this, @"Export successful.", @"Export result", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
	
		}

		private void UncheckAllProjects()
		{
			while (projListbox.CheckedIndices.Count > 0)
			{
				projListbox.SetItemChecked(projListbox.CheckedIndices[0], false);
			}
		}

		/// <summary>
		/// Gets the path to Reports folder from Studio project folder
		/// </summary>
		/// <param name="projectFolderPath"></param>
		/// <returns></returns>
		private string GetReportFolderPath(string projectFolderPath)
		{
			var reportFolderPath =
				projectFolderPath.Substring(0, projectFolderPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
			return reportFolderPath;
		}

		private void targetBtn_Click(object sender, EventArgs e)
		{
			//var selectedProject = GetSelectedProject();
			//if (selectedProject != null)
			//{
			//	if (Directory.Exists(selectedProject.ReportPath))
			//	{
			//		Process.Start("explorer.exe", "\"" + selectedProject.ReportPath + "\"");
			//	}
			//}
			if (!IsNullOrEmpty(reportOutputPath.Text))
			{
				Process.Start("explorer.exe", "\"" + reportOutputPath.Text + "\"");
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

		private void browseBtn_Click(object sender, EventArgs e)
		{
			var folderPath = new FolderSelectDialog();
			if (folderPath.ShowDialog())
			{
				reportOutputPath.Text = folderPath.FileName;
			}

			var selectedProject = GetSelectedProject();
			selectedProject.ReportPath = folderPath.FileName;

		}

		private void reportOutputPath_KeyUp(object sender, KeyEventArgs e)
		{
			var reportPath = ((TextBox) sender).Text;
			if (!IsNullOrWhiteSpace(reportPath))
			{
				var selectedProject = GetSelectedProject();
				selectedProject.ReportPath = reportPath;
			}
			
		}

		private void projectStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedStatus = ((ComboBox) sender).SelectedItem;

			if (selectedStatus.Equals("InProgress"))
			{
				var inProgressProjects = _allStudioProjectsDetails.Where(s => s.Status.Equals("InProgress")).ToList();
				projListbox.Items.Clear();
				foreach (var project in inProgressProjects)
				{
					projListbox.Items.Add(project, false);
				}
			}
			if (selectedStatus.Equals("Completed"))
			{
				var inProgressProjects = _allStudioProjectsDetails.Where(s => s.Status.Equals("Completed")).ToList();
				projListbox.Items.Clear();
				foreach (var project in inProgressProjects)
				{
					projListbox.Items.Add(project, false);
				}
			}
			if (selectedStatus.Equals("All"))
			{
				projListbox.Items.Clear();
				foreach (var project in _allStudioProjectsDetails)
				{
					projListbox.Items.Add(project, false);
				}
			}
		}
	}
}
