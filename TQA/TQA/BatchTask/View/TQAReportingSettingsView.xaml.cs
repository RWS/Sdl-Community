using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Community.TQA.BatchTask.ViewModel;

namespace Sdl.Community.TQA.BatchTask.View
{

	/// <summary>
	/// Interaction logic for TQAReportingSettingsView.xaml
	/// </summary>
	public partial class TQAReportingSettingsView : IUISettingsControl, ISettingsAware<TQAReportingSettings>
	{
		public TQAReportingSettingsView()
		{
			InitializeComponent();
			ReportingSettingsViewModel = new TQAReportingSettingsViewModel();
			DataContext = ReportingSettingsViewModel;
			
		}

		public TQAReportingSettings Settings { get; set; }

		public TQAReportingSettingsViewModel ReportingSettingsViewModel { get; set; }

		public void Dispose()
		{
		}

		public bool ValidateChildren()
		{
			return Validation.GetErrors(ReportOutputLocationTextBox).Count == 0
							 && Validation.GetErrors(TQAQualityLevelComboBox).Count == 0;
		}

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			ReportingSettingsViewModel.SetupQualitiesItems();
		}
	}
}