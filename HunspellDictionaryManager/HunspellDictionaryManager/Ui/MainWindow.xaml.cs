using Sdl.Community.HunspellDictionaryManager.Helpers;
using Sdl.Community.HunspellDictionaryManager.ViewModel;
using System;
using System.Windows;

namespace Sdl.Community.HunspellDictionaryManager.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Utils.InitializeWpfApplicationSettings();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            DataContext = new MainWindowViewModel(this);
        }

	    protected override void OnClosed(EventArgs e)
	    {
		    base.OnClosed(e);
		    Application.Current.Shutdown();
	    }
	}
}