using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Plugin.ExtractTerms
{
    public class TextExtractionBilingualContentHandler : AbstractBilingualContentHandler
    {
        public List<string> SourceText { get; private set; }
        public List<string> TargetText { get; private set; }

        public TextExtractionBilingualContentHandler()
        {
            SourceText = new List<string>();
            TargetText = new List<string>();
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            SourceText.Add(PlainTextExtractor.GetPlainText(paragraphUnit.Source));
            TargetText.Add(PlainTextExtractor.GetPlainText(paragraphUnit.Target));
            base.ProcessParagraphUnit(paragraphUnit);
        }
    }
}
