using System.Windows;
using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace StudioViews.View
{
	/// <summary>
	/// Interaction logic for StudioViewsEditorView.xaml
	/// </summary>
	public partial class StudioViewsEditorView : UserControl, IUIControl
	{
		public StudioViewsEditorView()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
		}

		private void UIElement_OnPreviewDragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.All;
			e.Handled = true;
		}
	}
}
