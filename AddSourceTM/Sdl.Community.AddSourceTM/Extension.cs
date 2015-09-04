using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AddSourceTM
{
    public static class Extension
    {
        public static Uri GetInnerProviderUri(this Uri addSourceTmProviderUri)
        {
            if (addSourceTmProviderUri.AbsoluteUri.StartsWith(AddSourceTmTranslationProvider.ProviderUriScheme, StringComparison.InvariantCultureIgnoreCase))
            {
                return
                    new Uri(
                        addSourceTmProviderUri.AbsoluteUri.Substring(
                            AddSourceTmTranslationProvider.ProviderUriScheme.Length));
            }

            return addSourceTmProviderUri;
        }
    }
}
