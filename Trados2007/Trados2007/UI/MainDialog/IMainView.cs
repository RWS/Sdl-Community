// ---------------------------------
// <copyright file="IMainView.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-01</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System.Windows.Forms;

    /// <summary>
    /// Decorator interface for MainForm\MainDialog
    /// </summary>
    public interface IMainView : IWin32Window
    {
        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// OK or Cancel
        /// </returns>
        DialogResult ShowDialog(IWin32Window owner);

        /// <summary>
        /// Closes the form and sets appropriateDialogResult.
        /// </summary>
        /// <param name="result">The result.</param>
        void Close(DialogResult result);
    }
}