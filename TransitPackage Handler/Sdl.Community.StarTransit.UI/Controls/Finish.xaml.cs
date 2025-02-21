using System.Windows.Controls;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI.Controls
{
	/// <summary>
	/// Interaction logic for Finish.xaml
	/// </summary>
	public partial class Finish : UserControl
	{
		public Finish(FinishViewModel finishViewModel)
		{
			DataContext = finishViewModel;
			InitializeComponent();
		}
	}
}