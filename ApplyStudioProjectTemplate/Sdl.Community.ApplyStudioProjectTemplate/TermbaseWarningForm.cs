// -----------------------------------------------------------------------
// <copyright file="TermbaseWarningForm.cs" company="SDL plc">
// © 2014 SDL plc
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Forms;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The terminology warning form
    /// </summary>
    public partial class TermbaseWarningForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermbaseWarningForm"/> class.
        /// </summary>
        public TermbaseWarningForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets a value indicating whether to show this dialog again.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we should show this dialog again; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAgain
        {
            get
            {
                return !this.DontShow.Checked;
            }
        }
    }
}
