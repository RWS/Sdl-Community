using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
        private  readonly PackageDetailsViewModel _packageDetailsViewModel;
        private readonly PackageDetails _packageDetails;
       private bool _isDetailsSelected;
        private bool _isTmSelected;
        private bool _isFinishSelected;
        private readonly FinishViewModel _finishViewModel;
        private PackageModel _package;
        private readonly ProjectService _projectService;
        private bool _active;
        private bool _isEnabled;
        private string _color;
        private bool _hasTm;
        private TranslationMemories _translationMemories;
        private TranslationMemoriesViewModel _translationMemoriesViewModel;
        private MetroWindow _window;
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

        public string Color
        {
            get { return _color; }
            set
            {
                if (Equals(value, _color))
                {
                    return;
                }
                _color = value;
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

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (Equals(value, _isEnabled))
                {
                    return;
                    
                }
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public Action CloseAction { get; set; }

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

        /// <summary>
        /// Check to see if the folder is empty, in case the user just paste the path in text box
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private async Task<bool> IsEmptyFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return false;
            
            bool isEmpty = !Directory
                .GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Any();
            var hasSubdirectories = Directory.GetDirectories(folderPath);
           
            if (hasSubdirectories.Count() != 0 || !isEmpty)
            {
                var dialog = new MetroDialogSettings
                {
                    AffirmativeButtonText = "OK"

                };
                MessageDialogResult result =
                    await _window.ShowMessageAsync("Folder not empty!", "Please select an empty folder",
                        MessageDialogStyle.Affirmative, dialog);
                return false;
            }
            return true;
        }

        public async void Next()
        {
            var model = _packageDetailsViewModel.GetPackageModel();
             _hasTm = false;
            var isEmpty = await IsEmptyFolder(_packageDetailsViewModel.TextLocation);

            if (isEmpty)
            {
                foreach (var pair in model.LanguagePairs)
                {
                    if (pair.StarTranslationMemoryMetadatas.Count != 0)
                    {
                        _hasTm = true;
                    }
                }//tm page is disabled
                if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm == false)
                {

                    DetailsSelected = false;
                    TmSelected = false;
                    FinishSelected = true;
                    CanExecuteBack = true;
                    CanExecuteNext = false;
                    _finishViewModel.Refresh();
                    CanExecuteCreate = true;
                    IsEnabled = false;
                    Color = "Gray";
                }//tm page
                else if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm)
                {
                    DetailsSelected = false;
                    TmSelected = true;
                    FinishSelected = false;
                    CanExecuteBack = true;
                    CanExecuteNext = true;
                    CanExecuteCreate = false;
                    IsEnabled = true;
                    Color = "#FF66290B";
                }//finish page
                else if (_packageDetails.FieldsAreCompleted() && TmSelected && _translationMemories.TmFieldIsCompleted())
                {
                    DetailsSelected = false;
                    CanExecuteNext = false;
                    CanExecuteCreate = true;
                    CanExecuteBack = true;
                    TmSelected = false;
                    IsEnabled = true;
                    FinishSelected = true;
                    _finishViewModel.Refresh();
                    Color = "#FFB69476";
                }
            }
            

        }

        public void Back()
        {
            if (DetailsSelected)
            {
                CanExecuteBack = false;
            }
            else if (FinishSelected && _hasTm==false)
            {
                CanExecuteBack = false;
                DetailsSelected = true;
                TmSelected = false;
                CanExecuteNext = true;
                FinishSelected = false;
                CanExecuteCreate = false;
            }else if (FinishSelected && _hasTm)
            {
                DetailsSelected = false;
                TmSelected = true;
                FinishSelected = false;
                CanExecuteBack = true;
                CanExecuteCreate = false;
                CanExecuteNext = true;
                Color = "#FF66290B";
            }
            else if (TmSelected)
            {
                DetailsSelected = true;
                TmSelected = false;
                FinishSelected = false;
                CanExecuteBack = false;
                CanExecuteCreate = false;
                CanExecuteNext = true;
                Color = "#FFB69476";
            }
        }

        public async void Create()
        {
            Active = true;
            var packageModel = _translationMemoriesViewModel.GetPackageModel();

            var isEmpty = await IsEmptyFolder(packageModel.Location);

            if (isEmpty)
            {
                await Task.Run(() => _projectService.CreateProject(packageModel));
            }
            Active = false;
            CloseAction();
        }


        public StarTransitMainWindowViewModel(PackageDetailsViewModel packageDetailsViewModel,PackageDetails packageDetails,TranslationMemories translationMemories,TranslationMemoriesViewModel translationMeloriesMemoriesViewModel,
            FinishViewModel finishViewModel,MetroWindow window)
        {
            _packageDetailsViewModel = packageDetailsViewModel;
            _packageDetails = packageDetails;
            _translationMemories = translationMemories;
            _translationMemoriesViewModel = translationMeloriesMemoriesViewModel;
            CanExecuteBack = false;
            CanExecuteCreate = false;
            CanExecuteNext = true;
            _isDetailsSelected = true;
            _isTmSelected = false;
            _isFinishSelected = false;
            _finishViewModel = finishViewModel;
            _projectService = new ProjectService();
            Color = "#FFB69476";
            _window = window;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
