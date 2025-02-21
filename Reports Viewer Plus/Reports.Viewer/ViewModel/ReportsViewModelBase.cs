using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Plus.Actions;
using Reports.Viewer.Plus.Commands;
using Sdl.TranslationStudioAutomation.IntegrationApi;

// I'm the Studio 2022 don't forget
namespace Reports.Viewer.Plus.ViewModel
{
    public class ReportsViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private ICommand _editReportCommand;
        private ICommand _removeReportCommand;
        private ICommand _openFolderCommand;
        private ICommand _printReportCommand;
        private ICommand _saveAsCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Report SelectedReport { get; set; }

        public virtual string ProjectLocalFolder { get; set; }

        public ICommand EditReportCommand => _editReportCommand ?? (_editReportCommand = new CommandHandler(EditReport));

        public ICommand RemoveReportCommand => _removeReportCommand ?? (_removeReportCommand = new CommandHandler(RemoveReport));

        public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new CommandHandler(OpenFolder));

        public ICommand PrintReportCommand => _printReportCommand ?? (_printReportCommand = new CommandHandler(PrintReport));

        public ICommand SaveAsCommand => _saveAsCommand ?? (_saveAsCommand = new CommandHandler(SaveAs));

        public void Dispose()
        {
        }

        private void EditReport(object parameter)
        {
            var action = SdlTradosStudio.Application.GetAction<EditReportAction>();
            action.Run();
        }

        private void RemoveReport(object parameter)
        {
            var action = SdlTradosStudio.Application.GetAction<RemoveReportAction>();
            action.Run();
        }

        private void OpenFolder(object parameter)
        {
            if (SelectedReport?.Path == null || string.IsNullOrEmpty(ProjectLocalFolder)
                                             || !Directory.Exists(ProjectLocalFolder))
            {
                return;
            }

            var path = Path.Combine(ProjectLocalFolder, SelectedReport.Path.Trim('\\'));

            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(path));
            }
        }

        private void PrintReport(object parameter)
        {
            var action = SdlTradosStudio.Application.GetAction<PrintReportAction>();
            action.Run();
        }

        private void SaveAs(object parameter)
        {
            var action = SdlTradosStudio.Application.GetAction<SaveAsReportAction>();
            action.Run();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
