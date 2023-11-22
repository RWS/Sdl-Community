using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.ViewModels;
using Multilingual.Excel.FileType.FileType.Views;
using Multilingual.Excel.FileType.Providers.PlaceholderPatterns;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.Excel.FileType.FileType.Pages
{
	[FileTypeSettingsPage(
		Id = FiletypeConstants.PlaceholdersId,
		Name = "MultilingualExcelFileType_PlaceholdersPage_Name",
		Description = "MultilingualExcelFileType_PlaceholdersPage_Description",
		HelpTopic = "")]
	public class PlaceholderPatternsPage : AbstractFileTypeSettingsPage<PlaceholderPatternsView, PlaceholderPatternSettings>
	{
		private PlaceholderPatternsView _placeholderPatternsView;
		private PlaceholderPatternsViewModel _placeholderPatternsViewModel;
		
		public override object GetControl()
		{
			if (_placeholderPatternsView != null)
			{
				return _placeholderPatternsView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				_placeholderPatternsView = base.GetControl() as PlaceholderPatternsView;
				if (_placeholderPatternsView != null)
				{
					_placeholderPatternsViewModel = new PlaceholderPatternsViewModel(Settings, new PlaceholderPatternsProvider());
					_placeholderPatternsView.Loaded += PlaceholderPatternsViewOnLoaded;
				}
			}));

			return _placeholderPatternsView;
		}

		private void PlaceholderPatternsViewOnLoaded(object sender, RoutedEventArgs e)
		{
			_placeholderPatternsView.DataContext = _placeholderPatternsViewModel;
		}

		public override void Save()
		{
			Settings = _placeholderPatternsViewModel.Settings;
			base.Save();
		}

		public override void ResetToDefaults()
		{
			Settings = _placeholderPatternsViewModel.ResetToDefaults();
		}

		public override void Dispose()
		{
			if (_placeholderPatternsView != null)
			{
				_placeholderPatternsView.Loaded -= PlaceholderPatternsViewOnLoaded;
			}

			base.Dispose();
		}
	}
}
