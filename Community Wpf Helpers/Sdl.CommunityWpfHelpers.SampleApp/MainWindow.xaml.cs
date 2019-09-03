using System;
using Sdl.CommunityWpfHelpers.Services;

namespace Sdl.CommunityWpfHelpers.SampleApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow 
	{
		public MainWindow()
		{
			InitializeComponent();
			var messageBoxService = new MessageBoxService();
			DataContext = new MainWindowViewModel(messageBoxService);
		}
	}
}
