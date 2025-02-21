using System.Collections.Generic;

namespace Multilingual.Excel.FileType.QuickTags.Styles
{
    internal class Fonts
    {
        private readonly IList<Font> _fonts;

        public Fonts()
        {
            _fonts = new List<Font>();
        }

        public void AddFont(Font font)
        {
            _fonts.Add(font);
        }

        public Font GetFont(int fontIndex)
        {
            return _fonts[fontIndex];
        }
    }
}