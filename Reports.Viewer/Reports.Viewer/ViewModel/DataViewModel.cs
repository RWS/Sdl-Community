using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Actions;
using Reports.Viewer.Plus.Commands;
using Reports.Viewer.Plus.CustomEventArgs;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.ViewModel
{
    public class DataViewModel : ReportsViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private string _windowTitle;
        private ObservableCollection<Report> _reports;
        private Report _selectedReport;
        private IList _selectedReports;
        private string _projectLocalFolder;
        private ICommand _clearSelectionCommand;
        private ICommand _dragDropCommand;
        private ICommand _mouseDoubleClick;

        public event EventHandler<ReportSelectionChangedEventArgs> ReportSelectionChanged;

        public ICommand ClearSelectionCommand => _clearSelectionCommand ?? (_clearSelectionCommand = new CommandHandler(ClearSelection));

        public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

        public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        public override string ProjectLocalFolder
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
            }
        }

        public ObservableCollection<Report> Reports
        {
            get => _reports;
            set
            {
                _reports = value;
                OnPropertyChanged(nameof(Reports));
                OnPropertyChanged(nameof(StatusLabel));
            }
        }

        public bool IsReportsSelected => SelectedReports?.Cast<Report>().ToList().Count > 0;

        public bool IsReportSelected => SelectedReports?.Cast<Report>().ToList().Count == 1;

        public override Report SelectedReport
        {
            get => _selectedReport;
            set
            {
                _selectedReport = value;
                OnPropertyChanged(nameof(SelectedReport));

                ReportSelectionChanged?.Invoke(this, new ReportSelectionChangedEventArgs
                {
                    SelectedReport = _selectedReport,
                    SelectedReports = SelectedReports?.Cast<Report>().ToList()
                });

                OnPropertyChanged(nameof(IsReportSelected));
                OnPropertyChanged(nameof(StatusLabel));
            }
        }

        public IList SelectedReports
        {
            get => _selectedReports;
            set
            {
                _selectedReports = value;
                OnPropertyChanged(nameof(SelectedReports));


                _selectedReport = _selectedReports?.Cast<Report>().ToList().FirstOrDefault();
                ReportSelectionChanged?.Invoke(this, new ReportSelectionChangedEventArgs
                {
                    SelectedReport = _selectedReport,
                    SelectedReports = _selectedReports?.Cast<Report>().ToList()
                });

                OnPropertyChanged(nameof(IsReportSelected));
                OnPropertyChanged(nameof(StatusLabel));
            }
        }

        public string StatusLabel
        {
            get
            {
                var message = string.Format(PluginResources.StatusLabel_ReportsSelected,
                    _reports?.Count ?? 0,
                    _selectedReports?.Count ?? 0);
                return message;
            }
        }

        private void DragDrop(object parameter)
        {
            var report = new Report();

            if (parameter == null || !(parameter is DragEventArgs eventArgs))
            {
                return;
            }

            var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
            if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
            {
                foreach (var fullPath in files)
                {
                    var fileAttributes = File.GetAttributes(fullPath);
                    if (!fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        if (string.IsNullOrEmpty(report.XsltPath) &&
                            (fullPath.ToLower().EndsWith(".xslt")
                             || fullPath.ToLower().EndsWith(".xsl")))
                        {
                            report.XsltPath = fullPath;
                        }
                        if (string.IsNullOrEmpty(report.Path) &&
                            (fullPath.ToLower().EndsWith(".html")
                             || fullPath.ToLower().EndsWith(".htm")
                             || fullPath.ToLower().EndsWith(".xml")))
                        {
                            report.Path = fullPath;
                        }
                    }
                }
            }

            var action = SdlTradosStudio.Application.GetAction<AddReportAction>();
            action.Run(report);
        }

        private void MouseDoubleClick(object parameter)
        {
            if (SelectedReport != null)
            {
                var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
                action.Run();
            }
        }

        private void ClearSelection(object parameter)
        {
            SelectedReports?.Clear();
            SelectedReport = null;
        }
    }
}
