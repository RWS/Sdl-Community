using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.SdlFreshstart.Properties;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class MainWindowViewModel:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public StudioViewModel StudioViewModel { get; set; }
		public MultiTermViewModel MultiTermViewModel { get; set; }

		public MainWindowViewModel(MainWindow mainWindow, IDialogCoordinator dialogCoordinator)
		{
			StudioViewModel = new StudioViewModel(mainWindow, dialogCoordinator);
			MultiTermViewModel = new MultiTermViewModel(mainWindow);
		}
		
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}