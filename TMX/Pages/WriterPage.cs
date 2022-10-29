using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using System.Xml.Linq;
using Sdl.Community.FileType.TMX.Settings;
using Sdl.Community.FileType.TMX.ViewModels;
using Sdl.Community.FileType.TMX.Views;

namespace Sdl.Community.FileType.TMX.Pages
{
	[FileTypeSettingsPage(
		Id = Constants.WriterId,
		Name = "TMX_WriterPage_Name",
		Description = "TMX_WriterPage_Description",
		HelpTopic = "")]
	internal class WriterPage : AbstractFileTypeSettingsPage<WriterView, WriterSettings>
	{
		private WriterView _view;
		public WriterViewModel ViewModel => (_view.DataContext as WriterViewModel);

		public override object GetControl()
		{
			if (_view != null)
				return _view;

			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				_view = new WriterView
				{
					DataContext = new WriterViewModel(Settings),
				};
			});

			return _view;
		}

		public override void Save()
		{
			Settings = ViewModel.Settings;
			base.Save();
		}

		public override void ResetToDefaults()
		{
			Settings = ViewModel.ResetToDefaults();
		}
	}
}
