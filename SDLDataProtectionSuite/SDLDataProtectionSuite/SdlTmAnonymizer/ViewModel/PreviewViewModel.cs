using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
	public class PreviewViewModel : ViewModelBase, IDisposable
	{
		private ObservableCollection<ContentSearchResult> _sourceSearchResults;
		private readonly ObservableCollection<AnonymizeTranslationMemory> _anonymizeTms;
		private bool _selectAllResults;
		private readonly TranslationMemoryViewModel _model;
		private ICommand _selectAllResultsCommand;
		private ICommand _applyCommand;
		private ContentSearchResult _selectedItem;
		private string _textBoxColor;
		private readonly Window _window;

		public PreviewViewModel(Window window, List<ContentSearchResult> searchResults,
			ObservableCollection<AnonymizeTranslationMemory> anonymizeTms, TranslationMemoryViewModel model)
		{
			_window = window;
			_textBoxColor = "White";
	
			SourceSearchResults = new ObservableCollection<ContentSearchResult>(searchResults);

			_model = model;
			_anonymizeTms = anonymizeTms;
		}

		public ICommand SelectAllResultsCommand => _selectAllResultsCommand ?? (_selectAllResultsCommand = new CommandHandler(SelectResults, true));

		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));

		public void ApplyChanges()
		{
			if (SourceSearchResults.Any(s => s.TuSelected))
			{
				var progressDialogSettings = new ProgressDialogSettings(_window, true, true, false);

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
						
						_model.TmService.AnonymizeServerBasedTm(ProgressDialog.Current, anonymizeTranslationMemories);
					}
				}, progressDialogSettings);

				if (result.Cancelled)
				{
					System.Windows.Forms.MessageBox.Show(StringResources.Process_cancelled_by_user, System.Windows.Forms.Application.ProductName);
					_window.Close();
				}

				if (result.OperationFailed)
				{
					System.Windows.Forms.MessageBox.Show(StringResources.Process_failed + Environment.NewLine + Environment.NewLine + result.Error.Message, System.Windows.Forms.Application.ProductName);
				}

				RemoveSelectedTusToAnonymize();

				_model.Refresh();
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

					if (selectedResult.SelectedWordsDetails.Count > 0)
					{
						selectedResult.IsSourceMatch = true;
					}

					if (selectedResult.TargetSelectedWordsDetails.Count > 0)
					{
						selectedResult.IsTargetMatch = true;
					}

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

			foreach (var result in SourceSearchResults)
			{
				result.PropertyChanged -= Result_PropertyChanged;
			}
		}
	}
}
