using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Sdl.Community.DeepLMTProvider.UI.Controls
{
	/// <summary>
	/// Interaction logic for BrowsePathControl.xaml
	/// </summary>
	public partial class BrowsePathControl : UserControl
	{
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(nameof(Text), typeof(string), typeof(BrowsePathControl),
				new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public BrowsePathControl()
		{
			InitializeComponent();
		}

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var importDialog = new OpenFileDialog
			{
				Filter = "TSV files (*.tsv)|*.tsv|All files (*.*)|*.*",
				Title = "Select TSV file"
			};

			if (importDialog.ShowDialog() == true)
			{
				Text = importDialog.FileName;
			}
		}
	}
}