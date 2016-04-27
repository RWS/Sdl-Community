using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class FinishViewModel: INotifyPropertyChanged
    {
        private ICommand _finishCommand;
        private bool _canExecute;
        private readonly ProjectService _projectService;
        private bool _active;
        private bool _completed;
        private  string _location;
        private   string _txtName;
        private  string _txtDescription;
        private   string _sourceLanguage;
        private  string _targetLanguage;
        private  string _templateName;
        private  string _customer;
        private  string _dueDate;
        private PackageDetailsViewModel _packageDetailsViewModel;

        public FinishViewModel(PackageDetailsViewModel packageDetailsViewModel)
        {
            _packageDetailsViewModel = packageDetailsViewModel;

            _projectService = new ProjectService();
             _canExecute = true;
            _completed = false;
            Refresh();
            
          
        }

        public void Refresh()
        {
            Name = _packageDetailsViewModel.Name;
            Location = _packageDetailsViewModel.TextLocation;
            if (_packageDetailsViewModel.HasDueDate)
            {
                DueDate =
                    Helpers.TimeHelper.SetDateTime(_packageDetailsViewModel.DueDate,
                        _packageDetailsViewModel.SelectedHour, _packageDetailsViewModel.SelectedMinute,
                        _packageDetailsViewModel.SelectedMoment).ToString();
            }
           
            if (_packageDetailsViewModel.Template != null)
            {
                TemplateName = _packageDetailsViewModel.Template.Name;
            }
            if (_packageDetailsViewModel.SelectedCustomer != null)
            {
                Customer = _packageDetailsViewModel.SelectedCustomer.Name;
            }
           
            Description = _packageDetailsViewModel.Description;
            SourceLanguage = _packageDetailsViewModel.SourceLanguage;
            TargetLanguage = _packageDetailsViewModel.TargetLanguage;
        
        }
   
        public string Name
        {
            get { return _packageDetailsViewModel.Name ; }
            set
            {
                if (Equals(value, _txtName))
                {
                    return;
                }
                _txtName = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _packageDetailsViewModel.Description; }
            set
            {
                if (Equals(value, _txtDescription))
                {
                    return;
                }
                _txtDescription = value;
                OnPropertyChanged();
            }
        }

        public string SourceLanguage
        {
            get { return _packageDetailsViewModel.SourceLanguage; }
            set
            {
                if (Equals(value, _sourceLanguage))
                {
                    return;

                }
                _sourceLanguage = value;
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
                _targetLanguage = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get { return _packageDetailsViewModel.TextLocation; }
            set
            {
                if (Equals(value, _location))
                {
                    return;
                }
                _location = value;
                OnPropertyChanged();
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                if (Equals(value, _active))
                {
                    return;
                }
                _active = value;
                OnPropertyChanged();
            }
        }

        public string TemplateName
        {
            get { return _packageDetailsViewModel.Template.Name; }
            set
            {
                if (Equals(value,_templateName))
                {
                    return;
                    
                }
                _templateName = value;
                OnPropertyChanged();
            }
        }

        public string Customer
        {
            get { return _packageDetailsViewModel.SelectedCustomer.Name; }
            set
            {
                if (Equals(value, _customer))
                {
                    return;
                    
                }
                _customer = value;
                OnPropertyChanged();
            }
        }

        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                if (Equals(value, _dueDate))
                {
                    return;
                }
                _dueDate = value;
                OnPropertyChanged();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand FinishCommand
        {
            get { return _finishCommand ?? (_finishCommand = new CommandHandler(Finish, _canExecute)); }
           
        }

        public async void Finish()
        {
            Active = true;
           await Task.Run(()=>_projectService.CreateProject(_packageDetailsViewModel.GetPackageModel()));
          
            
             Active = false;

        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
