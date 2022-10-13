using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.ViewModels;
using Multilingual.XML.FileType.FileType.Views;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Pages
{
	[FileTypeSettingsPage(
		Id = Constants.WriterId,
		Name = "MultilingualXMLFileType_WriterPage_Name",
		Description = "MultilingualXMLFileType_WriterPage_Description",
		HelpTopic = "")]
	public class WriterPage : AbstractFileTypeSettingsPage<WriterView, WriterSettings>
	{
		private WriterView _writerView;
		private WriterViewModel _writerViewModel;
		private object lockObject = new object();
		
		public override object GetControl()
		{
			lock (lockObject)
			{
				if (_writerView != null)
				{
					return _writerView;
				}

				Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
				{
					_writerView = base.GetControl() as WriterView;
					if (_writerView != null)
					{
						_writerViewModel = new WriterViewModel(Settings);
						_writerView.Loaded += WriterViewOnLoaded;
					}
				}));
			}

			return _writerView;
		}

		private void WriterViewOnLoaded(object sender, RoutedEventArgs e)
		{
			_writerView.DataContext = _writerViewModel;
		}

		public override void Save()
		{
			Settings = _writerViewModel.Settings;
			base.Save();
		}

		public override void ResetToDefaults()
		{
			Settings = _writerViewModel.ResetToDefaults();
		}

		public override void Dispose()
		{
			if (_writerView != null)
			{
				_writerView.Loaded -= WriterViewOnLoaded;
			}

			base.Dispose();
		}
	}
}
