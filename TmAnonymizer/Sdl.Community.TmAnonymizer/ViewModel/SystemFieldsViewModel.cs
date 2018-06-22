using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class SystemFieldsViewModel:ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<UniqueUsername> _uniqueUsernames;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private readonly BackgroundWorker _backgroundWorker;
		private ICommand _selectAllCommand;
		private ICommand _removeUserCommand;
		private ICommand _applyChangesCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private IList _selectedItems;


		public SystemFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_uniqueUsernames = new ObservableCollection<UniqueUsername>();
			_selectedItems = new List<UniqueUsername>();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_translationMemoryViewModel = translationMemoryViewModel;
			if (_tmsCollection != null)
			{
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					foreach (var serverTm in serverTms)
					{
						UniqueUsernames = SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
					}
					
				}

				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						UniqueUsernames = SystemFields.GetUniqueFileBasedSystemFields(fileTm);
					}
				}
			}
			
			//foreach (var userName in _uniqueUsernames)
			//{
			//	userName.PropertyChanged += UserName_PropertyChanged;
			//}
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
			_translationMemoryViewModel.PropertyChanged += _translationMemoryViewModel_PropertyChanged;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
		}

		//private void UserName_PropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//		var x = Tm.GetListOfUniqueSystemFields(_tmsCollection);
		//		x = UniqueUsernames;

			//}

			//private void UniqueUsernames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			//{
			//	if (e.Action == NotifyCollectionChangedAction.Add)
			//	{
			//		foreach (var item in e.NewItems)
			//		{
			//			var userName = (UniqueUsername)item;
			//			userName.PropertyChanged += UserName_PropertyChanged;
			//		}
			//	}
			//}
		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public ObservableCollection<UniqueUsername> UniqueUsernames
		{
			get => _uniqueUsernames;

			set
			{
				if (Equals(value, _uniqueUsernames))
				{
					return;
				}
				_uniqueUsernames = value;
				OnPropertyChanged(nameof(UniqueUsernames));
			}
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllUserNames, true));
		public ICommand RemoveUserCommand => _removeUserCommand ?? (_removeUserCommand = new CommandHandler(RemoveAllUserNames, true));
		public ICommand ApplyChangesCommand => _applyChangesCommand ?? (_applyChangesCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));


		private void SelectAllUserNames()
		{
			throw new NotImplementedException();
		}

		private void RemoveAllUserNames()
		{
			throw new NotImplementedException();
		}

		private void ApplyChanges()
		{
			throw new NotImplementedException();
		}
		private void Import()
		{
			throw new NotImplementedException();
		}

		private void Export()
		{
			throw new NotImplementedException();
		}

		private void _translationMemoryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//if (e.PropertyName.Equals("TmsCollection"))
			//{
			//	//removed from tm collection
			//	RefreshPreviewWindow();
			//}
			if ((e.PropertyName.Equals("IsSelected")) && _tmsCollection != null)
			{
				//var tmViewModel = sender as TranslationMemoryViewModel;
				//var selectedTms = tmViewModel.SelectedItems;
				//foreach (var tm in collection)
				//{

				//}
				//var checkedTms = selectedTms.Where(t => t.IsSelected).ToList();
				
			}
		}

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
				foreach (TmFile newTm in e.NewItems)
				{
					
					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				var tm = sender as TmFile;
				if (tm.IsSelected)
				{
					if (tm.IsServerTm)
					{
						var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							_translationMemoryViewModel.Credentials.UserName,
							_translationMemoryViewModel.Credentials.Password);
						var names = SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
						foreach (var name in names)
						{
							UniqueUsernames.Add(name);
						}
					}
					else
					{
						var names = SystemFields.GetUniqueFileBasedSystemFields(tm);
						foreach (var name in names)
						{
							UniqueUsernames.Add(name);
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
						var names = SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
						var newList = UniqueUsernames.ToList();
						foreach (var name in names)
						{
							newList.RemoveAll(n=>n.Username == name.Username);
						}
						UniqueUsernames = new ObservableCollection<UniqueUsername>(newList);
					}
					else
					{
						var names = SystemFields.GetUniqueFileBasedSystemFields(tm);
						var newList = UniqueUsernames.ToList();
						foreach (var name in names)
						{
							newList.RemoveAll(n => n.Username == name.Username);
						}
						UniqueUsernames = new ObservableCollection<UniqueUsername>(newList);
					}
				}
			}
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			throw new NotImplementedException();
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
	}
}
