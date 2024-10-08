﻿using System;
using System.Collections.Generic;

namespace InterpretBank.Interface
{
    public interface IUserInteractionService
    {
        event Action<string, string, string> GotTermDetailsEvent;

        bool Confirm(string message);

        bool GetFilePath(out string filepath, string filter = "All Supported Formats (*.db;*.tbx;*.xlsx)|*.db;*.tbx;*.xlsx|Interpret Bank Database (*.db)|*.db|TermBase eXchange (*.tbx)|*.tbx|Microsoft Excel spreadsheet (*.xlsx)|*.xlsx");

        public bool GetInfoFromUser(out string info, string request);

        public void GetNewTermDetailsFromUser(List<string> glossaries, string sourceLanguage, string targetLanguage,
                    string sourceTerm, string targetTerm);

        void WarnUser(string message);
    }
}