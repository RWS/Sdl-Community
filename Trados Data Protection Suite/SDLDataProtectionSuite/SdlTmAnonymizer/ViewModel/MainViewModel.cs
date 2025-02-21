﻿using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private TranslationMemoryViewModel _translationMemoryViewModel;
		private int _selectedTabIndex;

		public MainViewModel(SettingsService settingsService, SDLTMAnonymizerView controller, GroupshareCredentialManager groupShareCredentialManager)
		{
			var contentParsingService = new ContentParsingService();
			var serializerService = new SerializerService();
			_translationMemoryViewModel = new TranslationMemoryViewModel(settingsService, contentParsingService, serializerService, controller, groupShareCredentialManager);

			var excelImportExportService = new ExcelImportExportService();
			var systemFieldsService = new SystemFieldsService(_translationMemoryViewModel.TmService, settingsService);
			var customFieldsService = new CustomFieldsService(_translationMemoryViewModel.TmService, settingsService);

			ContentFilteringRulesViewModel = new ContentFilteringRulesViewModel(_translationMemoryViewModel, excelImportExportService, groupShareCredentialManager);
			SystemFieldsViewModel = new SystemFieldsViewModel(_translationMemoryViewModel, systemFieldsService, excelImportExportService, serializerService, groupShareCredentialManager);
			CustomFieldsViewModel = new CustomFieldsViewModel(_translationMemoryViewModel, customFieldsService, excelImportExportService, serializerService, groupShareCredentialManager);

			LogViewModel = new LogViewModel(_translationMemoryViewModel, serializerService, excelImportExportService);		
		}

		public TranslationMemoryViewModel TranslationMemoryViewModel
		{
			get => _translationMemoryViewModel;
			set
			{
				_translationMemoryViewModel = value;

				OnPropertyChanged(nameof(TranslationMemoryViewModel));
			}
		}

		public ContentFilteringRulesViewModel ContentFilteringRulesViewModel { get; set; }

		public SystemFieldsViewModel SystemFieldsViewModel { get; set; }

		public CustomFieldsViewModel CustomFieldsViewModel { get; set; }

		public LogViewModel LogViewModel { get; set; }

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged(nameof(SelectedTabIndex));
			}
		}
	}
}
