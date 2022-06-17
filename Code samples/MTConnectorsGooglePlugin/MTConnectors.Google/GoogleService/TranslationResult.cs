using System.Collections.Generic;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class TranslationResult<T>
    {
        public IList<T> translations
        {
            get;
            set;
        }

        public T responseData
        {
            get
            {
                if (translations == null)
                {
                    return default(T);
                }
                else if (translations.Count > 0)
                {
                    return translations[0];
                }
                else
                {
                    return default(T);
                }
            }
        }

        public object responseDetails = null;
        public System.Net.HttpStatusCode responseStatus = System.Net.HttpStatusCode.OK;
    }
}
