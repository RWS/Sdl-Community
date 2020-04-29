using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class SelectServersView
	{
		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private const int GWL_STYLE = -16;

		private const int WS_MAXIMIZEBOX = 0x10000; //maximize button
		private const int WS_MINIMIZEBOX = 0x20000; //minimize button

		public SelectServersView()
		{
			InitializeComponent();
			Visibility = Visibility.Visible;
			SourceInitialized += MainWindow_SourceInitialized;
		}
		private IntPtr _windowHandle;
		private void MainWindow_SourceInitialized(object sender, System.EventArgs e)
		{
			_windowHandle = new WindowInteropHelper(this).Handle;

			//disable minimize button
			DisableMinimizeButton();
		}

		protected void DisableMinimizeButton()
		{
			if (_windowHandle == IntPtr.Zero)
				throw new InvalidOperationException("The window has not yet been completely initialized");

			SetWindowLong(_windowHandle, GWL_STYLE, GetWindowLong(_windowHandle, GWL_STYLE) & ~WS_MINIMIZEBOX);
		}

		public void Refresh()
		{
			var collectionView = CollectionViewSource.GetDefaultView(DataGridServers.ItemsSource);
			collectionView.Refresh();
		}

		private void OKButton_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
