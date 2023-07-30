using System.Windows;
using System.Windows.Controls;

namespace InterpretBank.TermbaseViewer.UI
{
	/// <summary>
	/// Interaction logic for TermbaseViewer.xaml
	/// </summary>
	public partial class TermbaseViewer : UserControl
	{
		public TermbaseViewer()
		{
			InitializeComponent();
		}


		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			SourceTerm_TextBlock.Visibility = Visibility.Collapsed;
			SourceTermComment1_TextBlock.Visibility = Visibility.Collapsed;
			SourceTermComment2_TextBlock.Visibility = Visibility.Collapsed;

			TargetTerm_TextBlock.Visibility = Visibility.Collapsed;
			TargetTermComment1_TextBlock.Visibility = Visibility.Collapsed;
			TargetTermComment2_TextBlock.Visibility = Visibility.Collapsed;

			CommentAll_TextBlock.Visibility = Visibility.Collapsed;

			
		}
	}
}
