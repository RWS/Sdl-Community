using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationViewModel : ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		//private bool _selectAllResults;
		private ICommand _selectAllCommand;
		private ICommand _previewCommand;
		//private ICommand _applyCommand;
		//private ICommand _selectAllResultsCommand;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;

		public TranslationViewModel(ObservableCollection<TmFile> tmsCollection)
		{
			_tmsCollection = tmsCollection;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
			_rules = Constants.GetDefaultRules();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));
		public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new CommandHandler(PreviewChanges, true));
		//public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));

		//public ICommand SelectAllResultsCommand => _selectAllResultsCommand ??
		//                                    (_selectAllResultsCommand = new CommandHandler(SelectResults, true));

		//private void SelectResults()
		//{
		//	foreach (var result in SourceSearchResults)
		//	{
		//		result.TuSelected = SelectAllResults;
		//	}
		//}

		//private void ApplyChanges()
		//{
		//	BackupTm();
		//	var selectedSearchResult = SourceSearchResults.Where(s => s.TuSelected).ToList();
		//	var tusToAnonymize = new List<AnonymizeTranslationMemory>();
			
		//	foreach (var selectedResult in selectedSearchResult)
		//	{
		//		foreach (var anonymizeUnits in _anonymizeTranslationMemories)
		//		{
		//			var tuToAnonymize =
		//				anonymizeUnits.TranslationUnits.FirstOrDefault(n => n.SourceSegment.ToPlain().Equals(selectedResult.SourceText));
		//			if (tuToAnonymize != null)
		//			{
		//				// if there is an tm with the same path add translation units to that tm
		//				var anonymizeTu = tusToAnonymize.FirstOrDefault(t => t.TmPath.Equals(anonymizeUnits.TmPath));
		//				if (anonymizeTu != null)
		//				{
		//					anonymizeTu.TranslationUnits.Add(tuToAnonymize);
		//				}
		//				else
		//				{
		//					var anonymizeTm = new AnonymizeTranslationMemory
		//					{
		//						TranslationUnits = new List<TranslationUnit>(),
		//						TmPath = anonymizeUnits.TmPath
		//					};
		//					anonymizeTm.TranslationUnits.Add(tuToAnonymize);
		//					tusToAnonymize.Add(anonymizeTm);
		//				}
		//			}
					
		//		}
		//	}
		//	Tm.AnonymizeTu(tusToAnonymize);
		//	RemoveSelectedTusToAnonymize();
		//}

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

		//private void BackupTm()
		//{
		//	var backupFolderPath = Constants.TmBackupPath;
		//	if (!Directory.Exists(backupFolderPath))
		//	{
		//		Directory.CreateDirectory(backupFolderPath);
		//	}
		//	foreach (var tm in _tmsCollection.Where(t=>t.IsSelected))
		//	{
		//		var tmInfo = new FileInfo(tm.Path);
		//		var backupFilePath = Path.Combine(backupFolderPath, tm.Name);
		//		if (!File.Exists(backupFilePath))
		//		{
		//			tmInfo.CopyTo(backupFilePath, false);
		//		}
		//	}
		//}
		//private void RemoveSelectedTusToAnonymize()
		//{
		//	foreach (var searchResult in SourceSearchResults.Where(s => s.TuSelected).ToList())
		//	{
		//		SourceSearchResults.Remove(searchResult);
		//	}

		//}
		private void PreviewChanges()
		{
			var selectedTms = _tmsCollection.Where(t => t.IsSelected).ToList();

			foreach (var tm in selectedTms)
			{
				//get all tus for selected translation memories
				var tus= Tm.GetTranslationUnits(tm.Path, SourceSearchResults,GetSelectedRules());
				if(!_anonymizeTranslationMemories.Exists(n => n.TmPath.Equals(tus.TmPath)))
				{
					_anonymizeTranslationMemories.Add(tus);
				}
			}
			var previewWindow = new PreviewWindow();
			var previewViewModel = new PreviewWindowViewModel(SourceSearchResults,_anonymizeTranslationMemories,_tmsCollection);
			previewWindow.DataContext = previewViewModel;
			previewWindow.Show();
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
		//public bool SelectAllResults
		//{
		//	get => _selectAllResults;

		//	set
		//	{
		//		if (Equals(value, _selectAllResults))
		//		{
		//			return;
		//		}
		//		_selectAllResults = value;
		//		OnPropertyChanged(nameof(SelectAllResults));
		//	}
		//}
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
