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
		private bool _overrideConfirmationStatus;

		public WizardPageImportOptionsViewModel(Window owner, object view, WizardContext wizardContext) : base(owner, view, wizardContext)
		{
			BackupFiles = wizardContext.ImportBackupFiles;
			OverwriteTranslations = wizardContext.ImportOverwriteTranslations;
			OverrideConfirmationStatus = wizardContext.ImportOverrideConfirmationStatus;
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
			IsValid = true;
		}

		public bool BackupFiles { get; set; }

		public bool OverwriteTranslations { get; set; }

		public bool OverrideConfirmationStatus
		{
			get => _overrideConfirmationStatus;
			set
			{
				if (value == _overrideConfirmationStatus)
				{
					return;
				}

				_overrideConfirmationStatus = value;
				OnPropertyChanged(nameof(OverrideConfirmationStatus));
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
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			WizardContext.ImportBackupFiles = BackupFiles;
			WizardContext.ImportOverwriteTranslations = OverwriteTranslations;
			WizardContext.ImportOverrideConfirmationStatus = OverrideConfirmationStatus;
			WizardContext.ImportConfirmationStatus = ConfirmationStatus;
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
