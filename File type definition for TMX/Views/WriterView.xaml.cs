using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sdl.Community.FileType.TMX.Settings;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Sdl.Community.FileType.TMX.Views
{
	/// <summary>
	/// Interaction logic for WriterView.xaml
	/// </summary>
	public partial class WriterView : UserControl, IUISettingsControl, IFileTypeSettingsAware<WriterSettings>
	{
		public WriterView()
		{
			InitializeComponent();
		}
		public WriterSettings Settings { get; set; }

		public void Dispose()
		{
			
		}

		public bool ValidateChildren()
		{
			return true;
		}

	}
}
