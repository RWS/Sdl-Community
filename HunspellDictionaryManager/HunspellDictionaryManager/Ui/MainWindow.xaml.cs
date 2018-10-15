using System.Windows;
using Sdl.Community.HunspellDictionaryManager.ViewModel;

namespace Sdl.Community.HunspellDictionaryManager.Ui
{
	/// <summary>
	/// Interaction logic for MainViewWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MainWindowViewModel();
		}
	}
}
