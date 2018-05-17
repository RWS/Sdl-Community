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
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.ViewModel;

namespace Sdl.Community.TmAnonymizer.Ui
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class AcceptWindow 
	{
		public AcceptWindow()
		{
			InitializeComponent();
			DataContext = new AcceptWindowViewModel();
		}
	}
}
