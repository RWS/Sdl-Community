using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.Toolkit.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

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
		private StarTranslationMemoryMetadata _tmMetadata;
        private  readonly string _initialFolderPath;
		private string _isTmErrorMessageVisible;
		private bool _importMTChecked = false;
		private string _importMTVisible;
		private ICommand _importMTCommand;

		public TranslationMemoriesViewModel(PackageDetailsViewModel packageDetailsViewModel)
        {
             _package = packageDetailsViewModel.GetPackageModel();
            var pairs = _package.LanguagePairs;
            foreach (var pair in pairs)
            {
                pair.PairNameIso = pair.SourceLanguage.TwoLetterISOLanguageName + "-" +
                                pair.TargetLanguage.TwoLetterISOLanguageName;
                pair.PairName = FormatPairName(pair.SourceLanguage.DisplayName, pair.TargetLanguage.DisplayName);
                pair.HasTm = false;
                IsNoneChecked = true;
            }

            _selectedIndex = 0;
            LanguagePairs = pairs;
            _buttonName = "Browse";
            _visibility = "Hidden";
			_isTmErrorMessageVisible = "Hidden";
            _isNoneChecked = true;
			_title = $"Please select Translation memory for pair {pairs[0].PairName}";
			_importMTVisible = "Hidden";

			var studioVersion = new Studio().GetStudioVersion();
            _initialFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    studioVersion.PublicVersion.Replace("SDL","").Trim(),
                    "Translation Memories");
        }

		public string TmMessage
		{
			get
			{
				return _isTmErrorMessageVisible;
			}
			set
			{
				if (Equals(value, _isTmErrorMessageVisible))
				{
					return;
				}
				_isTmErrorMessageVisible = value;
				OnPropertyChanged();
			}
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

		public string ImportMTVisible
		{
			get { return _importMTVisible; }
			set
			{
				if (Equals(value, _importMTVisible))
				{
					return;
				}
				_importMTVisible = value;
				OnPropertyChanged();
			}
		}

		public bool IsImportMTChecked
		{
			get { return _importMTChecked; }
			set
			{
				if (Equals(value, _importMTChecked))
				{
					return;
				}
				_importMTChecked = value;
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

		public ICommand ImportMTCommand
		{
			get { return _importMTCommand ?? (_importMTCommand = new CommandHandler(ImportMT, true)); }
		}

		private void SetBtnName()
        {
           
            if (IsCreateChecked)
            {
                Visibility = "Hidden";
                var tmName = string.Format("{0}.{1}.{2}",
                    _package.Name,
                    SelectedItem.PairNameIso,
                    "sdltm");
                TmName = tmName;
                SelectedItem.TmName = TmName;
                SelectedItem.TmPath = Path.Combine(_initialFolderPath, tmName);
                SelectedItem.CreateNewTm = true;
                SelectedItem.HasTm = true;
                IsEnabled = true;
				TmMessage = "Hidden";
				ImportMTVisible = "Visible";

				var tmPenaltiesWindow = new TranslationMemoriesPenaltiesWindow(new TranslationMemoriesPenaltiesViewModel(_package));
				tmPenaltiesWindow.Show();
			}
			if (IsBrowseChecked)
            {
                ButtonName = "Browse";
                TmName = string.Empty;
                Visibility = "Visible";
                IsEnabled = false;
				TmMessage = "Hidden";
				ImportMTVisible = "Hidden";
			}
            if (IsNoneChecked)
            {
                Visibility = "Hidden";
                TmName = string.Empty;
                IsEnabled = false;
                if (SelectedItem != null)
                {
                    SelectedItem.TmName = null;
                    SelectedItem.HasTm = false;
                }
				TmMessage = "Hidden";
				ImportMTVisible = "Hidden";
			}
           
        }

		private void ImportMT()
		{
			if (IsImportMTChecked)
			{
				_package.MTMemories = new List<string>();

				foreach (var filePath in SelectedItem.StarTranslationMemoryMetadatas)
				{
					if (Path.GetFileName(filePath.TargetFile ?? "").Contains("_AEXTR_MT_"))
						_package.MTMemories.Add(filePath.TargetFile);
				}
			}
			else
				_package.MTMemories.Clear();
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

                folderDialog.InitialDirectory = _initialFolderPath;
                folderDialog.Filter = @"Text Files (.sdltm)|*.sdltm";

                var result = folderDialog.ShowDialog();
				

				if (result == DialogResult.OK)
                {
                    var selectedTm = folderDialog.FileName;
					_tmMetadata = GetTmLanguageDirection(selectedTm);
					if (TmLanguageMatchesProjectLanguage())
					{
						SelectedItem.ChoseExistingTm = true;
						SelectedItem.TmPath = selectedTm;
						TmName = GetTmName(selectedTm);
						SelectedItem.TmName = TmName;
						SelectedItem.HasTm = true;
						IsEnabled = false;
						TmMessage = "Hidden";
					}
					else
					{
						TmMessage = "Visible";
					}
					
					
					
				}
            }
      
        }

		private bool TmLanguageMatchesProjectLanguage()
		{
			if(SelectedItem.SourceLanguage.Name.Equals(_tmMetadata.SourceLanguage)&&
				SelectedItem.TargetLanguage.Name.Equals(_tmMetadata.TargetLanguage))
			{
				return true;
			}
			return false;
		}
		private StarTranslationMemoryMetadata GetTmLanguageDirection(string tmPath)
		{
			var tmInfo = new FileBasedTranslationMemory(tmPath);
			var tmLanguageDirection = tmInfo.LanguageDirection;
			var tmMetadata = new StarTranslationMemoryMetadata
			{
				SourceLanguage = tmLanguageDirection.SourceLanguage.Name,
				TargetLanguage = tmLanguageDirection.TargetLanguage.Name
			};
			return tmMetadata;
		}
		public PackageModel GetPackageModel()
        {
            _package.LanguagePairs = LanguagePairs;
            
            return _package;
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
                Title = SelectedItem.PairName;
                IsNoneChecked = true;
                SetBtnName();
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
