namespace Sdl.Community.EmbeddedContentProcessor.Settings
{
    public enum SegmentationHint
    {
        // Summary:
        //     The value has not been explicitly specified, or the default segmentation
        //     behavior is implied.
        //
        // Remarks:
        //      A segmentation engine may examine the value of the IPlaceholderTagProperties.TextEquivalent
        //     property in order to determine whether the tag should be included in a segment
        //     when it appears at the segment boundary.
        Undefined = 0,
        //
        // Summary:
        //     If possible, the segmentation engine should always include this tag inside
        //     a segment.
        //
        // Remarks:
        //      Tags with this property should be treated similar to tags with "TagHandlingAlwaysInclude"
        //     in Filter Framework 1 and "placeholder" Trados tools.
        Include = 1,
        //
        // Summary:
        //     The tag may be left outside of the segment, e.g. if it appears at a segment
        //     boundary.
        //
        // Remarks:
        //      There should be no need for a segmentation engine to check the IPlaceholderTagProperties.TextEquivalent
        //     property, as this value explicitly states that the tag can be excluded from
        //     segments.
        MayExclude = 2,
        //
        // Summary:
        //     The tag will be included in a segment if the segment also contains text content,
        //     otherwise it will not be included in a segment (in order to prevent segment-only
        //     tags).
        IncludeWithText = 3,
        //
        // Summary:
        //     The tag will be excluded from a segment, even if this means changing the
        //     segmentation. This is used for sub-content processing.
        //
        // Remarks:
        //     There are two circumstances where a tag with SegmentationHint set to Exclude
        //     will not be excluded from a segment; where the tag is inside a tag pair with
        //     SegmentationHint NOT set to Exclude or where the tag is inside a review marker.
        Exclude = 4,
    }
}
