using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class SelectServersWindow
	{
		public SelectServersWindow()
		{
			InitializeComponent();
			Visibility = Visibility.Visible;			
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
