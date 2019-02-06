namespace Sdl.Community.FailSafeTask
{
    using Sdl.FileTypeSupport.Framework.BilingualApi;

    internal class ClearTarget : AbstractBilingualContentHandler
    {
        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (paragraphUnit.IsStructure) { return; }

            foreach (var pair in paragraphUnit.SegmentPairs)
            {
                pair.Target.Clear();
            }
        }
    }
}