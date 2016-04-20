using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
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
        private PackageDetails _packageDetails;
        //private TranslationMemories _translationMemories;
        //private Finish _finish;
        //private System.Windows.Controls.ListViewItem _selectedItem;
        //private object _content;
        private bool _isDetailsSelected;
        private bool _isTmSelected;
        private bool _isFinishSelected;

        private PackageModel _package;

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

       
        //public object Content
        //{
        //    get { return _content; }
        //    set
        //    {
        //        if (Equals(value, _content))
        //        {
        //            return;
        //        }
        //        _content= value;
        //        OnPropertyChanged();
        //    }
        //}

        //public System.Windows.Controls.ListViewItem SelectedItem
        //{
        //    get
        //    {
        //        return _selectedItem;
        //    }
        //    set
        //    {
        //        if (Equals(value, _selectedItem))
        //        {
        //            return;
                    
        //        }
        //        _selectedItem = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public string TmItem { get; }

        //public string FinishItem { get; }
        //public string DescriptionItem { get;  }
        


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

        public void Next()
        {
            //var x = SelectedItem;
            if (_packageDetails.FieldsAreCompleted() && DetailsSelected)
            {
               
                TmSelected = true;
                DetailsSelected = false;
                CanExecuteBack = true;
                CanExecuteNext = true;



            }
            else if(TmSelected && !DetailsSelected)
            {
                CanExecuteBack = true;
                CanExecuteNext = false;
                FinishSelected = true;
                TmSelected = false;
                CanExecuteCreate = true;
            }
       
            
        }

        public void Back()
        {
            if (DetailsSelected)
            {
                CanExecuteBack = false;
            }else if (TmSelected)
            {
                CanExecuteBack = false;
                TmSelected = false;
                DetailsSelected = true;
                CanExecuteNext = true;
                CanExecuteCreate = false;
            }else if (FinishSelected)
            {
                CanExecuteBack = true;
                TmSelected = true;
                CanExecuteNext = true;
                FinishSelected = false;
                CanExecuteCreate = false;
            }
        }

        public void Create()
        {
            
        }






        public StarTransitMainWindowViewModel(PackageDetails packageDetails, TranslationMemories translationMemories, Finish finish)
        {
            _packageDetails = packageDetails;
            //_translationMemories = translationMemories;
            //_finish = finish;
            CanExecuteBack = false;
            CanExecuteCreate = false;
            CanExecuteNext = true;
            _isDetailsSelected = true;
            _isTmSelected = false;
            _isFinishSelected = false;
        }

            //public StarTransitMainWindowViewModel(PackageModel package)
            //{
            //    CanExecuteBack = false;
            //    CanExecuteCreate = false;
            //    CanExecuteNext = true;
            //    _isDetailsSelected = true;
            //    _isTmSelected = false;
            //    _isFinishSelected = false;
            //    _package = package;

            //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
