using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class PreviewWindowViewModel:ViewModelBase
	{
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private bool _selectAllResults;
		private TranslationMemoryViewModel _tmViewModel;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;
		private readonly BackgroundWorker _backgroundWorker;
		private ScheduledServerTranslationMemoryExport tmExporter;
		private string filePath;
		public PreviewWindowViewModel(ObservableCollection<SourceSearchResult> searchResults,
			List<AnonymizeTranslationMemory> anonymizeTranslationMemories, ObservableCollection<TmFile> tmsCollection,
			TranslationMemoryViewModel tmViewModel)
		{
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_sourceSearchResults = searchResults;
			_tmViewModel = tmViewModel;
			_anonymizeTranslationMemories = anonymizeTranslationMemories;
			_tmsCollection = tmsCollection;
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			using (Stream outputStream = new FileStream(filePath, FileMode.Create))
			{
				var export  =tmExporter.DownloadExport(outputStream);
			}
		}

		public ICommand SelectAllResultsCommand => _selectAllResultsCommand ??
		                                           (_selectAllResultsCommand = new CommandHandler(SelectResults, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		private void ApplyChanges()
		{
			var selectedSearchResult = SourceSearchResults.Where(s => s.TuSelected).ToList();
			var tusToAnonymize = new List<AnonymizeTranslationMemory>();
			//file base tms
			var fileBasedSearchResult = selectedSearchResult.Where(t => !t.IsServer).ToList();
			if (fileBasedSearchResult.Count > 0)
			{
				BackupFileBasedTm();
				tusToAnonymize =GetTranslationUnitsToAnonymize(fileBasedSearchResult);
				Tm.AnonymizeFileBasedTu(tusToAnonymize);
			}
			//server based tms
			var serverBasedSearchResult = selectedSearchResult.Where(t => t.IsServer).ToList();
			if (serverBasedSearchResult.Count > 0)
			{
				tusToAnonymize = GetTranslationUnitsToAnonymize(serverBasedSearchResult);
				var uri = new Uri(_tmViewModel.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false, _tmViewModel.Credentials.UserName,
					_tmViewModel.Credentials.Password);

				BackupServerBasedTm(translationProvider, tusToAnonymize);
				Tm.AnonymizeServerBasedTu(translationProvider,tusToAnonymize);
			}
			RemoveSelectedTusToAnonymize();
		}

		private void BackupServerBasedTm(TranslationProviderServer translationProvider, List<AnonymizeTranslationMemory> tusToAnonymize)
		{
			if (!Directory.Exists(Constants.ServerTmBackupPath))
			{
				 Directory.CreateDirectory(Constants.ServerTmBackupPath);
			}
			try
			{
				foreach (var tuToAonymize in tusToAnonymize)
				{
					var translationMemory =
						translationProvider.GetTranslationMemory(tuToAonymize.TmPath, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;
					foreach (var languageDirection in languageDirections)
					{
						tmExporter = new ScheduledServerTranslationMemoryExport(languageDirection)
						{
							ContinueOnError = true
						};
						tmExporter.Queue();
						tmExporter.Refresh();

						var continueWaiting = true;
						while (continueWaiting)
						{
							switch (tmExporter.Status)
							{
								case ScheduledOperationStatus.Abort:
								case ScheduledOperationStatus.Aborted:
								case ScheduledOperationStatus.Cancel:
								case ScheduledOperationStatus.Cancelled:
								case ScheduledOperationStatus.Completed:
								case ScheduledOperationStatus.Error:
									continueWaiting = false;
									break;
								case ScheduledOperationStatus.Aborting:
								case ScheduledOperationStatus.Allocated:
								case ScheduledOperationStatus.Cancelling:
								case ScheduledOperationStatus.NotSet:
								case ScheduledOperationStatus.Queued:
								case ScheduledOperationStatus.Recovered:
								case ScheduledOperationStatus.Recovering:
								case ScheduledOperationStatus.Recovery:
									continueWaiting = true;
									tmExporter.Refresh();
									break;
								default:
									continueWaiting = false;
									break;
							}
						}
						if (tmExporter.Status == ScheduledOperationStatus.Completed)
						{
							var folderPath = Path.Combine(Constants.ServerTmBackupPath, translationMemory.Name,
								languageDirection.TargetLanguageCode);
							if (!Directory.Exists(folderPath))
							{
								Directory.CreateDirectory(folderPath);
							}
							var fileName = translationMemory.Name + languageDirection.TargetLanguageCode + ".tmx.gz";
							filePath = Path.Combine(folderPath, fileName);
							if (!File.Exists(filePath))
							{
								var file = File.Create(filePath);
								file.Close();
							}
							_backgroundWorker.RunWorkerAsync();
						}
						else if (tmExporter.Status == ScheduledOperationStatus.Error)
						{
							MessageBox.Show(tmExporter.ErrorMessage,
								"", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}

					}
				}
			}
			catch (Exception exception)
			{
				if (exception.Message.Equals("One or more errors occurred."))
				{
					if (exception.InnerException != null)
					{
						MessageBox.Show(exception.InnerException.Message,
							"", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show(exception.Message,
						"", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

		}

		private List<AnonymizeTranslationMemory> GetTranslationUnitsToAnonymize(List<SourceSearchResult> selectedSearchResult)
		{
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
			return tusToAnonymize;
		}
		private void RemoveSelectedTusToAnonymize()
		{
			foreach (var searchResult in SourceSearchResults.Where(s => s.TuSelected).ToList())
			{
				SourceSearchResults.Remove(searchResult);
			}

		}
		private void BackupFileBasedTm()
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
