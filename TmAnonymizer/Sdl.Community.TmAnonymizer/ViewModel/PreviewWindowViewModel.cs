using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class PreviewWindowViewModel : ViewModelBase, IDisposable
	{
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly ObservableCollection<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private bool _selectAllResults;
		private readonly TranslationMemoryViewModel _tmViewModel;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;
		private readonly BackgroundWorker _backgroundWorker;
		private ScheduledServerTranslationMemoryExport _tmExporter;
		private readonly List<ServerTmBackUp> _backupTms;
		private string _filePath;
		private WaitWindow _waitWindow;
		private SourceSearchResult _selectedItem;
		private string _textBoxColor;

		public PreviewWindowViewModel(List<SourceSearchResult> searchResults,
			ObservableCollection<AnonymizeTranslationMemory> anonymizeTranslationMemories, ObservableCollection<TmFile> tmsCollection,
			TranslationMemoryViewModel tmViewModel)
		{
			_textBoxColor = "White";

			_backupTms = new List<ServerTmBackUp>();
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;

			SourceSearchResults = new ObservableCollection<SourceSearchResult>(searchResults);

			_tmViewModel = tmViewModel;
			_anonymizeTranslationMemories = anonymizeTranslationMemories;
			_tmsCollection = tmsCollection;
		}

		private async void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			await Task.WhenAll(Task.Run(() => Parallel.ForEach(_backupTms, tm =>
			{
				using (Stream outputStream = new FileStream(tm.FilePath, FileMode.Create))
				{
					tm.ScheduledExport.DownloadExport(outputStream);
				}
			})));
		}

		public ICommand SelectAllResultsCommand => _selectAllResultsCommand ?? (_selectAllResultsCommand = new CommandHandler(SelectResults, true));

		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));

		public void ApplyChanges()
		{
			if (SourceSearchResults.Any(s => s.TuSelected))
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate
				{
					_waitWindow = new WaitWindow();
					_waitWindow.Show();
				});
				DoEvents();

				var selectedSearchResult = SourceSearchResults.Where(s => s.TuSelected).ToList();
				List<AnonymizeTranslationMemory> tusToAnonymize;
				//file base tms
				var fileBasedSearchResult = selectedSearchResult.Where(t => !t.IsServer).ToList();
				if (fileBasedSearchResult.Count > 0)
				{
					BackupFileBasedTm();
					tusToAnonymize = GetTranslationUnitsToAnonymize(fileBasedSearchResult);
					_tmViewModel.TmService.AnonymizeFileBasedTu(tusToAnonymize);
				}

				//server based tms
				var serverBasedSearchResult = selectedSearchResult.Where(t => t.IsServer).ToList();
				if (serverBasedSearchResult.Count > 0)
				{
					tusToAnonymize = GetTranslationUnitsToAnonymize(serverBasedSearchResult);

					//TODO - resolve for server based TMs
					var uri = new Uri(_tmViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false, _tmViewModel.Credentials.UserName, _tmViewModel.Credentials.Password);

					BackupServerBasedTm(translationProvider, tusToAnonymize);

					_tmViewModel.TmService.AnonymizeServerBasedTu(translationProvider, tusToAnonymize);
				}

				RemoveSelectedTusToAnonymize();

				_waitWindow?.Close();
			}
			else
			{
				MessageBox.Show(StringResources.ApplyChanges_Please_select_at_least_one_translation_unit_to_apply_the_changes, Application.ProductName);
			}
		}

		private static void DoEvents()
		{
			System.Windows.Application.Current.Dispatcher.Invoke(delegate { }, DispatcherPriority.Background);
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

				if (_sourceSearchResults != null)
				{
					foreach (var result in _sourceSearchResults)
					{
						result.PropertyChanged -= Result_PropertyChanged;
					}
				}

				_sourceSearchResults = value;

				if (_sourceSearchResults != null)
				{
					foreach (var result in _sourceSearchResults)
					{
						result.PropertyChanged += Result_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(SourceSearchResults));
			}
		}

		public SourceSearchResult SelectedItem
		{
			get => _selectedItem;

			set
			{
				if (Equals(value, _selectedItem))
				{
					return;
				}
				_selectedItem = value;
				TextBoxColor = "#f4fef4";
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		public string TextBoxColor
		{
			get => _textBoxColor;
			set
			{
				if (Equals(value, _textBoxColor))
				{
					return;
				}
				_textBoxColor = value;
				OnPropertyChanged(nameof(TextBoxColor));
			}
		}

		private string GetDateTimeString()
		{
			var dt = DateTime.Now;
			return dt.Year +
				   dt.Month.ToString().PadLeft(2, '0') +
				   dt.Day.ToString().PadLeft(2, '0') +
				   "T" +
				   dt.Hour.ToString().PadLeft(2, '0') +
				   dt.Minute.ToString().PadLeft(2, '0') +
				   dt.Second.ToString().PadLeft(2, '0');
		}

		private void BackupServerBasedTm(TranslationProviderServer translationProvider, IEnumerable<AnonymizeTranslationMemory> tusToAnonymize)
		{
			_backupTms.Clear();

			try
			{
				foreach (var tuToAonymize in tusToAnonymize)
				{
					var translationMemory = translationProvider.GetTranslationMemory(tuToAonymize.TmPath, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;

					foreach (var languageDirection in languageDirections)
					{
						var folderPath = Path.Combine(_tmViewModel.SettingsService.PathInfo.BackupFullPath, translationMemory.Name,
							languageDirection.TargetLanguageCode);

						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						var fileName = translationMemory.Name + languageDirection.TargetLanguageCode + "." + GetDateTimeString() + ".tmx.gz";
						_filePath = Path.Combine(folderPath, fileName);

						//if tm does not exist download it
						if (!File.Exists(_filePath))
						{
							_tmExporter = new ScheduledServerTranslationMemoryExport(languageDirection)
							{
								ContinueOnError = true
							};

							_tmExporter.Queue();
							_tmExporter.Refresh();

							var continueWaiting = true;
							while (continueWaiting)
							{
								switch (_tmExporter.Status)
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
										_tmExporter.Refresh();
										break;
									default:
										continueWaiting = false;
										break;
								}
							}

							if (_tmExporter.Status == ScheduledOperationStatus.Completed)
							{
								var backup = new ServerTmBackUp
								{
									ScheduledExport = _tmExporter,
									FilePath = _filePath
								};
								_backupTms.Add(backup);
							}
							else if (_tmExporter.Status == ScheduledOperationStatus.Error)
							{
								MessageBox.Show(_tmExporter.ErrorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
					if (!_backgroundWorker.IsBusy)
					{
						_backgroundWorker.RunWorkerAsync();
					}
				}
			}
			catch (Exception exception)
			{
				if (exception.Message.Equals("One or more errors occurred."))
				{
					if (exception.InnerException != null)
					{
						MessageBox.Show(exception.InnerException.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private List<AnonymizeTranslationMemory> GetTranslationUnitsToAnonymize(IEnumerable<SourceSearchResult> selectedSearchResult)
		{
			var tusToAnonymize = new List<AnonymizeTranslationMemory>();
			foreach (var selectedResult in selectedSearchResult)
			{
				foreach (var anonymizeUnits in _anonymizeTranslationMemories)
				{
					var sourceTranslationUnit =
						anonymizeUnits.TranslationUnits.FirstOrDefault(n => n.SourceSegment.ToPlain().Equals(selectedResult.SourceText.TrimEnd()));
					var targetTranslationUnit =
						anonymizeUnits.TranslationUnits.FirstOrDefault(n => n.TargetSegment.ToPlain().Equals(selectedResult.TargetText.TrimEnd()));
					//TranslationUnit tuToAnonymize;

					if (sourceTranslationUnit != null || targetTranslationUnit != null)
					{
						// if there is an tm with the same path add translation units to that tm
						var anonymizeTu = tusToAnonymize.FirstOrDefault(t => t.TmPath.Equals(anonymizeUnits.TmPath));
						var tuToAnonymize = new TranslationUnit();

						if (sourceTranslationUnit != null)
						{
							tuToAnonymize = sourceTranslationUnit;
						}
						if (targetTranslationUnit != null)
						{
							tuToAnonymize = targetTranslationUnit;
						}
						//added for select custom words functionality
						var tranlationUnitDetails = new TranslationUnitDetails
						{
							TranslationUnit = tuToAnonymize,
							SelectedWordsDetails = selectedResult.SelectedWordsDetails,
							RemovedWordsFromMatches = selectedResult.DeSelectedWordsDetails,
							IsSourceMatch = selectedResult.IsSourceMatch,
							IsTargetMatch = selectedResult.IsTargetMatch,
							TargetSelectedWordsDetails = selectedResult.TargetSelectedWordsDetails,
							TargetRemovedWordsFromMatches = selectedResult.TargetDeSelectedWordsDetails
						};

						if (anonymizeTu != null)
						{
							anonymizeTu.TranslationUnitDetails.Add(tranlationUnitDetails);
							anonymizeTu.TranslationUnits.Add(tuToAnonymize);
						}
						else
						{
							var anonymizeTm = new AnonymizeTranslationMemory
							{
								TranslationUnits = new List<TranslationUnit>(),
								TmPath = anonymizeUnits.TmPath,
								TranslationUnitDetails = new List<TranslationUnitDetails>(),

							};
							anonymizeTm.TranslationUnitDetails.Add(tranlationUnitDetails);

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
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				var tmInfo = new FileInfo(tm.Path);

				var extension = Path.GetExtension(tm.Name);
				var tmName = tm.Name;
				if (extension?.Length > 0)
				{
					tmName = tmName.Substring(0, tmName.Length - extension.Length);
				}

				var backupFilePath = Path.Combine(_tmViewModel.SettingsService.PathInfo.BackupFullPath, tmName + ". " + GetDateTimeString() + extension);

				if (!File.Exists(backupFilePath))
				{
					tmInfo.CopyTo(backupFilePath, false);
				}
			}
		}

		private void SelectResults()
		{
			var value = SelectAllResults;
			foreach (var result in _sourceSearchResults)
			{
				result.TuSelected = value;
			}
		}

		private void UpdateCheckedAllState()
		{
			if (SourceSearchResults.Count > 0)
			{
				SelectAllResults = SourceSearchResults.Count(a => !a.TuSelected) <= 0;
			}
			else
			{
				SelectAllResults = false;
			}
		}

		private void Result_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateCheckedAllState();
		}

		public void Dispose()
		{
			_tmViewModel?.Dispose();

			if (_backgroundWorker != null)
			{
				_backgroundWorker.DoWork -= BackgroundWorker_DoWork;
				_backgroundWorker.Dispose();
			}

			foreach (var result in SourceSearchResults)
			{
				result.PropertyChanged -= Result_PropertyChanged;
			}
		}
	}
}
