using System;
using System.Windows;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Wizard.ViewModel.BackTranslation
{
	public class WizardPageBackTranslationOptionsViewModel : WizardPageViewModelBase, IDisposable
	{
		private readonly IDialogService _dialogService;
		private bool _copySourceToTargetForEmptyTranslations;
		private bool _copySourceToTargetForEmptyTranslationsEnabled;
		private bool _overwriteExistingBackTranslations;
		private bool _overwriteExistingBackTranslationsEnabled;

		public WizardPageBackTranslationOptionsViewModel(Window owner, object view, TaskContext taskContext, IDialogService dialogService) 
			: base(owner, view, taskContext)
		{
			_dialogService = dialogService;

			CopySourceToTargetForEmptyTranslationsEnabled = false;
			CopySourceToTargetForEmptyTranslations = true; //always true... for now

			OverwriteExistingBackTranslationsEnabled = true;
			OverwriteExistingBackTranslations = taskContext.BackTranslationOptions.OverwriteExistingBackTranslations;

			LoadPage += OnLoadPage;
			LeavePage += OnLeavePage;
		}

		public bool CopySourceToTargetForEmptyTranslations
		{
			get => _copySourceToTargetForEmptyTranslations;
			set
			{
				if (_copySourceToTargetForEmptyTranslations == value)
				{
					return;
				}

				_copySourceToTargetForEmptyTranslations = value;
				OnPropertyChanged(nameof(CopySourceToTargetForEmptyTranslations));

				VerifyIsValid();
			}
		}

		public bool CopySourceToTargetForEmptyTranslationsEnabled
		{
			get => _copySourceToTargetForEmptyTranslationsEnabled;
			set
			{
				if (_copySourceToTargetForEmptyTranslationsEnabled == value)
				{
					return;
				}

				_copySourceToTargetForEmptyTranslationsEnabled = value;
				OnPropertyChanged(nameof(CopySourceToTargetForEmptyTranslationsEnabled));			
			}
		}

		public bool OverwriteExistingBackTranslations
		{
			get => _overwriteExistingBackTranslations;
			set
			{
				if (_overwriteExistingBackTranslations == value)
				{
					return;
				}

				_overwriteExistingBackTranslations = value;
				OnPropertyChanged(nameof(OverwriteExistingBackTranslations));

				VerifyIsValid();
			}
		}

		public string OverwriteExistingBackTranslationsToolTip => PluginResources.ToolTip_Option_OverwreteExistingBackTranslationFiles;

		public bool OverwriteExistingBackTranslationsEnabled
		{
			get => _overwriteExistingBackTranslationsEnabled;
			set
			{
				if (_overwriteExistingBackTranslationsEnabled == value)
				{
					return;
				}

				_overwriteExistingBackTranslationsEnabled = value;
				OnPropertyChanged(nameof(OverwriteExistingBackTranslationsEnabled));
			}
		}

		public override string DisplayName => PluginResources.PageName_Options;

		public override bool IsValid { get; set; }

		private void VerifyIsValid()
		{
			IsValid = true;
		}

		private void OnLoadPage(object sender, EventArgs e)
		{			
			VerifyIsValid();
		}

		private void OnLeavePage(object sender, EventArgs e)
		{
			TaskContext.BackTranslationOptions.CopySourceToTargetForEmptyTranslations = CopySourceToTargetForEmptyTranslations;
			TaskContext.BackTranslationOptions.OverwriteExistingBackTranslations = OverwriteExistingBackTranslations;
		}

		public void Dispose()
		{
			LoadPage -= OnLoadPage;
			LeavePage -= OnLeavePage;
		}
	}
}
