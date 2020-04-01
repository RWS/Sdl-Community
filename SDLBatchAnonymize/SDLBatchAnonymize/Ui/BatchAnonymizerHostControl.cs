using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.ViewModel;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	public partial class BatchAnonymizerHostControl : UserControl, ISettingsAware<BatchAnonymizerSettings>
	{
		public BatchAnonymizerHostControl()
		{
			InitializeComponent();

			BatchAnonymizerSettingsViewModel = new BatchAnonymizerSettingsViewModel();
			var batchAnonymizerControl = new BatchAnonymizerSettingsWpfControl
			{
				DataContext = BatchAnonymizerSettingsViewModel
			};
			batchAnonymizerControl.InitializeComponent();
			batchAnonymizerHost.Child = batchAnonymizerControl;
		}

		public BatchAnonymizerSettingsViewModel BatchAnonymizerSettingsViewModel { get; set; }
		public BatchAnonymizerSettings Settings { get; set; }
	}
}
