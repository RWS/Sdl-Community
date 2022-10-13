using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.ViewModels;
using Multilingual.XML.FileType.FileType.Views;
using Multilingual.XML.FileType.Providers;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Pages
{
	[FileTypeSettingsPage(
		Id = Constants.CommentMappingId,
		Name = "MultilingualXMLFileType_CommentMappingPage_Name",
		Description = "MultilingualXMLFileType_CommentMappingPage_Description",
		HelpTopic = "")]
	public class CommentMappingsPage : AbstractFileTypeSettingsPage<CommentMappingView, CommentMappingSettings>
	{
		private CommentMappingView _commentMappingView;
		private CommentMappingViewModel _commentMappingViewModel;
		
		public override object GetControl()
		{
			if (_commentMappingView != null)
			{
				return _commentMappingView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				_commentMappingView = base.GetControl() as CommentMappingView;
				if (_commentMappingView != null)
				{
					_commentMappingViewModel = new CommentMappingViewModel(Settings, new StudioCommentPropertyProvider());
					_commentMappingView.Loaded += CommentMappingViewOnLoaded;
				}
			}));

			return _commentMappingView;
		}

		private void CommentMappingViewOnLoaded(object sender, RoutedEventArgs e)
		{
			_commentMappingView.DataContext = _commentMappingViewModel;
		}

		public override void Save()
		{
			Settings = _commentMappingViewModel.Settings;
			base.Save();
		}

		public override void ResetToDefaults()
		{
			Settings = _commentMappingViewModel.ResetToDefaults();
		}

		public override void Dispose()
		{
			if (_commentMappingView != null)
			{
				_commentMappingView.Loaded -= CommentMappingViewOnLoaded;
			}

			base.Dispose();
		}
	}
}
