using System;
using System.Windows.Controls;
using LanguageWeaverProvider.Studio.BatchTask.Model;
using LanguageWeaverProvider.Studio.BatchTask.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace LanguageWeaverProvider.Studio.BatchTask.View
{
	/// <summary>
	/// Interaction logic for CreateQeReportView.xaml
	/// </summary>
	public partial class CreateQeReportView : UserControl, ISettingsAware<CreateQeReportSettings>, IUISettingsControl
	{
		public CreateQeReportView()
		{
			InitializeComponent();
			DataContext = ViewModel ??= new CreateQeReportViewModel();
		}

		public CreateQeReportSettings Settings { get; set; }

		public CreateQeReportViewModel ViewModel { get; set; }

		public void Dispose() { }

		public bool ValidateChildren() => true;

		public void SaveSettings()
		{
			Settings.ExcludeLockedSegments = (DataContext as CreateQeReportViewModel).ExcludeLockedSegments;
		}
	}
}
