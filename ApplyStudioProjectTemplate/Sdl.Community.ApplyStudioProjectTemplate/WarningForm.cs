using System.Windows.Forms;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The terminology warning form
    /// </summary>
    public partial class WarningForm : Form
    {
        public WarningForm(string text)
        {
            InitializeComponent();
            WarningText.Text = text;
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
                return !DontShow.Checked;
            }
        }
    }
}
