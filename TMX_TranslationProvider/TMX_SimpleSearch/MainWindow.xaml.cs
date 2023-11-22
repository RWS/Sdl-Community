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

namespace TMX_SimpleSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
	    private MainWindowViewModel vm => DataContext as MainWindowViewModel;
        public MainWindow()
        {
            InitializeComponent();
			vm.PropertyChanged += Vm_PropertyChanged;
        }

		private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
		}

		private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			await vm.InitAsync();
			searchText.Focus();
		}

		private async void search_click(object sender, RoutedEventArgs e)
		{
			if (searchText.Text == "")
				return;
			this.IsEnabled = false;
			await vm.DoSearchNow();
			this.IsEnabled = true;
		}

		private void SearchText_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
				search_click(null,null);
		}
    }
}
