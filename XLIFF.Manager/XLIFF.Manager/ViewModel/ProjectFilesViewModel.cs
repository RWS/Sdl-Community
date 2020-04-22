using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFilesViewModel : INotifyPropertyChanged, IDisposable
	{
		public ProjectFilesViewModel()
		{
			//TODO
		}

		public void Dispose()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
