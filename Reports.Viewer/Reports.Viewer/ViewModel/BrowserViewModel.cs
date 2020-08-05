using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class BrowserViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _htmlUri;
		private string _windowTitle;

		public BrowserViewModel()
		{
			//Browser = new WebBrowser();
		}

		//public WebBrowser Browser { get; set; }

		public string HtmlUri
		{
			get => _htmlUri;
			set
			{
				_htmlUri = value;
				OnPropertyChanged(nameof(HtmlUri));

				//if (!string.IsNullOrEmpty(_htmlUri) && File.Exists(_htmlUri))
				//{
				//	var ms = new MemoryStream();
				//	using (var file = new FileStream(_htmlUri, FileMode.Open, FileAccess.Read))
				//	{
				//		file.CopyTo(ms);
				//	}

				//	ms.Position = 0;
				//	Browser.NavigateToStream(ms);

				//	return;
				//}

				//Browser.NavigateToString("<html><div style=\"text - align:center\"><p>Empty</p></div></html>");
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
