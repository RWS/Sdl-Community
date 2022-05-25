using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Commands;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Versioning;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class TranslationMemoriesViewModel : BaseViewModel, IDataErrorInfo
	{
		private List<LanguagePair> _languagePairs;
		private ICommand _command;
		private string _title;
		private int _selectedIndex;
		private string _tmName;
		private bool _isEnabled;
		private PackageModel _package;
		private bool _browseChecked;
		private bool _createChecked;
		private ICommand _setBtnNameCommand;
		private string _visibility;
		private bool _isNoneChecked;
		private LanguagePair _selectedItem;
		private StarTranslationMemoryMetadata _tmMetadata;
		private readonly string _initialFolderPath;
		private string _isTmErrorMessageVisible;
		private bool _importMtChecked;
		private string _importMtVisible;
		private ICommand _importMtCommand;

		public TranslationMemoriesViewModel(PackageDetailsViewModel packageDetailsViewModel)
		{
			_package = packageDetailsViewModel.GetPackageModel();
			if(!(_package is null))
			{
				_package.MTMemories = new List<string>();
			}
			SetPackageLanguagePairs();

			_selectedIndex = 0;
			_visibility = "Collapsed";
			_isTmErrorMessageVisible = "Collapsed";
			_isNoneChecked = true;
			_title = $"Please select Translation memory for pair {LanguagePairs?[0]?.PairName}";
			_importMtVisible = "Collapsed";

			var studioVersion = new StudioVersionService().GetStudioVersion();
			_initialFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					studioVersion.StudioDocumentsFolderName, "Translation Memories");
		}

		public string TmMessage
		{
			get => _isTmErrorMessageVisible;
			set
			{
				if (Equals(value, _isTmErrorMessageVisible))
				{
					return;
				}
				_isTmErrorMessageVisible = value;
				OnPropertyChanged(nameof(TmMessage));
			}
		}
		public bool IsNoneChecked
		{
			get => _isNoneChecked;
			set
			{
				if (Equals(value, _isNoneChecked)) { return; }
				_isNoneChecked = value;
				OnPropertyChanged(nameof(IsNoneChecked));
			}
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				if (Equals(_selectedIndex, value))
				{
					return;
				}
				_selectedIndex = value;
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}

		public string Title
		{
			get => _title;
			set
			{
				if (Equals(value, _title))
				{
					return;
				}
				_title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (Equals(value, _isEnabled))
				{
					return;
				}
				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public string TmName
		{
			get => _tmName;
			set
			{
				if (Equals(value, _tmName))
				{
					return;
				}
				_tmName = value;
				OnPropertyChanged(nameof(TmName));
			}
		}

		public bool IsCreateChecked
		{
			get => _createChecked;
			set
			{
				if (Equals(value, _createChecked))
				{
					return;
				}
				_createChecked = value;
				OnPropertyChanged(nameof(IsCreateChecked));
			}
		}

		public string ImportMTVisible
		{
			get => _importMtVisible;
			set
			{
				if (Equals(value, _importMtVisible))
				{
					return;
				}
				_importMtVisible = value;
				OnPropertyChanged(nameof(ImportMTVisible));
			}
		}

		public bool IsImportMTChecked
		{
			get => _importMtChecked;
			set
			{
				if (Equals(value, _importMtChecked))
				{
					return;
				}
				_importMtChecked = value;
				OnPropertyChanged(nameof(IsImportMTChecked));
			}
		}

		public bool IsBrowseChecked
		{
			get => _browseChecked;
			set
			{
				if (Equals(value, _browseChecked))
				{
					return;
				}
				_browseChecked = value;
				OnPropertyChanged(nameof(IsBrowseChecked));
			}
		}

		public string Visibility
		{
			get => _visibility;
			set
			{
				if (Equals(value, _visibility))
				{
					return;
				}
				_visibility = value;
				OnPropertyChanged(nameof(Visibility));
			}
		}

		public List<LanguagePair> LanguagePairs
		{
			get => _languagePairs;
			set
			{
				_languagePairs = value;
				OnPropertyChanged(nameof(LanguagePairs));
			}
		}

		public LanguagePair SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (Equals(value, _selectedItem)) { return; }
				_selectedItem = value;

				OnPropertyChanged(nameof(SelectedItem));
				Title = SelectedItem.PairName;
				IsNoneChecked = true;
				SetBtnName();
			}
		}

		public ObservableCollection<LanguagePair> Pairs { get; set; }

		public string this[string columnName]
		{
			get
			{
				if (columnName.Equals("TmName"))
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

		public ICommand SetBtnNameCommand => _setBtnNameCommand ?? (_setBtnNameCommand = new CommandHandler(SetBtnName, true));

		public ICommand ImportMTCommand => _importMtCommand ?? (_importMtCommand = new CommandHandler(ImportMT, true));

		private void SetBtnName()
		{
			if (IsCreateChecked)
			{
				Visibility = "Collapsed";
				var tmName = $"{_package.Name}.{SelectedItem.PairNameIso}.sdltm";
				TmName = tmName;
				SelectedItem.TmName = TmName;
				SelectedItem.TmPath = Path.Combine(_initialFolderPath, tmName);
				SelectedItem.CreateNewTm = true;
				SelectedItem.HasTm = true;
				IsEnabled = true;
				TmMessage = "Collapsed";
				ImportMTVisible = "Visible";

				var tmPenaltiesWindow = new TranslationMemoriesPenaltiesWindow(new TranslationMemoriesPenaltiesViewModel(_package));
				tmPenaltiesWindow.ShowDialog();
			}
			if (IsBrowseChecked)
			{
				TmName = string.Empty;
				Visibility = "Visible";
				IsEnabled = false;
				TmMessage = "Collapsed";
				ImportMTVisible = "Collapsed";
				if (SelectedItem != null)
				{
					SelectedItem.CreateNewTm = false;
				}
			}
			if (IsNoneChecked)
			{
				Visibility = "Collapsed";
				TmName = string.Empty;
				IsEnabled = false;
				if (SelectedItem != null)
				{
					SelectedItem.TmName = null;
					SelectedItem.HasTm = false;
				}
				TmMessage = "Collapsed";
				ImportMTVisible = "Collapsed";
			}
		}

		private void ImportMT()
		{
			if (IsImportMTChecked)
			{
				if (SelectedItem?.StarTranslationMemoryMetadatas?.Count > 0)
				{
					foreach (var filePath in SelectedItem?.StarTranslationMemoryMetadatas)
					{
						if (Path.GetFileName(filePath?.TargetFile ?? "").Contains("_AEXTR_MT_"))
						{
							_package.MTMemories.Add(filePath.TargetFile);
						}
					}
				}
			}
			else
			{
				_package.MTMemories.Clear();
			}
		}

		public ICommand Command => _command ?? (_command = new CommandHandler(CommandBtn, true));

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
						TmMessage = "Collapsed";
					}
					else
					{
						TmMessage = "Visible";
					}
				}
			}
		}

		private void SetPackageLanguagePairs()
		{
			var pairs = _package?.LanguagePairs;
			if (pairs != null)
			{
				foreach (var pair in pairs)
				{
					pair.PairNameIso = $"{pair.SourceLanguage.TwoLetterISOLanguageName}-{pair.TargetLanguage.TwoLetterISOLanguageName}";
					pair.PairName = FormatPairName(pair.SourceLanguage.DisplayName, pair.TargetLanguage.DisplayName);
					pair.HasTm = false;
					IsNoneChecked = true;
				}
				LanguagePairs = pairs;
			}
		}

		private bool TmLanguageMatchesProjectLanguage()
		{
			if (SelectedItem?.SourceLanguage != null && SelectedItem?.TargetLanguage != null)
			{
				if (SelectedItem.SourceLanguage.Name.Equals(_tmMetadata.SourceLanguage) && SelectedItem.TargetLanguage.Name.Equals(_tmMetadata.TargetLanguage))
				{
					return true;
				}
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

			return $"{source} to {target}";
		}

		private string GetTmName(string path)
		{
			var name = path.Substring(path.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
			return name;
		}		
	}
}