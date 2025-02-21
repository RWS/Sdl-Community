using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.ViewModels;
using Multilingual.Excel.FileType.FileType.Views;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.Excel.FileType.FileType.Pages
{
	[FileTypeSettingsPage(
		Id = FiletypeConstants.LanguageMappingId,
		Name = "MultilingualExcelFileType_LanguageMappingPage_Name",
		Description = "MultilingualExcelFileType_LanguageMappingPage_Description",
		HelpTopic = "")]
	public class LanguageMappingPage : AbstractFileTypeSettingsPage<LanguageMappingView, LanguageMappingSettings>
	{
		private LanguageMappingView _languageMappingView;
		private LanguageMappingViewModel _languageMappingViewModel;
		private ImageService _imageService;
		private ColorService _colorService;
		private LanguageFilterService _languageFilterService;

		public override object GetControl()
		{
			if (_languageMappingView != null)
			{
				return _languageMappingView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				_languageMappingView = base.GetControl() as LanguageMappingView;
				if (_languageMappingView != null)
				{
					_imageService = new ImageService();
					_colorService = new ColorService();
					_languageFilterService = new LanguageFilterService();

					_languageMappingViewModel = new LanguageMappingViewModel(Settings, _imageService, _colorService, _languageFilterService);
					_languageMappingView.Loaded += LanguageMappingViewOnLoaded;
				}
			}));

			return _languageMappingView;
		}

		private void LanguageMappingViewOnLoaded(object sender, RoutedEventArgs e)
		{
			_languageMappingView.DataContext = _languageMappingViewModel;
		}

		public override void Save()
		{
			Settings = _languageMappingViewModel.Settings;
			base.Save();
		}

		public override void ResetToDefaults()
		{
			Settings = _languageMappingViewModel.ResetToDefaults();
		}

		public override void Dispose()
		{
			if (_languageMappingView != null)
			{
				_languageMappingView.Loaded -= LanguageMappingViewOnLoaded;
			}

			base.Dispose();
		}
	}
}
