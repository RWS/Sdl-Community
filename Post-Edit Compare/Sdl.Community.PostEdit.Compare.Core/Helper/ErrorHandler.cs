using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
    public static class ErrorHandler
    {
        public static void AddVariable(this List<string> variableList, string name, string value) =>
            variableList.Add($"{name}: {value}\r\n");

        public static void ShowError(Exception ex, IWin32Window owner = null, List<string> variableValues = null,
            [CallerMemberName] string callingMethod = "") =>
            MessageBox.Show(owner,
                $"{ex.Message}. " +
                $"({ex.InnerException?.Message}) - " +
                $"{ex.StackTrace}." +
                Environment.NewLine + 
                Environment.NewLine +
                $"{GetVariablesString(variableValues)}",
                callingMethod,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        public static void ShowError(string message, IWin32Window owner = null, [CallerMemberName] string callingMethod = "") => MessageBox.Show(owner,
            message,
            callingMethod,
            MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static string GetVariablesString(List<string> variableValues)
        {
            var explicitErrors = ErrorHandlerSettingsSerializer.ReadSettings().ExplicitErrors;

            var variablesString = "";
            if (variableValues is not null && variableValues.Any() && explicitErrors)
                variablesString += string.Join("\r\n", variableValues);

            return variablesString;
        }


        private static string LogFilePath => Path.Combine(SharedStrings.PostEditCompareSettingsFolder, "ErrorLog.txt");

        public static void LogError(Exception exception, List<string> variableValues = null)
        {
            File.AppendAllText(LogFilePath, "");

            var alreadyExisting = File.ReadAllText(LogFilePath);

            var errorMessage = $"<{DateTime.Now} ERROR>";
            errorMessage += Environment.NewLine +
                          $"{exception.Message}. " +
                          Environment.NewLine +
                          $"{exception.InnerException?.Message}" +
                          Environment.NewLine +
                          $"{exception.StackTrace}" +
                          Environment.NewLine +
                          $"{GetVariablesString(variableValues)}";
            errorMessage += "<\\ERROR>" +
                            Environment.NewLine +
                            Environment.NewLine;


            File.WriteAllText(LogFilePath, errorMessage + alreadyExisting);
        }
        
        public static void Log(string message, List<string> variableValues = null)
        {
            File.AppendAllText(LogFilePath, "");

            var alreadyExisting = File.ReadAllText(LogFilePath);

            var infoMessage = $"<{DateTime.Now} INFO>";
            infoMessage += Environment.NewLine +
                          $"{message}. " +
                          Environment.NewLine +
                          $"{GetVariablesString(variableValues)}";
            infoMessage += $"<\\INFO>" +
                           Environment.NewLine +
                           Environment.NewLine;

            File.WriteAllText(LogFilePath, infoMessage + alreadyExisting);
        }
    }
}