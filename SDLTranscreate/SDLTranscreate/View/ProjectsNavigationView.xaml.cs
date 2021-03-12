using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Trados.Transcreate.View
{
	/// <summary>
	/// Interaction logic for ProjectsNavigationView.xaml
	/// </summary>
	public partial class ProjectsNavigationView : UserControl, IUIControl
	{
		public ProjectsNavigationView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}
	}
}
