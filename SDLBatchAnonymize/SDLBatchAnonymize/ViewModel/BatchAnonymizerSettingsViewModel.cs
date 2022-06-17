using System.Windows.Input;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Community.SDLBatchAnonymize.Command;
using Sdl.Community.SDLBatchAnonymize.Model;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.ViewModel
{
	public class BatchAnonymizerSettingsViewModel : ModelBase, ISettingsAware<BatchAnonymizerSettings>
	{
		private bool _anonymizeAllSettings;
		private bool _changeMtChecked;
		private bool _changeTmChecked;
		private bool _clearSettings;
		private string _commentAuthorName;
		private bool _commentChecked;
		private bool _createdByChecked;
		private string _createdByName;
		private decimal _fuzzyScore;
		private bool _isFuzzyEnabled;
		private ICommand _loadWindowAction;
		private bool _modifyByChecked;
		private string _modifyByName;
		private bool _removeMtCloudMetadata;
		private bool _setSpecificResChecked;
		private string _tmName;
		private bool _trackedChecked;
		private string _trackedName;
		private bool _useGeneral;

		public bool AnonymizeAllSettings
		{
			get => _anonymizeAllSettings;
			set
			{
				if (_anonymizeAllSettings == value) return;
				_anonymizeAllSettings = value;
				SetOptions(value);
				OnPropertyChanged(nameof(AnonymizeAllSettings));
			}
		}

		public bool ChangeMtChecked
		{
			get => _changeMtChecked;
			set
			{
				if (_changeMtChecked == value) return;
				_changeMtChecked = value;
				OnPropertyChanged(nameof(ChangeMtChecked));
				EnableFuzzy();
			}
		}

		public bool ChangeTmChecked
		{
			get => _changeTmChecked;
			set
			{
				if (_changeTmChecked == value) return;
				_changeTmChecked = value;
				OnPropertyChanged(nameof(ChangeTmChecked));
				EnableFuzzy();
			}
		}

		public bool ClearSettings
		{
			get => _clearSettings;
			set
			{
				if (_clearSettings == value) return;
				_clearSettings = value;
				OnPropertyChanged(nameof(ClearSettings));
			}
		}

		public string CommentAuthorName
		{
			get => _commentAuthorName;
			set
			{
				if (_commentAuthorName == value) return;
				_commentAuthorName = value;
				if (!string.IsNullOrEmpty(value))
				{
					CommentChecked = true;
				}
				OnPropertyChanged(nameof(CommentAuthorName));
			}
		}

		public bool CommentChecked
		{
			get => _commentChecked;
			set
			{
				if (_commentChecked == value) return;
				_commentChecked = value;
				if (!value)
				{
					CommentAuthorName = string.Empty;
				}
				OnPropertyChanged(nameof(CommentChecked));
			}
		}

		public bool CreatedByChecked
		{
			get => _createdByChecked;
			set
			{
				if (_createdByChecked == value) return;
				_createdByChecked = value;
				if (!value)
				{
					CreatedByName = string.Empty;
				}
				OnPropertyChanged(nameof(CreatedByChecked));
			}
		}

		public string CreatedByName
		{
			get => _createdByName;
			set
			{
				if (_createdByName == value) return;
				_createdByName = value;
				if (!string.IsNullOrEmpty(value))
				{
					CreatedByChecked = true;
				}
				OnPropertyChanged(nameof(CreatedByName));
			}
		}

		public decimal FuzzyScore
		{
			get => _fuzzyScore;
			set
			{
				if (_fuzzyScore == value) return;
				_fuzzyScore = value;
				if (value > 0)
				{
					SetSpecificResChecked = true;
				}
				OnPropertyChanged(nameof(FuzzyScore));
			}
		}

		public bool IsFuzzyEnabled
		{
			get => _isFuzzyEnabled;
			set
			{
				if (_isFuzzyEnabled == value) return;
				_isFuzzyEnabled = value;
				OnPropertyChanged(nameof(IsFuzzyEnabled));
			}
		}

		public ICommand LoadWindowAction => _loadWindowAction ?? (_loadWindowAction = new CommandHandler(WindowLoaded));

		public bool ModifyByChecked
		{
			get => _modifyByChecked;
			set
			{
				if (_modifyByChecked == value) return;
				_modifyByChecked = value;
				if (!value)
				{
					ModifyByName = string.Empty;
				}
				OnPropertyChanged(nameof(ModifyByChecked));
			}
		}

		public string ModifyByName
		{
			get => _modifyByName;
			set
			{
				if (_modifyByName == value) return;
				_modifyByName = value;
				if (!string.IsNullOrEmpty(value))
				{
					ModifyByChecked = true;
				}
				OnPropertyChanged(nameof(ModifyByName));
			}
		}

		public bool RemoveMtCloudMetadata
		{
			get => _removeMtCloudMetadata;
			set
			{
				_removeMtCloudMetadata = value;
				OnPropertyChanged(nameof(RemoveMtCloudMetadata));
			}
		}

		public bool SetSpecificResChecked
		{
			get => _setSpecificResChecked;
			set
			{
				if (_setSpecificResChecked == value) return;
				_setSpecificResChecked = value;
				if (!value)
				{
					FuzzyScore = 0;
					TmName = string.Empty;
				}
				OnPropertyChanged(nameof(SetSpecificResChecked));
			}
		}

		public BatchAnonymizerSettings Settings { get; set; }

		public string TmName
		{
			get => _tmName;
			set
			{
				if (_tmName == value) return;
				_tmName = value;
				if (!string.IsNullOrEmpty(value))
				{
					SetSpecificResChecked = true;
				}
				OnPropertyChanged(nameof(TmName));
			}
		}

		public bool TrackedChecked
		{
			get => _trackedChecked;
			set
			{
				if (_trackedChecked == value) return;
				_trackedChecked = value;
				if (!value)
				{
					TrackedName = string.Empty;
				}
				OnPropertyChanged(nameof(TrackedChecked));
			}
		}

		public string TrackedName
		{
			get => _trackedName;
			set
			{
				if (_trackedName == value) return;
				_trackedName = value;
				if (!string.IsNullOrEmpty(value))
				{
					TrackedChecked = true;
				}
				OnPropertyChanged(nameof(TrackedName));
			}
		}

		public bool UseGeneral
		{
			get => _useGeneral;
			set
			{
				if (_useGeneral == value) return;
				_useGeneral = value;
				OnPropertyChanged(nameof(UseGeneral));
			}
		}

		private void EnableFuzzy()
		{
			if (AnonymizeAllSettings)
			{
				IsFuzzyEnabled = false;
			}
			else if (ChangeTmChecked && !ChangeMtChecked)
			{
				IsFuzzyEnabled = false;
			}
			else
			{
				IsFuzzyEnabled = true;
			}
		}

		/// <summary>
		/// Set/Reset values for anonymize all option
		/// </summary>
		private void SetOptions(bool anonymizeAll)
		{
			CreatedByChecked = anonymizeAll;
			ModifyByChecked = anonymizeAll;
			CommentChecked = anonymizeAll;
			TrackedChecked = anonymizeAll;
			ChangeMtChecked = anonymizeAll;
			RemoveMtCloudMetadata = anonymizeAll;
			if (!anonymizeAll) return;
			CreatedByName = string.Empty;
			ModifyByName = string.Empty;
			CommentAuthorName = string.Empty;
			TrackedName = string.Empty;
			TmName = string.Empty;
			ChangeTmChecked = false;
			SetSpecificResChecked = false;
			FuzzyScore = 0;
		}

		private void WindowLoaded(object obj)
		{
			if (Settings == null) return;
			AnonymizeAllSettings = Settings.AnonymizeComplete;
			UseGeneral = Settings.UseGeneral;
			ClearSettings = Settings.ClearSettings;
			CreatedByChecked = Settings.CreatedByChecked;
			CreatedByName = Settings.CreatedByName;
			ModifyByChecked = Settings.ModifyByChecked;
			ModifyByName = Settings.ModifyByName;
			CommentChecked = Settings.CommentChecked;
			CommentAuthorName = Settings.CommentAuthorName;
			TrackedChecked = Settings.TrackedChecked;
			TrackedName = Settings.TrackedName;
			ChangeMtChecked = Settings.ChangeMtChecked;
			ChangeTmChecked = Settings.ChangeTmChecked;
			SetSpecificResChecked = Settings.SetSpecificResChecked;
			TmName = Settings.TmName;
			FuzzyScore = Settings.FuzzyScore;
			RemoveMtCloudMetadata = Settings.RemoveMtCloudMetadata;
		}
	}
}