using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Trados.Transcreate.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class ProjectFilesView :  UserControl, IUIControl
	{
		public ProjectFilesView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}
	}
}
