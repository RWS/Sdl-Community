using InterpretBank.Interface;
using InterpretBank.TermbaseViewer.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InterpretBank.CommonServices
{
    public class UserInteractionService : IUserInteractionService
    {
        private static UserInteractionService _dialogService;
        public static UserInteractionService Instance { get; set; } = _dialogService ??= new UserInteractionService();
        public bool Confirm(string message)
        {
            var dialogResult = MessageBox.Show(message, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dialogResult == DialogResult.Yes;
        }

        public bool GetFilePath(out string filepath, string filter = "All Supported Formats (*.db;*.tbx;*.xlsx)|*.db;*.tbx;*.xlsx|Interpret Bank Database (*.db)|*.db|TermBase eXchange (*.tbx)|*.tbx|Microsoft Excel spreadsheet (*.xlsx)|*.xlsx")
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = filter };
            var showDialog = dialog.ShowDialog();

            filepath = dialog.FileName;
            return showDialog.HasValue && showDialog.Value;
        }

        public string GetGlossaryNameFromUser(List<string> glossaries)
        {
            var chooseGlossaryWindow = new ChooseGlossaryWindow(glossaries);
            return chooseGlossaryWindow.ShowDialog() ?? false ? chooseGlossaryWindow.SelectedGlossary : null;
        }

        public void WarnUser(string message) => MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}