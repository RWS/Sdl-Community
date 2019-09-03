using System;
using System.Windows.Input;
using Sdl.Community.DtSearch4Studio.Provider.Commands;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Interfaces;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.Community.DtSearch4Studio.Provider.Service;

namespace Sdl.Community.DtSearch4Studio.Provider.ViewModel
{
	public class SettingsViewModel : BaseViewModel
	{
		#region Private Fields
		private ICommand _okCommand;
		private ICommand _browseCommand;
		private ICommand _cancelCommand;
		private string _indexLocation;
		private bool _isIndexSelected;
		private readonly IMessageBoxService _messageBoxService;

		#endregion

		#region Public Constructors
		public SettingsViewModel(ProviderSettings providerSettings, MessageBoxService messageBoxService)
		{
			_messageBoxService = messageBoxService;
			SetFieldsSelection(providerSettings);
		}
		#endregion

		#region Public Properties				
		public ProviderSettings ProviderSettings { get; set; }

		public bool IsIndexSelected
		{
			get => _isIndexSelected;
			set
			{
				_isIndexSelected = value;
				OnPropertyChanged(nameof(IsIndexSelected));
			}
		}

		public string IndexLocation
		{
			get => _indexLocation;
			set
			{
				_indexLocation = value;
				IsIndexSelected = false;

				OnPropertyChanged(nameof(IndexLocation));
			}
		}
		#endregion

		#region Commands
		public ICommand OkCommand => _okCommand ?? (_okCommand = new CommandHandler(OkAction, true));
		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(BrowseAction, true));
		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true));
		public Action CloseAction { get; set; }
		#endregion

		#region Actions		
		private void CancelAction()
		{
			_messageBoxService.ShowInformationMessage(Constants.CancelProviderMessage, string.Empty);
			CloseAction();
		}

		private void OkAction()
		{
			if (!string.IsNullOrEmpty(IndexLocation))
			{
				IsIndexSelected = false;
				ProviderSettings = new ProviderSettings
				{
					IndexPath = IndexLocation
				};

				var persistenceService = new PersistenceService();
				persistenceService.AddSettings(ProviderSettings);

				OnSaveSettingsCommandRaised?.Invoke();
			}
			else
			{
				IsIndexSelected = true;
			}
		}

		/// <summary>
		/// Select index from the local machine
		/// </summary>
		private void BrowseAction()
		{
			var fbd = new FolderSelectDialog();
			if (fbd.ShowDialog())
			{
				if (!string.IsNullOrEmpty(fbd.FileName))
				{
					// user can select only one index
					IndexLocation = fbd.FileName;
				}
				else
				{
					_messageBoxService.ShowWarningMessage(Constants.NoIndexSelected, string.Empty);
				}
			}
		}
		#endregion

		#region PublicMethods
		#endregion

		#region Private Methods

		// Set UI Settings fields selection based on the provider settings file (for indexes).
		private void SetFieldsSelection(ProviderSettings providerSettings)
		{
			if (providerSettings != null)
			{
				IndexLocation = providerSettings.IndexPath;
			}
		}
		#endregion

		#region Events
		public delegate ProviderSettings SaveSettingsEventRaiser();
		public event SaveSettingsEventRaiser OnSaveSettingsCommandRaised;
		#endregion
	}
}