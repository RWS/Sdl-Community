using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	public partial class BatchAnonymizerHostControl : UserControl, ISettingsAware<BatchAnonymizerSettings>
	{
		public BatchAnonymizerHostControl()
		{
			InitializeComponent();

			//TODO: add view model
			var batchAnonymizerControl = new BatchAnonymizerSettingsWpfControl();
			batchAnonymizerControl.InitializeComponent();
			batchAnonymizerHost.Child = batchAnonymizerControl;
		}

		public BatchAnonymizerSettings Settings { get; set; }
	}
}
