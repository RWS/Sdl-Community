﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp;
using Microsoft.Win32;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.View;
using Sdl.ProjectAutomation.Core;

namespace Reports.Viewer.Plus.ViewModel
{
    public class ReportViewModel : INotifyPropertyChanged, IDisposable
    {
        private string _windowTitle;
        private readonly BrowserView _browserView;
        private readonly BrowserViewModel _browserViewModel;
        private readonly DataView _dataView;
        private readonly DataViewModel _dataViewModel;
        private string _projectLocalFolder;
        private ContentControl _currentView;
        private string _address;
        private Report _currentReport;

        public ReportViewModel(
            BrowserViewModel browserViewModel,
            BrowserView browserView,
            DataViewModel dataViewModel,
            DataView dataView,
            IProject selectedProject)
        {
            _browserViewModel = browserViewModel;
            _browserView = browserView;
            _dataViewModel = dataViewModel;
            _dataView = dataView;
            SelectedProject = selectedProject;
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

                if (_browserViewModel != null) _browserViewModel.Address = _address;
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

        public IProject SelectedProject { get; set; }

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

                if (_browserViewModel != null)
                {
                    _browserViewModel.ProjectLocalFolder = _projectLocalFolder;
                }
            }
        }

        public void Print()
        {
            _browserView.WebBrowser.Print();
        }

        public void SaveReport()
        {
            var reports = CurrentView.GetType() == typeof(BrowserView)
                ? new List<Report> { _currentReport }
                : _dataViewModel.SelectedReports.Cast<Report>().ToList();

            var projectInfo = SelectedProject.GetProjectInfo();
            var projectPath = projectInfo.LocalProjectFolder;

            if (reports.Count == 1)
            {
                Report report = reports[0];
                var filter = GetFileDialogFilter(report);

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
                    SaveSingleReport(report, dialog.FileName);
                }
            }
            else
            {
                var saveMultipleReportsViewModel = new SaveMultipleReportsViewModel(SelectedProject, new Service.DialogService());
                var saveMultipleReports = new SaveMultipleReportsWindow(saveMultipleReportsViewModel);
                if (saveMultipleReports.ShowDialog() == true)
                {
                    foreach (var r in reports)
                    {
                        var fullPath = CreateReportFileName(r, saveMultipleReportsViewModel.Path, saveMultipleReportsViewModel.SelectedFormat, saveMultipleReportsViewModel.CreateSubFolders);
                        SaveSingleReport(r, fullPath);
                    }
                }
                
            }
        }

        public void UpdateReport(Report report)
        {
            CurrentView = _browserView;

            WebBrowserNavigateToReport(report);
            if (_browserViewModel != null) _browserViewModel.SelectedReport = report;
        }

        public void UpdateData(List<Report> reports)
        {
            CurrentView = _dataView;
            _dataViewModel.Reports = new ObservableCollection<Report>(reports);
            _dataViewModel.SelectedReports = null;
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

        private void SaveSingleReport(Report report, string fileName)
        {
            try
            {
                var extension = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
                switch (extension.ToLower())
                {
                    case "xlsx":
                        {
                            if (report.IsStudioReport)
                            {
                                SelectedProject.SaveTaskReportAs(report.Id, fileName, ReportFormat.Excel);
                            }
                            else
                            {
                                SaveReportsAsXlsx(report, fileName);
                            }

                            break;
                        }
                    case "html":
                        {
                            if (report.IsStudioReport)
                            {
                                SelectedProject.SaveTaskReportAs(report.Id, fileName, ReportFormat.Html);
                            }
                            else
                            {
                                SaveReportAsHtml(report, fileName);
                            }

                            break;
                        }
                    case "mht":
                        {
                            SelectedProject.SaveTaskReportAs(report.Id, fileName, ReportFormat.Mht);
                            break;
                        }
                    case "xml":
                        {
                            if (report.IsStudioReport)
                            {
                                SelectedProject.SaveTaskReportAs(report.Id, fileName, ReportFormat.Xml);
                            }
                            else
                            {
                                SaveReportAsXml(report, fileName);
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private string CreateReportFileName(Report report, string path, string format, bool createSubFolders)
        {
            string baseFileName = $"{report.Name} - {report.Group} - {report.Language}.{format}";

            string folderPath = path;
            if (createSubFolders)
            {
                folderPath = Path.Combine(path, report.Group, report.Language);
                Directory.CreateDirectory(folderPath);  // Create subfolders if they don't exist
            }

            string fullPath = Path.Combine(folderPath, baseFileName);
            int counter = 1;

            while (File.Exists(fullPath))
            {
                string newFileName = $"{report.Name} - {report.Group} - {report.Language} ({counter}).{format}";
                fullPath = Path.Combine(folderPath, newFileName);
                counter++;
            }

            return fullPath;
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
            //var processes = Process.GetProcessesByName("CefSharp.BrowserSubprocess");
            //foreach (var process in processes)
            //{
            //	process.Dispose();
            //}

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

        private void SaveReportAsHtml(Report report, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var fullFilePath = Path.Combine(ProjectLocalFolder, report.Path);
            File.Copy(fullFilePath, fileName);
        }

        private void SaveReportAsXml(Report report, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var xmlFile = report.Path.Substring(0,
                report.Path.LastIndexOf(".", StringComparison.Ordinal));
            var fullFilePath = Path.Combine(ProjectLocalFolder, xmlFile);
            if (!File.Exists(fullFilePath))
            {
                throw new Exception(string.Format(PluginResources.ErrorMessage_UnableToLocateXmlFile, fullFilePath));
            }

            File.Copy(fullFilePath, fileName);
        }

        private void SaveReportsAsXlsx(Report report, string fileName)
        {
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                xlApp.Visible = false;
                object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;

                var fullFilePath = Path.Combine(ProjectLocalFolder, report.Path);
                var xlWorkbook = xlApp.Workbooks.Open(fullFilePath);
                xlWorkbook.SaveAs(fileName, format);

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
