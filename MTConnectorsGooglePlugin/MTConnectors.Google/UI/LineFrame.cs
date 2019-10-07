using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.MTConnectors.Google.UI
{

    public class LineFrame : System.Windows.Forms.UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public LineFrame()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Visible)
            {
                Graphics g = pe.Graphics;
                using (Pen p = new Pen(ForeColor))
                {
                    g.DrawLine(p, new Point(0, 0), new Point(Width, 0));
                }
                using (Pen p = new Pen(BackColor))
                {
                    g.DrawLine(p, new Point(0, 1), new Point(Width, 1));
                }
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        [DefaultValue(typeof(Size), "75, 2")]
        protected new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Gets/Sets the width of the line")]
        public new int Width
        {
            get
            {
                return Size.Width;
            }
            set
            {
                Size = new Size(value, 2);
            }
        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LineFrame
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Name = "LineFrame";
            this.Size = new System.Drawing.Size(150, 2);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
