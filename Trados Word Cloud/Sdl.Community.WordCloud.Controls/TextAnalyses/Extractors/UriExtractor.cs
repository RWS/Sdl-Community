using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors
{
    public class UriExtractor : FileExtractor
    {
        private readonly Uri m_Uri;
        private bool m_IsScriptMode;

        public UriExtractor(Uri uri, IProgressIndicator progressIndicator) 
            : base(null, progressIndicator)
        {
            m_Uri = uri;
        }

        public override IEnumerable<string> GetWords()
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(m_Uri);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream==null)
                    {
                        yield break;
                    }

                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        foreach (string word in GetWords(reader))
                        {
                            yield return word;
                        }
                    }
                }
            }
        }

        protected override IEnumerable<string> GetWords(string text)
        {
            text = RemoveXmlTags(text);
            text = RemoveScript(text);
            return base.GetWords(text);
        }

        private string RemoveScript(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            int indexOfStart = 0;
            int indexOfEnd = text.Length;
            if (!m_IsScriptMode)
            {
                indexOfStart = text.IndexOf("{");
                if (indexOfStart < 0)
                {
                    return text;
                }
                m_IsScriptMode = true;
            }

            if (m_IsScriptMode)
            {
                indexOfEnd = text.IndexOf("}");
                if (indexOfEnd < 0)
                {
                    return text.Remove(indexOfStart);
                }
                m_IsScriptMode = false;
            }

            int count = indexOfEnd - indexOfStart + 2;
            if (indexOfStart + count < text.Length)
            {
                return text.Remove(indexOfStart, indexOfEnd - indexOfStart + 2);
            }
            return text.Remove(indexOfStart);
        }

        private static string RemoveXmlTags(string text)
        {
            string result = text;
            int indexOfStart = result.IndexOf("<");
            while (!(indexOfStart < 0) && indexOfStart + 1 < text.Length)
            {
                int indexOfEnd = result.IndexOf(">", indexOfStart + 1);
                if (indexOfEnd < 0)
                {
                    break;
                }
                result = result.Remove(indexOfStart, indexOfEnd - indexOfStart + 1);
                indexOfStart = result.IndexOf("<");
            }
            return result;
        }
    }
}
