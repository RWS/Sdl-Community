using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export
{
	public class WizardPageExportSummaryViewModel : WizardPageViewModelBase
	{
		private string _summaryText;

		public WizardPageExportSummaryViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			IsValid = true;
			PropertyChanged += WizardPageSummaryViewModel_PropertyChanged;
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
			var project = WizardContext.ProjectFiles[0].ProjectModel;

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
			summaryText += indent + string.Format(PluginResources.Label_XliffSupport, WizardContext.Support) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_WorkingFolder, WizardContext.WorkingFolder) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_IncludeTranslations, WizardContext.IncludeTranslations) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_CopySourceToTarget, WizardContext.CopySourceToTarget) + Environment.NewLine;

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Files + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TotalFiles, WizardContext.ProjectFiles.Count) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_SelectedFiles, WizardContext.ProjectFiles.Count(a => a.Selected)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_SelectedLanguages, GetSelectedLanguagesString()) + Environment.NewLine;

			var targetLanguages = WizardContext.ProjectFiles.Where(a => a.Selected)
				.Select(a => a.TargetLanguage.CultureInfo).Distinct();
			foreach (var targetLanguage in targetLanguages)
			{
				var languageFolder = WizardContext.GetLanguageFolder(targetLanguage);
				var languageFolderLocation = string.IsNullOrEmpty(WizardContext.ProjectFolder)
					? languageFolder
					: Path.Combine("..", languageFolder.Replace(WizardContext.ProjectFolder, string.Empty).Trim('\\'));

				summaryText += Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Language, targetLanguage.DisplayName) + Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Folder, languageFolderLocation) + Environment.NewLine;

				var targetLanguageFiles =
					WizardContext.ProjectFiles.Where(a => a.Selected && Equals(a.TargetLanguage.CultureInfo, targetLanguage));
				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					summaryText += indent + targetLanguageFile.Path + targetLanguageFile.Name + Environment.NewLine;
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

		private void WizardPageSummaryViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					LoadView();
				}
				else
				{
					LeaveView();
				}
			}
		}

		private void LeaveView()
		{
		}

		private void LoadView()
		{
			SummaryText = GetSummaryText();
		}
	}
}
