using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Helpers;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.ExportAnalysisReports.Service;
using static System.String;
using Help = Sdl.Community.ExportAnalysisReports.Helpers.Help;

namespace Sdl.Community.ExportAnalysisReports
{
	public partial class ReportExporterControl : Form
	{
		public static readonly Log Log = Log.Instance;

		private string _projectXmlPath;
		private OptionalInformation _optionalInformation;
		private List<ProjectDetails> _allStudioProjectsDetails;
		private readonly BindingList<LanguageDetails> _languages = new BindingList<LanguageDetails>();
		private BindingList<ProjectDetails> _projectsDataSource = new BindingList<ProjectDetails>();
		private readonly IMessageBoxService _messageBoxService;

		public ReportExporterControl()
		{
			InitializeComponent();
			InitializeSettings();
			_messageBoxService = new MessageBoxService();
		}

		public ReportExporterControl(List<string> studioProjectsPath)
		{
			InitializeComponent();
			InitializeSettings();
			_messageBoxService = new MessageBoxService();

			foreach (var path in studioProjectsPath)
			{
				var selectedProject = _projectsDataSource.FirstOrDefault(p => p.ProjectPath.Equals(path));
				if (selectedProject != null)
				{
					PrepareProjectToExport(selectedProject);
				}
			}
			RefreshProjectsListBox();
		}

