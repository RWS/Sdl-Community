using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.View
{
	/// <summary>
	/// Interaction logic for TargetRenamerSettingsView.xaml
	/// </summary>
	public partial class TargetRenamerSettingsView : ISettingsAware<TargetRenamerSettings>
	{
		public TargetRenamerSettingsView()
		{
			InitializeComponent();
			AddErrorHandlers();
		}

		public TargetRenamerSettings Settings { get; set; }
		public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel => (TargetRenamerSettingsViewModel)DataContext;

		private bool CustomLocationHasErrors { get; set; }
		private bool CustomStringHasErrors { get; set; }
		private bool DelimiterHasErrors { get; set; }
		private bool RegexReplaceWithHasErrors { get; set; }

		public void AddErrorHandlers()
		{
			Validation.AddErrorHandler(CustomLocation, ErrorHandler);
			Validation.AddErrorHandler(Delimiter, ErrorHandler);
			Validation.AddErrorHandler(RegexReplaceWith, ErrorHandler);
			Validation.AddErrorHandler(CustomString, ErrorHandler);
		}

		public void Dispose()
		{
		}

		private void ErrorHandler(object sender, ValidationErrorEventArgs e)
		{
			var errorSource = e.OriginalSource as TextBox;

			switch (errorSource?.Name)
			{
				case nameof(CustomLocation):
					CustomLocationHasErrors = Validation.GetErrors(CustomLocation).Count > 0;
					break;

				case nameof(Delimiter):
					DelimiterHasErrors = Validation.GetErrors(Delimiter).Count > 0;
					break;

				case nameof(RegexReplaceWith):
					RegexReplaceWithHasErrors = Validation.GetErrors(RegexReplaceWith).Count > 0;
					break;

				case nameof(CustomString):
					CustomStringHasErrors = Validation.GetErrors(CustomString).Count > 0;
					break;
			}

			SetHasErrorOnViewModel();
		}

		private void SetHasErrorOnViewModel()
		{
			TargetRenamerSettingsViewModel.HasErrors = CustomLocationHasErrors || DelimiterHasErrors || RegexReplaceWithHasErrors || CustomStringHasErrors;
		}
	}
}