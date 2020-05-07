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

namespace Sdl.Community.ExportAnalysisReports
{
	public partial class ReportExporterControl : Form
	{
		private string _projectXmlPath;
		private OptionalInformation _optionalInformation;
		private List<ProjectDetails> _allStudioProjectsDetails;
		private readonly BindingList<LanguageDetails> _languages = new BindingList<LanguageDetails>();
		private BindingList<ProjectDetails> _projectsDataSource = new BindingList<ProjectDetails>();
		private readonly IMessageBoxService _messageBoxService;
		private bool _isAnyLanguageUnchecked;
		private bool _isAnyProjectUnchecked;
		private bool _isStatusChanged;
		private Helpers.Help _help;
		private string _reportsFolderPath;
		private readonly IStudioService _studioService;
		private readonly IReportService _reportService;

		public ReportExporterControl()
		{
			_messageBoxService = new MessageBoxService();
			_studioService = new StudioService();
			_reportService = new ReportService(_messageBoxService, _studioService);

			InitializeComponent();
			InitializeSettings(new List<string>());
		}

		public ReportExporterControl(List<string> studioProjectsPath)
		{
			_messageBoxService = new MessageBoxService();
			_studioService = new StudioService();
			_reportService = new ReportService(_messageBoxService, _studioService);

			InitializeComponent();
			InitializeSettings(studioProjectsPath);

			foreach (var path in studioProjectsPath)
			{
				var selectedProject = _projectsDataSource.FirstOrDefault(p => p.ProjectPath.Equals(path));
				if (selectedProject != null)
				{
					PrepareProjectToExport(selectedProject);
				}
				else
				{
					// disable the Copy to clipboard and Export to CSV buttons if the selected project is null
					DisableButtons();
				}
			}

			ConfigureCheckedItems();
		}

