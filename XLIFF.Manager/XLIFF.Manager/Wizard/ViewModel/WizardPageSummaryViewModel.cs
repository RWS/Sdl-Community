using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageSummaryViewModel : WizardPageViewModelBase
	{		
		private string _summaryText;

		public WizardPageSummaryViewModel(Window owner, object view, WizardContextModel wizardContext) : base(owner, view, wizardContext)
		{		
			IsValid = true;
			PropertyChanged += WizardPageSummaryViewModel_PropertyChanged;
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

		private string GetSummaryText()
		{
			var project = WizardContext.ProjectFileModels[0].ProjectModel;

			var summaryText = "Id: " + project.Id + Environment.NewLine;
			summaryText += "Name: " + project.Name + Environment.NewLine;
			summaryText += "Location: " + project.Path + Environment.NewLine;
			summaryText += "Created: " + project.Created.ToString(CultureInfo.InvariantCulture);
			summaryText += "Due Date: " + project.DueDate.ToString(CultureInfo.InvariantCulture) + Environment.NewLine;
			summaryText += "Soruce Language: " + project.SourceLanguage.CultureInfo.DisplayName + Environment.NewLine;
			summaryText += "Target Languages: " + GetProjectTargetLanguagesString(project) + Environment.NewLine;
			summaryText += "Project Type: " + project.ProjectType + Environment.NewLine;
			summaryText += "Customer: " + project.Customer?.Name + Environment.NewLine;

			summaryText += Environment.NewLine;

			summaryText += "Total Files: " + WizardContext.ProjectFileModels.Count + Environment.NewLine;
			summaryText += "Selected Files: " + WizardContext.ProjectFileModels.Count(a => a.Selected) + Environment.NewLine;
			summaryText += "Selected Languages: " + GetSelectedLanguagesString() + Environment.NewLine;

			var targetLanguages = WizardContext.ProjectFileModels.Where(a => a.Selected)
				.Select(a => a.TargetLanguage.CultureInfo).Distinct();
			foreach (var targetLanguage in targetLanguages)
			{
				summaryText += Environment.NewLine;
				summaryText += "Language: " + targetLanguage.DisplayName + Environment.NewLine;

				var targetLanguageFiles =
					WizardContext.ProjectFileModels.Where(a => a.Selected && Equals(a.TargetLanguage.CultureInfo, targetLanguage));
				foreach (var targetLanguageFile in targetLanguageFiles)
				{
					summaryText += "\t" + targetLanguageFile.Path + targetLanguageFile.Name + Environment.NewLine;
				}
			}

			return summaryText;
		}

		private static string GetProjectTargetLanguagesString(ProjectModel project)
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
			var selected = WizardContext.ProjectFileModels.Where(a => a.Selected);

			var selectedLanguages = string.Empty;
			foreach (var cultureInfo in selected.Select(a => a.TargetLanguage.CultureInfo).Distinct())
			{
				selectedLanguages += (string.IsNullOrEmpty(selectedLanguages) ? string.Empty : ", ") +
									 cultureInfo.DisplayName;
			}

			return selectedLanguages;
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

		public override string DisplayName => "Summary";

		public sealed override bool IsValid { get; set; }
	}
}
