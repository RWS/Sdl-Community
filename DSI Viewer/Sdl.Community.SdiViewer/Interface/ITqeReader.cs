using System.Collections.Generic;
using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.DsiViewer.Interface
{
    public interface ITqeReader
    {
        TranslationOriginData GetCurrentTqe(ITranslationOrigin translationOrigin);
        List<TranslationOriginData> GetPreviousTqeData();
    }
}