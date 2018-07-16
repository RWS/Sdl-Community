using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Community.StarTransit.UI.Annotations;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class FinishViewModel: INotifyPropertyChanged
    {
        private bool _active;
        private  string _location;
        private   string _txtName;
        private  string _txtDescription;
        private   string _sourceLanguage;
        private  ObservableCollection<string> _targetLanguage = new ObservableCollection<string>();
        private  string _templateName;
        private  string _customer;
        private  string _dueDate;
        private string _tmName;
        private string _tmPath;
        private readonly PackageDetailsViewModel _packageDetailsViewModel;
        private readonly TranslationMemoriesViewModel _translationMemoriesViewModel;
        private PackageModel _package; 


        public FinishViewModel(TranslationMemoriesViewModel translationMemoriesViewModel, PackageDetailsViewModel packageDetailsViewModel)
        {
            _translationMemoriesViewModel = translationMemoriesViewModel;
            _packageDetailsViewModel = packageDetailsViewModel;
            _package = _translationMemoriesViewModel.GetPackageModel();
            Refresh();
        }

        public void Refresh()
        {
            _package = _translationMemoriesViewModel.GetPackageModel();
            _targetLanguage.Clear();
            Name = _package.Name;
                Location = _package.Location;
                if (_package.HasDueDate)
                {
                    DueDate =
                        Helpers.TimeHelper.SetDateTime(_package.DueDate,
                            _packageDetailsViewModel.SelectedHour, _packageDetailsViewModel.SelectedMinute,
                            _packageDetailsViewModel.SelectedMoment).ToString();
                }

                if (_package.ProjectTemplate != null)
                {
                    TemplateName = _package.ProjectTemplate.Name; 
                }
                if (_package.Customer != null)
                {
                    Customer = _package.Customer.Name;
                    
                }

                Description = _packageDetailsViewModel.Description;
                SourceLanguage = _packageDetailsViewModel.SourceLanguage;
           

            foreach (var pair in _package.LanguagePairs)
            {
                var tmPair = string.Empty;
                if (pair.TmName == null)
                {
                    tmPair = "No TM selected for "+pair.PairName +" pair.";
                }
                else
                {
                    tmPair = pair.TargetLanguage.DisplayName +@",translation units will be imported "+Environment.NewLine+"into Translation Memory named: " +pair.TmName ;
                }
                
                _targetLanguage.Add(tmPair);
            }
            

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
            get { return _txtDescription; }
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

        public ObservableCollection<string>TargetLanguage
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
            get { return _templateName; }
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
            get { return _customer; }
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

        public string TmName
        {
            get { return _tmName; }
            set
            {
                if (Equals(value, _tmName)) { return;}
                _tmName = value;
                OnPropertyChanged();
            }
        }

        public string TmPath
        {
            get { return _tmPath; }
            set
            {
                if (Equals(_tmPath, value)) { return;}
                _tmPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
      
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
