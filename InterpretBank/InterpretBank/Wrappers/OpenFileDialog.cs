using System.Windows.Forms;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.Wrappers
{
	public class OpenFileDialog : IOpenFileDialog
	{
		private System.Windows.Forms.OpenFileDialog Dialog { get; set; } = new();

		public string GetFilePath(string filter = ".db")
		{
			Dialog.Filter = filter;
			return Dialog.ShowDialog() == DialogResult.OK ? Dialog.FileName : "";
		}
	}
}