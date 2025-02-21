using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class TextExtractionBilingualSourceContentHandler : AbstractBilingualContentHandler
    {
        public List<string> SourceText { get; private set; }

        public TextExtractionBilingualSourceContentHandler()
        {
            SourceText = new List<string>();
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            SourceText.Add(PlainTextExtractor.GetPlainText(paragraphUnit.Source));
            base.ProcessParagraphUnit(paragraphUnit);
        }
    }
}
