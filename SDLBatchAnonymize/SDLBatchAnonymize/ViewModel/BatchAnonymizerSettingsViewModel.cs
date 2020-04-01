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
		private bool _createdByChecked;
		private bool _modifyByChecked;
		private bool _commentChecked;
		private bool _trackedChecked;
		private bool _changeMtChecked;
		private bool _changeTmChecked;
		private bool _setSpecificResChecked;
		private string _createdByName;
		private string _modifyByName;
		private string _commentAuthorName;
		private string _trackedName;
		private string _tmName;
		private decimal _fuzzyScore;
		private ICommand _loadWindowAction;
		public BatchAnonymizerSettings Settings { get; set; }
		
		public bool AnonymizeAllSettings
		{
			get => _anonymizeAllSettings;
			set
			{
				if (_anonymizeAllSettings == value)return;
				_anonymizeAllSettings = value;
				OnPropertyChanged(nameof(AnonymizeAllSettings));
			}
		}

		public bool CreatedByChecked
		{
			get => _createdByChecked;
			set
			{
				if (_createdByChecked == value) return;
				_createdByChecked = value;
				OnPropertyChanged(nameof(CreatedByChecked));
			}
		}

		public bool ModifyByChecked
		{
			get => _modifyByChecked;
			set
			{
				if (_modifyByChecked == value) return;
				_modifyByChecked = value;
				OnPropertyChanged(nameof(ModifyByChecked));
			}
		}

		public bool CommentChecked
		{
			get => _commentChecked;
			set
			{
				if (_commentChecked == value) return;
				_commentChecked = value;
				OnPropertyChanged(nameof(CommentChecked));
			}
		}

		public bool TrackedChecked
		{
			get => _trackedChecked;
			set
			{
				if (_trackedChecked == value) return;
				_trackedChecked = value;
				OnPropertyChanged(nameof(TrackedChecked));
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
			}
		}
		public bool SetSpecificResChecked
		{
			get => _setSpecificResChecked;
			set
			{
				if (_setSpecificResChecked == value) return;
				_setSpecificResChecked = value;
				OnPropertyChanged(nameof(SetSpecificResChecked));
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
			}
		}

		public string CreatedByName
		{
			get => _createdByName;
			set
			{
				if (_createdByName == value) return;
				_createdByName = value;
				OnPropertyChanged(nameof(CreatedByName));
			}
		}

		public string ModifyByName
		{
			get => _modifyByName;
			set
			{
				if (_modifyByName == value) return;
				_modifyByName = value;
				OnPropertyChanged(nameof(ModifyByName));
			}
		}

		public string CommentAuthorName
		{
			get => _commentAuthorName;
			set
			{
				if (_commentAuthorName == value) return;
				_commentAuthorName = value;
				OnPropertyChanged(nameof(CommentAuthorName));
			}
		}

		public string TrackedName
		{
			get => _trackedName;
			set
			{
				if (_trackedName == value) return;
				_trackedName = value;
				OnPropertyChanged(nameof(TrackedName));
			}
		}
		public string TmName
		{
			get => _tmName;
			set
			{
				if (_tmName == value) return;
				_tmName = value;
				OnPropertyChanged(nameof(TmName));
			}
		}

		public decimal FuzzyScore
		{
			get => _fuzzyScore;
			set
			{
				if (_fuzzyScore == value) return;
				_fuzzyScore = value;
				OnPropertyChanged(nameof(FuzzyScore));
			}
		}

		public ICommand LoadWindowAction => _loadWindowAction ?? (_loadWindowAction = new CommandHandler(WindowLoaded));

		private void WindowLoaded(object obj)
		{
			AnonymizeAllSettings = Settings.AnonymizeComplete;
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
		}
	}
}
