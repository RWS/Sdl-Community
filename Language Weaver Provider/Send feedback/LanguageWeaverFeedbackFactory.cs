using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace LanguageWeaverProvider.Send_feedback
{
    public static class LanguageWeaverFeedbackFactory
    {
        public static LanguageWeaverFeedback Create(ISegmentPair segmentPair)
        {
            var origin = GetOrigin(segmentPair.Properties);

            return origin switch
            {
                Constants.PluginNameCloud => new CloudFeedback(segmentPair),
                Constants.PluginNameEdge => new EdgeFeedback(segmentPair),
                _ => null
            };
        }

        private static string GetOrigin(ISegmentPairProperties segmentPairProperties)
        {
            if (IsCurrentTranslationLanguageWeaverSource(segmentPairProperties))
                return segmentPairProperties.TranslationOrigin.OriginSystem;

            if (IsPreviousTranslationLanguageWeaver(segmentPairProperties))
                return segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem;

            return null;
        }

        private static bool IsCurrentTranslationLanguageWeaverSource(ISegmentPairProperties segmentProperties)
        {
            return segmentProperties.TranslationOrigin?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
        }

        private static bool IsLanguageWeaverSource(ISegmentPairProperties segmentProperties)
        {
            return IsCurrentTranslationLanguageWeaverSource(segmentProperties)
                   || IsPreviousTranslationLanguageWeaver(segmentProperties);
        }

        private static bool IsPreviousTranslationLanguageWeaver(ISegmentPairProperties segmentProperties)
        {
            return segmentProperties.TranslationOrigin?.OriginBeforeAdaptation?.OriginSystem?.StartsWith(Constants.PluginShortName) == true;
        }
    }
}