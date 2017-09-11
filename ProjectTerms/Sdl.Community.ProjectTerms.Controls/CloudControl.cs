using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Controls.Geometry;
using Sdl.Community.ProjectTerms.Controls.Interfaces;

namespace Sdl.Community.ProjectTerms.Controls
{
    public class CloudControl : Panel
    {
        private IEnumerable<ITerm> m_Terms;
        readonly Color[] m_DefaultPalette = new[] { Color.DarkRed, Color.DarkBlue, Color.DarkGreen, Color.Navy, Color.DarkCyan, Color.DarkOrange, Color.DarkGoldenrod, Color.DarkKhaki, Color.Blue, Color.Red, Color.Green };
        private Color[] m_Palette;
        private LayoutType m_LayoutType;

        private int m_MaxFontSize;
        private int m_MinFontSize;
        private ILayout m_Layout;
        private Color m_BackColor;
        private LayoutItem m_ItemUderMouse;
        private int m_MinTermWeight;
        private int m_MaxTermWeight;

        public CloudControl()
        {
            m_MinTermWeight = 0;
            m_MaxTermWeight = 0;

            MaxFontSize = 50;
            MinFontSize = 10;
           
            this.BorderStyle = BorderStyle.FixedSingle;
            this.ResizeRedraw = true;
            
            m_Palette = m_DefaultPalette;
            m_BackColor = Color.White;
            m_LayoutType = LayoutType.Spiral;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_Terms == null) { return; }
            if (m_Layout == null) { return; }

            IEnumerable<LayoutItem> termsToRedraw = m_Layout.GetTermsInArea(e.ClipRectangle);
            int a = termsToRedraw.Count();
            using (Graphics graphics = e.Graphics)
            using (IGraphicEngine graphicEngine =
                    new GdiGraphicEngine(graphics, this.Font.FontFamily, FontStyle.Regular, m_Palette, MinFontSize, MaxFontSize, m_MinTermWeight, m_MaxTermWeight))
            {
                foreach (LayoutItem currentItem in termsToRedraw)
                {
                    if (m_ItemUderMouse == currentItem)
                    {
                        graphicEngine.DrawEmphasized(currentItem);
                    }
                    else
                    {
                        graphicEngine.Draw(currentItem);                        
                    }
                }
            }
        }

        private void BuildLayout()
        {
            if (m_Terms == null) { return; }

            using (Graphics graphics = this.CreateGraphics())
            {
                IGraphicEngine graphicEngine =
                    new GdiGraphicEngine(graphics, this.Font.FontFamily, FontStyle.Regular, m_Palette, MinFontSize, MaxFontSize, m_MinTermWeight, m_MaxTermWeight);
                m_Layout = LayoutFactory.CrateLayout(m_LayoutType, this.Size);
                m_Layout.Arrange(m_Terms, graphicEngine);
            }
        }

        public LayoutType LayoutType
        {
            get { return m_LayoutType; }
            set
            {
                if (value == m_LayoutType)
                {
                    return;
                }

                m_LayoutType = value;
                BuildLayout();
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            LayoutItem nextItemUnderMouse;
            Point mousePositionRelativeToControl = this.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            this.TryGetItemAtLocation(mousePositionRelativeToControl, out nextItemUnderMouse);
            if (nextItemUnderMouse != m_ItemUderMouse)
            {
                if (nextItemUnderMouse != null)
                {
                    Rectangle newRectangleToInvalidate = RectangleGrow(nextItemUnderMouse.Rectangle, 6);
                    this.Invalidate(newRectangleToInvalidate);
                }
                if (m_ItemUderMouse != null)
                {
                    Rectangle prevRectangleToInvalidate = RectangleGrow(m_ItemUderMouse.Rectangle, 6);
                    this.Invalidate(prevRectangleToInvalidate);
                }
                m_ItemUderMouse = nextItemUnderMouse;
            }
            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            BuildLayout();
            base.OnResize(eventargs);
        }

        private static Rectangle RectangleGrow(RectangleF original, int growByPixels)
        {
            return new Rectangle(
                (int)(original.X - growByPixels),
                (int)(original.Y - growByPixels),
                (int)(original.Width + growByPixels + 1),
                (int)(original.Height + growByPixels + 1));
        }


        public override Color BackColor
        {
            get
            {
                return m_BackColor;
            }
            set
            {
                if (m_BackColor == value)
                {
                    return;
                }
                m_BackColor = value;
                Invalidate();
            }
        }

        public int MaxFontSize
        {
            get { return m_MaxFontSize; }
            set
            {
                m_MaxFontSize = value;
                BuildLayout();
                Invalidate();
            }
        }

        public int MinFontSize
        {
            get { return m_MinFontSize; }
            set
            {
                m_MinFontSize = value;
                BuildLayout();
                Invalidate();
            }
        }

        public Color[] Palette
        {
            get { return m_Palette; }
            set
            {
                m_Palette = value;
                BuildLayout();
                Invalidate();
            }
        }

        public IEnumerable<ITerm> WeightedTerms
        {
            get { return m_Terms; }
            set
            {
                m_Terms = value;
                if (value==null) {return;}

                ITerm first = m_Terms.FirstOrDefault();
                if (first!=null)
                {
                    m_MaxTermWeight = first.Occurrences;
                    m_MinTermWeight = m_Terms.Last().Occurrences;
                }

                BuildLayout();
                Invalidate();
            }
        }

        public IEnumerable<LayoutItem> GetItemsInArea(RectangleF area)
        {
            if (m_Layout == null)
            {
                return new LayoutItem[] {};
            }

            return m_Layout.GetTermsInArea(area);
        }

        public bool TryGetItemAtLocation(Point location, out LayoutItem foundItem)
        {
            foundItem = null;
            IEnumerable<LayoutItem> itemsInArea = GetItemsInArea(new RectangleF(location, new SizeF(0, 0)));
            foreach (LayoutItem item in itemsInArea)
            {
                foundItem = item;
                return true;
            }
            return false;
        }
    }
}
