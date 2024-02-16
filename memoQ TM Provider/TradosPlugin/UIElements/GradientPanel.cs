using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TradosPlugin
{
    public class GradientPanel : Panel
    {
        public enum GradientModes
        {
            None,
            TwoPoint,
            ThreePoint
        }

        private GradientModes gradientMode = GradientModes.None;
        private Orientation gradientOrientation = Orientation.Horizontal;
        private Color backStartColor = SystemColors.ControlDark;
        private Color backEndColor = SystemColors.ControlLight;

        public GradientPanel()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        [Category("Appearance")]
        [Description("No gradient: BackgroundColor is used; TwoPoint: Start and End colors are used; ThreePoint: middle color is BackgroundColor")]
        public GradientModes GradientMode
        {
            get { return gradientMode; }
            set { gradientMode = value; }
        }

        [Category("Appearance")]
        [Description("Start color (left or top) of the background gradient")]
        public Color BackStartColor
        {
            get { return backStartColor; }
            set { backStartColor = value; }
        }

        [Category("Appearance")]
        [Description("End color (right or bottom) of the background gradient")]
        public Color BackEndColor
        {
            get { return backEndColor; }
            set { backEndColor = value; }
        }

        [Category("Appearance")]
        [Description("Defines whether gradient goes left to right or top to bottom")]
        public Orientation GradientOrientation
        {
            get { return gradientOrientation; }
            set { gradientOrientation = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        private void doPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // No gradient mode: just a solid background
            if (gradientMode == GradientModes.None)
            {
                using (Brush b = new SolidBrush(BackColor))
                {
                    g.FillRectangle(b, new Rectangle(0, 0, Width, Height));
                }
                return;
            }
            // Paint gradient: horizontal
            if (gradientOrientation == Orientation.Horizontal)
            {
                // Two-point gradient
                if (gradientMode == GradientModes.TwoPoint)
                {
                    using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(-1, 0, Width + 1, Height), backStartColor, backEndColor, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(b, new Rectangle(0, 0, Width, Height));
                    }
                }
                // Three-point gradient
                else
                {
                    int mid = Width / 2;
                    using (LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(-1, 0, mid + 1, Height), backStartColor, BackColor, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(b1, new Rectangle(0, 0, mid, Height));
                    }
                    int rw = Width - mid;
                    using (LinearGradientBrush b2 = new LinearGradientBrush(new Rectangle(mid - 1, 0, rw + 1, Height), BackColor, backEndColor, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(b2, new Rectangle(mid, 0, rw, Height));
                    }
                }
            }
            // Paint gradient: vertical
            else
            {
                // Two-point gradient
                if (gradientMode == GradientModes.TwoPoint)
                {
                    using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(0, -1, Width, Height + 1), backStartColor, backEndColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(b, new Rectangle(0, 0, Width, Height));
                    }
                }
                // Three-point gradient
                else
                {
                    int mid = Height / 2;
                    using (LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(0, -1, Width, mid + 1), backStartColor, BackColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(b1, new Rectangle(0, 0, Width, mid));
                    }
                    int rw = Height - mid;
                    using (LinearGradientBrush b2 = new LinearGradientBrush(new Rectangle(0, mid - 1, Width, rw + 1), BackColor, backEndColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(b2, new Rectangle(0, mid, Width, rw));
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // NOP
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            doPaint(e);
        }
    }
}
