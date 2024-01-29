using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.Desktop.IntegrationApi;

namespace LanguageWeaverProvider.Studio.BatchTask.Model
{
	public class CreateQeReportSettingsPage : DefaultSettingsPage<CreateQeReportView, CreateQeReportSettings>
	{
		public override void Save()
		{
			base.Save();
			var createQeReportView = GetControl() as CreateQeReportView;
			var createQeReportViewModel = createQeReportView.DataContext as CreateQeReportViewModel;

			var settings = createQeReportView.Settings;
			settings.ExcludeLockedSegments = createQeReportViewModel.ExcludeLockedSegments;
		}
	}
}