		private void InitializeSettings()
		{
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
			includeHeaderCheck.Checked = true;
			_projectXmlPath = Help.GetStudioProjectsPath();
			_allStudioProjectsDetails = new List<ProjectDetails>();
			LoadProjectsList(_projectXmlPath);
			reportOutputPath.Text = Help.GetJsonReportPath(Help.JsonPath);
			targetBtn.Enabled = !string.IsNullOrEmpty(reportOutputPath.Text) ? true : false;

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

		private void FillLanguagesList()
		{
			try
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
				if (!_languages.Any(l => l.LanguageName.Equals("All languages")))
				{
					_languages.Insert(0, new LanguageDetails
					{
						LanguageName = "All languages",
						IsChecked = false
					});
				}
				languagesListBox.DataSource = _languages;
				languagesListBox.DisplayMember = "LanguageName";
				languagesListBox.ValueMember = "IsChecked";
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"FillLanguagesList method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Reads studio projects from project.xml
		/// Adds projects to listbox
		/// </summary>
		private void LoadProjectsList(string projectXmlPath)
		{
			try
			{
				var projectXmlDocument = new XmlDocument();
				if (!string.IsNullOrEmpty(projectXmlPath))
				{
					projectXmlDocument.Load(projectXmlPath);

					var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
					if (projectsNodeList == null) return;
					foreach (var item in projectsNodeList)
					{
						var projectInfo = ((XmlNode)item).SelectSingleNode("./ProjectInfo");
						if (projectInfo?.Attributes != null && projectInfo.Attributes["IsInPlace"].Value != "true")
						{
							var reportExist = ReportFolderExist((XmlNode)item);
							if (reportExist)
							{
								var projectDetails = CreateProjectDetails((XmlNode)item);
								_projectsDataSource.Add(projectDetails);
								_allStudioProjectsDetails.Add(projectDetails);
							}
						}
					}
					projListbox.DataSource = _projectsDataSource;
					projListbox.ValueMember = "ShouldBeExported";
					projListbox.DisplayMember = "ProjectName";
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadProjectsList method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private bool ReportFolderExist(XmlNode projectInfoNode)
		{
			try
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
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ReportFolderExist method: {ex.Message}\n {ex.StackTrace}");
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

			try
			{
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
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}
			return projectDetails;
		}

		private void SetLanguagesForProject(ProjectDetails project, Dictionary<string, LanguageDirection> languages)
		{
			try
			{
				foreach (var language in languages)
				{
					project.LanguagesForPoject.Add(language.Value.TargetLang.EnglishName, false);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetLanguagesForProject method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (projListbox.SelectedItem == null) return;
				var projectName = ((CheckedListBox)sender).Text;
				var selectedProject = _projectsDataSource.FirstOrDefault(n => n.ProjectName.Equals(projectName));

				var selectedProjectIndex = _projectsDataSource.IndexOf(selectedProject);
				if (selectedProjectIndex > -1)
				{
					var shouldExportProject = ((CheckedListBox)sender).GetItemChecked(selectedProjectIndex);

					if (shouldExportProject)
					{
						PrepareProjectToExport(selectedProject);
					}//that means user deselected a project
					else
					{
						if (selectedProject != null)
						{
							selectedProject.ShouldBeExported = false;
							ShouldUnselectLanguages(selectedProject);
						}
					}
				}
				IsClipboardEnabled();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"projListbox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void ShouldUnselectLanguages(ProjectDetails selectedProject)
		{
			try
			{
				var selectedLanguagesFromProject = selectedProject.LanguagesForPoject.Where(n => n.Value).Select(n => n.Key).ToList();
				var count = 0;
				foreach (var languageName in selectedLanguagesFromProject)
				{
					//unselect language for project in data source list
					selectedProject.LanguagesForPoject[languageName] = false;

					var projectsToBeExported = _projectsDataSource.Where(n => n.LanguagesForPoject.ContainsKey(languageName)
																			  && n.ShouldBeExported).ToList();
					foreach (var project in projectsToBeExported)
					{
						var languageShouldBeExported = project.LanguagesForPoject[languageName];
						if (languageShouldBeExported)
						{
							count++;
						}
					}

					//that means no other project has this language selected so we can uncheck the language ox
					if (count.Equals(0))
					{
						var languageToBeDeleted = _languages.FirstOrDefault(l => l.LanguageName.Equals(languageName));
						if (languageToBeDeleted != null)
						{
							_languages.Remove(languageToBeDeleted);
						}
					}
				}

				// if the are any projects selected clear language list
				if (_projectsDataSource.Count(p => p.ShouldBeExported).Equals(0))
				{
					_languages.Clear();
				}
				RefreshLanguageListbox();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ShouldUnselectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void PrepareProjectToExport(ProjectDetails selectedProject)
		{
			try
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

						selectedProject.ShouldBeExported = true;
						//if an project has only one language select that language
						if (selectedProject.LanguagesForPoject != null)
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

						copyBtn.Enabled = projListbox.SelectedItems.Count == 1;
						if (projListbox.SelectedItems.Count > 0)
						{
							csvBtn.Enabled = true;
						}
						RefreshLanguageListbox();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"PrepareProjectToExport method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void languagesListBox_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			try
			{
				var selectedLanguage = (LanguageDetails)languagesListBox.SelectedItem;
				if (selectedLanguage != null)
				{
					var index = languagesListBox.SelectedIndex;
					var shouldExportLang = languagesListBox.GetItemChecked(index);
					if (selectedLanguage.LanguageName.Equals("All languages"))
					{
						for (int i = 0; i < languagesListBox.Items.Count; i++)
						{
							SetLanguageCheckedState(i, shouldExportLang);
						}
					}
					else
					{
						SetLanguageCheckedState(index, shouldExportLang);
					}
				}

				RefreshLanguageListbox();
				IsClipboardEnabled();
				IsCsvBtnEnabled();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"languagesListBox_SelectedIndexChanged_1 method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// Set the language checkbox value based on user's selection: checked/not checked and the index of the checkbox
		private void SetLanguageCheckedState(int index, bool isChecked)
		{
			try
			{
				var languageToUpdate = (LanguageDetails)languagesListBox.Items[index];

				var projectsWithSelectedLaguage = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
				foreach (var project in projectsWithSelectedLaguage)
				{
					var language = project.LanguagesForPoject.FirstOrDefault(l => l.Key.Equals(languageToUpdate.LanguageName));
					if (language.Key != null)
					{
						project.LanguagesForPoject[language.Key] = isChecked;
					}
				}
				languageToUpdate.IsChecked = isChecked;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetLanguageCheckedState method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void RefreshLanguageListbox()
		{
			try
			{
				for (var i = 0; i < languagesListBox.Items.Count; i++)
				{
					var language = (LanguageDetails)languagesListBox.Items[i];
					languagesListBox.SetItemChecked(i, language.IsChecked);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"RefreshLanguageListbox method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void copyBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var projectsToBeExported = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
				foreach (var selectedProject in projectsToBeExported)
				{
					var csvTextBuilder = new StringBuilder();
					if (selectedProject?.LanguagesForPoject.Count(c => c.Value) > 0)
					{
						var selectedLanguages = selectedProject.LanguagesForPoject.Where(l => l.Value == true);
						if (selectedProject.LanguageAnalysisReportPaths != null)
						{
							foreach (var selectedLanguage in selectedLanguages)
							{
								var languageAnalysisReportPath = selectedProject.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(selectedLanguage.Key));
								var copyReport = new StudioAnalysisReport(languageAnalysisReportPath.Value);
								var csvText = copyReport.ToCsv(includeHeaderCheck.Checked, _optionalInformation);
								csvTextBuilder.Append(csvText);
							}
							_messageBoxService.ShowOwnerInformationMessage(this, "Copy to clipboard was performed with success.", "Copy result");
							Clipboard.SetText(csvTextBuilder.ToString());
						}
						else
						{
							_messageBoxService.ShowOwnerInformationMessage(this, "No Analysis report was found for the selected language.", "Copy result");
						}
					}
					else
					{
						_messageBoxService.ShowOwnerInformationMessage(this, "Please select at least one language to copy the corresponding analysis file", "Copy result");
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				Log.Logger.Error($"copyBtn_Click method: {exception.Message}\n {exception.StackTrace}");
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
			var isSamePath = IsSameReportPath();
			if (!isSamePath)
			{
				Help.SaveExportPath(reportOutputPath.Text);
			}
			GenerateReport();
		}

		private bool IsSameReportPath()
		{
			var jsonReportPath = Help.GetJsonReportPath(Help.JsonPath);
			if (!string.IsNullOrEmpty(jsonReportPath)
				&& !string.IsNullOrEmpty(reportOutputPath.Text)
				&& jsonReportPath.Equals(reportOutputPath.Text))
			{
				return true;
			}
			return false;
		}

		private void GenerateReport()
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
						if (checkedLanguages.Count == 0)
						{
							_messageBoxService.ShowOwnerInformationMessage(this, "Please select at least one language to export the report!", "Export result");
							return;
						}
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
								if (project.LanguageAnalysisReportPaths != null)
								{
									var analyseReportPath = project.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(languageReport.Key));
									var report = new StudioAnalysisReport(analyseReportPath.Value);
									sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
								}
							}
						}
					}

					//Clear all lists
					UncheckAllProjects();
					_languages.Clear();
					selectAll.Checked = false;
					_messageBoxService.ShowOwnerInformationMessage(this, "The analysis files were exported with success.", "Export result");
				}
				else
				{
					_messageBoxService.ShowOwnerInformationMessage(this, "Please select the output path to export the reports", string.Empty);
				}

			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				Log.Logger.Error($"GenerateReport method: {exception.Message}\n {exception.StackTrace}");
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
			var reportPath = ((TextBox)sender).Text;
			if (!IsNullOrWhiteSpace(reportPath))
			{
				targetBtn.Enabled = true;
			}
			if (e.KeyCode == Keys.Enter)
			{
				GenerateReport();
			}
		}

		private void projectStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var selectedStatus = ((ComboBox)sender).SelectedItem;
				var projectsBindingList = new BindingList<ProjectDetails>();
				_languages.Clear();

				var projects = _allStudioProjectsDetails;
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
			catch (Exception ex)
			{
				Log.Logger.Error($"projectStatusComboBox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void loadBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var loadFolderPath = new FolderSelectDialog();
				var doc = new XmlDocument();
				if (loadFolderPath.ShowDialog())
				{
					var externalProjectsBindingList = new BindingList<ProjectDetails>();
					_languages.Clear();
					_projectsDataSource.Clear();
					var projectsPathList = Directory.GetFiles(loadFolderPath.FileName, "*.sdlproj", SearchOption.AllDirectories);
					foreach (var projectPath in projectsPathList)
					{
						var reportFolderPath = Path.Combine(projectPath.Substring(0, projectPath.LastIndexOf(@"\", StringComparison.Ordinal)), "Reports");
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
			catch (Exception ex)
			{
				Log.Logger.Error($"loadBtn_Click method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void clearBtn_Click(object sender, EventArgs e)
		{
			_projectsDataSource.Clear();
			_languages.Clear();
			foreach (var project in _allStudioProjectsDetails)
			{
				_projectsDataSource.Add(project);
			}

			projListbox.DataSource = _projectsDataSource;
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
		}

		private void selectAll_CheckedChanged(object sender, EventArgs e)
		{
			var selectAll = ((CheckBox)sender).Checked;

			foreach (var project in _projectsDataSource)
			{
				project.ShouldBeExported = selectAll;
				foreach (var language in project.LanguagesForPoject.ToList())
				{
					project.LanguagesForPoject[language.Key] = selectAll;
				}
			}
			RefreshProjectsListBox();
			if (selectAll)
			{
				foreach (var language in _languages)
				{
					language.IsChecked = true;
				}
			}
			else
			{
				_languages.Clear();
			}
			RefreshLanguageListbox();
		}

		private void reportOutputPath_TextChanged(object sender, EventArgs e)
		{
			var selectedOutputPath = ((TextBox)sender).Text;
			if(!string.IsNullOrEmpty(selectedOutputPath))
			{
				reportOutputPath.Text = selectedOutputPath;
				targetBtn.Enabled = true;
			}
		}
	}
}