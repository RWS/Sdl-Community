using System.Drawing;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.MTConnectors.Google.UI
{
    /// <summary>
    /// WizardTitleControl class is responsible for representing the wizard title control.
    /// </summary>
    public partial class WizardTitleControl : UserControl
    {
        private Bitmap _titleBitmap;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WizardTitleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TitleText property represents the title text.
        /// </summary>
        public string TitleText
        {
            get
            {
                return _titleLabel.Text;
            }
            set
            {
                _titleLabel.Text = value;
            }
        }

        /// <summary>
        /// SubTitleText property represents the sub-title text.
        /// </summary>
        public string SubTitleText
        {
            get
            {
                return _subtitleLabel.Text;
            }
            set
            {
                _subtitleLabel.Text = value;
            }
        }

        /// <summary>
        /// TitleIcon property represents the title icon.
        /// </summary>
        public System.Drawing.Bitmap TitleBitmap
        {
            get
            {
                return _titleBitmap;
            }
            set
            {
                Bitmap oldTitleBitmap = _titleBitmap;
                try
                {
                    _titleBitmap = value;
                    if (_titleBitmap != null)
                    {
                        _pictureBox.Image = _titleBitmap;
                    }
                    else
                    {
                        _pictureBox.Image = null;
                    }
                }
                finally
                {
                    if (oldTitleBitmap != null)
                    {
                        oldTitleBitmap.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// ShowLine property determines whether the line should be shown.
        /// </summary>
        public bool ShowLine
        {
            get
            {
                return _lineFrame.Visible;
            }
            set
            {
                _lineFrame.Visible = value;
            }
        }
    }
}