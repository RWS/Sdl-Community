using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using Microsoft.Win32;
using Sdl.Community.Reports.Viewer.View;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class ReportViewModel : INotifyPropertyChanged, IDisposable
	{
		private string _windowTitle;
		private readonly BrowserView _browserView;
		private readonly DataView _dataView;
		private readonly DataViewModel _dataViewModel;
		private readonly IProject _selectedProject;
		private string _projectLocalFolder;
		private ContentControl _currentView;
		private string _address;
		private Report _currentReport;

		public ReportViewModel(BrowserView browserView,
			DataViewModel dataViewModel, DataView dataView, IProject selectedProject)
		{
			_browserView = browserView;
			_dataViewModel = dataViewModel;
			_dataView = dataView;
			_selectedProject = selectedProject;
			_dataViewModel.ReportSelectionChanged += DataViewModel_ReportSelectionChanged;

			CurrentView = _dataView;
		}

		public string Address
		{
			get => _address;
			set
			{
				if (_address == value)
				{
					return;
				}

				_address = value;
				OnPropertyChanged(nameof(Address));
			}
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
			_browserView.WebBrowser.Print();
		}

		public void ShowPageSetupDialog()
		{
			//_browserView.WebBrowser.ShowPageSetupDialog();
		}

		public void ShowPrintPreviewDialog()
		{
			//_browserView.WebBrowser.ShowPrintPreviewDialog();
		}

		public void SaveReport()
		{
			var report = CurrentView.GetType() == typeof(BrowserView)
				? _currentReport
				: _dataViewModel.SelectedReport;

			var projectInfo = _selectedProject.GetProjectInfo();
			var projectPath = projectInfo.LocalProjectFolder;
			var filter = GetFileDialogFilter(report);

			try
			{
				var dialog = new SaveFileDialog
				{
					FileName = report.Name,
					InitialDirectory = projectPath,
					Title = PluginResources.ReportsViewer_SaveReportAs,
					ValidateNames = true,
					AddExtension = true,
					Filter = filter
				};

				if (dialog.ShowDialog() == true)
				{
					var extension =
						dialog.FileName.Substring(dialog.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
					switch (extension.ToLower())
					{
						case "xlsx":
							{
								if (report.IsStudioReport)
								{
									_selectedProject.SaveTaskReportAs(new Guid(report.Id), dialog.FileName, ReportFormat.Excel);
								}
								else
								{
									SaveReportsAsXlsx(report, dialog);
								}

								break;
							}
						case "html":
							{
								if (report.IsStudioReport)
								{
									_selectedProject.SaveTaskReportAs(new Guid(report.Id), dialog.FileName, ReportFormat.Html);
								}
								else
								{
									SaveReportAsHtml(report, dialog);
								}

								break;
							}
						case "mht":
							{
								_selectedProject.SaveTaskReportAs(new Guid(report.Id), dialog.FileName, ReportFormat.Mht);
								break;
							}
						case "xml":
							{
								if (report.IsStudioReport)
								{
									_selectedProject.SaveTaskReportAs(new Guid(report.Id), dialog.FileName, ReportFormat.Xml);
								}
								else
								{
									SaveReportAsXml(report, dialog);
								}
								
								break;
							}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void UpdateReport(Report report)
		{
			CurrentView = _browserView;

			WebBrowserNavigateToReport(report);
		}

		public void UpdateData(List<Report> reports)
		{
			CurrentView = _dataView;
			_dataViewModel.Reports = new ObservableCollection<Report>(reports);
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

		private string GetFileDialogFilter(Report report)
		{
			string filter;
			if (report.IsStudioReport)
			{
				filter = "Excel files(*.xlsx)|*.xlsx|Html files(*.html)|*.html|MHT files(*.mht)|*.mht|XML files(*.xml)|*.xml";
			}
			else
			{
				filter = "Excel files(*.xlsx)|*.xlsx|Html files(*.html)|*.html";
				if (!string.IsNullOrEmpty(report.XsltPath) && File.Exists(Path.Combine(ProjectLocalFolder, report.XsltPath)))
				{
					var xmlFile = report.Path.Substring(0,
						report.Path.LastIndexOf(".", StringComparison.Ordinal));
					var fullFilePath = Path.Combine(ProjectLocalFolder, xmlFile);
					if (File.Exists(fullFilePath))
					{
						filter += "|XML files(*.xml)|*.xml";
					}
				}
			}

			return filter;
		}

		private void WebBrowserNavigateToReport(Report report)
		{
			_currentReport = report;

			string file = null;
			if (report != null)
			{
				file = Path.Combine(ProjectLocalFolder, report.Path);
				if (!File.Exists(file))
				{
					file = null;
				}
			}

			Address = file != null ? "file://" + file : null;
		}

		private void SaveReportAsHtml(Report report, FileDialog dialog)
		{
			if (File.Exists(dialog.FileName))
			{
				File.Delete(dialog.FileName);
			}

			var fullFilePath = Path.Combine(ProjectLocalFolder, report.Path);
			File.Copy(fullFilePath, dialog.FileName);
		}

		private void SaveReportAsXml(Report report, FileDialog dialog)
		{
			if (File.Exists(dialog.FileName))
			{
				File.Delete(dialog.FileName);
			}

			var xmlFile = report.Path.Substring(0,
				report.Path.LastIndexOf(".", StringComparison.Ordinal));
			var fullFilePath = Path.Combine(ProjectLocalFolder, xmlFile);
			if (!File.Exists(fullFilePath))
			{
				throw new Exception(string.Format(PluginResources.ErrorMessage_UnableToLocateXmlFile, fullFilePath));
			}

			File.Copy(fullFilePath, dialog.FileName);
		}

		private void SaveReportsAsXlsx(Report report, FileDialog dialog)
		{
			var xlApp = new Microsoft.Office.Interop.Excel.Application();

			try
			{
				if (File.Exists(dialog.FileName))
				{
					File.Delete(dialog.FileName);
				}

				xlApp.Visible = false;
				object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;

				var fullFilePath = Path.Combine(ProjectLocalFolder, report.Path);
				var xlWorkbook = xlApp.Workbooks.Open(fullFilePath);
				xlWorkbook.SaveAs(dialog.FileName, format);

				GC.Collect();
				GC.WaitForPendingFinalizers();

				xlWorkbook.Close();
				Marshal.ReleaseComObject(xlWorkbook);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				xlApp.Quit();
				Marshal.ReleaseComObject(xlApp);
			}
		}
		
		private void DataViewModel_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			if (CurrentView is DataView)
			{
				WebBrowserNavigateToReport(e.SelectedReports?.Count > 0 ? e.SelectedReports[0] : null);
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
