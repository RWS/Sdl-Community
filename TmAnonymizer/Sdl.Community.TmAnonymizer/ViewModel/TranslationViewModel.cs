using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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
		private TmFile _selectedTm;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		private ICommand _selectAllCommand;
		private ICommand _previewCommand;
		private ICommand _removeRuleCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private readonly BackgroundWorker _backgroundWorker;
		private WaitWindow _waitWindow;
		private IList _selectedItems;

		public TranslationViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_selectedItems = new List<Rule>();
			_translationMemoryViewModel = translationMemoryViewModel;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
			_rules = SettingsMethods.GetRules();
			foreach (var rule in _rules)
			{
				rule.PropertyChanged += Rule_PropertyChanged;
			}
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
			_translationMemoryViewModel.PropertyChanged += _translationMemoryViewModel_PropertyChanged;
			RulesCollection.CollectionChanged += RulesCollection_CollectionChanged;
		}

		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}
		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_waitWindow.Close();

			//open preview window
			var previewWindow = new PreviewWindow();
			var previewViewModel = new PreviewWindowViewModel(SourceSearchResults, _anonymizeTranslationMemories,
				_tmsCollection, _translationMemoryViewModel);
			previewWindow.DataContext = previewViewModel;
			previewWindow.Closing += PreviewWindow_Closing;
			previewWindow.Show();
			
		}

		private void PreviewWindow_Closing(object sender, CancelEventArgs e)
		{
			SourceSearchResults.Clear();
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
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
			}
			else
			{
				MessageBox.Show(@"Please select at least one translation memory and a rule to preview the changes",
					"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			
		}

		private void _translationMemoryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("TmsCollection"))
			{
				//removed from tm collection
				RefreshPreviewWindow();
			}
		}
		private void RulesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					var rule = (Rule)item;
					rule.PropertyChanged += Rule_PropertyChanged;
				}
			}
		}

		private void Rule_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var settings = SettingsMethods.GetSettings();
			settings.Rules = RulesCollection;

			SettingsMethods.SaveSettings(settings);
		}
		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));
		public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new CommandHandler(PreviewChanges, true));

		public ICommand RemoveRuleCommand => _removeRuleCommand ??(_removeRuleCommand = new CommandHandler(RemoveRule, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var selectedRules = new List<Rule>();
				var fileDialog = new SaveFileDialog
				{
					Title = @"Export selected expressions",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (Rule rule in SelectedItems)
					{
						selectedRules.Add(rule);
					}
					Expressions.ExportExporessions(fileDialog.FileName, selectedRules);
					MessageBox.Show(@"File was exported successfully to selected location", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(@"Please select at least one row to export", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}


		private void Import()
		{
			var fileDialog = new OpenFileDialog
			{
				Title = @"Please select the files you want to import",
				Filter = @"Excel |*.xlsx",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "xlsx",
				Multiselect = true
			};
			var result = fileDialog.ShowDialog();
			if (result == DialogResult.OK && fileDialog.FileNames.Length > 0)
			{
				var importedExpressions = Expressions.GetImportedExpressions(fileDialog.FileNames.ToList());

				foreach (var expression in importedExpressions)
				{
					var ruleExist = RulesCollection.FirstOrDefault(s => s.Name.Equals(expression.Name));
					if (ruleExist == null)
					{
						RulesCollection.Add(expression);
					}
				}
				var settings = SettingsMethods.GetSettings();
				settings.Rules = RulesCollection;
				SettingsMethods.SaveSettings(settings);
			}
		}
		private void RemoveRule()
		{
			var message =MessageBox.Show(@"Are you sure you want to remove selected rules?",
				"", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (message == DialogResult.OK)
			{
				if (SelectedItems != null)
				{
					var selectedRules = new List<Rule>();

					foreach (Rule selectedItem in SelectedItems)
					{
						
						var rule = new Rule
						{
							Id = selectedItem.Id
						};
						selectedRules.Add(rule);
					}
					SelectedItems.Clear();
					foreach (var rule in selectedRules)
					{
						var ruleRoRemove = RulesCollection.FirstOrDefault(r => r.Id.Equals(rule.Id));
						if (ruleRoRemove != null)
						{
							RulesCollection.Remove(ruleRoRemove);
						}
					}
				}

				var settings = SettingsMethods.GetSettings();
				settings.Rules = RulesCollection;
				SettingsMethods.SaveSettings(settings);
			}
			
		}

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
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					newTm.PropertyChanged += NewTm_PropertyChanged;	
				}
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshPreviewWindow();
		}

		private void RefreshPreviewWindow()
		{
			var unselectedTms = _tmsCollection.Where(t => !t.IsSelected).ToList();
			foreach (var tm in unselectedTms)
			{
				var anonymizedTmToRemove = _anonymizeTranslationMemories.FirstOrDefault(t => t.TmPath.Equals(tm.Path));
				if (anonymizedTmToRemove != null)
				{
					_anonymizeTranslationMemories.Remove(anonymizedTmToRemove);
				}

				//remove search results for that tm
				var searchResultsForTm = SourceSearchResults.Where(r => r.TmFilePath.Equals(tm.Path)).ToList();
				foreach (var result in searchResultsForTm)
				{
					SourceSearchResults.Remove(result);
				}
			}
		}

		private void PreviewChanges()
		{
			_backgroundWorker.RunWorkerAsync();
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

		public TmFile SelectedTm
		{
			get => _selectedTm;
			set
			{
				_selectedTm = value;
				OnPropertyChanged(nameof(SelectedTm));
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
