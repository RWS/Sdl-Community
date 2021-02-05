using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Trados.Transcreate.Common;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.Import
{
	public class WizardPageImportSummaryViewModel : WizardPageViewModelBase, IDisposable
	{
		private string _summaryText;

		public WizardPageImportSummaryViewModel(Window owner, object view, TaskContext taskContext) : base(owner, view, taskContext)
		{
			IsValid = true;

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public string SummaryText
		{
			get => _summaryText;
			set
			{
				if (_summaryText == value)
				{
					return;
				}

				_summaryText = value;
				OnPropertyChanged(nameof(SummaryText));
			}
		}

		public override string DisplayName => PluginResources.PageName_Summary;

		public sealed override bool IsValid { get; set; }

		private string GetSummaryText()
		{
			var project = TaskContext.ProjectFiles[0].Project;

			var indent = "   ";

			var summaryText = PluginResources.Label_Project + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Id, project.Id) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Name, project.Name) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Location, project.Path) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Created, project.Created.ToString(CultureInfo.InvariantCulture)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_DueDate, project.DueDate.ToString(CultureInfo.InvariantCulture)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_SourceLanguage, project.SourceLanguage.CultureInfo.DisplayName) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TargetLanguages, GetProjectTargetLanguagesString(project)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_ProjectType, project.ProjectType) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Customer, project.Customer?.Name) + Environment.NewLine;

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Options + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_BackupFiles, TaskContext.ImportOptions.BackupFiles) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_OverwriteExistingTranslations, TaskContext.ImportOptions.OverwriteTranslations) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_OriginSystem, TaskContext.ImportOptions.OriginSystem) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_StatusTranslationsUpdated, GetConfirmationStatusName(TaskContext.ImportOptions.StatusTranslationUpdatedId)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_StatusTranslationsNotUpdated, GetConfirmationStatusName(TaskContext.ImportOptions.StatusTranslationNotUpdatedId)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_StatusSegmentsNotImported, GetConfirmationStatusName(TaskContext.ImportOptions.StatusSegmentNotImportedId)) + Environment.NewLine;
			if (TaskContext.ImportOptions.ExcludeFilterIds.Count > 0)
			{
				summaryText += indent + string.Format(PluginResources.Label_ExcludeFilters, GetFitlerItemsString(TaskContext.ImportOptions.ExcludeFilterIds)) + Environment.NewLine;
			}

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Files + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_ImportFiles, TaskContext.ProjectFiles.Count(a => a.Selected)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()) + Environment.NewLine;

			var targetLanguages = TaskContext.ProjectFiles.Where(a => a.Selected &&
				 !string.IsNullOrEmpty(a.ExternalFilePath) && File.Exists(a.ExternalFilePath))
					.Select(a => a.TargetLanguage).Distinct();

			foreach (var targetLanguage in targetLanguages)
			{
				var languageFolder = TaskContext.GetLanguageFolder(targetLanguage);

				summaryText += Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Language, targetLanguage) + Environment.NewLine;

				var targetLanguageFiles =
					TaskContext.ProjectFiles.Where(a => a.Selected &&
						  !string.IsNullOrEmpty(a.ExternalFilePath) && File.Exists(a.ExternalFilePath) &&
						  Equals(a.TargetLanguage, targetLanguage));

				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					var folder = Path.Combine(languageFolder, targetLanguageFile.Path.TrimStart('\\'));
					var archiveFile = Path.Combine(folder, targetLanguageFile.Name + ".docx");
					var sdlXliffBackup = Path.Combine(folder, targetLanguageFile.Name);

					summaryText += indent + string.Format(PluginResources.Label_SdlXliffFile, targetLanguageFile.Location) + Environment.NewLine;
					if (TaskContext.ImportOptions.BackupFiles)
					{
						summaryText += indent + string.Format(PluginResources.Label_BackupFile, sdlXliffBackup) + Environment.NewLine;
					}
					summaryText += indent + string.Format(PluginResources.Label_ImportFile, targetLanguageFile.ExternalFilePath) + Environment.NewLine;
					summaryText += indent + string.Format(PluginResources.Label_ArchiveFile, archiveFile) + Environment.NewLine;

					summaryText += Environment.NewLine;
				}
			}

			return summaryText;
		}

		private string GetConfirmationStatusName(string id)
		{
			return string.IsNullOrEmpty(id) ? "[none]" : id;
		}
		
		private string GetFitlerItemsString(List<string> ids)
		{
			var allFilterItems = Enumerators.GetFilterItems();
			var filterItems = Enumerators.GetFilterItems(allFilterItems, ids);
			var items = string.Empty;
			foreach (var filterItem in filterItems)
			{
				items += (string.IsNullOrEmpty(items) ? string.Empty : ", ") +
				         filterItem.Name;
			}

			if (string.IsNullOrEmpty(items))
			{
				items = "[none]";
			}

			return items;
		}

		private string GetProjectTargetLanguagesString(Interfaces.IProject project)
		{
			var targetLanguages = string.Empty;
			foreach (var languageInfo in project.TargetLanguages)
			{
				targetLanguages += (string.IsNullOrEmpty(targetLanguages) ? string.Empty : ", ") +
								   languageInfo.CultureInfo.DisplayName;
			}

			return targetLanguages;
		}

		private string GetSelectedLanguagesString()
		{
			var selected = TaskContext.ProjectFiles.Where(a => a.Selected);

			var selectedLanguages = string.Empty;
			foreach (var name in selected.Select(a => a.TargetLanguage).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") +
									 name;
			}

			return selectedLanguages;
		}

		private void OnLoadPage(object sender, EventArgs e)
		{
			SummaryText = GetSummaryText();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
