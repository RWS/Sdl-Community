using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.XliffCompare
{
    internal class ComboBoxEx : ComboBox
    {
        public ImageList ImageList { get; set; }

        public ComboBoxEx()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs ea)
        {
            ea.DrawBackground();
            ea.DrawFocusRectangle();

            var imageSize = ImageList.ImageSize;
            var bounds = ea.Bounds;

            try
            {
                var item = (ComboBoxExItem)Items[ea.Index];

                if (item.ImageIndex != -1)
                {
                    ImageList.Draw(ea.Graphics, bounds.Left, bounds.Top,
                    item.ImageIndex);
                    ea.Graphics.DrawString(item.Text, ea.Font, new
                    SolidBrush(ea.ForeColor), bounds.Left + imageSize.Width, bounds.Top);
                }
                else
                {
                    ea.Graphics.DrawString(item.Text, ea.Font, new
                    SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
            }
            catch
            {
                if (ea.Index != -1)
                {
                    ea.Graphics.DrawString(Items[ea.Index].ToString(), ea.Font, new
                    SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    ea.Graphics.DrawString(Text, ea.Font, new
                    SolidBrush(ea.ForeColor), bounds.Left, bounds.Top);
                }
            }

            base.OnDrawItem(ea);
        }
    }

    internal class ComboBoxExItem
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public int ImageIndex { get; set; }

        public ComboBoxExItem()
            : this("")
        {
        }

        public ComboBoxExItem(string text)
            : this(text, -1)
        {
        }

        public ComboBoxExItem(string text, int imageIndex)
        {
            _text = text;
            ImageIndex = imageIndex;
        }

        public override string ToString()
        {
            return _text;
        }
    }

}


