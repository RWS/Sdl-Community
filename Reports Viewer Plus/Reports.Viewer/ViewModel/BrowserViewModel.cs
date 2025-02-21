using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Commands;

namespace Reports.Viewer.Plus.ViewModel
{
    public class BrowserViewModel : ReportsViewModelBase, INotifyPropertyChanged, IDisposable
    {
        private string _address;
        private Report _selectedReport;
        private bool _isLoading;
        private string _projectLocalFolder;

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(IsValidReport));
                IsLoading = true;
            }
        }

        public override Report SelectedReport
        {
            get => _selectedReport;
            set
            {
                _selectedReport = value;
                OnPropertyChanged(nameof(Report));
                OnPropertyChanged(nameof(IsValidReport));
                IsLoading = true;
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool IsValidReport => Address != null && SelectedReport != null;

        public override string ProjectLocalFolder
        {
            get => _projectLocalFolder;
            set
            {
                _projectLocalFolder = value;
                OnPropertyChanged(nameof(ProjectLocalFolder));
            }
        }
    }
}
