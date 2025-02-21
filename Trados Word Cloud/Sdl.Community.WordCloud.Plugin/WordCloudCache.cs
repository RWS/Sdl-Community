using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Processing;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.WordCloud.Plugin
{
    class WordCloudCache
    {
        public void Save(string cloudTag, FileBasedProject project, List<IWord> words)
        {
            XDocument doc = new XDocument();
            doc.Add(
                new XElement("wordcloud", 
                    new XElement("hash", GetWordCloudHash(project)),
                    new XElement("words",
                        from word in words
                            select new XElement("word", new XAttribute("text", word.Text), new XAttribute("count", word.Occurrences))))
                );
            string cacheFile = CreateCacheFilePath(cloudTag, project);
            
            doc.Save(cacheFile);
        }

        public bool TryLoad(string cloudTag, FileBasedProject project, out List<IWord> words)
        {
            words = null;
            string cacheFile = CreateCacheFilePath(cloudTag, project);
            if (!File.Exists(cacheFile))
            {
                return false;
            }

            try
            {
                XDocument doc = XDocument.Load(cacheFile);
                int storedHash = Int32.Parse(doc.XPathSelectElement("/wordcloud/hash").Value);
                int hash = GetWordCloudHash(project);
                if (storedHash != hash)
                {
                    return false;
                }


                words = new List<IWord>();
                foreach (XElement w in doc.XPathSelectElements("/wordcloud/words/word"))
                {
                    words.Add(new Word { Text=w.Attribute("text").Value, Occurrences = Int32.Parse(w.Attribute("count").Value)});
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string CreateCacheFilePath(string cloudTag, FileBasedProject project)
        {
            return Path.Combine(Path.GetDirectoryName(project.FilePath), Path.GetFileNameWithoutExtension(project.FilePath) + "-" + cloudTag + ".xml");
        }

        private int GetWordCloudHash(FileBasedProject project)
        {
            return 1;
        }
    }
}
