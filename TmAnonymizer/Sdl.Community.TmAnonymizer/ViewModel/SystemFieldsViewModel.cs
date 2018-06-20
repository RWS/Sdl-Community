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
			_selectedItems = new List<UniqueUsername>();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_translationMemoryViewModel = translationMemoryViewModel;
			if (_tmsCollection != null)
			{
				_uniqueUsernames = Tm.GetListOfUniqueSystemFields(_tmsCollection);
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
			if (e.PropertyName.Equals("TmsCollection"))
			{
				//removed from tm collection
				RefreshPreviewWindow();
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
				UniqueUsernames = Tm.GetListOfUniqueSystemFields(_tmsCollection);
			}
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					//if (_tmsCollection.Count != 0)
					//{
						UniqueUsernames = Tm.GetListOfUniqueSystemFields(_tmsCollection);
					//}
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
