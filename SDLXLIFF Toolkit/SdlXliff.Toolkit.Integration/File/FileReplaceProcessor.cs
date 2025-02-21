using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLXLIFFSliceOrChange;
using System.Text.RegularExpressions;

namespace SdlXliff.Toolkit.Integration.File
{
    public class FileReplaceProcessor : AbstractBilingualContentProcessor
    {
        public FileReplaceProcessor(ReplaceSettings settings)
        {
            SegmentVisitor = new SegmentVisitor(settings);
            IsSource = settings.SourceSearchText is not null && !string.IsNullOrEmpty(settings.SourceSearchText);
        }

        private bool IsSource { get; set; }

        private SegmentVisitor SegmentVisitor { get; }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (IsSource)
                foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    segmentPair.Source.AcceptVisitor(SegmentVisitor);
            else
                foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    segmentPair.Target.AcceptVisitor(SegmentVisitor);

            base.ProcessParagraphUnit(paragraphUnit);
        }
    }
}