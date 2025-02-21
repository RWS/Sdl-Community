using System;
using System.Windows.Input;
using NLog;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Commands;
using Sdl.Community.StarTransit.UI.Controls;
using Sdl.Community.StarTransit.UI.Interfaces;
using Sdl.ProjectAutomation.Core;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class StarTransitMainWindowViewModel : BaseViewModel, IWindowActions
	{
		private ICommand _nextCommand;
		private ICommand _backCommand;
		private ICommand _createCommand;
		private bool _canExecuteNext;
		private bool _canExecuteBack;
		private bool _canExecuteCreate;
		private bool _isDetailsSelected;
		private bool _isTmSelected;
		private bool _isFinishSelected;
		private bool _active;
		private bool _isEnabled;
		private bool _hasTm;
		private string _color;
		private readonly PackageDetailsViewModel _packageDetailsViewModel;
		private readonly PackageDetails _packageDetails;
		private readonly FinishViewModel _finishViewModel;
		private readonly ProjectService _projectService;
		private readonly TranslationMemories _translationMemories;
		private readonly TranslationMemoriesViewModel _translationMemoriesViewModel;
        private readonly IMessageBoxService _messageBoxService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public IProject CreatedProject { get; set; }

		public StarTransitMainWindowViewModel(
			PackageDetailsViewModel packageDetailsViewModel,
			PackageDetails packageDetails,
			TranslationMemories translationMemories,
			TranslationMemoriesViewModel translationMemoriesViewModel,
			FinishViewModel finishViewModel,
            IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
			_packageDetailsViewModel = packageDetailsViewModel;
			_packageDetails = packageDetails;
			_translationMemories = translationMemories;
			_translationMemoriesViewModel = translationMemoriesViewModel;
			CanExecuteBack = false;
			CanExecuteCreate = false;
			CanExecuteNext = true;
			_isDetailsSelected = true;
			_isTmSelected = false;
			_isFinishSelected = false;
			_finishViewModel = finishViewModel;
			Color = "#FFB69476";
			var helpers = new Shared.Utils.Helpers();
			_projectService = new ProjectService(helpers);
		}
		public bool DetailsSelected
		{
			get => _isDetailsSelected;
			set
			{
				if (Equals(value, _isDetailsSelected))
				{
					return;
				}
				_isDetailsSelected = value;
				OnPropertyChanged(nameof(DetailsSelected));
			}
		}

		public bool TmSelected
		{
			get => _isTmSelected;
			set
			{
				if (Equals(value, _isTmSelected))
				{
					return;
				}
				_isTmSelected = value;
				OnPropertyChanged(nameof(TmSelected));
			}
		}

		public bool FinishSelected
		{
			get => _isFinishSelected;
			set
			{
				if (Equals(value, _isFinishSelected))
				{
					return;
				}
				_isFinishSelected = value;
				OnPropertyChanged(nameof(FinishSelected));
			}
		}

		public bool CanExecuteNext
		{
			get => _canExecuteNext;
			set
			{
				if (Equals(value, _canExecuteNext))
				{
					return;
				}
				_canExecuteNext = value;
				OnPropertyChanged(nameof(CanExecuteNext));
			}
		}

		public bool CanExecuteBack
		{
			get => _canExecuteBack;
			set
			{
				if (Equals(value, _canExecuteBack))
				{
					return;
				}

				_canExecuteBack = value;
				OnPropertyChanged(nameof(CanExecuteBack));
			}
		}

		public string Color
		{
			get => _color;
			set
			{
				if (Equals(value, _color))
				{
					return;
				}
				_color = value;
				OnPropertyChanged(nameof(Color));
			}
		}

		public bool CanExecuteCreate
		{
			get => _canExecuteCreate;
			set
			{
				if (Equals(value, _canExecuteCreate))
				{
					return;
				}
				_canExecuteCreate = value;
				OnPropertyChanged(nameof(CanExecuteCreate));
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (Equals(value, _isEnabled))
				{
					return;
				}
				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public bool Active
		{
			get => _active;
			set
			{
				if (Equals(value, _active))
				{
					return;
				}
				_active = value;
				OnPropertyChanged(nameof(Active));
			}
		}
		public Action CloseAction { get; set; }

		public ICommand NextCommand => _nextCommand ?? (_nextCommand = new CommandHandler(Next, true));

		public ICommand BackCommand
		{
			get => _backCommand ?? (_backCommand = new CommandHandler(Back, true));
			set
			{
				if (Equals(value, _backCommand))
				{
					return;
				}
				_backCommand = value;
				OnPropertyChanged();
			}
		}

		public ICommand CreateCommand => _createCommand ?? (_createCommand = new CommandHandler(Create, true));

		public void Next()
		{
			var model = _packageDetailsViewModel.GetPackageModel();
			_hasTm = false;
			var isEmpty = IsFolderEmpty(_packageDetailsViewModel.TextLocation);

			if (isEmpty)
			{
				foreach (var pair in model.LanguagePairs)
				{
					if (pair.StarTranslationMemoryMetadatas.Count != 0)
					{
						_hasTm = true;
					}
				} //tm page is disabled

				if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm == false)
				{
					DetailsSelected = false;
					TmSelected = false;
					FinishSelected = true;
					CanExecuteBack = true;
					CanExecuteNext = false;
					_finishViewModel.Refresh();
					CanExecuteCreate = true;
					IsEnabled = false;
					Color = "Gray";
				} //tm page
				else if (_packageDetails.FieldsAreCompleted() && DetailsSelected && _hasTm)
				{
					DetailsSelected = false;
					TmSelected = true;
					FinishSelected = false;
					CanExecuteBack = true;
					CanExecuteNext = true;
					CanExecuteCreate = false;
					IsEnabled = true;
					Color = "#FF66290B";
				} //finish page
				else if (_packageDetails.FieldsAreCompleted() && TmSelected &&
				         _translationMemories.TmFieldIsCompleted())
				{
					DetailsSelected = false;
					CanExecuteNext = false;
					CanExecuteCreate = true;
					CanExecuteBack = true;
					TmSelected = false;
					IsEnabled = true;
					FinishSelected = true;
					_finishViewModel.Refresh();
					Color = "#FFB69476";
				}
			}
		}

		public void Back()
		{
			if (DetailsSelected)
			{
				CanExecuteBack = false;
			}
			else if (FinishSelected && _hasTm == false)
			{
				CanExecuteBack = false;
				DetailsSelected = true;
				TmSelected = false;
				CanExecuteNext = true;
				FinishSelected = false;
				CanExecuteCreate = false;
			}
			else if (FinishSelected && _hasTm)
			{
				DetailsSelected = false;
				TmSelected = true;
				FinishSelected = false;
				CanExecuteBack = true;
				CanExecuteCreate = false;
				CanExecuteNext = true;
				Color = "#FF66290B";
			}
			else if (TmSelected)
			{
				DetailsSelected = true;
				TmSelected = false;
				FinishSelected = false;
				CanExecuteBack = false;
				CanExecuteCreate = false;
				CanExecuteNext = true;
				Color = "#FFB69476";
			}
		}

		public async void Create()
		{
			try
			{
				Active = true;
				CanExecuteBack = CanExecuteCreate = false;
				var packageModel = _translationMemoriesViewModel.GetPackageModel();
				var isEmpty = IsFolderEmpty(packageModel?.Location);
				var messageModel = new MessageModel();
				CloseAction();
				if (isEmpty)
				{
					//await Task.Run(() => (messageModel, CreatedProject) = _projectService.CreateProject(packageModel));
				}
				if (messageModel == null)
				{
					CanExecuteBack = CanExecuteCreate = false;
					Active = false;
					CloseAction();
				}
				else
				{
					Active = false;
					CanExecuteBack = CanExecuteCreate = false;
					_messageBoxService.ShowInformationResultMessage(messageModel.Message, messageModel.Title);
					CloseAction.Invoke(); // close window
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Check to see if the folder is empty, in case the user just paste the path in text box
		/// </summary>
		private bool IsFolderEmpty(string folderPath)
		{
			if(string.IsNullOrEmpty(folderPath))
			{
                _messageBoxService.ShowWarningMessage("All fields are required!", "Warning");
				return false;
			}

			if (Helpers.Utils.IsFolderEmpty(folderPath)) return true;
			_messageBoxService.ShowWarningMessage("Please select an empty folder", "Folder not empty!");
			return false;
		}
	}
}