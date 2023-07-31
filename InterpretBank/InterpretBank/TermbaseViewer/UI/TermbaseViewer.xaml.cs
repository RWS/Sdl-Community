using System.Windows;
using System.Windows.Controls;
using InterpretBank.TermbaseViewer.Model;

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

		public void SetEditing(bool editing)
		{
			((TermModel)Term_ListBox.SelectedItem).IsEditing = editing;
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			SetEditing(true);
		}

		private void StopEdit_Button_OnClick(object sender, RoutedEventArgs e)
		{
			SetEditing(false);
		}
	}
}