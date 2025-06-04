using LanguageWeaverProvider.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace LanguageWeaverProvider.Extensions
{
    public static class TranslationOriginExtension
    {
        //private static readonly Dictionary<string, string> QESCoreMap = new()
        //{
        //    ["poor"] = "33",
        //    ["adequate"] = "66",
        //    ["good"] = "80"
        //};

        public static int GetLastTqeIndex(this ITranslationOrigin translationOrigin)
        {
            var lastIndex = 0;
            while (translationOrigin.MetaDataContainsKey(Constants.METADATA_EVALUATED_AT_PREFIX + (lastIndex + 1)))
            {
                lastIndex++;
            }
            return lastIndex;
        }

        public static PluginVersion ToPluginVersion(this Uri translationProviderUri)
        => translationProviderUri.AbsoluteUri switch
        {
            Constants.CloudFullScheme => PluginVersion.LanguageWeaverCloud,
            Constants.EdgeFullScheme => PluginVersion.LanguageWeaverEdge,
            _ => PluginVersion.None,
        };

        public static void SetMetaData(this ITranslationOrigin translationOrigin, TranslationData translationData)
        {
            //var evaluationTime = DateTime.Now.ToUniversalTime();

            //if (!string.IsNullOrWhiteSpace(translationData.QualityEstimation))
            //{
            //    translationOrigin.StoreTQEDataInMetaData(translationData.Index, Constants.METADATA_EVALUATED_AT_PREFIX,
            //        evaluationTime.ToString(Constants.METADATA_EVALUATED_AT_FORMAT));
            //    translationOrigin.StoreTQEDataInMetaData(translationData.Index, Constants.METADATA_SYSTEM_PREFIX,
            //        Constants.METADATA_SYSTEM_NAME);
            //    translationOrigin.StoreTQEDataInMetaData(translationData.Index, Constants.METADATA_SCORE_PREFIX,
            //        GetScoreFromQE(translationData.QualityEstimation));
            //    translationOrigin.StoreTQEDataInMetaData(translationData.Index, Constants.METADATA_MODEL_PREFIX,
            //        translationData.ModelName);
            //    translationOrigin.StoreTQEDataInMetaData(translationData.Index, Constants.METADATA_DESCRIPTION_PREFIX,
            //        string.Format(Constants.METADATA_DESCRIPTION, Constants.METADATA_SYSTEM_NAME,
            //            translationData.ModelName));
            //}

            translationOrigin.SetMetaData(Constants.SegmentMetadata_QE, translationData.QualityEstimation);
            translationOrigin.SetMetaData(Constants.SegmentMetadata_LongModelName, translationData.ModelName);
            translationOrigin.SetMetaData(Constants.SegmentMetadata_ShortModelName, translationData.Model);
            translationOrigin.SetMetaData(Constants.SegmentMetadata_Translation, translationData.Translation);
            translationOrigin.SetMetaData(Constants.SegmentMetadata_Feedback, translationData.AutoSendFeedback.ToString());
        }

        //private static void StoreTQEDataInMetaData(this ITranslationOrigin translationOrigin, int index, string prefix, string value)
        //{
        //    translationOrigin.SetMetaData(prefix + index, value);
        //}

        //private static string GetScoreFromQE(string qualityEstimation) => QESCoreMap[qualityEstimation.ToLower()];
    }
}