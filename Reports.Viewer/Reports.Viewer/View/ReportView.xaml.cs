using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.Reports.Viewer.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class ReportView : UserControl, IUIControl
	{
		public ReportView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}
	}
}
