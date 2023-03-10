using System.Windows;
using System.Windows.Controls;
using Sdl.Community.MTEdge.Provider.ViewModel;

namespace Sdl.Community.MTEdge.Provider.View
{
	/// <summary>
	/// Interaction logic for LanguageMappingView.xaml
	/// </summary>
	public partial class LanguageMappingView : UserControl
	{
		public LanguageMappingView()
		{
			var x = DataContext as LanguageMappingViewModel;
			InitializeComponent();
			var y = DataContext as LanguageMappingViewModel;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var z = DataContext as LanguageMappingViewModel;
		}
	}
}