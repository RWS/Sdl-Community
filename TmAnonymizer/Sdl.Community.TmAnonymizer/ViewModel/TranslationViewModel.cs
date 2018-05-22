using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationViewModel : ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		private ICommand _selectAllCommand;
		private ICommand _previewCommand;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private readonly TranslationMemoryViewModel _translationMemoryViewModel;

		public TranslationViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_translationMemoryViewModel = translationMemoryViewModel;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
			_rules = Constants.GetDefaultRules();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));
		public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new CommandHandler(PreviewChanges, true));

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null) return;
				foreach (TmFile removedTm in e.OldItems)
				{
					var tusForRemovedTm = SourceSearchResults.Where(t => t.TmFilePath.Equals(removedTm.Path)).ToList();
					foreach (var tu in tusForRemovedTm)
					{
						SourceSearchResults.Remove(tu);
					}
				}
			}
		}

		private void PreviewChanges()
		{
			var selectedTms = _tmsCollection.Where(t => t.IsSelected).ToList();
			var selectedRulesCount = RulesCollection.Count(r => r.IsSelected);
			if (selectedTms.Count > 0 && selectedRulesCount > 0)
			{
				var serverTms = selectedTms.Where(s => s.IsServerTm).ToList();
				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					//get all tus for selected translation memories
					foreach (var serverTm in serverTms)
					{
						var tus = Tm.ServerBasedTmGetTranslationUnits(translationProvider, serverTm.Path,
							SourceSearchResults, GetSelectedRules());
						if (!_anonymizeTranslationMemories.Exists(n => n.TmPath.Equals(tus.TmPath)))
						{
							_anonymizeTranslationMemories.Add(tus);
						}
					}
				}
			
				//file based tms
				foreach (var tm in selectedTms.Where(s => !s.IsServerTm))
				{
					var tus = Tm.FileBaseTmGetTranslationUnits(tm.Path, SourceSearchResults, GetSelectedRules());
					if (!_anonymizeTranslationMemories.Exists(n => n.TmPath.Equals(tus.TmPath)))
					{
						_anonymizeTranslationMemories.Add(tus);
					}
				}
				var previewWindow = new PreviewWindow();
				var previewViewModel = new PreviewWindowViewModel(SourceSearchResults, _anonymizeTranslationMemories,
					_tmsCollection, _translationMemoryViewModel);
				previewWindow.DataContext = previewViewModel;
				previewWindow.Show();
			}
			else
			{
				MessageBox.Show(@"Please select at least one translation memory and a rule to preview the changes",
					"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

		}

		private List<Rule> GetSelectedRules()
		{
			return RulesCollection.Where(r => r.IsSelected).ToList();
		}
		private void SelectAllRules()
		{
			foreach (var rule in RulesCollection)
			{
				rule.IsSelected = SelectAll;
			}
		}
		public bool SelectAll
		{
			get => _selectAll;

			set
			{
				if (Equals(value, _selectAll))
				{
					return;
				}
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}
		public ObservableCollection<SourceSearchResult> SourceSearchResults
		{
			get => _sourceSearchResults;

			set
			{
				if (Equals(value, _sourceSearchResults))
				{
					return;
				}
				_sourceSearchResults = value;
				OnPropertyChanged(nameof(SourceSearchResults));
			}
		}

		public ObservableCollection<Rule> RulesCollection
		{
			get => _rules;

			set
			{
				if (Equals(value, _rules))
				{
					return;
				}
				_rules = value;
				OnPropertyChanged(nameof(RulesCollection));
			}
		}

		public Rule SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
				if (RulesCollection.Any(r => r.Id == null))
				{
					SetIdForNewRules();
				}
			}
		}

		private void SetIdForNewRules()
		{
			var newRules = RulesCollection.Where(r => r.Id == null).ToList();
			foreach (var rule in newRules)
			{
				rule.Id = Guid.NewGuid().ToString();
			}
		}
	}
}
