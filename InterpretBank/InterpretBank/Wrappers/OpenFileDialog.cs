using System.Windows.Forms;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.Wrappers
{
	public class Dialog : IDialog
	{
		public string GetFilePath(string filter = "Interpret Bank Databases|*.db")
		{
			using var dialog = new System.Windows.Forms.OpenFileDialog();
			dialog.Filter = filter;
			return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : "";
		}

		public bool Confirm(string message)
		{
			var dialogResult = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo);
			return dialogResult == DialogResult.Yes;
		}
	}
}