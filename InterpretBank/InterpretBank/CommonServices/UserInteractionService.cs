using InterpretBank.Interface;
using InterpretBank.SettingsService.UI;
using InterpretBank.TermbaseViewer.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace InterpretBank.CommonServices
{
    public class UserInteractionService : IUserInteractionService
    {
        public event Action<string, string, string> GotTermDetailsEvent;

        public AddTermPopup ChooseGlossaryWindow { get; private set; }

        //private static UserInteractionService _dialogService;
        //public static UserInteractionService Instance { get; set; } = _dialogService ??= new UserInteractionService();
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

        public bool GetInfoFromUser(out string info, string request)
        {
            var createGlossaryWindow = new CreateGlossaryWindow { Text = request };
            var result = createGlossaryWindow.ShowDialog();

            info = createGlossaryWindow.InputTextBox.Text;
            return result ?? false;
        }

        public void GetNewTermDetailsFromUser(List<string> glossaries, string sourceLanguage, string targetLanguage,
                    string sourceTerm, string targetTerm)
        {
            ChooseGlossaryWindow = new AddTermPopup
            {
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage,
                SourceTerm = sourceTerm,
                TargetTerm = targetTerm,
                Glossaries = glossaries,
                SelectedGlossary = glossaries[0]
            };

            ChooseGlossaryWindow.Closed += ChooseGlossaryWindow_Closed;

            ElementHost.EnableModelessKeyboardInterop(ChooseGlossaryWindow);
            ChooseGlossaryWindow.Show();
        }

        public void WarnUser(string message) => MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void ChooseGlossaryWindow_Closed(object sender, EventArgs e)
        {
            ChooseGlossaryWindow.Closed -= ChooseGlossaryWindow_Closed;
            if (!ChooseGlossaryWindow.TermAdded) return;

            GotTermDetailsEvent?.Invoke(ChooseGlossaryWindow.SourceTerm,
                ChooseGlossaryWindow.TargetTerm, ChooseGlossaryWindow.SelectedGlossary);

            ChooseGlossaryWindow = null;
        }
    }
}