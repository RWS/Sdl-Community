using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Sdl.Community.TuToTm.Commands;
using Sdl.Community.TuToTm.Helpers;
using Sdl.Community.TuToTm.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TuToTm.ViewModel
{
	public class MainWindowViewModel:BaseModel
	{
		private ObservableCollection<TmDetails> _tmCollection;
		private TmDetails _selectedTm;
		private SolidColorBrush _textMessageBrush;
		private readonly TmHelper _tmHelper;
		private ICommand _removeTmCommand;
		private ICommand _updateCommand;
		private ICommand _addCommand;
		private string _textMessage;
		private string _textMessageVisibility;
		private string _sourceText;
		private string _targetText;

		public MainWindowViewModel()
		{
			_tmCollection = new ObservableCollection<TmDetails>();
			_tmCollection.CollectionChanged += _tmCollection_CollectionChanged;
			_tmHelper = new TmHelper();
			_textMessageVisibility = "Collapsed";
			var tmsDetails = _tmHelper.LoadLocalUserTms();
			foreach (var tm in tmsDetails)
			{
				_tmCollection.Add(tm);
			}
		}

		private void _tmCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (TmDetails tm in e.OldItems)
				{
					tm.PropertyChanged -= Tm_PropertyChanged;
				}
			}
			if (e.NewItems == null) return;
			foreach (TmDetails tm in e.NewItems)
			{
				tm.PropertyChanged += Tm_PropertyChanged;
			}
		}
		private void Tm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				TextMessageVisibility = "Collapsed";
			}
		}

		public ObservableCollection<TmDetails> TmsCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged(nameof(TmsCollection));
			}
		}

		public TmDetails SelectedTm
		{
			get => _selectedTm;
			set
			{
				_selectedTm = value;
				OnPropertyChanged(nameof(SelectedTm));
			}
		}

		public string SourceText
		{
			get => _sourceText;
			set
			{
				if (_sourceText == value)
				{
					return;
				}
				_sourceText = value;
				TextMessageVisibility = "Collapsed";
				OnPropertyChanged(nameof(SourceText));
			}
		}
		public string TargetText
		{
			get => _targetText;
			set
			{
				if (_targetText == value)
				{
					return;
				}
				_targetText = value;
				TextMessageVisibility = "Collapsed";
				OnPropertyChanged(nameof(TargetText));
			}
		}
		public string TextMessage
		{
			get => _textMessage;
			set
			{
				if (_textMessage == value) { return;}
				_textMessage = value;
				OnPropertyChanged(nameof(TextMessage));
			}
		}
		public string TextMessageVisibility
		{
			get => _textMessageVisibility;
			set
			{
				if(_textMessageVisibility == value) { return;}
				_textMessageVisibility = value;
				OnPropertyChanged(nameof(TextMessageVisibility));
			}
		}
		public SolidColorBrush TextMessageBrush
		{
			get => _textMessageBrush;
			set
			{
				_textMessageBrush = value;
				OnPropertyChanged(nameof(TextMessageBrush));
			}
		}
		public ICommand RemoveTmCommand => _removeTmCommand ?? (_removeTmCommand = new CommandHandler(RemoveTm, true));
		public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new CommandHandler(UpdateTm, true));
		public ICommand AddCommand => _addCommand ?? (_addCommand = new CommandHandler(AddTm, true));

		private void AddTm()
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "SDL TM Files |*.sdltm",
				Multiselect = true
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var tmsPath = openFileDialog.FileNames;
				foreach (var tmPath in tmsPath)
				{
					var tmExists = TmsCollection.FirstOrDefault(t => t.TmPath.Equals(tmPath));
					if (tmExists == null)
					{
						var sdlTm = new FileBasedTranslationMemory(tmPath);
						var tmDetails = new TmDetails
						{
							TmPath = tmPath,
							Name = sdlTm.Name,
							SourceFlag = new Language(sdlTm.LanguageDirection.SourceLanguage.Name).GetFlagImage(),
							TargetFlag = new Language(sdlTm.LanguageDirection.TargetLanguage.Name).GetFlagImage(),
							FileBasedTranslationMemory = sdlTm
						};
						TmsCollection.Add(tmDetails);
					}
				}
			}
		}

		private void UpdateTm()
		{
			TextMessageVisibility = "Collapsed";
			var selectedTms = TmsCollection.Where(t => t.IsSelected).ToList();

			if (!AreTextFieldsCompleted())
			{
				ShowMessage(@"Please add source and target text", "#FF2121");
				return;
			}
			if (selectedTms.Any())
			{
				foreach (var selectedTm in selectedTms)
				{
					_tmHelper.AddTu(selectedTm, SourceText, TargetText);
				}
				ShowMessage(@"Selected TMs were successfully updated", "#00A8EB");
			}
			else
			{
				ShowMessage(@"Please select at least one TM", "#FF2121");
			}
		}

		private bool AreTextFieldsCompleted()
		{
			return !string.IsNullOrEmpty(SourceText) && !string.IsNullOrEmpty(TargetText);
		}

		private void RemoveTm()
		{
			if (SelectedTm != null)
			{
				var tmToRemove = TmsCollection.FirstOrDefault(t => t.TmPath.Equals(SelectedTm.TmPath));
				if (tmToRemove != null)
				{
					TmsCollection.Remove(tmToRemove);
				}
			}
		}
		private void ShowMessage(string message, string color)
		{
			TextMessage = message;
			TextMessageVisibility = "Visible";
			TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
		}
	}
}
