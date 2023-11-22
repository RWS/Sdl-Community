using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

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
		}

		public TQAReportingSettings Settings { get; set; }

		
		public void Dispose()
		{
		}

		public bool ValidateChildren()
		{
			return Validation.GetErrors(TQAQualityLevelComboBox).Count == 0;
		}

	}
}