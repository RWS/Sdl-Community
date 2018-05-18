using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class PreviewWindowViewModel:ViewModelBase
	{
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private bool _selectAllResults;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;

		public PreviewWindowViewModel(ObservableCollection<SourceSearchResult> searchResults,
			List<AnonymizeTranslationMemory> anonymizeTranslationMemories, ObservableCollection<TmFile> tmsCollection)
		{
			_sourceSearchResults = searchResults;
			_anonymizeTranslationMemories = anonymizeTranslationMemories;
			_tmsCollection = tmsCollection;
		}
		
		public ICommand SelectAllResultsCommand => _selectAllResultsCommand ??
		                                           (_selectAllResultsCommand = new CommandHandler(SelectResults, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		private void ApplyChanges()
		{
			BackupTm();
			var selectedSearchResult = SourceSearchResults.Where(s => s.TuSelected).ToList();
			var tusToAnonymize = new List<AnonymizeTranslationMemory>();

			foreach (var selectedResult in selectedSearchResult)
			{
				foreach (var anonymizeUnits in _anonymizeTranslationMemories)
				{
					var tuToAnonymize =
						anonymizeUnits.TranslationUnits.FirstOrDefault(n => n.SourceSegment.ToPlain().Equals(selectedResult.SourceText));
					if (tuToAnonymize != null)
					{
						// if there is an tm with the same path add translation units to that tm
						var anonymizeTu = tusToAnonymize.FirstOrDefault(t => t.TmPath.Equals(anonymizeUnits.TmPath));
						if (anonymizeTu != null)
						{
							anonymizeTu.TranslationUnits.Add(tuToAnonymize);
						}
						else
						{
							var anonymizeTm = new AnonymizeTranslationMemory
							{
								TranslationUnits = new List<TranslationUnit>(),
								TmPath = anonymizeUnits.TmPath
							};
							anonymizeTm.TranslationUnits.Add(tuToAnonymize);
							tusToAnonymize.Add(anonymizeTm);
						}
					}

				}
			}
			Tm.AnonymizeTu(tusToAnonymize);
			RemoveSelectedTusToAnonymize();
		}
		private void RemoveSelectedTusToAnonymize()
		{
			foreach (var searchResult in SourceSearchResults.Where(s => s.TuSelected).ToList())
			{
				SourceSearchResults.Remove(searchResult);
			}

		}
		private void BackupTm()
		{
			var backupFolderPath = Constants.TmBackupPath;
			if (!Directory.Exists(backupFolderPath))
			{
				Directory.CreateDirectory(backupFolderPath);
			}
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				var tmInfo = new FileInfo(tm.Path);
				var backupFilePath = Path.Combine(backupFolderPath, tm.Name);
				if (!File.Exists(backupFilePath))
				{
					tmInfo.CopyTo(backupFilePath, false);
				}
			}
		}
		public bool SelectAllResults
		{
			get => _selectAllResults;

			set
			{
				if (Equals(value, _selectAllResults))
				{
					return;
				}
				_selectAllResults = value;
				OnPropertyChanged(nameof(SelectAllResults));
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

		private void SelectResults()
		{
			foreach (var result in _sourceSearchResults)
			{
				result.TuSelected = SelectAllResults;
			}
		}
	}
}
