using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.SDLBatchAnonymize.Control
{
	public partial class BatchAnonymizerHostControl : UserControl, ISettingsAware<BatchAnonymizerSettings>, IUISettingsControl
	{
		public BatchAnonymizerHostControl()
		{
			InitializeComponent();
			BatchAnonymizerSettingsViewModel = new BatchAnonymizerSettingsViewModel();
			var batchAnonymizerControl = new View.BatchAnonymizerSettingsView
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
