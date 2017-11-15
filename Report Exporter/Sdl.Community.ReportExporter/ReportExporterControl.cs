using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private string _projectXmlPath;
		private OptionalInformation _optionalInformation;
		private List<ProjectDetails> _allStudioProjectsDetails;
		private List<ProjectDetails> _externalProjects = new List<ProjectDetails>();
		private readonly BindingList<LanguageDetails> _languages = new BindingList<LanguageDetails>();
		private BindingList<ProjectDetails> _projectsDataSource = new BindingList<ProjectDetails>();
		private bool _areExternalStudioProjects;

		public ReportExporterControl()
		{
			InitializeComponent();
			InitializeSettings();
		}

		private void InitializeSettings()
		{
			_areExternalStudioProjects = false;
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
			targetBtn.Enabled = false;
			includeHeaderCheck.Checked = true;
			_projectXmlPath = Help.GetStudioProjectsPath();
			_allStudioProjectsDetails = new List<ProjectDetails>();
			LoadProjectsList(_projectXmlPath);

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
			
			foreach (var path in studioProjectsPath)
			{
				var selectedProject = _projectsDataSource.FirstOrDefault(p => p.ProjectPath.Equals(path));
				if (selectedProject != null)
				{
					PrepareProjectToExport(selectedProject, true);
				}
			}

			RefreshProjectsListBox();
		}

		private void FillLanguagesList()
		{
			var selectedProjectsToExport = _projectsDataSource.Where(e => e.ShouldBeExported).ToList();

			foreach (var selectedProject in selectedProjectsToExport)
			{
				foreach (var language in selectedProject.LanguagesForPoject.ToList())
				{
					var languageDetails = _languages.FirstOrDefault(n => n.LanguageName.Equals(language.Key));
					if (languageDetails == null)
					{
						var newLanguage = new LanguageDetails
						{
							LanguageName = language.Key,
							IsChecked = false
						};
						_languages.Add(newLanguage);
					}

				}
			}
			languagesListBox.DataSource = _languages;
			languagesListBox.DisplayMember = "LanguageName";
			languagesListBox.ValueMember = "IsChecked";
		}

		/// <summary>
		/// Reads studio projects from project.xml
		/// Adds projects to listbox
		/// </summary>
		private void LoadProjectsList(string projectXmlPath)
		{
			var projectXmlDocument = new XmlDocument();

			projectXmlDocument.Load(projectXmlPath);

			var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
			if (projectsNodeList == null) return;
			foreach (var item in projectsNodeList)
			{
				var projectInfo = ((XmlNode) item).SelectSingleNode("./ProjectInfo");
				if (projectInfo?.Attributes != null && projectInfo.Attributes["IsInPlace"].Value != "true")
				{
					var reportExist = ReportFolderExist((XmlNode) item);
					if (reportExist)
					{
						var projectDetails = CreateProjectDetails((XmlNode) item);
						_projectsDataSource.Add(projectDetails);
						_allStudioProjectsDetails.Add(projectDetails);
					}
				}
			}
			projListbox.DataSource = _projectsDataSource;
			projListbox.ValueMember = "ShouldBeExported";
			projListbox.DisplayMember = "ProjectName";
		}

		private bool ReportFolderExist(XmlNode projectInfoNode)
		{
			if (projectInfoNode?.Attributes != null)
			{
				var filePath = Empty;

				if (projectInfoNode.Attributes["ProjectFilePath"] != null)
				{
					filePath = projectInfoNode.Attributes["ProjectFilePath"].Value;
					if (!Path.IsPathRooted(filePath))
					{
						//project is located inside "Projects" folder in Studio
						var projectsFolderPath = _projectXmlPath.Substring
							(0, _projectXmlPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						var projectName = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal));
						filePath = Path.Combine(projectsFolderPath, projectName, "Reports");
					}
					else
					{
						//is external project
						var reportsPath = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
						filePath = Path.Combine(reportsPath, "Reports");
					}
				}
				return Help.ReportFileExist(filePath);
			}
			return false;
		}

		/// <summary>
		/// Creates project details for given project from xml file
		/// </summary>
		/// <param name="projNode"></param>
		/// <returns></returns>
		private ProjectDetails CreateProjectDetails(XmlNode projNode)
		{
			var projectDetails = new ProjectDetails
			{
				LanguagesForPoject = new Dictionary<string, bool>(),
				ShouldBeExported = false
			};
			var projectFolderPath = Empty;
			var doc = new XmlDocument();

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

			doc.Load(projectDetails.ProjectPath);

			var projectLanguages = Help.LoadLanguageDirections(doc);

			SetLanguagesForProject(projectDetails, projectLanguages);

			projectDetails.Status = projectStatus;
			return projectDetails;
		}

		private void SetLanguagesForProject(ProjectDetails project, Dictionary<string, LanguageDirection> languages)
		{
			foreach (var language in languages)
			{
				project.LanguagesForPoject.Add(language.Value.TargetLang.EnglishName, false);
			}
		}

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (projListbox.SelectedItem == null) return;
			var projectName = ((CheckedListBox) sender).Text;
			var selectedProject = _projectsDataSource.FirstOrDefault(n => n.ProjectName.Equals(projectName));

			var selectedProjectIndex = _projectsDataSource.IndexOf(selectedProject);
			if (selectedProjectIndex > -1)
			{
				var shouldExportProject = ((CheckedListBox)sender).GetItemChecked(selectedProjectIndex);
				PrepareProjectToExport(selectedProject, shouldExportProject);
			}
			IsClipboardEnabled();

		}

		private void PrepareProjectToExport(ProjectDetails selectedProject,bool shouldExportProject)
		{
			if (selectedProject != null)
			{
				var doc = new XmlDocument();
				var selectedProjectIndex = _projectsDataSource.IndexOf(selectedProject);

				if (selectedProjectIndex > -1)
				{
					//Read sdlproj
					doc.Load(selectedProject.ProjectPath);
					Help.LoadReports(doc, selectedProject.ProjectFolderPath, selectedProject);
					
					selectedProject.ShouldBeExported = shouldExportProject;
					//if an project has only one language select that language
					if (selectedProject.LanguagesForPoject!=null)
					{
						if (selectedProject.LanguagesForPoject.Count.Equals(1))
						{
							var languageName = selectedProject.LanguagesForPoject.First().Key;
							var languageToBeSelected = _languages.FirstOrDefault(n => n.LanguageName.Equals(languageName));
							if (languageToBeSelected != null)
							{
								languageToBeSelected.IsChecked = true;

							}
							else
							{
								var newLanguage = new LanguageDetails
								{
									LanguageName = languageName,
									IsChecked = true
								};
								_languages.Add(newLanguage);
							}
							selectedProject.LanguagesForPoject[languageName] = true;
						}
					}

					var languagesAlreadySelectedForExport = _languages.Where(l => l.IsChecked).ToList();

					foreach (var language in languagesAlreadySelectedForExport)
					{
						if (selectedProject.LanguagesForPoject != null && selectedProject.LanguagesForPoject.ContainsKey(language.LanguageName))
						{
							selectedProject.LanguagesForPoject[language.LanguageName] = true;
						}
					}
					//show languages in language list box
					FillLanguagesList();

					reportOutputPath.Text = selectedProject.ReportPath ?? Empty;

					copyBtn.Enabled = projListbox.SelectedItems.Count == 1;
					if (projListbox.SelectedItems.Count > 0)
					{
						csvBtn.Enabled = true;
					}
					RefreshLanguageListbox();
				}
			}
		}

		private void languagesListBox_SelectedIndexChanged_1(object sender, EventArgs e)
		{

			var selectedLanguage = (LanguageDetails) languagesListBox.SelectedItem;
			if (selectedLanguage != null)
			{
				var index = languagesListBox.SelectedIndex;
				var shouldExportLanguage = languagesListBox.GetItemChecked(index);

				var projectsWithSelectedLaguage = _projectsDataSource
					.Where(p => p.ShouldBeExported && p.LanguagesForPoject.ContainsKey(selectedLanguage.LanguageName)).ToList();
				foreach (var project in projectsWithSelectedLaguage)
				{
					var language = project.LanguagesForPoject.FirstOrDefault(l => l.Key.Equals(selectedLanguage.LanguageName));
					project.LanguagesForPoject[language.Key] = shouldExportLanguage;
				}

				var languageToUpdate = _languages.FirstOrDefault(n => n.LanguageName.Equals(selectedLanguage.LanguageName));
				if (languageToUpdate != null)
				{
					languageToUpdate.IsChecked = shouldExportLanguage;
				}
			}

			RefreshLanguageListbox();
			IsClipboardEnabled();
			IsCsvBtnEnabled();
		}

		private void RefreshLanguageListbox()
		{
			for (var i = 0; i < languagesListBox.Items.Count; i++)
			{
				var language = (LanguageDetails)languagesListBox.Items[i];
				languagesListBox.SetItemChecked(i, language.IsChecked);
			}
		}

		private void copyBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var selectedProject = _projectsDataSource.FirstOrDefault(p => p.ShouldBeExported);
				if (selectedProject != null)
				{
					if (selectedProject.LanguagesForPoject.Count(c => c.Value) > 0)
					{
						var copyReport = new StudioAnalysisReport(selectedProject.PathToAnalyseResult);
						Clipboard.SetText(copyReport.ToCsv(includeHeaderCheck.Checked, _optionalInformation));

						MessageBox.Show(this, @"Copy to clipboard successful.", @"Copy result", MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					}
					else
					{
						MessageBox.Show(this, @"Please select at least one language for export", @"Copy result", MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					}

				}
			
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}

		}

		private void IsClipboardEnabled()
		{
			if (_projectsDataSource.Count(p => p.ShouldBeExported) > 1)
			{
				copyBtn.Enabled = false;
			}
			else
			{
				copyBtn.Enabled = true;
			}
		}

		private void IsCsvBtnEnabled()
		{
			csvBtn.Enabled = _projectsDataSource.Count(p => p.ShouldBeExported) >= 1;
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
				if (!IsNullOrEmpty(reportOutputPath.Text))
				{
					var projectsToBeExported = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
					foreach (var project in projectsToBeExported)
					{
						// check which languages to export
						var checkedLanguages = project.LanguagesForPoject.Where(l => l.Value).ToList();
						foreach (var languageReport in checkedLanguages)
						{

							if (project.ReportPath == null)
							{
								project.ReportPath = reportOutputPath.Text;
							}

							//write report to Reports folder
							using (var sw = new StreamWriter(project.ReportPath + Path.DirectorySeparatorChar + project.ProjectName + "_" +
							                                 languageReport.Key + ".csv"))
							{
								var report = new StudioAnalysisReport(project.PathToAnalyseResult);
								//var report = new StudioAnalysisReport(@"C:\Users\aghisa\Desktop\enhanced_analysis.xml");
								sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
							}
						}
					}

					//Clear all lists
					UncheckAllProjects();
					_languages.Clear();
					if (_areExternalStudioProjects)
					{
						_projectsDataSource.Clear();
					}

					MessageBox.Show(this, @"Export successful.", @"Export result", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(this, @"Please select output path to export reports", Empty, MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}

			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private void UncheckAllProjects()
		{
			var projectsToUncheck = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
			foreach (var project in projectsToUncheck)
			{
				project.ShouldBeExported = false;
				foreach (var language in project.LanguagesForPoject.ToList())
				{
					project.LanguagesForPoject[language.Key] = false;
				}
			}
			RefreshProjectsListBox();
		}

		private void RefreshProjectsListBox()
		{
			for (var i = 0; i < projListbox.Items.Count; i++)
			{
				var project = (ProjectDetails)projListbox.Items[i];
				projListbox.SetItemChecked(i, project.ShouldBeExported);
				if (project.ShouldBeExported)
				{
					projListbox.SetSelected(i, true);
				}
			}
		}

		private void targetBtn_Click(object sender, EventArgs e)
		{
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
		}

		private void reportOutputPath_KeyUp(object sender, KeyEventArgs e)
		{
			var reportPath = ((TextBox) sender).Text;
			if (!IsNullOrWhiteSpace(reportPath))
			{
				targetBtn.Enabled = true;
			}
		}

		private void projectStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedStatus = ((ComboBox) sender).SelectedItem;
			var projectsBindingList = new BindingList<ProjectDetails>();
			_languages.Clear();

			var projects = _areExternalStudioProjects ? _externalProjects : _allStudioProjectsDetails;
			if (selectedStatus.Equals("InProgress"))
			{
				var inProgressProjects = projects.Where(s => s.Status.Equals("InProgress")).ToList();

				foreach (var project in inProgressProjects)
				{
					projectsBindingList.Add(project);
				}

			}
			if (selectedStatus.Equals("Completed"))
			{
				var completedProjects = projects.Where(s => s.Status.Equals("Completed")).ToList();
				foreach (var project in completedProjects)
				{
					projectsBindingList.Add(project);
				}
			}
			if (selectedStatus.Equals("All"))
			{
				foreach (var project in projects)
				{
					projectsBindingList.Add(project);
				}
			}
			_projectsDataSource = projectsBindingList;
			projListbox.DataSource = _projectsDataSource;
		}

		private void loadBtn_Click(object sender, EventArgs e)
		{
			var loadFolderPath = new FolderSelectDialog();
			var doc = new XmlDocument();
			if (loadFolderPath.ShowDialog())
			{
				var externalProjectsBindingList = new BindingList<ProjectDetails>();
				_areExternalStudioProjects = true;
				_languages.Clear();
				_projectsDataSource.Clear();
				var projectsPathList = Directory.GetFiles(loadFolderPath.FileName, "*.sdlproj", SearchOption.AllDirectories);
				foreach (var projectPath in projectsPathList)
				{
					var reportFolderPath = Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)),"Reports");
					if (Help.ReportFileExist(reportFolderPath))
					{
						var projectDetails = ProjectInformation.GetExternalProjectDetails(projectPath);

						doc.Load(projectDetails.ProjectPath);
						Help.LoadReports(doc, projectDetails.ProjectFolderPath, projectDetails);
						externalProjectsBindingList.Add(projectDetails);
					}
				}
				foreach (var item in externalProjectsBindingList)
				{
					_projectsDataSource.Add(item);
				}

				projListbox.DataSource = _projectsDataSource;
				RefreshProjectsListBox();
				RefreshLanguageListbox();
				
			}
		
		}

		private void clearBtn_Click(object sender, EventArgs e)
		{
			_areExternalStudioProjects = false;
			projListbox.Items.Clear();
			_languages.Clear();

			foreach (var project in _allStudioProjectsDetails)
			{
				projListbox.Items.Add(project, false);
			}
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
		}
	}
}
