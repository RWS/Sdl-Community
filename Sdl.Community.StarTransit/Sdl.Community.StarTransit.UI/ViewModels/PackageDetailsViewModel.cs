using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class PackageDetailsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _textLocation;
        private string _txtName;
        private string _txtDescription;
        private List<ProjectTemplateInfo> _studioTemplates;
        private bool _hasDueDate;
        private DateTime _dueDate;
        private string _sourceLanguage;
        private string _targetLanguage;
        private PackageModel _packageModel;
        private ICommand _browseCommand;
        private bool _canExecute;
        private ProjectTemplateInfo _template;

        public PackageDetailsViewModel(PackageModel package)
        {
            _packageModel = package;
            _txtName = package.Name;
            _txtDescription = package.Description;
            _studioTemplates = package.StudioTemplates;
            _textLocation = package.Location;
            _sourceLanguage = package.SourceLanguage.DisplayName;

            var targetLanguage = string.Empty;
            foreach (var language in package.TargetLanguage)
            {
                targetLanguage = targetLanguage + language.DisplayName;
            }
            _targetLanguage = targetLanguage;

            _canExecute = true;
        }

        public ICommand BrowseCommand
        {
            get { return _browseCommand ?? (_browseCommand = new CommandHandler(Browse, _canExecute)); }
           
        }

     

        public void Browse()
        {
            var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
               _textLocation = folderDialog.SelectedPath;
               OnPropertyChanged(nameof(TextLocation));
            }
        }

  
        public string TextLocation
        {
            get { return _textLocation; }
            set
            {
                if (Equals(value, _textLocation))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _txtName; }
            set
            {
                if (Equals(value, _txtName))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _txtDescription; }
            set
            {
                if (Equals(value, _txtDescription))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public List<ProjectTemplateInfo> StudioTemplates
        {
            get
            {
                return _studioTemplates;
            }
            set
            {
                if (Equals(value, _studioTemplates))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public ProjectTemplateInfo Template
        {
            get { return _template; }
            set
            {
                if (Equals(value, _template))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public bool HasDueDate
        {
            get { return _hasDueDate; }
            set
            {
                if (Equals(value,_hasDueDate))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public DateTime DueDate
        {
            get { return _dueDate; }
            set
            {
                if (Equals(value, _dueDate))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string SourceLanguage
        {
            get { return _sourceLanguage; }
            set
            {
                if (Equals(value, _sourceLanguage))
                {
                    return;
                    
                }
                OnPropertyChanged();
            }
        }

        public string TargetLanguage
        {
            get { return _targetLanguage; }
            set
            {
                if (Equals(value, _targetLanguage))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TextLocation")
                {if(string.IsNullOrEmpty(TextLocation))
                        
                    return "Location is required";
                }
                if (columnName == "Template" && Template==null)
                {
                    
                        return "Template is rquired";
                   
                }
                return null;
            }
           
        }

        public string Error { get; }

        public PackageModel GetPackageModel()
        {
            _packageModel.Name = _txtName;
            _packageModel.Description = _txtDescription;
            _packageModel.Location = _textLocation;
           // _packageModel.DueDate = _dueDate;
           // _packageModel.te
           return _packageModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
