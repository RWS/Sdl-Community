using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Documents;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private readonly BrowserView _browserView;
		private readonly DataView _dataView;
		private readonly BrowserViewModel _browserViewModel;
		private readonly DataViewModel _dataViewModel;
		private string _projectLocalFolder;
		private ContentControl _currentView;

		public ReportViewModel(BrowserViewModel browserViewModel, BrowserView browserView,
			DataViewModel dataViewModel, DataView dataView)
		{
			_browserViewModel = browserViewModel;
			_browserView = browserView;

			_dataViewModel = dataViewModel;
			_dataView = dataView;

			CurrentView = _dataView;
		}

		public ContentControl CurrentView
		{
			get => _currentView;
			set
			{
				_currentView = value;
				OnPropertyChanged(nameof(CurrentView));
			}
		}

		public string ProjectLocalFolder
		{
			get => _projectLocalFolder;
			set
			{
				if (_projectLocalFolder == value)
				{
					return;
				}

				_projectLocalFolder = value;
				OnPropertyChanged(nameof(ProjectLocalFolder));

				if (_dataViewModel != null)
				{
					_dataViewModel.ProjectLocalFolder = _projectLocalFolder;
				}				
			}
		}

		public void Print()
		{
			//var printDialog = new PrintDialog();
			//printDialog.PrintDocument(((IDocumentPaginatorSource)_browserView.WebBrowser.Document).DocumentPaginator, "Reports Viewer");			

			var doc = _browserView.WebBrowser.Document as mshtml.IHTMLDocument2;
			doc.execCommand("Print", true, null);
		}

		public void ShowPageSetupDialog()
		{
			//_webBrowser.ShowPageSetupDialog();
		}

		public void ShowPrintPreviewDialog()
		{
			//_webBrowser.ShowPrintPreviewDialog();
		}

		public void SaveReport()
		{
			//_mhtReport = Path.Combine(System.IO.Path.GetTempPath(), _report.Guid.ToString() + ".html");

			//ReportFormat htmlFormat = Array.Find<ReportFormat>(_report.AvailableFormats, delegate (ReportFormat format)
			//{
			//	return format.Name == "HTML";
			//});

			//ReportFormat xmlFormat = null;
			//if (htmlFormat == null)
			//{
			//	xmlFormat = _report.AvailableFormats.FirstOrDefault(x => x.Name == "XML");
			//	_mhtReport = System.IO.Path.ChangeExtension(_mhtReport, "xml");
			//}

			//if (htmlFormat == null && xmlFormat == null)
			//{
			//	throw new InvalidOperationException(StringResources.ReportRendererNotFound);
			//}

			//_report.SaveAs(_mhtReport, htmlFormat ?? xmlFormat);
			//_reportLoaded = true;
		}

		public void UpdateReport(Report report)
		{
			CurrentView = _browserView;

			string file = null;
			if (report != null)
			{
				file = Path.Combine(ProjectLocalFolder, report.Path);
				if (!File.Exists(file))
				{
					file = null;
				}
			}

			_browserViewModel.HtmlUri = file;
		}

		public void UpdateData(List<Report> reports)
		{
			CurrentView = _dataView;
			_dataViewModel.Reports = reports;
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
