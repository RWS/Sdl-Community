using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
			PreviewKeyDown += TermbaseViewer_KeyUp;
		}

		private void TermbaseViewer_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SetEditing(false);
			}

			
		}


		public void SetEditing(bool editing)
		{
			((TermModel)Term_ListBox.SelectedItem).IsEditing = editing;
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			SetEditing(true);
		}

		private void SaveEdit_Button_OnClick(object sender, RoutedEventArgs e)
		{
			SetEditing(false);
		}
	}
}