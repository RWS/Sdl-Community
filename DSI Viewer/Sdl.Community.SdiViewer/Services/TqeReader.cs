using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.DsiViewer.Interface;
using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.DsiViewer.Services
{
    public class TqeReader : ITqeReader
    {
        public int GetLastTqeIndex(ITranslationOrigin translationOrigin)
        {
            var lastIndex = 1;
            while (translationOrigin.MetaDataContainsKey(Constants.METADATA_EVALUATED_AT_PREFIX + (lastIndex + 1)))
            {
                lastIndex++;
            }
            return lastIndex;
        }

        public static string GetTQEDataInMetaData(ITranslationOrigin translationOrigin, int index, string prefix)
        {
            return translationOrigin.GetMetaData(prefix + index);
        }

        public TranslationOriginData GetCurrentTqe(ITranslationOrigin translationOrigin)
        {
            var lastIndex = GetLastTqeIndex(translationOrigin);

            return new TranslationOriginData()
            {
                System = GetTQEDataInMetaData(translationOrigin, lastIndex, Constants.METADATA_SYSTEM_PREFIX),
                Description = GetTQEDataInMetaData(translationOrigin, lastIndex, Constants.METADATA_DESCRIPTION_PREFIX),
                Model = GetTQEDataInMetaData(translationOrigin, lastIndex, Constants.METADATA_MODEL_PREFIX),
                QualityEstimation = translationOrigin.GetMetaData(Constants.QualityEstimation)
            };
        }

        public List<TranslationOriginData> GetPreviousTqeData()
        {
            throw new NotImplementedException();
        }
    }
}
