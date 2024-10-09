using System.Collections.Generic;
using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.DsiViewer.Interface
{
    public interface ITqeReader
    {
        TranslationOriginData GetCurrentTqe(ITranslationOrigin translationOrigin);
        List<TranslationOriginData> GetPreviousTqeData(ITranslationOrigin translationOrigin);
        bool HasTqeData(ITranslationOrigin translationOrigin);
        bool HasCurrentTqeData(ITranslationOrigin translationOrigin);
        bool HasPreviousTqeData(ITranslationOrigin translationOrigin);
    }
}