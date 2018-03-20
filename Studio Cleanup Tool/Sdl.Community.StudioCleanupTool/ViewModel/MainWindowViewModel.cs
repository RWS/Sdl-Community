using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.StudioCleanupTool.Annotations;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.ViewModel
{
	public class MainWindowViewModel:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public StudioViewModel StudioViewModel { get; set; }
		public MultiTermViewModel MultiTermViewModel { get; set; }

		public MainWindowViewModel(MainWindow mainWindow)
		{
			StudioViewModel = new StudioViewModel(mainWindow);
			MultiTermViewModel = new MultiTermViewModel(mainWindow);

		}
		
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
