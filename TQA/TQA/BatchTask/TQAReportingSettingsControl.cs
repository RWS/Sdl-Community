using System;
using Sdl.Community.TQA.Providers;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using UserControl = System.Windows.Forms.UserControl;

namespace Sdl.Community.TQA.BatchTask
{
	public partial class TQAReportingSettingsControl : UserControl, ISettingsAware<TQAReportingSettings>
	{
		public TQAReportingSettingsControl()
		{
			InitializeComponent();
		}

		public TQAReportingSettings Settings { get; set; }

		public ISettingsBundle ProjectSettings { get; set; }

		public ReportProvider ReportProvider { get; set; }

		public CategoriesProvider CategoriesProvider { get; set; }

		public QualitiesProvider QualitiesProvider { get; set; }

		private void ResetSettingsToDefault(object obj)
		{
			ResetToDefaults();
		}

		public TQAReportingSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();
			Settings.TQAReportingQuality = TQAQualityLevelComboBox.SelectedItem.ToString();

			return Settings;
		}

		public TQAReportingSettings SaveSettings()
		{
			Settings.TQAReportingQuality = TQAQualityLevelComboBox.SelectedItem.ToString();

			return Settings;
		}

		private void Initialize()
		{
			TQAQualityLevelComboBox.Items.Clear();
			
			var tqaCategories = CategoriesProvider.GetAssessmentCategories(ProjectSettings);
			var tqaProfileType = CategoriesProvider.GetTQAProfileType(tqaCategories);

			var qualities = QualitiesProvider.GetQualities(tqaProfileType);
			foreach (var quality in qualities)
			{
				TQAQualityLevelComboBox.Items.Add(quality);
			}

			TQAQualityLevelComboBox.SelectedItem = Settings.TQAReportingQuality;

			TQAProfileName.Text = ReportProvider.GetProfileTypeName(tqaProfileType);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Initialize();
		}

		private void TQAQualityLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}
	}
}
