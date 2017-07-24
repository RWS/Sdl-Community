using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class TextExtractionBilingualContentHandler : AbstractBilingualContentHandler
    {
        public List<string> Text { get; private set; }

        public TextExtractionBilingualContentHandler()
        {
            Text = new List<string>();
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            Text.Add(PlainTextExtractor.GetPlainText(paragraphUnit.Source));
            base.ProcessParagraphUnit(paragraphUnit);
        }
    }
}
