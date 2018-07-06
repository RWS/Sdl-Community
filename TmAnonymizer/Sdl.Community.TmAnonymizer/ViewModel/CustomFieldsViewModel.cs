using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Ui;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private bool _selectAll;
		private ObservableCollection<CustomField> _customFields;
		private ICommand _selectAllCommand;
		private ICommand _applyCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private readonly BackgroundWorker _backgroundWorker;
		private WaitWindow _waitWindow;

		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_customFields = new ObservableCollection<CustomField>();
			_selectedItems = new List<CustomField>();
			if (_tmsCollection != null)
			{
				PopulateCustomFieldGrid(_tmsCollection, _translationMemoryViewModel);
			}
			_translationMemoryViewModel = translationMemoryViewModel;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;

		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectFields, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var selectedFields = new List<CustomField>();
				var fileDialog = new SaveFileDialog
				{
					Title = @"Export selected custom fields",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (CustomField field in SelectedItems)
					{
						selectedFields.Add(field);
					}
					//Expressions.ExportExporessions(fileDialog.FileName, selectedRules);
					MessageBox.Show(@"File was exported successfully to selected location", "", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(@"Please select at least one row to export", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void ApplyChanges()
		{
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				if (!tm.IsServerTm)
				{
					CustomFieldsHandler.AnonymizeFileBasedSystemFields(tm, CustomFieldsCollection.ToList());
				}
				RefreshCustomFields();
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
			}
		}

		private void SelectFields()
		{
			foreach (var field in CustomFieldsCollection)
			{
				field.IsSelected = SelectAll;
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
		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		

		public ObservableCollection<CustomField> CustomFieldsCollection
		{
			get => _customFields;

			set
			{
				if (Equals(value, _customFields))
				{
					return;
				}
				_customFields = value;
				OnPropertyChanged(nameof(CustomFieldsCollection));

			}
		}
		

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					if (!newTm.IsServerTm)
					{
						CustomFieldsCollection = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(newTm));
					}

					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				if (!_backgroundWorker.IsBusy)
				{
					_backgroundWorker.RunWorkerAsync(sender);
				}
			}
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_waitWindow != null)
			{
				_waitWindow.Close();
			}
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{

			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			if (tm.IsSelected)
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(tm, translationProvider));
					foreach (var field in customFields)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(() =>
						{
							CustomFieldsCollection.Add(field);
						});
					}
				}
				else
				{
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
					foreach (var field in customFields)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(() =>
						{
							CustomFieldsCollection.Add(field);
						});
					}
				}
			}
			else
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(tm, translationProvider));
					var newList = CustomFieldsCollection.ToList();
					foreach (var field in customFields)
					{
						newList.RemoveAll(n => n.Name.Equals(field.Name));
					}
					CustomFieldsCollection = new ObservableCollection<CustomField>(newList);
				}
				else
				{
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
					var newList = CustomFieldsCollection.ToList();
					foreach (var field in customFields)
					{
						newList.RemoveAll(n => n.Name.Equals(field.Name));
					}
					CustomFieldsCollection = new ObservableCollection<CustomField>(newList);
				}
			}
		}


		private void PopulateCustomFieldGrid(ObservableCollection<TmFile> tmsCollection, TranslationMemoryViewModel translationMemoryViewModel)
		{
			var serverBasedTms = tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
			var fileBasedTms = tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
			if (serverBasedTms.Any())
			{
				var uri = new Uri(translationMemoryViewModel.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false,
					translationMemoryViewModel.Credentials.UserName,
					translationMemoryViewModel.Credentials.Password);
				foreach (var serverTm in serverBasedTms)
				{
					CustomFieldsCollection =
						 new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(serverTm, translationProvider));
				}
			}

			if (fileBasedTms.Any())
			{
				foreach (var fileTm in fileBasedTms)
				{
					CustomFieldsCollection = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(fileTm));
				}
			}
		}

		private void RefreshCustomFields()
		{
			if (_tmsCollection != null)
			{
				CustomFieldsCollection = new ObservableCollection<CustomField>();
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						var fields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(fileTm));
						foreach (var field in fields)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(() =>
							{
								CustomFieldsCollection.Add(field);
							});
						}
					}
				}
				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					foreach (var serverTm in serverTms)
					{
						var fields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(serverTm, translationProvider));
						foreach (var field in fields)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(() =>
							{
								CustomFieldsCollection.Add(field);
							});
						}
					}

				}
			}
		}
	}
}



