using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Trados.Transcreate.View
{
	/// <summary>
	/// Interaction logic for ProjectFileActivitiesView.xaml
	/// </summary>
	public partial class ProjectFileActivityView : UserControl, IUIControl
	{
		public ProjectFileActivityView()
		{
			InitializeComponent();
		}

		
		public void Dispose()
		{
		}
	}
}
