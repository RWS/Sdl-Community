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
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class PreviewWindowViewModel : ViewModelBase, IDisposable
	{
		private ObservableCollection<ContentSearchResult> _sourceSearchResults;
		private readonly ObservableCollection<AnonymizeTranslationMemory> _anonymizeTms;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private bool _selectAllResults;
		private readonly TranslationMemoryViewModel _model;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;
		private readonly BackgroundWorker _backgroundWorker;
		private ScheduledServerTranslationMemoryExport _tmExporter;
		private readonly List<ServerTmBackUp> _backupTms;
		private string _filePath;
		private ContentSearchResult _selectedItem;
		private string _textBoxColor;
		private readonly Window _window;

		public PreviewWindowViewModel(Window window, List<ContentSearchResult> searchResults,
			ObservableCollection<AnonymizeTranslationMemory> anonymizeTms,
			ObservableCollection<TmFile> tmsCollection,
			TranslationMemoryViewModel model)
		{
			_window = window;
			_textBoxColor = "White";

			_backupTms = new List<ServerTmBackUp>();
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;

			SourceSearchResults = new ObservableCollection<ContentSearchResult>(searchResults);

			_model = model;
			_anonymizeTms = anonymizeTms;
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
					List<AnonymizeTranslationMemory> anonymizeTranslationMemories;

					//file base tms
					var fileBasedSearchResult = selectedSearchResult.Where(t => !t.IsServer).ToList();
					if (fileBasedSearchResult.Count > 0)
					{
						anonymizeTranslationMemories = GetTranslationUnitsToAnonymize(ProgressDialog.Current, fileBasedSearchResult);

						if (ProgressDialog.Current != null && ProgressDialog.Current.CheckCancellationPending())
						{
							ProgressDialog.Current.ThrowIfCancellationPending();
						}

						BackupFileBasedTms(ProgressDialog.Current, anonymizeTranslationMemories.Select(a => a.TmFile.Path).ToList());

						_model.TmService.AnonymizeFileBasedTm(ProgressDialog.Current, anonymizeTranslationMemories);
					}

					//server based tms
					var serverBasedSearchResult = selectedSearchResult.Where(t => t.IsServer).ToList();
					if (serverBasedSearchResult.Count > 0)
					{
						anonymizeTranslationMemories = GetTranslationUnitsToAnonymize(ProgressDialog.Current, serverBasedSearchResult);

						if (ProgressDialog.Current != null && ProgressDialog.Current.CheckCancellationPending())
						{
							ProgressDialog.Current.ThrowIfCancellationPending();
						}

						BackupServerBasedTm(ProgressDialog.Current, anonymizeTranslationMemories.Select(a => a.TmFile.Path).ToList());

						_model.TmService.AnonymizeServerBasedTm(ProgressDialog.Current, anonymizeTranslationMemories);
					}
				}, settings);

				if (result.Cancelled)
				{
					System.Windows.Forms.MessageBox.Show(StringResources.Process_cancelled_by_user, System.Windows.Forms.Application.ProductName);
					_window.Close();
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

		public ObservableCollection<ContentSearchResult> SourceSearchResults
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

		public ContentSearchResult SelectedItem
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

		public int SelectedCount
		{
			get { return SourceSearchResults?.Count(a => a.TuSelected) ?? 0; }
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

		private void BackupServerBasedTm(ProgressDialogContext context, IEnumerable<string> paths)
		{
			_backupTms.Clear();

			try
			{
				foreach (var path in paths)
				{
					var tm = _tmsCollection.FirstOrDefault(a => a.IsServerTm && a.Path.Equals(path));

					if (tm == null)
					{
						continue;
					}

					var uri = new Uri(tm.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false, tm.Credentials.UserName, tm.Credentials.Password);

					context.Report(0, "Backup " + tm.Path);

					var translationMemory = translationProvider.GetTranslationMemory(path, TranslationMemoryProperties.All);
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
								System.Windows.Forms.MessageBox.Show(_tmExporter.ErrorMessage, System.Windows.Forms.Application.ProductName,
									MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void BackupFileBasedTms(ProgressDialogContext context, IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				var tm = _tmsCollection.FirstOrDefault(a => !a.IsServerTm && a.Path.Equals(path));

				if (tm == null)
				{
					continue;
				}

				context.Report(0, "Backup " + tm.Path);

				var tmInfo = new FileInfo(tm.Path);

				var extension = Path.GetExtension(tm.Name);
				var tmName = tm.Name;
				if (extension?.Length > 0)
				{
					tmName = tmName.Substring(0, tmName.Length - extension.Length);
				}

				var backupFilePath = Path.Combine(_model.SettingsService.PathInfo.BackupFullPath, tmName + "." + GetDateTimeString() + extension);

				if (!File.Exists(backupFilePath))
				{
					tmInfo.CopyTo(backupFilePath, false);
				}
			}
		}

		private List<AnonymizeTranslationMemory> GetTranslationUnitsToAnonymize(ProgressDialogContext context, IReadOnlyCollection<ContentSearchResult> selectedSearchResults)
		{
			if (selectedSearchResults == null)
			{
				return null;
			}

			decimal iCurrent = 0;
			decimal iTotalUnits = selectedSearchResults.Count;

			var tusToAnonymize = new List<AnonymizeTranslationMemory>();
			foreach (var selectedResult in selectedSearchResults)
			{
				iCurrent++;
				if (iCurrent % 1000 == 0)
				{
					if (context != null && context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress), "Analyzing: " + iCurrent + " of " + iTotalUnits + " Translation Units");
				}

				var anonymizeTranslationMemory = _anonymizeTms.FirstOrDefault(a => a.TmFile.Path == selectedResult.TmFilePath);

				if (anonymizeTranslationMemory != null && selectedResult.TranslationUnit != null)
				{
					// if there is an tm with the same path add translation units to that tm
					var anonymizeTu = tusToAnonymize.FirstOrDefault(t => t.TmFile.Path.Equals(anonymizeTranslationMemory.TmFile.Path));

					//added for select custom words functionality
					var tranlationUnitDetails = new TranslationUnitDetails
					{
						TranslationUnit = selectedResult.TranslationUnit,
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
						anonymizeTu.TranslationUnits.Add(selectedResult.TranslationUnit);
					}
					else
					{
						var anonymizeTm = new AnonymizeTranslationMemory
						{
							TranslationUnits = new List<TmTranslationUnit>(),
							TmFile = anonymizeTranslationMemory.TmFile,
							TranslationUnitDetails = new List<TranslationUnitDetails>()
						};

						anonymizeTm.TranslationUnitDetails.Add(tranlationUnitDetails);
						anonymizeTm.TranslationUnits.Add(selectedResult.TranslationUnit);
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

			OnPropertyChanged(nameof(SelectedCount));
		}

		private bool SelectingAllAction { get; set; }

		private void SelectResults()
		{
			var value = SelectAllResults;

			try
			{
				SelectingAllAction = true;
				foreach (var result in _sourceSearchResults)
				{
					result.TuSelected = value;
				}
			}
			finally
			{
				SelectingAllAction = false;

				UpdateCheckedAllState();
				OnPropertyChanged(nameof(SelectedCount));
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
			if (!SelectingAllAction)
			{
				if (e.PropertyName == nameof(ContentSearchResult.TuSelected))
				{
					UpdateCheckedAllState();

					OnPropertyChanged(nameof(SelectedCount));
				}
			}
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
