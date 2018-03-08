// ---------------------------------
// <copyright file="MainForm.cs" company="SDL International">
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
    /// MainForm class.
    /// </summary>
    public partial class MainForm : Form, IMainView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the MVP presenter.
        /// </summary>
        /// <value>
        /// The presenter.
        /// </value>
        public MainPresenter Presenter { get; set; }

        /// <summary>
        /// Gets or sets the form that owns this form.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Forms.Form"/> that represents the form that is the owner of this form.
        /// </returns>
        /// <exception cref="T:System.Exception">
        /// A top-level window cannot have an owner.
        /// </exception>
        public new IWin32Window Owner
        {
            get
            {
                return this.Owner;
            }
            set
            {
                this.Owner = value;
            }
        }

        /// <summary>
        /// Closes the form and sets appropriateDialogResult.
        /// </summary>
        /// <param name="result">The result.</param>
        public void Close(DialogResult result)
        {
            this.DialogResult = result;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the fileBasedButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnFileBasedButtonClick(object sender, System.EventArgs e)
        {
            if (this.Presenter != null)
            {
                this.Presenter.SelectFileBasedTranslationMemory();
            }
        }

        /// <summary>
        /// Handles the Click event of the serverBasedButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnServerBasedButtonClick(object sender, System.EventArgs e)
        {
            if (this.Presenter != null)
            {
                this.Presenter.SelectServerBasedTranslationMemory();
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the helpLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void OnHelpLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.Presenter != null)
            {
                this.Presenter.ShowOnlineHelp();
            }

            this.helpLinkLabel.LinkVisited = true;
        }

        /// <summary>
        /// Handles the Click event of the cancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCancelButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
