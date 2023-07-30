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

		public void SetEditing(bool editing)
		{
			SourceTerm_EditableTextBlock.IsEditing = editing;
			SourceTermComment1_EditableTextBlock.IsEditing = editing;
			SourceTermComment2_EditableTextBlock.IsEditing = editing;

			TargetTerm_EditableTextBlock.IsEditing = editing;
			TargetTermComment1_EditableTextBlock.IsEditing = editing;
			TargetTermComment2_EditableTextBlock.IsEditing = editing;

			CommentAll_EditableTextBlock.IsEditing = editing;
		}

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
			SetEditing(true);
		}

		private void SaveChanges_Click(object sender, RoutedEventArgs e)
		{
			SetEditing(false);
		}
	}
}