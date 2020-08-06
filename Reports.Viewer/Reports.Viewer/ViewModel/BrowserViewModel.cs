using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class BrowserViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _htmlUri;
		private string _windowTitle;


		public string HtmlUri
		{
			get => _htmlUri;
			set
			{
				_htmlUri = value;
				OnPropertyChanged(nameof(HtmlUri));
			}
		}

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
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
