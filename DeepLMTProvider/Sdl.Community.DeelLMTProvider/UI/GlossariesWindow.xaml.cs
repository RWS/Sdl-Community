using Sdl.Community.DeepLMTProvider.Model;
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

namespace Sdl.Community.DeepLMTProvider.UI
{
	/// <summary>
	/// Interaction logic for GlossaryImportWindow.xaml
	/// </summary>
	public partial class GlossariesWindow : Window
	{
		public GlossariesWindow()
		{
			InitializeComponent();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			// Check if the pressed key is the "Escape" key (key code 27)
			if (e.Key == Key.Escape)
			{
				// Close the window
				Close();
			}
		}
    }
}
