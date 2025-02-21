using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NLog;
using Sdl.Community.ExportAnalysisReports.Controls;
using Sdl.Community.ExportAnalysisReports.Helpers;
using Sdl.Community.ExportAnalysisReports.Interfaces;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.ExportAnalysisReports.Service;
using static System.String;

namespace Sdl.Community.ExportAnalysisReports
{
	public partial class ReportExporterControl : Form
	{
		private readonly BindingList<LanguageDetails> _languages = new BindingList<LanguageDetails>();
		private readonly SettingsService _settingsService;
		private readonly IMessageBoxService _messageBoxService;
		private readonly IReportService _reportService;
		private readonly IProjectService _projectService;
		private List<ProjectDetails> _allStudioProjectsDetails;
		private Helpers.Help _help;
		private bool _isAnyLanguageUnchecked;
		private bool _isAnyProjectUnchecked;
		private bool _isStatusChanged;
		private OptionalInformation _optionalInformation;
		private BindingList<ProjectDetails> _projectsDataSource = new BindingList<ProjectDetails>();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ReportExporterControl(List<string> studioProjectsPaths, SettingsService settingsService,
			IProjectService projectService, IMessageBoxService messageBoxService, IReportService reportService)
		{
			_settingsService = settingsService;
			_projectService = projectService;
			_messageBoxService = messageBoxService;
			_reportService = reportService;

			InitializeComponent();
			InitializeSettings(studioProjectsPaths);
			PrepareSelectedProjects(studioProjectsPaths);
		}

		private void AdaptiveMT_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeAdaptiveBaseline = adaptiveMT.Checked;
		}

