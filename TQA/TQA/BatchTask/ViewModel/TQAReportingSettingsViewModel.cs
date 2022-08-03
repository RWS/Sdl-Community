using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using System.Windows.Input;
//using Microsoft.Win32;
using Sdl.Desktop.IntegrationApi;
using Sdl.Community.TQA.BatchTask;
using Sdl.Community.TQA.Model;
using Sdl.Community.TQA.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi;


namespace Sdl.Community.TQA.BatchTask.ViewModel
{
	public class TQAReportingSettingsViewModel : BaseModel, ISettingsAware<TQAReportingSettings>
	{

		private ICommand resetToDefault;
		private string selectedComboBoxItem;
		private ICommand selectReportingFolder;
		private SaveFileDialog outputSaveDialog = new SaveFileDialog();



		public TQAReportingSettingsViewModel()
		{
			TQAQualityItems = new ObservableCollection<string>();
			SetupSaveDialog();

		}

		internal void SetupQualitiesItems()
		{
			if (ReportQualities == null)
			{
				var filesController = SdlTradosStudio.Application.GetController<FilesController>();
				var currentProject= filesController?.CurrentProject;
				var reportingTask = new TQAReportingTask(currentProject);
				if (currentProject != null)
				{
					
					ReportQualities = reportingTask.GetQualitiesForTQAStandard();
					ReportQualities.ForEach(TQAQualityItems.Add);
				}
			}
			else
			{
				foreach (var qualityName in ReportQualities)
				{
					if (!TQAQualityItems.Contains(qualityName))
					   TQAQualityItems.Add(qualityName);
				}
				
			}
		}

		public ObservableCollection<string> TQAQualityItems { get; }

		public string ReportOutputLocation
		{
			get => Settings.TQAReportOutputLocation;
			set
			{
				if (Settings.TQAReportOutputLocation == value) return;
				Settings.TQAReportOutputLocation = value;
				OnPropertyChanged(nameof(ReportOutputLocation));
			}
		}

		public string ReportQuality
		{
			get => Settings.TQAReportingQuality;
			set
			{
				if (Settings.TQAReportingQuality == value) return;
				Settings.TQAReportingQuality = value;
				OnPropertyChanged(nameof(ReportQuality));
			}
		}

		public List<string> ReportQualities
		{
			get => Settings.TQAReportingQualities;
			set
			{
				if (Settings.TQAReportingQualities == value) return;
				Settings.TQAReportingQualities = value;
				OnPropertyChanged(nameof(ReportQualities));
			}
		}

		public ICommand ResetToDefault => resetToDefault ?? (resetToDefault = new CommandHandler(ResetSettingsToDefault));

		public string SelectedComboBoxItem
		{
			get => selectedComboBoxItem;
			set
			{
				if (selectedComboBoxItem == value) return;
				OnSelectedComboBoxValueChanged(value);

				selectedComboBoxItem = value;
				OnPropertyChanged(nameof(SelectedComboBoxItem));
				Settings.TQAReportingQuality = value;
			}
		}

		public ICommand SelectReportOutputFolder =>
			selectReportingFolder ?? (selectReportingFolder = new CommandHandler(SelectFolder));
		private TQAReportingSettings _settings;
		public TQAReportingSettings Settings
		{
			get => _settings;
			set
			{
				if (string.IsNullOrWhiteSpace(value.TQAReportOutputLocation)) value.TQAReportOutputLocation =TQAReportingSettings.GetReportingOutputFolder();// Path.GetTempPath()
				if (string.IsNullOrWhiteSpace(value.TQAReportingQuality)) value.TQAReportingQuality = TQAReportingSettings.TQAReportingDefaultQuality;

				_settings = value;

				SelectedComboBoxItem = value.TQAReportingQuality;
			}
		}


		private void OnSelectedComboBoxValueChanged(string value)
		{

		}

		private void SetupSaveDialog()
		{
			outputSaveDialog.InitialDirectory = TQAReportingSettings.GetReportingOutputFolder();
			outputSaveDialog.Filter = @"Excel Macro - Enabled Workbook | *.xlsm";
			outputSaveDialog.DefaultExt = TQStandardsFactory.reportingFileExtension;
		}

	

		private void ResetSettingsToDefault(object obj)
		{
			SelectedComboBoxItem = "Premium";
			var projectFolder= TQAReportingSettings.GetReportingOutputFolder();
			ReportOutputLocation = projectFolder;
			outputSaveDialog.InitialDirectory = projectFolder;
			Settings.TQAReportOutputLocation = projectFolder;
			Settings.TQAReportingQuality = SelectedComboBoxItem;
		}

		private void SelectFolder(object obj)
		{
			if (outputSaveDialog.ShowDialog() == DialogResult.OK)
				ReportOutputLocation = outputSaveDialog.FileName;

		}
	}
}