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
    public class TranslationMemoriesViewModel: INotifyPropertyChanged
    {
        private List<LanguagePair> _languagePairs;
        private string _pair;
        private LanguagePair _selectedItem;
        private ICommand _browseCommand;
        private ICommand _createNewCommand;
        private string _title;
        private int _selectedIndex;

        public TranslationMemoriesViewModel(PackageDetailsViewModel packageDetailsViewModel)
        {
            var package = packageDetailsViewModel.GetPackageModel();
            var pairs = package.LanguagePairs;
            foreach (var pair in pairs)
            {
                pair.PairNameIso = pair.SourceLanguage.TwoLetterISOLanguageName + "-" +
                                pair.TargetLanguage.TwoLetterISOLanguageName;
                pair.PairName = pair.SourceLanguage.DisplayName + "-" + pair.TargetLanguage.DisplayName;
            }

            _selectedIndex = 0;
            LanguagePairs = pairs;

            _title = "Please select TM  for pair " +Environment.NewLine+ pairs[0].PairName;
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

        public ICommand BrowseCommand
        {
            get { return _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true)); }
        }

        public ICommand CreateNewCommand {
            get { return _createNewCommand ?? (_createNewCommand = new CommandHandler(Create, true)); }
        } 

        private void Create()
        {
           
        }

        private void Browse()
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

            }
         
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
    }
}
