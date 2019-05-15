using System.Security.Policy;
using System.Windows.Controls;
using Sdl.Community.GSVersionFetch.Commands;

namespace Sdl.Community.GSVersionFetch.View
{
	/// <summary>
	/// Interaction logic for LoginView.xaml
	/// </summary>
	public partial class LoginView : UserControl
	{
		public LoginView()
		{
			InitializeComponent();
			//ClearCommand = new DelegateCommand(ExecuteClearCommand, CanExecuteClearCommand);

		}

		//public DelegateCommand ClearCommand { get; }

		//private bool CanExecuteClearCommand()
		//{
		//	return true;
		//}

		//private void ExecuteClearCommand()
		//{
			
		//}
	}
}
