using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.DsiViewer.View
{
	/// <summary>
	/// Interaction logic for SdiWpfControl.xaml
	/// </summary>
	public partial class DsiViewerView : UserControl, IUIControl
    {
        public DsiViewerView()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
        }
    }
}