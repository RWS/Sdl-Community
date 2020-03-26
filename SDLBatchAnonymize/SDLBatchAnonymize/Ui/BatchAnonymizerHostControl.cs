using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Service;
using Sdl.Community.SDLBatchAnonymize.ViewModel;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	public partial class BatchAnonymizerHostControl : UserControl, ISettingsAware<BatchAnonymizerSettings>
	{
		public BatchAnonymizerHostControl()
		{
			InitializeComponent();

			var userNameService = new UserNameService();
			var batchAnonymizerControl = new BatchAnonymizerSettingsWpfControl
			{
				DataContext = new BatchAnonymizerSettingsViewModel(userNameService)
			};
			batchAnonymizerControl.InitializeComponent();
			batchAnonymizerHost.Child = batchAnonymizerControl;
		}

		public BatchAnonymizerSettings Settings { get; set; }
	}
}