		private void BrowseBtn_Click(object sender, EventArgs e)
		{
			var folderPath = new FolderSelectDialog();
			if (folderPath.ShowDialog())
			{
				reportOutputPath.Text = folderPath.FileName;
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
				_logger.Error($"languagesListBox_SelectedIndexChanged_1 method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void IncludeSingleFileProjects_CheckedChanged(object sender, EventArgs e)
		{
			var isChecked = ((CheckBox)sender).Checked;

			// Add the current languages to dictionary, because it will be used to keep the current languages selection
			var languagesDictionary = _help.AddToDictionary(_languages);

			// Load all Studio single file projects within the projects list
			if (isChecked)
			{
				LoadSingleFileProjects(_projectService.ProjectsXmlPath);
			}
			else
			{
				// Remove the single file projects from the list
				RemoveSingleFileProjects(languagesDictionary);
			}
			SetProjectsDataSource(projectStatusComboBox.Text);
			RefreshProjectsListBox();

			// Clear the _languages, because it was populated automatically on RefreshProjectsListBox(), and for the selected projects, all languages became automatically selected
			_languages.Clear();

			// Uncheck the "Select all languages" option, for cases when not all languages are checked
			chkBox_SelectAllLanguages.Checked = false;

			// Populate the _languages with the values saved within the dictionary, so the previews languages selection is kept only if at least one project is selected
			// (the languages selection made by user before including/removing the single file projects)
			if (projListbox.CheckedItems.Count > 0)
			{
				AddLanguageFromDictionary(_languages, languagesDictionary);
				RefreshLanguageListbox();
			}

			// Keep the "Select all languages" checked when all languages were checked and at least one project is selected
			chkBox_SelectAllLanguages.Checked = languagesListBox.CheckedItems.Count.Equals(_languages.Count) && !chkBox_SelectAllLanguages.Checked && projListbox.CheckedItems.Count > 0;
			UpdateCsvBtnEnabled();
		}

		private void SelectAllLanguages_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				// change all the languages checkbox values only when the "Select all languages" option is checked/unchecked
				var isChecked = ((CheckBox)sender).Checked;
				if (!isChecked)
				{
					DisableButtons();
				}
				else if (languagesListBox.Items.Count > 0)
				{
					UpdateClipboardEnabled();
					UpdateCsvBtnEnabled();
				}

				if (!_isAnyLanguageUnchecked)
				{
					ChangeLanguagesCheckbox(isChecked);
				}

				_isAnyLanguageUnchecked = false;
			}
			catch (Exception ex)
			{
				_logger.Error($"chkBox_SelectAllLanguages_CheckedChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			_projectsDataSource.Clear();
			_languages.Clear();
			ReloadProjects();
		}

		// Clear all projects and languages lists after the export is finished
		private void ClearItemsAfterExport()
		{
			UncheckProjects();
			_languages.Clear();
			chkBox_SelectAllProjects.Checked = false;
			chkBox_SelectAllLanguages.Checked = false;
		}

		/// <summary>
		/// Configure the checked items: selected project(s) with "All selected languages" options
		/// </summary>
		private void ConfigureCheckedItems()
		{
			chkBox_SelectAllLanguages.Checked = true;
			RefreshProjectsListBox();
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

		private void ContextMatch_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeContextMatch = contextMatch.Checked;
		}

		private void CopyBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var projectsToBeExported = _projectsDataSource.Where(p => p.ShouldBeExported).ToList();
				foreach (var selectedProject in projectsToBeExported)
				{
					var csvTextBuilder = new StringBuilder();
					if (!(selectedProject?.ProjectLanguages.Count(c => c.Value) > 0))
					{
						_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.SelectLanguage_Copy_Message, PluginResources.CopyResult_Label);
					}
					else
					{
						var selectedLanguages = selectedProject.ProjectLanguages.Where(l => l.Value);
						if (selectedProject.LanguageAnalysisReportPaths == null)
						{
							_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.NoAnalyseReportForLanguage_Message, PluginResources.CopyResult_Label);
						}
						else
						{
							foreach (var selectedLanguage in selectedLanguages)
							{
								var languageAnalysisReportPath = selectedProject.LanguageAnalysisReportPaths.FirstOrDefault(l => l.Key.Equals(selectedLanguage.Key));
								_reportService.PrepareAnalysisReport(languageAnalysisReportPath.Value);

								var csvContent = _reportService.GetCsvContent(includeHeaderCheck.Checked, _optionalInformation);
								csvTextBuilder.Append(csvContent);
							}

							_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.CopyToClipboard_Success_Message, PluginResources.CopyResult_Label);
							Clipboard.SetText(csvTextBuilder.ToString());
						}
					}
				}
			}
			catch (Exception exception)
			{
				_logger.Error($"copyBtn_Click method: {exception.Message}\n {exception.StackTrace}");
				throw;
			}
		}

		private void CrossRep_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeCrossRep = crossRep.Checked;
		}

		private void CsvBtn_Click(object sender, EventArgs e)
		{
			var isSamePath = _reportService.IsSameReportPath(reportOutputPath.Text);
			if (!isSamePath)
			{
				// Save the new selected export folder path if it was changed by the user
				var settings = _settingsService.GetSettings();
				settings.ExportPath = reportOutputPath.Text;
				_settingsService.SaveSettings(settings);
			}

			GenerateReport();
		}

		// Disable the 'Select all projects' option when one of the project is unchecked
		private void DisableAllProjectsOption()
		{
			if (!chkBox_SelectAllProjects.Checked || !_isAnyProjectUnchecked) return;
			chkBox_SelectAllProjects.Checked = false;
		}

		private void DisableButtons()
		{
			copyBtn.Enabled = false;
			csvBtn.Enabled = false;
		}

		private void ExitBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void FragmentMatches_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeAdaptiveLearnings = adaptiveLearnings.Checked;
		}

		private void GenerateReport()
		{
			if (!IsNullOrEmpty(reportOutputPath.Text))
			{
				var isReportGenerated = _reportService.GenerateReportFile(_projectsDataSource, _optionalInformation, reportOutputPath.Text, includeHeaderCheck.Checked);
				if (isReportGenerated)
				{
					ClearItemsAfterExport();
					_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.ExportSuccess_Message, PluginResources.ExportResult_Label);
				}
			}
			else
			{
				_messageBoxService.ShowOwnerInformationMessage(this, PluginResources.SelectFolder_Message, string.Empty);
			}
		}

		private BindingList<ProjectDetails> GetProjects(List<ProjectDetails> projectDetails, BindingList<ProjectDetails> newProjectDetails)
		{
			if (projectDetails != null && projectDetails.Count > 0)
			{
				SetProjectDetails(projectDetails, newProjectDetails);
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

			return newProjectDetails;
		}

		private void IncludeHeaderCheck_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void InitializeSettings(List<string> studioProjectsPaths = null)
		{
			DisableButtons();
			_help = new Helpers.Help();
			includeHeaderCheck.Checked = true;
			_allStudioProjectsDetails = new List<ProjectDetails>();
			LoadProjectsList(_projectService.ProjectsXmlPath, studioProjectsPaths);
			reportOutputPath.Text = _settingsService.GetSettings().ExportPath;
			targetBtn.Enabled = !IsNullOrEmpty(reportOutputPath.Text);
			_optionalInformation = SetOptionalInformation();
			projectStatusComboBox.SelectedIndex = 0;
		}

		private void InternalFuzzies_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeInternalFuzzies = internalFuzzies.Checked;
		}

		private void UpdateClipboardEnabled()
		{
			var isMultipleProjectsSelected = _projectsDataSource.Count(p => p.ShouldBeExported) > 1;
			copyBtn.Enabled = !isMultipleProjectsSelected;
		}

		private void UpdateCsvBtnEnabled()
		{
			csvBtn.Enabled = _projectsDataSource.Count(p => p.ShouldBeExported) >= 1;
		}

		// Verify if the SelectAll options (Project and Languages) should be automatically checked, when all list box items are manually checked, one by one.
		private bool IsSelectAllChecked(CheckedListBox listbox)
		{
			return listbox.CheckedItems.Count == listbox.Items.Count;
		}

		private void LanguagesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
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
				_logger.Error($"languagesListBox_ItemCheck method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void LanguagesListBox_SelectedIndexChanged(object sender, EventArgs e)
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
					UpdateClipboardEnabled();
					UpdateCsvBtnEnabled();
				}

				ConfigureCheckedOptions(languagesListBox);
			}
			catch (Exception ex)
			{
				_logger.Error($"languagesListBox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void LoadBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var loadFolderPath = new FolderSelectDialog();
				if (loadFolderPath.ShowDialog())
				{
					_languages.Clear();
					_projectsDataSource.Clear();

					var externalProjects = new BindingList<ProjectDetails>(_reportService.GetExternalProjectReportInfo(loadFolderPath.FileName));

					foreach (var externalProject in externalProjects)
					{
						_projectsDataSource.Add(externalProject);
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
				_logger.Error($"loadBtn_Click method: {ex.Message}\n {ex.StackTrace}");
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
				var filePathNames = _projectService.AddFilePaths(studioProjectsPaths);
				if (!IsNullOrEmpty(projectXmlPath))
				{
					var projectsWithoutAnalysis = new List<string>();
					projectXmlDocument.Load(projectXmlPath);

					var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
					if (projectsNodeList == null)
					{
						return;
					}

					var studioProjects = _projectService.GetSelectedStudioProjects();

					foreach (XmlNode xmlNode in projectsNodeList)
					{
						var projectId = GetAttributeValue(xmlNode, "Guid");
						var projectInfo = studioProjects.FirstOrDefault(a => a.GetProjectInfo()?.Id.ToString() == projectId)?.GetProjectInfo();
						if (projectInfo == null)
						{
							continue;
						}

						var reportsFolderPath = Path.Combine(projectInfo.LocalProjectFolder, "Reports");
						var projectInfoNode = xmlNode.SelectSingleNode("./ProjectInfo");
						if (projectInfoNode?.Attributes != null)
						{
							var reportExist = _reportService.ReportFolderExist(xmlNode, _projectService.ProjectsXmlPath);
							if (reportExist)
							{
								SetProjectDetails(reportsFolderPath, projectInfoNode, xmlNode, filePathNames);
							}
							else
							{
								projectsWithoutAnalysis.Add(projectInfo.Name);
							}
						}
					}

					SetProjectDataSource();

					if (projectsWithoutAnalysis.Count > 0 && !_settingsService.GetSettings().DontShowInfoMessage)
					{
						var messageBox = new InformationMessage(_settingsService, projectsWithoutAnalysis);
						messageBox.ShowDialog();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"LoadProjectsList method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private static string GetAttributeValue(XmlNode xmlNode, string name)
		{
			if (xmlNode.Attributes != null)
			{
				foreach (XmlAttribute attribute in xmlNode.Attributes)
				{
					if (attribute.Name != name)
					{
						continue;
					}

					return attribute.Value;
				}
			}

			return null;
		}

		// Load all single file projects from projects.xml file inside UI's list
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
							var reportExist = _reportService.ReportFolderExist((XmlNode)item, _projectService.ProjectsXmlPath);
							if (reportExist)
							{
								var projectDetails = _projectService.CreateProjectDetails((XmlNode)item, true, _settingsService.GetSettings().ExportPath);
								if (!_projectsDataSource.Any(p => p.ProjectName.Equals(projectDetails.ProjectName)))
								{
									if (chkBox_SelectAllProjects.Checked)
									{
										projectDetails.ShouldBeExported = true;
									}
									_projectsDataSource.Add(projectDetails);
									_projectService.SetAllProjectDetails(_allStudioProjectsDetails, projectDetails);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"LoadSingleFileProjects method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void Locked_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludeLocked = locked.Checked;
		}

		private void PerfectMatch_CheckedChanged(object sender, EventArgs e)
		{
			_optionalInformation.IncludePerfectMatch = perfectMatch.Checked;
		}

		private void PrepareProjectToExport(ProjectDetails selectedProject)
		{
			try
			{
				if (selectedProject == null) return;
				var selectedProjectIndex = _projectsDataSource.IndexOf(selectedProject);

				if (selectedProjectIndex <= -1) return;
				_reportService.SetReportInformation(selectedProject);

				selectedProject.ShouldBeExported = true;

				SetProjectLanguages(selectedProject);

				copyBtn.Enabled = projListbox.SelectedItems.Count == 1;
				if (projListbox.SelectedItems.Count > 0)
				{
					csvBtn.Enabled = true;
				}
				RefreshLanguageListbox();
			}
			catch (Exception ex)
			{
				_logger.Error($"PrepareProjectToExport method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void ProjectStatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var selectedStatus = ((ComboBox)sender).SelectedItem?.ToString();
				if (chkBox_IncludeSingleFileProjects.Checked)
				{
					LoadSingleFileProjects(_projectService.ProjectsXmlPath);
				}
				SetProjectsDataSource(selectedStatus);

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
				_logger.Error($"projectStatusComboBox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void ProjListbox_ItemCheck(object sender, ItemCheckEventArgs e)
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
					if (selectedProjectIndex > -1 && shouldExportProject)
					{
						PrepareProjectToExport(selectedProject);
					}
					else
					{
						// Uncheck the project when user deselects it
						if (selectedProject != null)
						{
							selectedProject.ShouldBeExported = false;
							UnselectProjectLanguages(selectedProject);
						}
					}
					_isAnyProjectUnchecked = !shouldExportProject && chkBox_SelectAllProjects.Checked;
					DisableAllProjectsOption();
					UpdateClipboardEnabled();
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"projListbox_ItemCheck method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void ProjListbox_SelectedIndexChanged(object sender, EventArgs e)
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
				_logger.Error($"projListbox_SelectedIndexChanged method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void SetProjectsDataSource(string selectedStatus)
		{
			_projectsDataSource = SetProjectsBasedOnStatus(selectedStatus);
			projListbox.DataSource = _projectsDataSource;
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
				_logger.Error($"RefreshLanguageListbox method: {ex.Message}\n {ex.StackTrace}");
			}
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

		private void RemoveLanguageFromProject(ProjectDetails selectedProject)
		{
			foreach (var language in selectedProject.ProjectLanguages)
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

		private void ReportOutputPath_KeyUp(object sender, KeyEventArgs e)
		{
			var reportPath = ((TextBox)sender).Text;
			if (!IsNullOrWhiteSpace(reportPath))
			{
				var settings = _settingsService.GetSettings();
				settings.ExportPath = reportOutputPath.Text;
				_settingsService.SaveSettings(settings);

				targetBtn.Enabled = true;
			}

			if (e.KeyCode == Keys.Enter)
			{
				GenerateReport();
			}
		}

		private void ReportOutputPath_TextChanged(object sender, EventArgs e)
		{
			var selectedOutputPath = ((TextBox)sender).Text;

			if (!IsNullOrEmpty(selectedOutputPath))
			{
				var settings = _settingsService.GetSettings();
				settings.ExportPath = reportOutputPath.Text;
				_settingsService.SaveSettings(settings);

				targetBtn.Enabled = true;
			}
			else
			{
				targetBtn.Enabled = false;
			}
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

		private void SelectAllProjects_CheckedChanged(object sender, EventArgs e)
		{
			try
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
			catch (Exception ex)
			{
				_logger.Error($"selectAllProjects_CheckedChanged method: {ex.Message}\n {ex.StackTrace}");
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
					_projectService.SetProjectLanguages(projectsToExport, isChecked, languageToUpdate.LanguageName);
				}

				var checkedLanguage = _languages?.FirstOrDefault(n => n.LanguageName.Equals(languageToUpdate.LanguageName));
				if (checkedLanguage != null)
				{
					checkedLanguage.IsChecked = isChecked;
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetLanguageCheckedState method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		// Set the languages for the selected project within the languages UI list 
		private void AddNewLanguages(ProjectDetails selectedProject)
		{
			try
			{
				var selectedProjectToExport = _projectsDataSource?.FirstOrDefault(e => e.ShouldBeExported && e.ProjectName.Equals(selectedProject.ProjectName));

				if (selectedProjectToExport?.ProjectLanguages != null)
				{
					foreach (var language in selectedProjectToExport.ProjectLanguages.ToList())
					{
						var languageReportExists = LanguageReportExists(selectedProject, language.Key);
						if (!languageReportExists)
						{
							continue;
						}

						var languageDetails = _languages?.FirstOrDefault(n => n.LanguageName.Equals(language.Key));
						if (languageDetails == null)
						{
							var newLanguage = new LanguageDetails { LanguageName = language.Key, IsChecked = true };
							_languages?.Add(newLanguage);
						}
					}
				}

				languagesListBox.DataSource = _languages;
				languagesListBox.DisplayMember = "LanguageName";
				languagesListBox.ValueMember = "IsChecked";
			}
			catch (Exception ex)
			{
				_logger.Error($"AddNewLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
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
							AddNewLanguages(project);
						}
					}
				}

				RefreshLanguageListbox();
				chkBox_SelectAllLanguages.Checked = languagesListBox.CheckedItems.Count.Equals(languagesListBox.Items.Count) && languagesListBox.Items.Count > 0;
			}
			catch (Exception ex)
			{
				_logger.Error($"SetNewProjectLanguage method: {ex.Message}\n {ex.StackTrace}");
			}
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

		private void SetProjectDataSource()
		{
			projListbox.DataSource = _projectsDataSource;
			projListbox.ValueMember = "ShouldBeExported";
			projListbox.DisplayMember = "ProjectName";
		}

		private void SetProjectDetails(string reportsFolderPath, XmlNode item, bool isSingleFileProject)
		{
			try
			{

				var projectDetails = _projectService.CreateProjectDetails(item, isSingleFileProject, reportsFolderPath);
				if (projectDetails != null)
				{
					_projectsDataSource.Add(projectDetails);
					_allStudioProjectsDetails.Add(projectDetails);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectDetails method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void SetProjectDetails(string reportsFolderPath, XmlNode projectInfoNode, XmlNode item, List<string> filePathNames)
		{
			if (projectInfoNode?.Attributes != null)
			{
				var xmlAttributeCollection = item?.Attributes;
				if (xmlAttributeCollection != null)
				{
					var projFileName = Path.GetFileName(xmlAttributeCollection["ProjectFilePath"]?.Value);
					var projPath = filePathNames.FirstOrDefault(p => p.Equals(projFileName));
					if (projectInfoNode.Attributes["IsInPlace"].Value.Equals("true") && !IsNullOrEmpty(projPath))
					{
						// Include the selected single file project ONLY when user selects it within Projects view -> right click -> Export Analysis Reports
						SetProjectDetails(reportsFolderPath, item, true);
					}
					else if (projectInfoNode.Attributes["IsInPlace"].Value.Equals("false"))
					{
						// Include all projects that are not single file project
						SetProjectDetails(reportsFolderPath, item, false);
					}
				}
			}
		}

		private BindingList<ProjectDetails> SetProjectsBasedOnStatus(string selectedStatus)
		{
			var projectsDetails = new BindingList<ProjectDetails>();
			_isStatusChanged = true;
			try
			{
				_languages.Clear();
				var projects = _allStudioProjectsDetails;

				switch (selectedStatus)
				{
					case "InProgress":
						var inProgressProjects = projects?.Where(s => s.Status.Equals("InProgress")).ToList();
						return GetProjects(inProgressProjects, projectsDetails);

					case "Completed":
						var completedProjects = projects?.Where(s => s.Status.Equals("Completed")).ToList();
						return GetProjects(completedProjects, projectsDetails);

					case "All":
						return GetProjects(projects, projectsDetails);
				}
				UpdateClipboardEnabled();
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectsBasedOnStatus method: {ex.Message}\n {ex.StackTrace}");
			}

			return projectsDetails;
		}

		private void SetProjectsInformation(bool isSelectAllProjects)
		{
			try
			{
				SetProjectsLanguages(isSelectAllProjects);
				RefreshProjectsListBox();

				SetAllLanguagesSelection(isSelectAllProjects);
				RefreshLanguageListbox();
			}
			catch (Exception ex)
			{
				_logger.Error($"SetProjectsInformation method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void UnselectProjectLanguages(ProjectDetails selectedProject)
		{
			try
			{
				if (selectedProject != null)
				{
					var selectedLanguagesFromProject = selectedProject.ProjectLanguages.Where(n => n.Value).Select(n => n.Key).ToList();
					if (!selectedLanguagesFromProject.Any() && !selectedProject.ShouldBeExported)
					{
						RemoveLanguageFromProject(selectedProject);
					}
					else
					{
						foreach (var languageName in selectedLanguagesFromProject)
						{
							RemoveSelectedLanguage(selectedProject, languageName);
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
				_logger.Error($"ShouldUnselectLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
		}
		private void TargetBtn_Click(object sender, EventArgs e)
		{
			if (!IsNullOrEmpty(reportOutputPath.Text))
			{
				Process.Start("explorer.exe", "\"" + reportOutputPath.Text + "\"");
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

		private void UncheckProjects()
		{
			var projectsToUncheck = _projectsDataSource?.Where(p => p.ShouldBeExported).ToList();
			if (projectsToUncheck != null)
			{
				foreach (var project in projectsToUncheck)
				{
					project.ShouldBeExported = false;
					foreach (var language in project.ProjectLanguages.ToList())
					{
						project.ProjectLanguages[language.Key] = false;
					}
				}
			}

			RefreshProjectsListBox();
		}

		// Prepare the selected projects for the export process
		private void PrepareSelectedProjects(List<string> studioProjectsPaths)
		{
			foreach (var path in studioProjectsPaths)
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

		// Remove the single file projects from the UI
		private void RemoveSingleFileProjects(Dictionary<string, bool> languagesDictionary)
		{
			IReadOnlyList<ProjectDetails> projectsToRemove = _projectsDataSource.Where(p => p.IsSingleFileProject).ToList();
			foreach (var project in projectsToRemove)
			{
				_projectsDataSource.Remove(project);
				var exportedProj = _projectsDataSource?.Where(p => p.ShouldBeExported).ToList();
				if (!exportedProj.Any(p => p.ProjectLanguages.Keys.Any(k => project.ProjectLanguages.Keys.Any(k.Equals))))
				{
					UnselectProjectLanguages(project);
				}
			}
			_projectService.RemoveAllSingleProjects(_allStudioProjectsDetails);

			// remove also the language corresponding to the single file project, when the "Is single file project" option is unchecked.
			_projectService.RemoveSingleFileProjectLanguages(languagesDictionary, _languages);
		}

		// Remove the selected language from the languages UI list
		private void RemoveSelectedLanguage(ProjectDetails selectedProject, string languageName)
		{
			// reset count for each language
			var count = 0;

			// unselect language for project in data source list
			selectedProject.ProjectLanguages[languageName] = false;

			var projectsToBeExported = _projectsDataSource.Where(n => n.ProjectLanguages.ContainsKey(languageName) && n.ShouldBeExported).ToList();
			foreach (var project in projectsToBeExported)
			{
				var languageReportExists = LanguageReportExists(project, languageName);
				if (!languageReportExists)
				{
					continue;
				}

				var languageShouldBeExported = project.ProjectLanguages[languageName];
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

		private static bool LanguageReportExists(ProjectDetails project, string languageName)
		{
			if (!project.LanguageAnalysisReportPaths.ContainsKey(languageName))
			{
				return false;
			}

			var languageReports = project.LanguageAnalysisReportPaths.Where(a => a.Key == languageName);
			return languageReports.Any(languageReport => !IsNullOrEmpty(languageReport.Value) && File.Exists(languageReport.Value));
		}

		// Set the projects languages
		private void SetProjectsLanguages(bool isSelectAllProjects)
		{
			if (_projectsDataSource != null)
			{
				foreach (var project in _projectsDataSource)
				{
					project.ShouldBeExported = isSelectAllProjects;
					if (project.ProjectLanguages != null)
					{
						foreach (var language in project.ProjectLanguages.ToList())
						{
							project.ProjectLanguages[language.Key] = isSelectAllProjects;
						}
					}
					AddNewLanguages(project);
				}
			}
		}

		// Set all languages selection based on the "Select all projects" option selection
		private void SetAllLanguagesSelection(bool isSelectAllProjects)
		{
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
		}

		private void SetProjectLanguages(ProjectDetails selectedProject)
		{
			// if an project has only one language select that language
			if (selectedProject?.ProjectLanguages != null && selectedProject.ProjectLanguages.Count.Equals(1))
			{
				var languageName = selectedProject.ProjectLanguages.FirstOrDefault().Key;
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
				selectedProject.ProjectLanguages[languageName] = true;
			}

			var languagesAlreadySelectedForExport = _languages.Where(l => l.IsChecked).ToList();
			foreach (var language in languagesAlreadySelectedForExport)
			{
				if (selectedProject?.ProjectLanguages != null && selectedProject.ProjectLanguages.ContainsKey(language.LanguageName))
				{
					selectedProject.ProjectLanguages[language.LanguageName] = true;
				}
			}

			AddNewLanguages(selectedProject);
		}

		private void AddLanguageFromDictionary(BindingList<LanguageDetails> languages, Dictionary<string, bool> languagesDictionary)
		{
			foreach (var item in languagesDictionary)
			{
				var language = new LanguageDetails { LanguageName = item.Key, IsChecked = item.Value };
				languages.Add(language);
			}
		}

		private void SetProjectDetails(List<ProjectDetails> projects, BindingList<ProjectDetails> newProjectDetails)
		{
			if (newProjectDetails == null) return;
			foreach (var project in projects)
			{
				newProjectDetails.Add(project);
			}
		}
	}
}