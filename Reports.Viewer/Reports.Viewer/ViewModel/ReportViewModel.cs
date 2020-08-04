using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.View;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged
	{
		private string _windowTitle;
		private string _htmlUri;
		private ReportView _view;
		private bool _isTest;

		public ReportViewModel(ReportView view)
		{
			_view = view;
			IsTest = true;
			Browser = new WebBrowser();
		}

		public WebBrowser Browser { get; set; }

		public bool IsTest
		{
			get => _isTest;
			set
			{
				if (_isTest == value)
				{
					return;
				}

				_isTest = value;
				OnPropertyChanged(nameof(IsTest));
			}
		}

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

		public void UpdateReport(string filePath)
		{
			//BrowserIsVisible = true;
			//DataIsVisible = false;
			//CurrentView = BrowserView;

			//_view.Dispatcher.Invoke(new Action(delegate
			//{
				//IsTest = true;
				HtmlUri = filePath;
			//}));

		}

		public void UpdateData(List<Report> reports)
		{
			//DataIsVisible = true;
			//BrowserIsVisible = false;

			//CurrentView = DataView;
			//_view.Dispatcher.Invoke(new Action(delegate
			//{
				//IsTest = false;
			//}));
			Browser.NavigateToString("<html><div style=\"text - align:center\"><p>[debug info: none selected]</p></div></html>");
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
