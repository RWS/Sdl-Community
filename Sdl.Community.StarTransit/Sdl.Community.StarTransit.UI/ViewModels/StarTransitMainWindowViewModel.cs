using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.StarTransit.UI.Controls;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class StarTransitMainWindowViewModel:INotifyPropertyChanged
    {
        private ICommand _nextCommand;
        private ICommand _backCommand;
        private ICommand _createCommand;
        private bool _canExecuteNext;
        private bool _canExecuteBack;
        private bool _canExecuteCreate;
        private PackageDetailsViewModel _packageDetailsViewModel;
        private PackageDetails _packageDetails;
       private bool _isDetailsSelected;
        private bool _isTmSelected;
        private bool _isFinishSelected;
        private FinishViewModel _finishViewModel;
        private PackageModel _package;
        private ProjectService _projectService;
        private bool _active;

        public bool DetailsSelected
        {
            get { return _isDetailsSelected; }
            set
            {
                if (Equals(value, _isDetailsSelected))
                {
                    return;
                }
                _isDetailsSelected = value;
                OnPropertyChanged();
            }
        }

        public bool TmSelected
        {
            get { return _isTmSelected; }
            set
            {
                if (Equals(value, _isTmSelected))
                {
                    return;
                }
                _isTmSelected = value;
                OnPropertyChanged();
            }
        }

        public bool FinishSelected
        {
            get
            {
                return _isFinishSelected;
            }
            set
            {
                if (Equals(value, _isFinishSelected))
                {
                    return;
                }
                _isFinishSelected = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecuteNext
        {
            get { return _canExecuteNext; }
            set
            {
                if (Equals(value, _canExecuteNext))
                {
                    return;
                }
                _canExecuteNext = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecuteBack
        {
            get { return _canExecuteBack; }
            set
            {
                if (Equals(value, _canExecuteBack))
                {
                    return;
                }

                _canExecuteBack = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecuteCreate
        {
            get { return _canExecuteCreate; }
            set
            {
                if (Equals(value, _canExecuteCreate))
                {
                    return;
                }
                _canExecuteCreate = value;
                OnPropertyChanged();
                
            }
            
        }

        public ICommand NextCommand
        {
            get { return _nextCommand ?? (_nextCommand = new CommandHandler(Next, true)); }
        }

        public ICommand BackCommand
        {
            get { return _backCommand ?? (_backCommand = new CommandHandler(Back, true)); }
            set
            {
                if (Equals(value, _backCommand))
                {
                    return;
                }
                _backCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateCommand
        {
            get { return _createCommand ?? (_createCommand = new CommandHandler(Create, true)); }
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

        public void Next()
        {
           
            //this is for navigation which include tm 
            //if (_packageDetails.FieldsAreCompleted() && DetailsSelected)
            //{
               
            //    TmSelected = true;
            //    DetailsSelected = false;
            //    CanExecuteBack = true;
            //    CanExecuteNext = true;



            //}
            //else if(TmSelected && !DetailsSelected)
            //{
                
                
            //    CanExecuteBack = true;
            //    CanExecuteNext = false;
            //    FinishSelected = true;
            //    TmSelected = false;
            //    CanExecuteCreate = true;
              
            //    _finishViewModel.Refresh();
               
            //}

            if (_packageDetails.FieldsAreCompleted() && DetailsSelected)
            {

                //TmSelected = true;
                DetailsSelected = false;
                TmSelected = false;
                FinishSelected = true;
                CanExecuteBack = true;
                CanExecuteNext = false;
                _finishViewModel.Refresh();
                CanExecuteCreate = true;

            }

        }

        public void Back()
        {
            //if (DetailsSelected)
            //{
            //    CanExecuteBack = false;
            //}else if (TmSelected)
            //{
            //    CanExecuteBack = false;
            //    TmSelected = false;
            //    DetailsSelected = true;
            //    CanExecuteNext = true;
            //    CanExecuteCreate = false;
            //}else if (FinishSelected)
            //{
            //    CanExecuteBack = true;
            //    TmSelected = true;
            //    CanExecuteNext = true;
            //    FinishSelected = false;
            //    CanExecuteCreate = false;
            //}

            if (DetailsSelected)
            {
                CanExecuteBack = false;
            }  else if (FinishSelected)
            {
                CanExecuteBack = false;
                DetailsSelected = true;
                TmSelected = false;
                CanExecuteNext = true;
                FinishSelected = false;
                CanExecuteCreate = false;
            }
        }

        public async void Create()
        {
            Active = true;
            await Task.Run(() => _projectService.CreateProject(_packageDetailsViewModel.GetPackageModel()));
            Active = false;
        }


        public StarTransitMainWindowViewModel(PackageDetailsViewModel packageDetailsViewModel,PackageDetails packageDetails,TranslationMemories translationMemories,
            FinishViewModel finishViewModel)
        {
            _packageDetailsViewModel = packageDetailsViewModel;
            _packageDetails = packageDetails;
            CanExecuteBack = false;
            CanExecuteCreate = false;
            CanExecuteNext = true;
            _isDetailsSelected = true;
            _isTmSelected = false;
            _isFinishSelected = false;
            _finishViewModel = finishViewModel;
            _projectService = new ProjectService();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
