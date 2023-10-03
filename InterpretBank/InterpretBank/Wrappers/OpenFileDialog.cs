using System.Windows.Forms;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.Wrappers
{
    public class Dialog : IDialog
    {
        public string GetFilePath(string filter = "All Supported Formats (*.db;*.tbx;*.xlsx)|*.db;*.tbx;*.xlsx|Interpret Bank Database (*.db)|*.db|TermBase eXchange (*.tbx)|*.tbx|Microsoft Excel spreadsheet (*.xlsx)|*.xlsx")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = filter };
            var showDialog = dialog.ShowDialog();
            return showDialog.HasValue
                ? showDialog.Value
                    ? dialog.FileName
                    : ""
                : "";
        }

        public bool Confirm(string message)
        {
            var dialogResult = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo);
            return dialogResult == DialogResult.Yes;
        }
    }
}