using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sdl.Community.WordCloud.Controls.Geometry;

namespace Sdl.Community.WordCloud.Controls
{
    public class GdiGraphicEngine : IGraphicEngine, IDisposable
    {
        private readonly Graphics m_Graphics;

        private readonly int m_MinWordWeight;
        private readonly int m_MaxWordWeight;
        private Font m_LastUsedFont;

        public FontFamily FontFamily { get; set; }
        public FontStyle FontStyle { get; set; }
        public Color[] Palette { get; private set; }
        public float MinFontSize { get; set; }
        public float MaxFontSize { get; set; }

        public GdiGraphicEngine(Graphics graphics, FontFamily fontFamily, FontStyle fontStyle, Color[] palette, float minFontSize, float maxFontSize, int minWordWeight, int maxWordWeight)
        {
            m_MinWordWeight = minWordWeight;
            m_MaxWordWeight = maxWordWeight;
            m_Graphics = graphics;
            FontFamily = fontFamily;
            FontStyle = fontStyle;
            Palette = palette;
            MinFontSize = minFontSize;
            MaxFontSize = maxFontSize;
            m_LastUsedFont = new Font(this.FontFamily, maxFontSize, this.FontStyle);
            m_Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        public SizeF Measure(string text, int weight)
        {
            Font font = GetFont(weight);
            //return m_Graphics.MeasureString(text, font);
            return TextRenderer.MeasureText(m_Graphics, text, font);
        }

        public void Draw(LayoutItem layoutItem)
        {
            Font font = GetFont(layoutItem.Word.Occurrences);
            Color color = GetPresudoRandomColorFromPalette(layoutItem);
            //m_Graphics.DrawString(layoutItem.Word, font, brush, layoutItem.Rectangle);
            Point point = new Point((int)layoutItem.Rectangle.X, (int)layoutItem.Rectangle.Y);
            TextRenderer.DrawText(m_Graphics, layoutItem.Word.Text, font, point, color);
        }

        public void DrawEmphasized(LayoutItem layoutItem)
        {
            Font font = GetFont(layoutItem.Word.Occurrences);
            Color color = GetPresudoRandomColorFromPalette(layoutItem);
            //m_Graphics.DrawString(layoutItem.Word, font, brush, layoutItem.Rectangle);
            Point point = new Point((int)layoutItem.Rectangle.X, (int)layoutItem.Rectangle.Y);
            TextRenderer.DrawText(m_Graphics, layoutItem.Word.Text, font, point, Color.LightGray);
            int offset = (int)(5 *font.Size / MaxFontSize)+1;
            point.Offset(-offset, -offset);
            TextRenderer.DrawText(m_Graphics, layoutItem.Word.Text, font, point, color);
        }

        private Font GetFont(int weight)
        {
			if ((m_MaxWordWeight != m_MinWordWeight) && (MaxFontSize != MinFontSize))
			{
				float fontSize = (float)(weight - m_MinWordWeight) / (m_MaxWordWeight - m_MinWordWeight) * (MaxFontSize - MinFontSize) + MinFontSize;
				if (m_LastUsedFont.Size != fontSize)
				{
					m_LastUsedFont = new Font(this.FontFamily, fontSize, this.FontStyle);
				}
			}
            return m_LastUsedFont;
        }

        private Color GetPresudoRandomColorFromPalette(LayoutItem layoutItem)
        {
            Color color = Palette[layoutItem.Word.Occurrences * layoutItem.Word.Text.Length % Palette.Length];
            return color;
        }

        public void Dispose()
        {
            m_Graphics.Dispose();
        }
    }
}
