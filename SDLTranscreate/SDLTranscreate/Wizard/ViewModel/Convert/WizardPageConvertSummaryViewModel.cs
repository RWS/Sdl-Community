using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.Convert
{
	public class WizardPageConvertSummaryViewModel : WizardPageViewModelBase
	{
		private string _summaryText;

		public WizardPageConvertSummaryViewModel(Window owner, object view, TaskContext taskContext)
			: base(owner, view, taskContext)
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
			summaryText += indent + string.Format(PluginResources.Label_MaxAlternativeTranslations, TaskContext.ConvertOptions.MaxAlternativeTranslations) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_UnloadOiriginalProject, TaskContext.ConvertOptions.CloseProjectOnComplete) + Environment.NewLine;

			var soruceLanguage = TaskContext.Project.SourceLanguage.CultureInfo.Name;

			summaryText += Environment.NewLine;
			summaryText += PluginResources.Label_Files + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_TotalFiles, TaskContext.ProjectFiles.Count(a => a.TargetLanguage != soruceLanguage)) + Environment.NewLine;
			summaryText += indent + string.Format(PluginResources.Label_Languages, GetProjectTargetLanguagesString(TaskContext.Project)) + Environment.NewLine;

			var targetLanguages = TaskContext.ProjectFiles.Where(a => a.Selected && a.TargetLanguage != soruceLanguage)
				.Select(a => a.TargetLanguage).Distinct();

			foreach (var targetLanguage in targetLanguages)
			{
				summaryText += Environment.NewLine;
				summaryText += string.Format(PluginResources.Label_Language, targetLanguage) + Environment.NewLine;

				var targetLanguageFiles =
					TaskContext.ProjectFiles.Where(a => a.Selected && Equals(a.TargetLanguage, targetLanguage));
				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					summaryText += indent + string.Format(PluginResources.Label_SdlXliffFile, targetLanguageFile.Location) + Environment.NewLine;
				}
			}

			return summaryText;
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
