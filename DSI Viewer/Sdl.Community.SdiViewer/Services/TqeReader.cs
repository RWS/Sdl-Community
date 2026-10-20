using Sdl.Community.DsiViewer.Interface;
using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DsiViewer.Services
{
    public class TqeReader : ITqeReader
    {
        public static string GetTQEDataInMetaData(ITranslationOrigin translationOrigin, int index, string prefix)
        {
            return translationOrigin.GetMetaData(prefix + index);
        }

        public TranslationOriginData GetCurrentTqe(ITranslationOrigin translationOrigin)
        {
            if (translationOrigin is null)
                throw new ArgumentNullException("Translation Origin is null");

            if (!HasCurrentTqeData(translationOrigin))
                return null;

            var currentIndex = GetLastTqeIndex(translationOrigin);
            var score = GetTQEDataInMetaData(translationOrigin, currentIndex, Constants.METADATA_SCORE_PREFIX);

            return new TranslationOriginData()
            {
                System = GetTQEDataInMetaData(translationOrigin, currentIndex, Constants.METADATA_SYSTEM_PREFIX),
                Description = GetTQEDataInMetaData(translationOrigin, currentIndex, Constants.METADATA_DESCRIPTION_PREFIX),
                Model = GetTQEDataInMetaData(translationOrigin, currentIndex, Constants.METADATA_MODEL_PREFIX),
                Score = score,
                // "quality_estimation" is only written by the Language Weaver Apply Metadata task / feedback panel; segments
                // translated in the Editor carry just the tqe-* keys, so derive the label from the score.
                QualityEstimation = NullIfWhiteSpace(translationOrigin.GetMetaData(Constants.QualityEstimation))
                                    ?? QualityEstimationLabel.FromScore(score)
            };
        }

        public List<TranslationOriginData> GetPreviousTqeData(ITranslationOrigin translationOrigin)
        {
            if (translationOrigin is null)
                throw new ArgumentNullException("Translation Origin is null");

            var translationData = new List<TranslationOriginData>();

            while (true)
            {
                translationOrigin = translationOrigin.OriginBeforeAdaptation;

                if (translationOrigin is null)
                    break;

                var currentData = GetCurrentTqe(translationOrigin);

                if (currentData is null 
                    || string.IsNullOrWhiteSpace(currentData.System)
                    && string.IsNullOrWhiteSpace(currentData.Description)
                    && string.IsNullOrWhiteSpace(currentData.Model)
                    && string.IsNullOrWhiteSpace(currentData.QualityEstimation)
                    && string.IsNullOrWhiteSpace(currentData.Score))
                {
                    continue;
                }

                translationData.Add(currentData);
            }

            return translationData.Any() ? translationData : null;
        }

        public bool HasCurrentTqeData(ITranslationOrigin translationOrigin)
        {
            if (translationOrigin is null) return false;
            var hasCurrentTqeData = GetLastTqeIndex(translationOrigin) != 0;
            return hasCurrentTqeData;
        }

        public bool HasPreviousTqeData(ITranslationOrigin translationOrigin)
        {
            if (translationOrigin is null) return false;

            var previous = GetPreviousTqeData(translationOrigin);
            return previous != null;
        }

        public bool HasTqeData(ITranslationOrigin translationOrigin)
        {
            if (translationOrigin is null) return false;
            return HasCurrentTqeData(translationOrigin) || HasPreviousTqeData(translationOrigin);
        }

        private static string NullIfWhiteSpace(string value) => string.IsNullOrWhiteSpace(value) ? null : value;

        private int GetLastTqeIndex(ITranslationOrigin translationOrigin)
        {
            var lastIndex = 0;
            while (translationOrigin.MetaDataContainsKey(Constants.METADATA_EVALUATED_AT_PREFIX + (lastIndex + 1)))
            {
                lastIndex++;
            }
            return lastIndex;
        }
    }
}