namespace Sdl.Community.FailSafeTask
{
    using Sdl.FileTypeSupport.Framework.BilingualApi;
    using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;

    internal class CopySourceToTarget : AbstractBilingualContentHandler
    {
        private readonly IDocumentItemFactory itemFactory = DefaultDocumentItemFactory.CreateInstance();

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (paragraphUnit.IsStructure) { return; }

            foreach (var pair in paragraphUnit.SegmentPairs)
            {
                var source = pair.Source;
                var target = pair.Target;

                ISegment clone = (ISegment)source.Clone();

                target.Clear();
                clone.MoveAllItemsTo(target);
            }
        }
    }
}