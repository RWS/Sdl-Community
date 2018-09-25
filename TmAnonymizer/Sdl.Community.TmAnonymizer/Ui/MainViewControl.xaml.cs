using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.Ui
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class MainViewControl 
	{
		public MainViewControl(SettingsService settingsService)
		{
			InitializeComponent();
			DataContext = new MainViewModel(settingsService);
		}

		private void ParentGrid_OnPreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Down)
			{
				ScrollViewer.LineDown();
			}
			if (e.Key == Key.Up)
			{
				ScrollViewer.LineUp();
			}
		}
	}
}
