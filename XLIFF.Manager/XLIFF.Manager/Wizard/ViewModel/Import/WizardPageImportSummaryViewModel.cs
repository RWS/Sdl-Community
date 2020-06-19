using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportSummaryViewModel : WizardPageViewModelBase, IDisposable
	{
		private string _summaryText;

		public WizardPageImportSummaryViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
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
			var project = WizardContext.ProjectFiles[0].Project;

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
			summaryText += indent + string.Format(PluginResources.Label_BackupFiles, WizardContext.ImportBackupFiles) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_OverwriteExistingTranslations, WizardContext.ImportOverwriteTranslations) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_OriginSystem, WizardContext.ImportOriginSystem) + Environment.NewLine;
			summaryText += indent + string.Format("Confirmation status for translations updated: {0}",  WizardContext.ImportConfirmationStatusTranslationUpdatedId) + Environment.NewLine;
			summaryText += indent + string.Format("Confirmation status for translations not updated: {0}", WizardContext.ImportConfirmationStatusTranslationNotUpdatedId) + Environment.NewLine;
			summaryText += indent + string.Format("Confirmation status for not imported: {0}", WizardContext.ImportConfirmationStatusNotImportedId) + Environment.NewLine;

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Files + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_ImportFiles, WizardContext.ProjectFiles.Count(a => a.Selected)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()) + Environment.NewLine;

			var targetLanguages = WizardContext.ProjectFiles.Where(a => a.Selected &&
				 !string.IsNullOrEmpty(a.XliffFilePath) && File.Exists(a.XliffFilePath))
					.Select(a => a.TargetLanguage.CultureInfo).Distinct();

			foreach (var targetLanguage in targetLanguages)
			{
				var languageFolder = WizardContext.GetLanguageFolder(targetLanguage);

				summaryText += Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Language, targetLanguage.DisplayName) + Environment.NewLine;

				var targetLanguageFiles =
					WizardContext.ProjectFiles.Where(a => a.Selected &&
						  !string.IsNullOrEmpty(a.XliffFilePath) && File.Exists(a.XliffFilePath) &&
						  Equals(a.TargetLanguage.CultureInfo, targetLanguage));

				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					var xliffFolder = Path.Combine(languageFolder, targetLanguageFile.Path.TrimStart('\\'));
					var xliffFilePath = Path.Combine(xliffFolder, targetLanguageFile.Name + ".xliff");
					var sdlXliffBackup = Path.Combine(xliffFolder, targetLanguageFile.Name);

					summaryText += indent + string.Format(PluginResources.label_SdlXliffFile, targetLanguageFile.Location) + Environment.NewLine;
					if (WizardContext.ImportBackupFiles)
					{
						summaryText += indent + string.Format(PluginResources.Label_BackupFile, sdlXliffBackup) + Environment.NewLine;
					}
					summaryText += indent + string.Format(PluginResources.label_XliffFile, targetLanguageFile.XliffFilePath) + Environment.NewLine;
					summaryText += indent + string.Format(PluginResources.Label_ArchiveFile, xliffFilePath) + Environment.NewLine;

					summaryText += Environment.NewLine;
				}
			}

			return summaryText;
		}

		private string GetProjectTargetLanguagesString(Project project)
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
			var selected = WizardContext.ProjectFiles.Where(a => a.Selected);

			var selectedLanguages = string.Empty;
			foreach (var cultureInfo in selected.Select(a => a.TargetLanguage.CultureInfo).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") +
									 cultureInfo.DisplayName;
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
