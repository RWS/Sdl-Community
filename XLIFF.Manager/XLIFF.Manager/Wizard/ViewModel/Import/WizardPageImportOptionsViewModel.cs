using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import
{
	public class WizardPageImportOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private List<ConfirmationLevel> _confirmationStatuses;
		private ConfirmationLevel _confirmationStatus;
		private string _originSystem;

		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			BackupFiles = wizardContext.ImportBackupFiles;
			OverwriteTranslations = wizardContext.ImportOverwriteTranslations;
			OriginSystem = wizardContext.ImportOriginSystem;
			ConfirmationStatuses = Enum.GetValues(typeof(ConfirmationLevel)).Cast<ConfirmationLevel>().ToList();
			ConfirmationStatus = wizardContext.ImportConfirmationStatus;

			VerifyIsValid();

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{
			IsValid = !string.IsNullOrEmpty(OriginSystem.Trim());
		}

		public bool BackupFiles { get; set; }

		public bool OverwriteTranslations { get; set; }

		public string OriginSystem
		{
			get => _originSystem;
			set
			{
				if (value == _originSystem)
				{
					return;
				}

				_originSystem = value;
				OnPropertyChanged(nameof(OriginSystem));
				VerifyIsValid();
			}
		}

		public ConfirmationLevel ConfirmationStatus
		{
			get => _confirmationStatus;
			set
			{
				if (value == _confirmationStatus)
				{
					return;
				}

				_confirmationStatus = value;
				OnPropertyChanged(nameof(ConfirmationStatus));				
			}
		}

		public List<ConfirmationLevel> ConfirmationStatuses
		{
			get => _confirmationStatuses;
			set
			{
				_confirmationStatuses = value;
				OnPropertyChanged(nameof(ConfirmationStatuses));
			}
		}

		private void OnLoadPage(object sender, EventArgs e)
		{
			OriginSystem = WizardContext.ImportOriginSystem;
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			WizardContext.ImportBackupFiles = BackupFiles;
			WizardContext.ImportOverwriteTranslations = OverwriteTranslations;
			WizardContext.ImportOriginSystem = OriginSystem;
			WizardContext.ImportConfirmationStatus = ConfirmationStatus;
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
