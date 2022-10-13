using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.XML.FileType.Processors
{
    public class RegexEmbeddedBilingualGenerator : AbstractBilingualContentProcessor
    {
        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            var clonedParagraphUnit = paragraphUnit.Clone() as IParagraphUnit;

            ProcessParagraph(paragraphUnit.Source, clonedParagraphUnit.Source);
            ProcessParagraph(paragraphUnit.Target, clonedParagraphUnit.Target);

            base.ProcessParagraphUnit(clonedParagraphUnit);
        }

        private void ProcessParagraph(IParagraph originalParagraph, IParagraph clonedParagraph)
        {
            var visitor = new EmbeddedContentGeneratorVisitor(ItemFactory, clonedParagraph);

            visitor.VisitParagraph(originalParagraph);
        }
    }
}
