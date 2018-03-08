using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    public static class MessagingHelpers
    {
        public static void ShowError(string text)
        {
            ShowErrorImpl(null, text);
        }

        public static void ShowError(Exception ex)
        {
            ShowErrorImpl(null, ex.Message);
        }

        public static void ShowError(Exception ex, string text)
        {
            ShowErrorImpl(null, String.Format("{0}{1}{2}", text, Environment.NewLine, ex.Message));
        }

        private static void ShowErrorImpl(IWin32Window owner, string text)
        {
            MessageBox.Show(owner, text, PluginResources.MessageBox_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarning(string text)
        {
            ShowWarning(null, text);
        }

        public static void ShowWarning(IWin32Window owner, string text)
        {
            MessageBox.Show(owner, text, PluginResources.MessageBox_WarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInformation(string text)
        {
            ShowInformation(null, text);
        }

        public static void ShowInformation(IWin32Window owner, string text)
        {
            MessageBox.Show(owner, text, PluginResources.MessageBox_InformationTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult AskYesNoQuestion(IWin32Window owner, string question)
        {
            return MessageBox.Show(owner, question, PluginResources.MessageBox_QuestionTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }


    }
}
