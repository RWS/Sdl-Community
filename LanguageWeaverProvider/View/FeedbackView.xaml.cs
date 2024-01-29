using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class FeedbackView : UserControl, IUIControl
	{
        public FeedbackView()
        {
            InitializeComponent();
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Window.GetWindow(this) is not Window window)
			{
				return;
			}

			window.DragMove();
		}

		public void Dispose() { }
	}
}