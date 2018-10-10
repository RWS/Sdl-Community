using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public sealed class ContentFilteringRulesViewModel : ViewModelBase, IDisposable
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private readonly ObservableCollection<AnonymizeTranslationMemory> _anonymizeTms;
		private readonly TranslationMemoryViewModel _model;
		private readonly BackgroundWorker _backgroundWorker;
		private readonly Settings _settings;
		private readonly SettingsService _settingsService;
		private TmFile _selectedTm;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		private ICommand _selectAllCommand;
		private ICommand _previewCommand;
		private ICommand _removeRuleCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private List<SourceSearchResult> _sourceSearchResults;
		private IList _selectedItems;

		public ContentFilteringRulesViewModel(TranslationMemoryViewModel model)
		{
			_model = model;

			_settingsService = _model.SettingsService;
			_settings = _settingsService.GetSettings();

			_anonymizeTms = new ObservableCollection<AnonymizeTranslationMemory>();

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

			_tmsCollection = _model.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			_model.PropertyChanged += ModelPropertyChanged;

			Rules.CollectionChanged += RulesCollection_CollectionChanged;

			UpdateCheckedAllState();
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));

		public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new CommandHandler(PreviewChanges, true));

		public ICommand RemoveRuleCommand => _removeRuleCommand ?? (_removeRuleCommand = new CommandHandler(RemoveRule, true));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		public IList SelectedItems
		{
			get => _selectedItems ?? (_selectedItems = new List<Rule>());
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public List<SourceSearchResult> SourceSearchResults
		{
			get => _sourceSearchResults ?? (_sourceSearchResults = new List<SourceSearchResult>());
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

		public ObservableCollection<Rule> Rules
		{
			get
			{
				if (_rules == null)
				{
					_rules = new ObservableCollection<Rule>(_settingsService.GetRules());
					foreach (var rule in _rules)
					{
						rule.PropertyChanged += Rule_PropertyChanged;
					}
				}

				return _rules;
			}
			set
			{
				if (Equals(value, _rules))
				{
					return;
				}

				if (_rules != null)
				{
					foreach (var rule in _rules)
					{
						rule.PropertyChanged -= Rule_PropertyChanged;
					}
				}

				_rules = value;

				if (_rules != null)
				{
					foreach (var rule in _rules)
					{
						rule.PropertyChanged += Rule_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(Rules));
			}
		}

		public Rule SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
				if (Rules.Any(r => r.Id == null))
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

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var selectedRules = new List<Rule>();
				var fileDialog = new SaveFileDialog
				{
					Title = StringResources.Export_Export_selected_expressions,
					Filter = @"Excel |*.xlsx"
				};

				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (var rule in SelectedItems.OfType<Rule>())
					{
						selectedRules.Add(rule);
					}

					Expressions.ExportExporessions(fileDialog.FileName, selectedRules);
					MessageBox.Show(StringResources.Export_File_was_exported_successfully_to_selected_location, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(StringResources.Export_Please_select_at_least_one_row_to_export, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Import()
		{
			var fileDialog = new OpenFileDialog
			{
				Title = StringResources.Import_Please_select_the_files_you_want_to_import,
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
					var ruleExist = Rules.FirstOrDefault(s => s.Name.Equals(expression.Name));
					if (ruleExist == null)
					{
						Rules.Add(expression);
					}
				}

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void RemoveRule()
		{
			var message = MessageBox.Show(StringResources.RemoveRule_Are_you_sure_you_want_to_remove_selected_rules,
				Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (message == DialogResult.OK)
			{
				if (SelectedItems != null)
				{
					var selectedRules = new List<Rule>();

					foreach (var selectedItem in SelectedItems)
					{
						if (!selectedItem.GetType().Name.Equals("NamedObject"))
						{
							var item = (Rule)selectedItem;
							var rule = new Rule
							{
								Id = item.Id
							};
							selectedRules.Add(rule);
						}

					}
					SelectedItems.Clear();
					foreach (var rule in selectedRules)
					{
						var ruleRoRemove = Rules.FirstOrDefault(r => r.Id.Equals(rule.Id));
						if (ruleRoRemove != null)
						{
							Rules.Remove(ruleRoRemove);
						}
					}
				}

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// check for error
			if (e.Error != null)
			{
				SourceSearchResults.Clear();
				MessageBox.Show(e.Error.Message, Application.ProductName);
			}
			else
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					var previewViewModel = new PreviewWindowViewModel(SourceSearchResults, _anonymizeTms,
						_tmsCollection, _model);

					var previewWindow = new PreviewWindow(previewViewModel);
					previewWindow.Closing += PreviewWindow_Closing;
					previewWindow.ShowDialog();
				});
			}
		}


		private void PreviewWindow_Closing(object sender, CancelEventArgs e)
		{
			SourceSearchResults.Clear();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				var settings = new ProgressDialogSettings(_model.ControlParent, true, true, false);
				var result = ProgressDialog.Execute(StringResources.Loading_data, () =>
				{
					ProcessData(ProgressDialog.Current);
				}, settings);


				if (result.Cancelled)
				{
					throw new Exception(StringResources.Process_cancelled_by_user);
				}
				if (result.OperationFailed)
				{
					throw new Exception(StringResources.Process_failed + "\r\n\r\n" + result.Error.Message);
				}
			});
		}


		private void ProcessData(ProgressDialogContext context)
		{
			var selectedTms = _tmsCollection.Where(t => t.IsSelected).ToList();
			var selectedRulesCount = Rules.Count(r => r.IsSelected);
			if (selectedTms.Count > 0 && selectedRulesCount > 0)
			{
				context.Report("Recovering content parsing rules...");

				var personalDataParsingService = new PersonalDataParsingService(GetSelectedRules());

				var serverTms = selectedTms.Where(s => s.IsServerTm).ToList();
				if (serverTms.Any())
				{
					var providers = new List<TranslationProviderServer>();

					foreach (var serverTm in serverTms)
					{
						if (context.CheckCancellationPending())
						{
							break;
						}

						context.Report("Connecting to: " + serverTm.Name);

						var uri = new Uri(serverTm.Credentials.Url);
						if (providers.Count(a => a.Uri == uri) == 0)
						{
							providers.Add(new TranslationProviderServer(uri, false,
								serverTm.Credentials.UserName,
								serverTm.Credentials.Password));
						}
					}

					//get all tus for selected translation memories
					foreach (var tm in serverTms)
					{
						if (context.CheckCancellationPending())
						{
							break;
						}

						context.Report(tm.Name);

						var translationProvider = providers.FirstOrDefault(a => a.Uri == new Uri(tm.Credentials.Url));
						if (translationProvider == null)
						{
							return;
						}

						var anonymizeTm = _model.TmService.ServerBasedTmGetTranslationUnits(context,
							tm, translationProvider,
							personalDataParsingService, out var searchResults);

						SourceSearchResults.AddRange(searchResults);

						if (!_anonymizeTms.Any(n => n.TmPath.Equals(anonymizeTm.TmPath)))
						{
							_anonymizeTms.Add(anonymizeTm);
						}
					}
				}

				//file based tms
				foreach (var tm in selectedTms.Where(s => !s.IsServerTm))
				{
					if (context.CheckCancellationPending())
					{
						break;
					}

					context.Report(tm.Name);

					var anonymizeTm = _model.TmService.FileBaseTmGetTranslationUnits(context,
					tm, personalDataParsingService, out var searchResults);

					SourceSearchResults.AddRange(searchResults);

					if (!_anonymizeTms.Any(n => n.TmPath.Equals(anonymizeTm.TmPath)))
					{
						_anonymizeTms.Add(anonymizeTm);
					}
				}
			}
			else
			{
				throw new Exception(StringResources.Please_select_at_least_one_translation_memory_and_a_rule_to_preview_the_changes);
			}
		}

		private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(nameof(TranslationMemoryViewModel.TmsCollection)))
			{
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
			_settings.Rules = Rules.ToList();
			_settingsService.SaveSettings(_settings);

			UpdateCheckedAllState();
		}

		private void UpdateCheckedAllState()
		{
			if (Rules.Count > 0)
			{
				SelectAll = Rules.Count(a => !a.IsSelected) <= 0;
			}
			else
			{
				SelectAll = false;
			}
		}

		private void TmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null) return;
				foreach (TmFile removedTm in e.OldItems)
				{
					//Remove search resoults for deleted tm
					var tusForRemovedTm = SourceSearchResults.Where(t => t.TmFilePath.Equals(removedTm.Path)).ToList();
					foreach (var tu in tusForRemovedTm)
					{
						SourceSearchResults.Remove(tu);
					}

					//remove the tm from the list use in preview windoew
					var removed = _anonymizeTms.FirstOrDefault(t => t.TmPath.Equals(removedTm.Path));
					if (removed != null)
					{
						_anonymizeTms.Remove(removed);
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
				var anonymizedTmToRemove = _anonymizeTms.FirstOrDefault(t => t.TmPath.Equals(tm.Path));
				if (anonymizedTmToRemove != null)
				{
					_anonymizeTms.Remove(anonymizedTmToRemove);
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
			return Rules.Where(r => r.IsSelected).ToList();
		}

		private void SelectAllRules()
		{
			var value = SelectAll;
			foreach (var rule in Rules)
			{
				rule.IsSelected = value;
			}
		}

		private void SetIdForNewRules()
		{
			var newRules = Rules.Where(r => r.Id == null).ToList();
			foreach (var rule in newRules)
			{
				rule.Id = Guid.NewGuid().ToString();
			}
		}

		public void Dispose()
		{
			_backgroundWorker.DoWork -= BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;
			_backgroundWorker?.Dispose();

			_tmsCollection.CollectionChanged -= TmsCollection_CollectionChanged;
			_model.PropertyChanged -= ModelPropertyChanged;

			foreach (var rule in _rules)
			{
				rule.PropertyChanged -= Rule_PropertyChanged;
			}

			Rules.CollectionChanged -= RulesCollection_CollectionChanged;
		}
	}
}
