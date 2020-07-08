using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel(MainWindow mainWindow, IDialogCoordinator dialogCoordinator)
		{
			StudioViewModel = new StudioViewModel(mainWindow, dialogCoordinator);
			MultiTermViewModel = new MultiTermViewModel(mainWindow);
			ReadMeViewModel = new ReadMeViewModel(new VersionService());
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public MultiTermViewModel MultiTermViewModel { get; set; }
		public ReadMeViewModel ReadMeViewModel { get; set; }
		public StudioViewModel StudioViewModel { get; set; }

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}