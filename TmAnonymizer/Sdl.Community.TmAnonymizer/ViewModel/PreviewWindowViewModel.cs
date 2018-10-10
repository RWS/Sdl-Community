using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
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
		private readonly TranslationMemoryViewModel _model;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;
		private readonly BackgroundWorker _backgroundWorker;
		private ScheduledServerTranslationMemoryExport _tmExporter;
		private readonly List<ServerTmBackUp> _backupTms;
		private string _filePath;
		private SourceSearchResult _selectedItem;
		private string _textBoxColor;
		private readonly Window _window;

		public PreviewWindowViewModel(Window window, List<SourceSearchResult> searchResults,
			ObservableCollection<AnonymizeTranslationMemory> anonymizeTranslationMemories, ObservableCollection<TmFile> tmsCollection,
			TranslationMemoryViewModel model)
		{
			_window = window;
			_textBoxColor = "White";

			_backupTms = new List<ServerTmBackUp>();
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;

			SourceSearchResults = new ObservableCollection<SourceSearchResult>(searchResults);

			_model = model;
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
				var settings = new ProgressDialogSettings(_window, true, true, false);
				var result = ProgressDialog.Execute(StringResources.Applying_changes, () =>
				{
					var selectedSearchResult = SourceSearchResults.Where(s => s.TuSelected).ToList();
					List<AnonymizeTranslationMemory> tusToAnonymize;

					//file base tms
					var fileBasedSearchResult = selectedSearchResult.Where(t => !t.IsServer).ToList();
					if (fileBasedSearchResult.Count > 0)
					{						
						BackupFileBasedTms(ProgressDialog.Current);
						tusToAnonymize = GetTranslationUnitsToAnonymize(fileBasedSearchResult);

						_model.TmService.AnonymizeFileBasedTu(ProgressDialog.Current, tusToAnonymize);
					}

					//server based tms
					var serverBasedSearchResult = selectedSearchResult.Where(t => t.IsServer).ToList();
					if (serverBasedSearchResult.Count > 0)
					{
						tusToAnonymize = GetTranslationUnitsToAnonymize(serverBasedSearchResult);

						foreach (var tuToAnonymize in tusToAnonymize)
						{
							var tm = _tmsCollection.FirstOrDefault(a => a.Path == tuToAnonymize.TmPath);

							if (tm == null)
							{
								continue;
							}

							var uri = new Uri(tm.Credentials.Url);
							var translationProvider = new TranslationProviderServer(uri, false, tm.Credentials.UserName, tm.Credentials.Password);

							ProgressDialog.Current.Report(0, "Backup " + tm.Path);
							BackupServerBasedTm(translationProvider, tusToAnonymize);

							_model.TmService.AnonymizeServerBasedTu(ProgressDialog.Current, translationProvider, tuToAnonymize);
						}
					}
				}, settings);

				if (result.Cancelled)
				{
					System.Windows.Forms.MessageBox.Show(StringResources.Process_cancelled_by_user, System.Windows.Forms.Application.ProductName);
				}
				if (result.OperationFailed)
				{
					System.Windows.Forms.MessageBox.Show(StringResources.Process_failed + "\r\n\r\n" + result.Error.Message, System.Windows.Forms.Application.ProductName);
				}

				RemoveSelectedTusToAnonymize();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show(StringResources.ApplyChanges_Please_select_at_least_one_translation_unit_to_apply_the_changes, System.Windows.Forms.Application.ProductName);
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

		private static string GetDateTimeString()
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
						var folderPath = Path.Combine(_model.SettingsService.PathInfo.BackupFullPath, translationMemory.Name,
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
								System.Windows.Forms.MessageBox.Show(_tmExporter.ErrorMessage, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
						System.Windows.Forms.MessageBox.Show(exception.InnerException.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show(exception.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private List<AnonymizeTranslationMemory> GetTranslationUnitsToAnonymize(IEnumerable<SourceSearchResult> selectedSearchResult)
		{
			var tusToAnonymize = new List<AnonymizeTranslationMemory>();
			foreach (var selectedResult in selectedSearchResult)
			{
				var anonymizeUnits = _anonymizeTranslationMemories.FirstOrDefault(a => a.TmPath == selectedResult.TmFilePath);

				var translationUnit = anonymizeUnits?.TranslationUnits.FirstOrDefault(n =>
					n.ResourceId.Guid.ToString() == selectedResult.Id &&
					n.ResourceId.Id.ToString() == selectedResult.SegmentNumber);

				if (translationUnit != null)
				{
					// if there is an tm with the same path add translation units to that tm
					var anonymizeTu = tusToAnonymize.FirstOrDefault(t => t.TmPath.Equals(anonymizeUnits.TmPath));

					//added for select custom words functionality
					var tranlationUnitDetails = new TranslationUnitDetails
					{
						TranslationUnit = translationUnit,
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
						anonymizeTu.TranslationUnits.Add(translationUnit);
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
						anonymizeTm.TranslationUnits.Add(translationUnit);
						tusToAnonymize.Add(anonymizeTm);
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

		private void BackupFileBasedTms(ProgressDialogContext context)
		{
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				context.Report(0, "Backup " + tm.Path);

				var tmInfo = new FileInfo(tm.Path);

				var extension = Path.GetExtension(tm.Name);
				var tmName = tm.Name;
				if (extension?.Length > 0)
				{
					tmName = tmName.Substring(0, tmName.Length - extension.Length);
				}

				var backupFilePath = Path.Combine(_model.SettingsService.PathInfo.BackupFullPath, tmName + ". " + GetDateTimeString() + extension);

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
			_model?.Dispose();

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
