using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
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
		private readonly Settings _settings;
		private readonly SettingsService _settingsService;
		private readonly ExcelImportExportService _excelImportExportService;
		private TmFile _selectedTm;
		private Rule _newRule;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		private List<ContentSearchResult> _sourceSearchResults;
		private IList _selectedItems;
		private bool _newRuleIsVisible;
		private ICommand _selectAllCommand;
		private ICommand _previewCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private ICommand _removeRuleCommand;
		private ICommand _createRuleCommand;
		private ICommand _cancelRuleCommand;
		private ICommand _addRuleCommand;
		private ICommand _moveRuleUpCommand;
		private ICommand _moveRuleDownCommand;

		public ContentFilteringRulesViewModel(TranslationMemoryViewModel model, ExcelImportExportService excelImportExportService)
		{
			_model = model;
			_excelImportExportService = excelImportExportService;

			_settingsService = _model.SettingsService;
			_settings = _settingsService.GetSettings();

			_anonymizeTms = new ObservableCollection<AnonymizeTranslationMemory>();

			_tmsCollection = _model.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			_model.PropertyChanged += ModelPropertyChanged;

			NewRuleIsVisible = false;
			UpdateCheckedAllState();
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));

		public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new CommandHandler(PreviewChanges, true));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		public ICommand CreateRuleCommand => _createRuleCommand ?? (_createRuleCommand = new CommandHandler(CreateRule, true));

		public ICommand AddRuleCommand => _addRuleCommand ?? (_addRuleCommand = new CommandHandler(AddRule, true));

		public ICommand RemoveRuleCommand => _removeRuleCommand ?? (_removeRuleCommand = new CommandHandler(RemoveRule, true));

		public ICommand CancelRuleCommand => _cancelRuleCommand ?? (_cancelRuleCommand = new CommandHandler(CancelRule, true));

		public ICommand MoveRuleUpCommand => _moveRuleUpCommand ?? (_moveRuleUpCommand = new CommandHandler(MoveRuleUp, true));

		public ICommand MoveRuleDownCommand => _moveRuleDownCommand ?? (_moveRuleDownCommand = new CommandHandler(MoveRuleDown, true));

		public bool NewRuleIsVisible
		{
			get => _newRuleIsVisible;
			set
			{
				_newRuleIsVisible = value;
				OnPropertyChanged(nameof(NewRuleIsVisible));
			}
		}

		public Rule NewRule
		{
			get => _newRule ?? (_newRule = new Rule());
			set
			{
				_newRule = value;
				OnPropertyChanged(nameof(NewRule));
			}
		}

		public IList SelectedItems
		{
			get => _selectedItems ?? (_selectedItems = new List<Rule>());
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public List<ContentSearchResult> SourceSearchResults
		{
			get => _sourceSearchResults ?? (_sourceSearchResults = new List<ContentSearchResult>());
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
					InitializeRules();
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

		private void InitializeRules()
		{
			var rules = _settingsService.GetRules();

			UpdateRuleOrder(rules);

			if (_rules != null)
			{
				foreach (var rule in _rules)
				{
					rule.PropertyChanged -= Rule_PropertyChanged;
				}
			}

			_rules = new ObservableCollection<Rule>(rules.OrderBy(a => a.Order));

			foreach (var rule in _rules)
			{
				rule.PropertyChanged += Rule_PropertyChanged;
			}
		}

		private static void UpdateRuleOrder(IReadOnlyCollection<Rule> rules)
		{
			var orders = new List<int>();
			foreach (var rule in rules.OrderBy(a => a.Order))
			{
				if (!orders.Contains(rule.Order))
				{
					orders.Add(rule.Order);
				}
				else
				{
					var i = 0;
					while (orders.Contains(i))
					{
						i++;
					}

					rule.Order = i;
					orders.Add(i);
				}
			}

			var orderIndex = 0;
			foreach (var rule in rules.OrderBy(a => a.Order))
			{
				rule.Order = orderIndex;
				orderIndex++;
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

					if (!fileDialog.FileName.ToLower().EndsWith(".xlsx"))
					{
						fileDialog.FileName += ".xlsx";
					}

					_excelImportExportService.ExportRules(fileDialog.FileName, selectedRules);

					MessageBox.Show(StringResources.Export_File_was_exported_successfully_to_selected_location, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

					if (SelectedItem != null && File.Exists(fileDialog.FileName))
					{
						System.Diagnostics.Process.Start("\"" + fileDialog.FileName + "\"");
					}
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
				var importedRules = _excelImportExportService.ImportedRules(fileDialog.FileNames.ToList());

				if (importedRules == null)
				{
					return;
				}

				var rules = new List<Rule>();
				foreach (var rule in Rules)
				{
					rules.Add(rule.Clone() as Rule);
				}

				foreach (var rule in importedRules)
				{
					var existingRule = rules.FirstOrDefault(s => s.Id.Equals(rule.Id, StringComparison.InvariantCultureIgnoreCase) || s.Name.Equals(rule.Name, StringComparison.InvariantCultureIgnoreCase));
					if (existingRule == null)
					{
						rules.Add(new Rule
						{
							Name = rule.Name,
							Description = rule.Description,
							Order = rules.Count
						});
					}
					else
					{
						existingRule.Name = rule.Name;
						existingRule.Description = rule.Description;
					}
				}
				UpdateRuleOrder(rules);

				Rules = new ObservableCollection<Rule>(rules.OrderBy(a => a.Order));

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void RemoveRule()
		{
			var message = MessageBox.Show(StringResources.RemoveRule_Are_you_sure_you_want_to_remove_selected_rules,
				Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (message == DialogResult.OK && SelectedItems != null)
			{
				var selectedRuleIds = new List<string>();
				foreach (var selectedItem in SelectedItems)
				{
					if (selectedItem is Rule rule)
					{
						selectedRuleIds.Add(rule.Id);
					}
				}

				var rules = new List<Rule>();
				foreach (var rule in Rules)
				{
					rules.Add(rule.Clone() as Rule);
				}

				SelectedItems.Clear();

				foreach (var id in selectedRuleIds)
				{
					var ruleRoRemove = rules.FirstOrDefault(r => r.Id.Equals(id));
					if (ruleRoRemove != null)
					{
						rules.Remove(ruleRoRemove);
					}
				}

				UpdateRuleOrder(rules);

				Rules = new ObservableCollection<Rule>(rules.OrderBy(a => a.Order));

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void CancelRule()
		{
			NewRuleIsVisible = false;
			NewRule = new Rule();
		}

		private void AddRule()
		{
			if (string.IsNullOrEmpty(NewRule.Name))
			{
				return;
			}
			var rules = new List<Rule>();
			foreach (var rule in Rules)
			{
				rules.Add(rule.Clone() as Rule);
			}

			var existingRule = rules.FirstOrDefault(s => s.Name.Equals(NewRule.Name, StringComparison.InvariantCultureIgnoreCase));
			if (existingRule == null)
			{
				var newRule = new Rule
				{
					Name = NewRule.Name,
					Description = NewRule.Description,
					IsSelected = true,
					Order = Rules.Count
				};

				rules.Add(newRule);

				UpdateRuleOrder(rules);

				Rules = new ObservableCollection<Rule>(rules.OrderBy(a => a.Order));
			}

			NewRuleIsVisible = false;
			NewRule = new Rule();

			_settings.Rules = Rules.ToList();
			_settingsService.SaveSettings(_settings);
		}

		private void CreateRule()
		{
			NewRule = new Rule();
			NewRuleIsVisible = true;
		}

		private void MoveRuleUp()
		{
			var selectedRule = SelectedItem;
			var selectedIndex = Rules.IndexOf(SelectedItem);

			if (selectedIndex > 0)
			{
				var previousRule = Rules[selectedIndex - 1];
				previousRule.Order = selectedIndex;

				selectedRule.Order = selectedIndex - 1;

				Rules.Move(selectedIndex, selectedIndex - 1);

				OnPropertyChanged(nameof(Rules));

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void MoveRuleDown()
		{
			var selectedRule = SelectedItem;
			var selectedIndex = Rules.IndexOf(SelectedItem);

			if (selectedIndex < Rules.Count - 1)
			{
				var nextRule = Rules[selectedIndex + 1];
				nextRule.Order = selectedIndex;

				selectedRule.Order = selectedIndex + 1;

				Rules.Move(selectedIndex, selectedIndex + 1);

				OnPropertyChanged(nameof(Rules));

				_settings.Rules = Rules.ToList();
				_settingsService.SaveSettings(_settings);
			}
		}

		private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(nameof(TranslationMemoryViewModel.TmsCollection)))
			{
				RefreshPreviewWindow();
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
					var removed = _anonymizeTms.FirstOrDefault(t => t.TmFile.Path.Equals(removedTm.Path));
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
				var anonymizedTmToRemove = _anonymizeTms.FirstOrDefault(t => t.TmFile.Path.Equals(tm.Path));
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
			var settings = new ProgressDialogSettings(_model.ControlParent, true, true, false);
			var result = ProgressDialog.Execute(StringResources.Loading_data, () =>
			{
				ProcessData(ProgressDialog.Current);
			}, settings);


			if (result.Cancelled)
			{
				SourceSearchResults.Clear();
				MessageBox.Show(StringResources.Process_cancelled_by_user, Application.ProductName);
			}
			else if (result.OperationFailed)
			{
				SourceSearchResults.Clear();
				MessageBox.Show(StringResources.Process_failed + "\r\n\r\n" + result.Error.Message, Application.ProductName);
			}
			else
			{
				LoadPreviewWindow();
			}
		}

		private void ProcessData(ProgressDialogContext context)
		{
			var selectedTms = _tmsCollection.Where(t => t.IsSelected).ToList();
			var selectedRulesCount = Rules.Count(r => r.IsSelected);
			if (selectedTms.Count > 0 && selectedRulesCount > 0)
			{
				context.Report("Initializing process...");

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

						var anonymizeTm = _model.TmService.GetAnonymizeTmServerBased(context, tm, translationProvider, out var searchResults);

						SourceSearchResults.AddRange(searchResults);

						if (!_anonymizeTms.Any(n => n.TmFile.Path.Equals(anonymizeTm.TmFile.Path)))
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

					var anonymizeTm = _model.TmService.GetAnonymizeTmFileBased(context, tm, out var searchResults);

					SourceSearchResults.AddRange(searchResults);

					if (!_anonymizeTms.Any(n => n.TmFile.Path.Equals(anonymizeTm.TmFile.Path)))
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

		private void LoadPreviewWindow()
		{
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				var previewWindow = new PreviewView();
				var previewViewModel = new PreviewViewModel(previewWindow, SourceSearchResults, _anonymizeTms, _model);

				previewWindow.DataContext = previewViewModel;

				previewWindow.Closing += PreviewWindow_Closing;
				previewWindow.ShowDialog();
			});
		}

		private void PreviewWindow_Closing(object sender, CancelEventArgs e)
		{
			SourceSearchResults.Clear();
		}

		private void SelectAllRules()
		{
			var value = SelectAll;
			foreach (var rule in Rules)
			{
				rule.IsSelected = value;
			}
		}

		public void Dispose()
		{
			_tmsCollection.CollectionChanged -= TmsCollection_CollectionChanged;
			_model.PropertyChanged -= ModelPropertyChanged;

			foreach (var rule in _rules)
			{
				rule.PropertyChanged -= Rule_PropertyChanged;
			}
		}
	}
}
