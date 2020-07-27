using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.Wizard.ViewModel.Convert
{
	public class WizardPageConvertSummaryViewModel : WizardPageViewModelBase
	{
		private string _summaryText;

		public WizardPageConvertSummaryViewModel(Window owner, object view, WizardContext wizardContext)
			: base(owner, view, wizardContext)
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
			summaryText += indent + string.Format(PluginResources.Label_ProjectBackup, WizardContext.ProjectBackupPath) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_MaxAlternativeTranslations, WizardContext.ConvertOptions.MaxAlternativeTranslations) + Environment.NewLine;

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Files + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_SelectedFiles, WizardContext.ProjectFiles.Count(a => a.Selected)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Languages, GetSelectedLanguagesString()) + Environment.NewLine;

			var targetLanguages = WizardContext.ProjectFiles.Where(a => a.Selected)
				.Select(a => a.TargetLanguage).Distinct();

			foreach (var targetLanguage in targetLanguages)
			{
				var languageFolder = WizardContext.GetLanguageFolder(targetLanguage);

				summaryText += Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Language, targetLanguage) + Environment.NewLine;

				var targetLanguageFiles =
					WizardContext.ProjectFiles.Where(a => a.Selected && Equals(a.TargetLanguage, targetLanguage));
				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					var xliffFolder = Path.Combine(languageFolder, targetLanguageFile.Path.TrimStart('\\'));
					var xliffFilePath = Path.Combine(xliffFolder, targetLanguageFile.Name.Substring(0, targetLanguageFile.Name.Length- ".sdlxliff".Length) + ".xliff");
					
					summaryText += indent + string.Format(PluginResources.label_SdlXliffFile, targetLanguageFile.Location) + Environment.NewLine;					
					summaryText += indent + string.Format(PluginResources.label_XliffFile, xliffFilePath + Environment.NewLine) + Environment.NewLine;
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
			foreach (var name in selected.Select(a => a.TargetLanguage).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") + name;
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
