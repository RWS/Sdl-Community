using System.Collections.Generic;

namespace InterpretBank.Interface
{
    public interface IUserInteractionService
    {
        bool Confirm(string message);
        bool GetFilePath(out string filepath, string filter = "All Supported Formats (*.db;*.tbx;*.xlsx)|*.db;*.tbx;*.xlsx|Interpret Bank Database (*.db)|*.db|TermBase eXchange (*.tbx)|*.tbx|Microsoft Excel spreadsheet (*.xlsx)|*.xlsx");
        public string GetGlossaryNameFromUser(List<string> glossaries);
        void WarnUser(string message);
    }
}