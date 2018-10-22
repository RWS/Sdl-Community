using System.Windows.Controls;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class LogViewControl : UserControl
	{
		public LogViewControl(MainViewModel model)
		{			
			InitializeComponent();

			DataContext = model;
		}	
	}
}
