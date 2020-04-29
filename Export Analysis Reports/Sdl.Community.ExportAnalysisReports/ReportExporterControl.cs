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
		private bool _isAnyLanguageUnchecked;
		private bool _isAnyProjectUnchecked;
		private bool _isStatusChanged;

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

			ConfigureCheckedItems();
		}

		private void InitializeSettings()
		{
			DisableButtons();
			includeHeaderCheck.Checked = true;
			_projectXmlPath = Help.GetStudioProjectsPath();
			_allStudioProjectsDetails = new List<ProjectDetails>();
			LoadProjectsList(_projectXmlPath);
			reportOutputPath.Text = Help.GetJsonReportPath(Help.JsonPath);
			targetBtn.Enabled = !IsNullOrEmpty(reportOutputPath.Text);
			_optionalInformation = SetOptionalInformation();
		}

		private OptionalInformation SetOptionalInformation()
		{
			return new OptionalInformation
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

		private void FillLanguagesList(ProjectDetails selectedProject)
		{
			try
			{
				var selectedProjectToExport = _projectsDataSource?.FirstOrDefault(e => e.ShouldBeExported && e.ProjectName.Equals(selectedProject.ProjectName));

				if (selectedProjectToExport != null)
				{
					foreach (var language in selectedProjectToExport.LanguagesForPoject.ToList())
					{
						var languageDetails = _languages.FirstOrDefault(n => n.LanguageName.Equals(language.Key));
						if (languageDetails == null)
						{
							var newLanguage = new LanguageDetails {LanguageName = language.Key, IsChecked = true};
							_languages.Add(newLanguage);
						}
					}

					languagesListBox.DataSource = _languages;
					languagesListBox.DisplayMember = "LanguageName";
					languagesListBox.ValueMember = "IsChecked";
				}
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
				LanguagesForPoject = new Dictionary<string, bool>(), ShouldBeExported = false
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

		private void ShouldUnselectLanguages(ProjectDetails selectedProject)
		{
			try
			{
				var selectedLanguagesFromProject = selectedProject.LanguagesForPoject.Where(n => n.Value).Select(n => n.Key).ToList();
				if (!selectedLanguagesFromProject.Any() && !selectedProject.ShouldBeExported)
				{
					RemoveLanguageFromProject(selectedProject);
				}
				else
				{
					foreach (var languageName in selectedLanguagesFromProject)
					{
						// reset count for each language
						var count = 0;
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

						// that means no other project has this language selected so we can uncheck the language from the "Select language(s) for export:" box
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
				}

				RefreshLanguageListbox();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ShouldUnselectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void RemoveLanguageFromProject(ProjectDetails selectedProject)
		{
			foreach (var language in selectedProject.LanguagesForPoject)
			{
				if (!language.Equals(new KeyValuePair<string, bool>()))
				{
					var languageToBeDeleted = _languages.FirstOrDefault(l => l.LanguageName.Equals(language.Key));
					if (languageToBeDeleted != null)
					{
						_languages.Remove(languageToBeDeleted);
					}
				}
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
										LanguageName = languageName, IsChecked = true
									};
									_languages.Add(newLanguage);
								}

								selectedProject.LanguagesForPoject[languageName] = true;
							}
						}

						var languagesAlreadySelectedForExport = _languages.Where(l => l.IsChecked).ToList();

						foreach (var language in languagesAlreadySelectedForExport)
						{
							if (selectedProject.LanguagesForPoject != null &&
								selectedProject.LanguagesForPoject.ContainsKey(language.LanguageName))
							{
								selectedProject.LanguagesForPoject[language.LanguageName] = true;
							}
						}

						//show languages in language list box
						FillLanguagesList(selectedProject);

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

		// Set the language checkbox value based on user's selection: checked/not checked and the index of the checkbox
		private void SetLanguageCheckedState(int index, bool isChecked)
		{
			try
			{
				var languageToUpdate = (LanguageDetails)languagesListBox.Items[index];
				var projectsToExport = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
				foreach (var project in projectsToExport)
				{
					var language = project.LanguagesForPoject.FirstOrDefault(l => l.Key.Equals(languageToUpdate.LanguageName));
					if (language.Key != null)
					{
						project.LanguagesForPoject[language.Key] = isChecked;
					}
				}

				var checkedLanguage = _languages.FirstOrDefault(n => n.LanguageName.Equals(languageToUpdate.LanguageName));
				if (checkedLanguage != null)
				{
					checkedLanguage.IsChecked = isChecked;
				}
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
								var languageAnalysisReportPath =
									selectedProject.LanguageAnalysisReportPaths.FirstOrDefault(l =>
										l.Key.Equals(selectedLanguage.Key));
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
			var isMultipleProjectsSelected = _projectsDataSource.Count(p => p.ShouldBeExported) > 1;
			copyBtn.Enabled = !isMultipleProjectsSelected;
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
					Help.CreateDirectory(reportOutputPath.Text);
					var projectsToBeExported = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
					var areCheckedLanguages = projectsToBeExported.Any(p => p.LanguagesForPoject.Any(l => l.Value));
					if (!areCheckedLanguages && projectsToBeExported.Count >= 1)
					{
						_messageBoxService.ShowOwnerInformationMessage(this, "Please select at least one language to export the report!", "Export result");
					}
					else
					{
						foreach (var project in projectsToBeExported)
						{
							// check which languages to export
							var checkedLanguages = project.LanguagesForPoject.Where(l => l.Value).ToList();

							foreach (var languageReport in checkedLanguages)
							{
								if (string.IsNullOrEmpty(project.ReportPath))
								{
									project.ReportPath = reportOutputPath.Text;
								}

								//write report to Reports folder
								var streamPath = Path.Combine($"{project.ReportPath}{Path.DirectorySeparatorChar}",
									$"{project.ProjectName}_{languageReport.Key}.csv");
								using (var sw = new StreamWriter(streamPath))
								{
									if (project?.LanguageAnalysisReportPaths != null)
									{
										var analyseReportPath =
											project.LanguageAnalysisReportPaths.FirstOrDefault(l =>
												l.Key.Equals(languageReport.Key));
										if (!analyseReportPath.Equals(new KeyValuePair<string, string>()))
										{
											var report = new StudioAnalysisReport(analyseReportPath.Value);
											sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
										}
									}
								}
							}
						}

						//Clear all lists
						UncheckAllProjects();
						_languages.Clear();
						chkBox_SelectAllProjects.Checked = false;
						_messageBoxService.ShowOwnerInformationMessage(this, "The analysis files were exported with success.", "Export result");
					}
				}
				else
				{
					_messageBoxService.ShowOwnerInformationMessage(this, "Please select an existing folder to export the reports!", string.Empty);
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

		/// <summary>
		/// Configure the checked items: selected project(s) with "All selected languages" options
		/// </summary>
		private void ConfigureCheckedItems()
		{
			chkBox_SelectAllLanguages.Checked = true;
			RefreshProjectsListBox();
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
				var selectedStatus = ((ComboBox)sender).SelectedItem?.ToString();
				//_languages.Clear();

				_projectsDataSource = BindProjectsBasedOnStatus(selectedStatus);
				projListbox.DataSource = _projectsDataSource;

				if (languagesListBox.Items.Count == 0)
				{
					SetNewProjectLanguage();
				}

				if (chkBox_SelectAllProjects.Checked)
				{
					SetProjectsInformation(true);
				}

				_isStatusChanged = false;
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
			DisableButtons();
		}

		private void reportOutputPath_TextChanged(object sender, EventArgs e)
		{
			var selectedOutputPath = ((TextBox)sender).Text;
			if (!string.IsNullOrEmpty(selectedOutputPath))
			{
				reportOutputPath.Text = selectedOutputPath;
				targetBtn.Enabled = true;
			}
			else
			{
				targetBtn.Enabled = false;
			}
		}

		private BindingList<ProjectDetails> BindProjects(List<ProjectDetails> projects, BindingList<ProjectDetails> projectsBindingList)
		{
			foreach (var project in projects)
			{
				projectsBindingList.Add(project);
			}

			return projectsBindingList;
		}

		private void languagesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			var checkBoxValue = e.NewValue == CheckState.Checked;

			SetLanguageCheckedState(e.Index, checkBoxValue);
			_isAnyLanguageUnchecked = !checkBoxValue && chkBox_SelectAllLanguages.Checked;
			UncheckAllLanguagesOption(checkBoxValue);
		}

		// Uncheck the "Select all languages" option if one of the languages is unchecked
		private void UncheckAllLanguagesOption(bool isChecked)
		{
			if (!isChecked && _isAnyLanguageUnchecked)
			{
				chkBox_SelectAllLanguages.Checked = false;
			}
		}

		// Select/deselect all languages
		private void ChangeLanguagesCheckbox(bool isLanguageChecked)
		{
			try
			{
				for (var i = 0; i < languagesListBox.Items.Count; i++)
				{
					SetLanguageCheckedState(i, isLanguageChecked);
					languagesListBox.SetItemChecked(i, isLanguageChecked);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"languagesListBox_SelectedIndexChanged_1 method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void SetNewProjectLanguage()
		{
			if (_projectsDataSource != null)
			{
				foreach (var project in _projectsDataSource)
				{
					if (project.ShouldBeExported)
					{
						FillLanguagesList(project);
					}
				}
			}

			RefreshLanguageListbox();
			if (languagesListBox.CheckedItems.Count.Equals(languagesListBox.Items.Count))
			{
				chkBox_SelectAllLanguages.Checked = true;
			}
		}

		private void selectAllProjects_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isAnyProjectUnchecked)
			{
				var isSelectAllProjects = ((CheckBox)sender).Checked;
				if (!isSelectAllProjects)
				{
					DisableButtons();
				}

				SetProjectsInformation(isSelectAllProjects);
			}

			_isAnyProjectUnchecked = false;
		}

		private void chkBox_SelectAllLanguages_CheckedChanged(object sender, EventArgs e)
		{
			// change all the languages checkbox values only when the "Select all languages" option is checked/unchecked
			var isChecked = ((CheckBox)sender).Checked;
			if (!isChecked)
			{
				DisableButtons();
			}
			else if(languagesListBox.Items.Count > 0)
			{
				IsClipboardEnabled();
				IsCsvBtnEnabled();
			}

			if (!_isAnyLanguageUnchecked)
			{
				ChangeLanguagesCheckbox(isChecked);
			}
			_isAnyLanguageUnchecked = false;
		}

		private void projListbox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			try
			{
				var selectedItem = (ProjectDetails)projListbox?.Items[e.Index];
				var shouldExportProject = e.NewValue == CheckState.Checked;

				if (selectedItem != null)
				{
					if (projListbox != null && projListbox.SelectedItem == null) return;

					var selectedProject = _projectsDataSource.FirstOrDefault(n => n.ProjectName.Equals(selectedItem.ProjectName));

					var selectedProjectIndex = _projectsDataSource.IndexOf(selectedProject);
					if (selectedProjectIndex > -1 &&  shouldExportProject) 
					{
						PrepareProjectToExport(selectedProject);
					}
					else
					{
						// Uncheck the project when user deselects it
						if (selectedProject != null)
						{
							selectedProject.ShouldBeExported = false;
							ShouldUnselectLanguages(selectedProject);
						}
					}
					_isAnyProjectUnchecked = !shouldExportProject && chkBox_SelectAllProjects.Checked;
					DisableAllProjectsOption();
					IsClipboardEnabled();
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"projListbox_ItemCheck method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isStatusChanged)
			{
				// Check the "Select all projects" option when all projects are all checked, one by one.
				var isAllProjectsChecked = IsSelectAllChecked(projListbox);
				if (isAllProjectsChecked)
				{
					chkBox_SelectAllProjects.Checked = true;
				}

				// check the 'Select all languages' option when a single project is selected
				if (projListbox.CheckedItems.Count == 1 && !chkBox_SelectAllLanguages.Checked)
				{
					chkBox_SelectAllLanguages.Checked = true;
				}

				ConfigureCheckedOptions(projListbox);
			}
		}

		private void languagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Check the "Select all languages" option when all languages are all checked, one by one.
			var isAllLanguagesChecked = IsSelectAllChecked(languagesListBox);
			if (isAllLanguagesChecked)
			{
				chkBox_SelectAllLanguages.Checked = true;
			}

			if (languagesListBox.Items.Count >= 1)
			{
				IsClipboardEnabled();
				IsCsvBtnEnabled();
			}

			ConfigureCheckedOptions(languagesListBox);
		}

		private BindingList<ProjectDetails> BindProjectsBasedOnStatus(string selectedStatus)
		{
			var projectsBindingList = new BindingList<ProjectDetails>();
			_isStatusChanged = true;
			try
			{
				//_languages.Clear();
				var projects = _allStudioProjectsDetails;

				switch (selectedStatus)
				{
					case "InProgress":
						var inProgressProjects = projects.Where(s => s.Status.Equals("InProgress")).ToList();
						projectsBindingList = BindProjects(inProgressProjects, projectsBindingList);
						break;

					case "Completed":
						var completedProjects = projects?.Where(s => s.Status.Equals("Completed")).ToList();
						if (completedProjects != null && completedProjects.Count > 0)
						{
							projectsBindingList = BindProjects(completedProjects, projectsBindingList);
						}
						else
						{
							_languages.Clear();
							DisableButtons();

							// uncheck the 'Select all projects' and 'Select all languages' when are checked and no projects has 'Completed' status
							if (chkBox_SelectAllProjects.Checked)
							{
								chkBox_SelectAllProjects.Checked = false;
							}

							if (chkBox_SelectAllLanguages.Checked)
							{
								chkBox_SelectAllLanguages.Checked = false;
							}
						}

						break;

					case "All":
						projectsBindingList = BindProjects(projects, projectsBindingList);
						break;

				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"BindProjectsBasedOnStatus method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectsBindingList;
		}

		private void SetProjectsInformation(bool isSelectAllProjects)
		{
			foreach (var project in _projectsDataSource)
			{
				project.ShouldBeExported = isSelectAllProjects;
				foreach (var language in project.LanguagesForPoject.ToList())
				{
					project.LanguagesForPoject[language.Key] = isSelectAllProjects;
				}

				FillLanguagesList(project);
			}

			RefreshProjectsListBox();
			if (isSelectAllProjects)
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

			chkBox_SelectAllLanguages.Checked = isSelectAllProjects;
			RefreshLanguageListbox();
		}

		// Disable the 'Select all projects' option when one of the project is unchecked
		private void DisableAllProjectsOption()
		{
			if (chkBox_SelectAllProjects.Checked && _isAnyProjectUnchecked)
			{
				chkBox_SelectAllProjects.Checked = false;
			}
		}

		// Verify if the SelectAll options (Project and Languages) should be automatically checked, when all list box items are manually checked, one by one.
		private bool IsSelectAllChecked(CheckedListBox listbox)
		{
			return listbox.CheckedItems.Count == listbox.Items.Count;
		}

		// Configure the UI elements display
		private void ConfigureCheckedOptions(CheckedListBox listbox)
		{
			if (listbox.CheckedItems.Count == 0)
			{
				DisableButtons();
			}

			if (listbox.CheckedItems.Count == 0 && chkBox_SelectAllLanguages.Checked)
			{
				// Uncheck 'Select all languages' option when no item is checked
				chkBox_SelectAllLanguages.Checked = false;
			}
		}

		private void DisableButtons()
		{
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
		}
	}
}