using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.SDLXLIFFSliceOrChange.User_Controls
{
    public partial class NoLabelTextBox : TextBox
    {
        private string _labelText;

        private bool _handleSetText = true;
        public string SDLText
        {
            get
            {
                return Text == LabelText ? String.Empty : Text;
            }
            set
            {
                if (value == string.Empty)
                {
                    Text = LabelText;
                    ForeColor = SystemColors.InactiveCaption;
                    Font = new Font(Font.FontFamily, Font.Size, FontStyle.Italic);
                }
                else
                {
                    Text = value;
                    ForeColor = SystemColors.ControlText;
                    Font = new Font(Font.FontFamily, Font.Size, FontStyle.Regular);
                }
            }
        }

        public String LabelText
        {
            get { return _labelText; }
            set
            {
                if (Text == String.Empty || Text == LabelText)
                {
                    _labelText = value; 
                    Text = value;
                    ForeColor = SystemColors.InactiveCaption;
                    Font = new Font(Font.FontFamily, Font.Size, FontStyle.Italic);
                }
            } 
        }

        public NoLabelTextBox()
        {
            InitializeComponent();
            this.GotFocus += NoLabelTextBox_GotFocus;
            this.LostFocus += NoLabelTextBox_LostFocus;
        }

        void NoLabelTextBox_LostFocus(object sender, EventArgs e)
        {
            if (Text == String.Empty)
            {
                Text = LabelText;
                ForeColor = SystemColors.InactiveCaption;
                Font = new Font(Font.FontFamily, Font.Size, FontStyle.Italic);
            }
        }

        void NoLabelTextBox_GotFocus(object sender, EventArgs e)
        {
            if (Text == LabelText)
            {
                Text = String.Empty;
                ForeColor = SystemColors.ControlText;
                Font = new Font(Font.FontFamily, Font.Size, FontStyle.Regular);
            }
        }
    }
}
