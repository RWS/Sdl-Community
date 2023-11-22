using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.ViewModels;
using Multilingual.XML.FileType.FileType.Views;
using Multilingual.XML.FileType.Services;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Pages
{
	[FileTypeSettingsPage(
		Id = Constants.LanguageMappingId,
		Name = "MultilingualXMLFileType_LanguageMappingPage_Name",
		Description = "MultilingualXMLFileType_LanguageMappingPage_Description",
		HelpTopic = "")]
	public class LanguageMappingPage : AbstractFileTypeSettingsPage<LanguageMappingView, LanguageMappingSettings>
	{
		private LanguageMappingView _languageMappingView;
		private LanguageMappingViewModel _languageMappingViewModel;
		private ImageService _imageService;
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
					_languageFilterService = new LanguageFilterService();

					_languageMappingViewModel = new LanguageMappingViewModel(Settings, _imageService, _languageFilterService);
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