		private void InitializeSettings(List<string> studioProjectsPath)
		{
			DisableButtons();
			_help = new Helpers.Help();
			includeHeaderCheck.Checked = true;
			_projectXmlPath = _studioService.GetStudioProjectsPath();
			_allStudioProjectsDetails = new List<ProjectDetails>();
			LoadProjectsList(_projectXmlPath, studioProjectsPath);
			reportOutputPath.Text = _reportService.GetJsonReportPath(_reportService.JsonPath);
			targetBtn.Enabled = !IsNullOrEmpty(reportOutputPath.Text);
			_optionalInformation = SetOptionalInformation();
			projectStatusComboBox.SelectedIndex = 0;
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

				if (selectedProjectToExport?.LanguagesForPoject != null)
				{
					foreach (var language in selectedProjectToExport.LanguagesForPoject.ToList())
					{
						var languageDetails = _languages?.FirstOrDefault(n => n.LanguageName.Equals(language.Key));
						if (languageDetails == null)
						{
							var newLanguage = new LanguageDetails {LanguageName = language.Key, IsChecked = true};
							_languages?.Add(newLanguage);
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
		private void LoadProjectsList(string projectXmlPath, List<string> studioProjectsPaths)
		{
			try
			{
				var projectXmlDocument = new XmlDocument();
				var filePathNames = new List<string>();
				
				// use the projPath to identify when user opens a single file project
				foreach (var path in studioProjectsPaths)
				{
					filePathNames.Add(Path.GetFileName(path));
				}

				if (!IsNullOrEmpty(projectXmlPath))
				{
					projectXmlDocument.Load(projectXmlPath);

					var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
					if (projectsNodeList == null) return;
					foreach (var item in projectsNodeList)
					{
						var projectInfo = ((XmlNode)item).SelectSingleNode("./ProjectInfo");
						if (projectInfo?.Attributes != null)
						{
							var reportExist = ReportFolderExist((XmlNode)item);
							if (reportExist)
							{
								var xmlAttributeCollection = ((XmlNode) item)?.Attributes;
								if (xmlAttributeCollection != null)
								{
									var projFileName = Path.GetFileName(xmlAttributeCollection?["ProjectFilePath"]?.Value);
									var projPath = filePathNames.FirstOrDefault(p => p.Equals(projFileName));
									if (projectInfo.Attributes["IsInPlace"].Value.Equals("true") && !IsNullOrEmpty(projPath))
									{
										// Include the selected single file project ONLY when user selects it within Projects view -> right click -> Export Analysis Reports
										SetProjectDetails((XmlNode) item, true);
									}
									else if(projectInfo.Attributes["IsInPlace"].Value.Equals("false"))
									{
										// Include all projects that are not single file project
										SetProjectDetails((XmlNode) item, false);
									}
								}
							}
						}
					}
					SetDataSource();
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadProjectsList method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void SetProjectDetails(XmlNode item, bool isSingleFileProject)
		{
			try
			{
				var projectDetails = CreateProjectDetails(item, isSingleFileProject);

				_projectsDataSource.Add(projectDetails);
				_allStudioProjectsDetails.Add(projectDetails);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void SetDataSource()
		{
			projListbox.DataSource = _projectsDataSource;
			projListbox.ValueMember = "ShouldBeExported";
			projListbox.DisplayMember = "ProjectName";
		}

		// Load all single file projects from projects.xml file
		private void LoadSingleFileProjects(string projectXmlPath)
		{
			try
			{
				var projectXmlDocument = new XmlDocument();
				if (!IsNullOrEmpty(projectXmlPath))
				{
					projectXmlDocument.Load(projectXmlPath);

					var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
					if (projectsNodeList == null) return;
					foreach (var item in projectsNodeList)
					{
						var projectInfo = ((XmlNode)item).SelectSingleNode("./ProjectInfo");
						if (projectInfo?.Attributes != null && projectInfo.Attributes["IsInPlace"].Value == "true")
						{
							var reportExist = ReportFolderExist((XmlNode)item);
							if (reportExist)
							{
								var projectDetails = CreateProjectDetails((XmlNode)item, true);
								if (!_projectsDataSource.Any(p => p.ProjectName.Equals(projectDetails.ProjectName)))
								{
									if (chkBox_SelectAllProjects.Checked)
									{
										projectDetails.ShouldBeExported = true;
									}
									_projectsDataSource.Add(projectDetails);
									_allStudioProjectsDetails.Add(projectDetails);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadSingleFileProjects method: {ex.Message}\n {ex.StackTrace}");
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
						filePath = projectInfoNode.Attributes["ProjectFilePath"]?.Value;
						if (!Path.IsPathRooted(filePath))
						{
							//project is located inside "Projects" folder in Studio
							var projectsFolderPath = _projectXmlPath.Substring(0, _projectXmlPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
							var projectName = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal));
							filePath = Path.Combine(projectsFolderPath, projectName, "Reports");
						}
						else
						{
							// is external or single file project
							var reportsPath = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
							filePath = Path.Combine(reportsPath, "Reports");
							if (!Directory.Exists(filePath))
							{
								// get the single file project Reports folder's path
								var directoryName = Path.GetDirectoryName(filePath);
								if (!IsNullOrEmpty(directoryName))
								{
									var projectName = Path.GetFileNameWithoutExtension(projectInfoNode.Attributes["ProjectFilePath"]?.Value);
									filePath = Path.Combine(directoryName, $"{projectName}.ProjectFiles", "Reports");
								}
							}
						}
					}

					_reportsFolderPath = filePath;
					return _reportService.ReportFileExist(filePath);
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
		/// <param name="isSingleFileProject"></param>
		/// <returns></returns>
		private ProjectDetails CreateProjectDetails(XmlNode projNode, bool isSingleFileProject)
		{
			var projectDetails = new ProjectDetails
			{
				LanguagesForPoject = new Dictionary<string, bool>(),
				ShouldBeExported = false,
				ReportsFolderPath = _reportsFolderPath
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
					var projectsFolderPath = _projectXmlPath.Substring(0, _projectXmlPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
					projectDetails.ProjectPath = projectsFolderPath + projectFolderPath;
				}

				var projectStatus = ProjectInformation.GetProjectStatus(projectDetails.ProjectPath);
				projectDetails.Status = projectStatus;
				projectDetails.IsSingleFileProject = isSingleFileProject;

				doc.Load(projectDetails.ProjectPath);
				var projectLanguages = _reportService.LoadLanguageDirections(doc);

				_studioService.SetLanguagesForProject(projectDetails, projectLanguages);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectDetails;
		}


		private void ShouldUnselectLanguages(ProjectDetails selectedProject)
		{
			try
			{
				if (selectedProject != null)
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

							var projectsToBeExported = _projectsDataSource.Where(n => n.LanguagesForPoject.ContainsKey(languageName) && n.ShouldBeExported).ToList();
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
						// Read sdlproj
						doc.Load(selectedProject.ProjectPath);
						_reportService.LoadReports(doc, selectedProject);

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
				var projectsToExport = _projectsDataSource?.Where(p => p.ShouldBeExported).ToList();
				if (projectsToExport != null)
				{
					foreach (var project in projectsToExport)
					{
						if (project.LanguagesForPoject != null)
						{
							var language = project.LanguagesForPoject.FirstOrDefault(l => l.Key.Equals(languageToUpdate.LanguageName));
							if (language.Key != null)
							{
								project.LanguagesForPoject[language.Key] = isChecked;
							}
						}
					}
				}

				var checkedLanguage = _languages?.FirstOrDefault(n => n.LanguageName.Equals(languageToUpdate.LanguageName));
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
								var languageAnalysisReportPath = selectedProject.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(selectedLanguage.Key));
								var copyReport = new StudioAnalysisReport(languageAnalysisReportPath.Value);
								var csvText = copyReport.ToCsv(includeHeaderCheck.Checked, _optionalInformation);
								csvTextBuilder.Append(csvText);
							}

							_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.CopyToClipboard_Success_Message, PluginResources.CopyResult_Label);
							Clipboard.SetText(csvTextBuilder.ToString());
						}
						else
						{
							_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.NoAnalyseReportForLanguage_Message, PluginResources.CopyResult_Label);
						}
					}
					else
					{
						_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.SelectLanguage_Copy_Message, PluginResources.CopyResult_Label);
					}
				}
			}
			catch (Exception exception)
			{
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
			var isSamePath = _reportService.IsSameReportPath(reportOutputPath.Text);
			if (!isSamePath)
			{
				_reportService.SaveExportPath(reportOutputPath.Text);
			}

			GenerateReport();
		}

		private void GenerateReport()
		{
			try
			{
				if (!IsNullOrEmpty(reportOutputPath.Text))
				{
					_help.CreateDirectory(reportOutputPath.Text);
					var projectsToBeExported = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
					var areCheckedLanguages = projectsToBeExported.Any(p => p.LanguagesForPoject.Any(l => l.Value));
					if (!areCheckedLanguages && projectsToBeExported.Count >= 1)
					{
						_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.SelectLanguage_Export_Message, PluginResources.ExportResult_Label);
					}
					else
					{
						foreach (var project in projectsToBeExported)
						{
							// check which languages to export
							if (project.LanguagesForPoject != null)
							{
								var checkedLanguages = project.LanguagesForPoject.Where(l => l.Value).ToList();

								foreach (var languageReport in checkedLanguages)
								{
									if (IsNullOrEmpty(project.ReportPath))
									{
										project.ReportPath = reportOutputPath.Text;
									}

									//write report to Reports folder
									var streamPath = Path.Combine($"{project.ReportPath}{Path.DirectorySeparatorChar}", $"{project.ProjectName}_{languageReport.Key}.csv");
									using (var sw = new StreamWriter(streamPath))
									{
										if (project.LanguageAnalysisReportPaths != null)
										{
											var analyseReportPath = project.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(languageReport.Key));
											if (!analyseReportPath.Equals(new KeyValuePair<string, string>()))
											{
												var report = new StudioAnalysisReport(analyseReportPath.Value);
												sw.Write(report.ToCsv(includeHeaderCheck.Checked, _optionalInformation));
											}
										}
									}
								}
							}
						}
						ClearItemsAfterExport();
						_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.ExportSuccess_Message, PluginResources.ExportResult_Label);
					}
				}
				else
				{
					_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.SelectFolder_Message, string.Empty);
				}

			}
			catch (Exception exception)
			{
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
				else
				{
					RefreshProjectsListBox();
					chkBox_SelectAllProjects.Checked = projListbox.CheckedItems.Count.Equals(projListbox.Items.Count) && projListbox.Items.Count > 0;
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
						if (_reportService.ReportFileExist(reportFolderPath))
						{
							var projectDetails = ProjectInformation.GetExternalProjectDetails(projectPath);

							doc.Load(projectDetails.ProjectPath);
							_reportService.LoadReports(doc, projectDetails);
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
					
					// reload the projects if the external project could not be opened
					if (projListbox.Items.Count == 0)
					{
						ReloadProjects();
					}
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
			ReloadProjects();
		}

		private void reportOutputPath_TextChanged(object sender, EventArgs e)
		{
			var selectedOutputPath = ((TextBox)sender).Text;
			if (!IsNullOrEmpty(selectedOutputPath))
			{
				reportOutputPath.Text = selectedOutputPath;
				targetBtn.Enabled = true;
			}
			else
			{
				targetBtn.Enabled = false;
			}
		}

		private void languagesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			try
			{
				var checkBoxValue = e.NewValue == CheckState.Checked;

				SetLanguageCheckedState(e.Index, checkBoxValue);
				_isAnyLanguageUnchecked = !checkBoxValue && chkBox_SelectAllLanguages.Checked;
				UncheckAllLanguagesOption(checkBoxValue);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"languagesListBox_ItemCheck method: {ex.Message}\n {ex.StackTrace}");
			}
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

		private void ReloadProjects()
		{
			foreach (var project in _allStudioProjectsDetails)
			{
				_projectsDataSource.Add(project);
			}

			projListbox.DataSource = _projectsDataSource;
			ResetSelection();
			DisableButtons();
		}

		private void SetNewProjectLanguage()
		{
			try
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
				chkBox_SelectAllLanguages.Checked = languagesListBox.CheckedItems.Count.Equals(languagesListBox.Items.Count) && languagesListBox.Items.Count > 0;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetNewProjectLanguage method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void selectAllProjects_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (!_isAnyProjectUnchecked)
				{
					var isSelectAllProjects = ((CheckBox) sender).Checked;
					if (!isSelectAllProjects)
					{
						DisableButtons();
					}

					SetProjectsInformation(isSelectAllProjects);
				}

				_isAnyProjectUnchecked = false;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"selectAllProjects_CheckedChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void chkBox_SelectAllLanguages_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				// change all the languages checkbox values only when the "Select all languages" option is checked/unchecked
				var isChecked = ((CheckBox) sender).Checked;
				if (!isChecked)
				{
					DisableButtons();
				}
				else if (languagesListBox.Items.Count > 0)
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
			catch (Exception ex)
			{
				Log.Logger.Error($"chkBox_SelectAllLanguages_CheckedChanged method: {ex.Message}\n {ex.StackTrace}");
			}
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
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"projListbox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void languagesListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"languagesListBox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void chkBox_IncludeSingleFileProjects_CheckedChanged(object sender, EventArgs e)
		{
			var isChecked = ((CheckBox)sender).Checked;

			// Add the current languages to dictionary, because it will be used to keep the current languages selection
			var languagesDictionary = _help.AddToDictionary(_languages);

			// Load all Studio single file projects within the projects list
			if (isChecked)
			{
				LoadSingleFileProjects(_projectXmlPath);
			}
			else
			{
				// Remove the single file projects from the list
				IReadOnlyList<ProjectDetails> projectsToRemove = _projectsDataSource.Where(p => p.IsSingleFileProject).ToList();
				foreach (var project in projectsToRemove)
				{
					_projectsDataSource.Remove(project);
					_allStudioProjectsDetails.Remove(project);
					var exportedProj = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
					if (!exportedProj.Any(p => p.LanguagesForPoject.Keys.Any(k => project.LanguagesForPoject.Keys.Any(pk => k.Equals(pk)))))
					{
						ShouldUnselectLanguages(project);
					}
				}

				// remove also the language corresponding to the single file project, when the "Is single file project" option is unchecked.
				_studioService.RemoveLanguages(languagesDictionary, _languages);
			}
			RefreshProjectsListBox();

			// Clear the _languages, because it was populated automatically on RefreshProjectsListBox(), and for the selected projects, all languages became automatically selected
			_languages.Clear();
			
			// Uncheck the "Select all languages" option, for cases when not all languages are checked
			chkBox_SelectAllLanguages.Checked = false;
			
			// Populate the _languages with the values saved within the dictionary, so the previews languages selection is kept only if at least one project is selected
			// (the languages selection made by user before including/removing the single file projects)
			if (projListbox.CheckedItems.Count > 0)
			{
				_help.AddFromDictionary(_languages, languagesDictionary);
				RefreshLanguageListbox();
			}

			// Keep the "Select all languages" checked when all languages were checked and at least one project is selected
			chkBox_SelectAllLanguages.Checked = languagesListBox.CheckedItems.Count.Equals(_languages.Count) && !chkBox_SelectAllLanguages.Checked && projListbox.CheckedItems.Count > 0;
			IsCsvBtnEnabled();
		}

		private BindingList<ProjectDetails> BindProjectsBasedOnStatus(string selectedStatus)
		{
			var projectsBindingList = new BindingList<ProjectDetails>();
			_isStatusChanged = true;
			try
			{
				_languages.Clear();
				var projects = _allStudioProjectsDetails;

				switch (selectedStatus)
				{
					case "InProgress":
						var inProgressProjects = projects.Where(s => s.Status.Equals("InProgress")).ToList();
						return GetProjectsBindingList(inProgressProjects, projectsBindingList);

					case "Completed":
						var completedProjects = projects?.Where(s => s.Status.Equals("Completed")).ToList();
						return GetProjectsBindingList(completedProjects, projectsBindingList);

					case "All":
						return GetProjectsBindingList(projects, projectsBindingList);
				}
				IsClipboardEnabled();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"BindProjectsBasedOnStatus method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectsBindingList;
		}

		private BindingList<ProjectDetails> GetProjectsBindingList(List<ProjectDetails> projectDetails, BindingList<ProjectDetails> projectsBindingList)
		{
			if (projectDetails != null && projectDetails.Count > 0)
			{
				projectsBindingList = _studioService.BindProjects(projectDetails, projectsBindingList);
			}
			else
			{
				_languages.Clear();
				DisableButtons();

				// uncheck the 'Select all projects' and 'Select all languages' when are checked and no projects are loaded for the selected status
				if (chkBox_SelectAllProjects.Checked)
				{
					chkBox_SelectAllProjects.Checked = false;
				}

				if (chkBox_SelectAllLanguages.Checked)
				{
					chkBox_SelectAllLanguages.Checked = false;
				}
			}

			return projectsBindingList;
		}

		private void SetProjectsInformation(bool isSelectAllProjects)
		{
			try
			{
				if (_projectsDataSource != null)
				{
					foreach (var project in _projectsDataSource)
					{
						project.ShouldBeExported = isSelectAllProjects;
						if (project.LanguagesForPoject != null)
						{
							foreach (var language in project.LanguagesForPoject.ToList())
							{
								project.LanguagesForPoject[language.Key] = isSelectAllProjects;
							}
						}
						FillLanguagesList(project);
					}
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
			catch (Exception ex)
			{
				Log.Logger.Error($"SetProjectsInformation method: {ex.Message}\n {ex.StackTrace}");
			}
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

		private void ResetSelection()
		{
			projectStatusComboBox.SelectedIndex = 0;
			chkBox_SelectAllProjects.Checked = false;
			chkBox_SelectAllLanguages.Checked = false;

			for (var i = 0; i < projListbox.Items.Count; i++)
			{
				projListbox.SetItemChecked(i, false);
			}
		}

		// Clear all projects and languages lists after the export is finished
		private void ClearItemsAfterExport()
		{
			UncheckAllProjects();
			_languages.Clear();
			chkBox_SelectAllProjects.Checked = false;
			chkBox_SelectAllLanguages.Checked = false;
		}
	}
}