using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.StarTransit.UI.Helpers;


namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class TranslationMemoriesViewModel: INotifyPropertyChanged,IDataErrorInfo
    {
        private List<LanguagePair> _languagePairs;
        private string _pair;
        private ICommand _command;
        private string _title;
        private int _selectedIndex;
        private string _tmName;
        private bool _isEnabled;
        private PackageModel _package;
        private bool _browseChecked;
        private bool _createChecked;
        private string _buttonName;
        private ICommand _setBtnNameCommand;
        private string _visibility;
        private bool _isNoneChecked;
        private LanguagePair _selectedItem;


        public TranslationMemoriesViewModel(PackageDetailsViewModel packageDetailsViewModel)
        {
             _package = packageDetailsViewModel.GetPackageModel();
            var pairs = _package.LanguagePairs;
            foreach (var pair in pairs)
            {
                pair.PairNameIso = pair.SourceLanguage.TwoLetterISOLanguageName + "-" +
                                pair.TargetLanguage.TwoLetterISOLanguageName;
                pair.PairName = FormatPairName(pair.SourceLanguage.DisplayName, pair.TargetLanguage.DisplayName);
            }

            _selectedIndex = 0;
            LanguagePairs = pairs;
            _buttonName = "Browse";
            _visibility = "Hidden";
            _isNoneChecked = true;
            _title = "Please select Translation memory  for pair " + pairs[0].PairName;
        }

        public bool IsNoneChecked
        {
            get { return _isNoneChecked; }
            set
            {
                if (Equals(value, _isNoneChecked)) { return;}
                _isNoneChecked = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (Equals(_selectedIndex, value))
                {
                    return;
                }
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (Equals(value, _title))
                {
                    return;
                }
                _title = value;
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

        public string TmName
        {
            get { return _tmName; }
            set
            {
                if (Equals(value, _tmName))
                {
                    return;
                }
                _tmName = value;
                OnPropertyChanged();
            }
        }

        public bool IsCreateChecked
        {
            get { return _createChecked; }
            set
            {
                if (Equals(value, _createChecked))
                {
                    return;
                }
                _createChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsBrowseChecked
        {
            get { return _browseChecked; }
            set
            {
                if (Equals(value, _browseChecked))
                {
                    return;
                }
                _browseChecked = value;
                OnPropertyChanged();
            }
        }

        public string Visibility
        {
            get { return _visibility; }
            set
            {
                if (Equals(value, _visibility))
                {
                    return;
                }
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public string ButtonName
        {
            get
            {
                return _buttonName;
                
            }
            set
            {
                if (Equals(value, _buttonName)) { return;}
                _buttonName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetBtnNameCommand {
            get { return _setBtnNameCommand ?? (_setBtnNameCommand = new CommandHandler(SetBtnName, true)); }
        }

        private void SetBtnName()
        {
            if (IsCreateChecked)
            {
                ButtonName = "Create Tm";
                TmName = string.Empty;
                Visibility = "Visible";
            }
            if (IsBrowseChecked)
            {
                ButtonName = "Browse";
                TmName = string.Empty;
                Visibility = "Visible";
            }
            if (IsNoneChecked)
            {
                Visibility = "Hidden";
                TmName = string.Empty;
            }
           
        }


        public ICommand Command
        {
            get { return _command ?? (_command = new CommandHandler(CommandBtn, true)); }
        }

        private void CommandBtn()
        {
            if (IsBrowseChecked)
            {
                var folderDialog = new OpenFileDialog();
                folderDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    @"Studio 2015\Translation Memories");
                folderDialog.Filter = @"Text Files (.sdltm)|*.sdltm";
                var result = folderDialog.ShowDialog();


                if (result == DialogResult.OK)
                {
                    var selectedTm = folderDialog.FileName;
                    SelectedItem.ChoseExistingTm = true;
                    SelectedItem.TmPath = selectedTm;
                    TmName = GetTmName(selectedTm);
                    IsEnabled = false;
                }
            }
            if (IsCreateChecked)
            {
                var tmName = _package.Name + SelectedItem.PairNameIso;
                TmName = tmName;
                IsEnabled = true;
            }
        }


        private string FormatPairName(string sourceLanguage, string targetLanguage)
        {
            var source = sourceLanguage.Substring(0, sourceLanguage.IndexOf("(", StringComparison.Ordinal));
            var target = targetLanguage.Substring(0, targetLanguage.IndexOf("(", StringComparison.Ordinal));

            return source + "to " + target;
        }

        private string GetTmName(string path)
        {
            var name = path.Substring(path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
            return name;
        }

        public List<LanguagePair> LanguagePairs
        {
            get { return _languagePairs; }
            set
            {
                if (Equals(value, _languagePairs))
                {
                    return;
                    
                }
                _languagePairs = value;
                OnPropertyChanged();
            }

        }

        public LanguagePair SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem)) { return;}
                _selectedItem = value;
                OnPropertyChanged();

            }
        }

        public ObservableCollection<LanguagePair> Pairs { get; set; }
 
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get {
                if (columnName == "TmName")
                {
                    if (IsCreateChecked && string.IsNullOrEmpty(TmName) ||
                        IsBrowseChecked && string.IsNullOrEmpty(TmName))
                    {
                        return "Translation memory is required.";
                    }
                }
                return null;
            }
        }

        public string Error { get; }
    }
}
