using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Reports.Viewer.Plus.ViewModel;

namespace Reports.Viewer.Plus.View
{	
	public partial class AppendTemplateWindow : Window
	{
		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private const int GWL_STYLE = -16;

		private const int WS_MAXIMIZEBOX = 0x10000; //maximize button
		private const int WS_MINIMIZEBOX = 0x20000; //minimize button

		private readonly AppendTemplateViewModel _model;

		public AppendTemplateWindow(AppendTemplateViewModel model, Window parentWindow)
		{			
			InitializeComponent();

			_model = model;

			if (parentWindow == null)
			{
				var windowInteropHelper = new WindowInteropHelper(this);
				windowInteropHelper.Owner = ApplicationInstance.GetActiveForm().Handle;
			}
			else
			{
				Owner = parentWindow;
			}

			SourceInitialized += MainWindow_SourceInitialized;

			Loaded += AppendTemplateWindow_Loaded;
		}

		private void AppendTemplateWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= AppendTemplateWindow_Loaded;
			DataContext = _model;
		}

		private IntPtr _windowHandle;
		private void MainWindow_SourceInitialized(object sender, EventArgs e)
		{
			_windowHandle = new WindowInteropHelper(this).Handle;

			DisableWindowButtons();
		}

		protected void DisableWindowButtons()
		{
			if (_windowHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException("The window has not yet been completely initialized");
			}

			SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
