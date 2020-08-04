using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Sdl.Community.Reports.Viewer.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class DataViewModel : BaseModel, IDisposable
	{
		private string _htmlUri;
		private string _windowTitle;

		public DataViewModel()
		{
			Browser = new WebBrowser();
		}


		public WebBrowser Browser { get; set; }


		public string HtmlUri
		{
			get => _htmlUri;
			set
			{
				_htmlUri = value;
				OnPropertyChanged(nameof(HtmlUri));

				if (!string.IsNullOrEmpty(_htmlUri) && File.Exists(_htmlUri))
				{
					var ms = new MemoryStream();
					using (var file = new FileStream(_htmlUri, FileMode.Open, FileAccess.Read))
					{
						file.CopyTo(ms);
					}

					ms.Position = 0;
					Browser.NavigateToStream(ms);

					return;
				}

				Browser.NavigateToString("<html><div style=\"text - align:center\"><p>Empty</p></div></html>");
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
	}
}
