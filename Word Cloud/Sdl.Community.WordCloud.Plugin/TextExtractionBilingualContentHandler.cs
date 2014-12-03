using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.WordCloud.Plugin
{
    class TextExtractionBilingualContentHandler : AbstractBilingualContentHandler
    {
        public TextExtractionBilingualContentHandler()
        {
            Text = new List<string>();
        }

        public List<string> Text
        {
            get;
            private set;
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            Text.Add(PlainTextExtractor.GetPlainText(paragraphUnit.Source));

            base.ProcessParagraphUnit(paragraphUnit);
        }
    }
}
