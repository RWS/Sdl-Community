using System;
using System.IO;
using System.Reflection;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Blacklist
{
    public static class DefaultBlacklists
    {
        public static IBlacklist GetDefaultBlacklist(string languageCode)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("CodingBreeze.WordCloud.Controls.TextAnalyses.Blacklist.Stopwords.stopwords_{0}.txt", languageCode.ToLowerInvariant())))
            {

                if (s == null)
                {
                    return new NullBlacklist();
                }

                return CommonBlacklist.CreateFromStream(s);
            }
        }
    }
}
