using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Sdl.Community.ReindexTms.TranslationMemory
{

    public class TranslationMemoryHelper
    {
        private string tmsConfigPath;
        private StringBuilder reindexStatus;

        public TranslationMemoryHelper()
        {
            tmsConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SDL\SDL Trados Studio\11.0.0.0\TranslationMemoryRepository.xml");
            reindexStatus = new StringBuilder();
        }

        public List<TranslationMemoryInfo> LoadLocalUserTms()
        {
            var tms = new List<TranslationMemoryInfo>();
            
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(tmsConfigPath);

            foreach (XmlElement tmElement in xmlDocument.SelectNodes("/TranslationMemoryRepository/TranslationMemories/TranslationMemory"))
            {
                string path = tmElement.GetAttribute("path");
                if(!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    tms.Add(new TranslationMemoryInfo(path, true));
                }
            }
            return tms;
        }
        public List<TranslationMemoryInfo> LoadTmsFromPath(string path)
        {
            return LoadTmsFromPath(new string[] { path });
        }


        public List<TranslationMemoryInfo> LoadTmsFromPath(string[] paths)
        {
            var tms = new List<TranslationMemoryInfo>();

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*.sdltm", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        tms.Add(new TranslationMemoryInfo(file, false));
                    }
                }
                else
                {
                    if (File.Exists(path) && Path.GetExtension(path).Equals(".sdltm", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tms.Add(new TranslationMemoryInfo(path, false));
                    }

                }


            }

           
            return tms;
        }

        public void Reindex(List<TranslationMemoryInfo> tms, BackgroundWorker bw)
        {
            //remove possible duplicates based on the URI
            var distinctTms = tms.GroupBy(k => k.Uri)
                 .Where(g => g.Count() >= 1)
                 .Select(g => g.FirstOrDefault())
                 .ToList();

            Parallel.ForEach(distinctTms, tm =>
            {
                reindexStatus.AppendLine(string.Format("Start reindex {0} translation memory", tm.Name));
                bw.ReportProgress(0, reindexStatus.ToString());
                FileBasedTranslationMemory fileBasedTm = new FileBasedTranslationMemory(tm.FilePath);
                if ((fileBasedTm.Recognizers & BuiltinRecognizers.RecognizeAlphaNumeric) == 0)
                {
                    fileBasedTm.Recognizers |= BuiltinRecognizers.RecognizeAlphaNumeric;
                }

                int tuCount = fileBasedTm.GetTranslationUnitCount();

                ITranslationMemoryLanguageDirection languageDirection = fileBasedTm.LanguageDirection;

                LanguagePlatform.TranslationMemory.RegularIterator iterator = new LanguagePlatform.TranslationMemory.RegularIterator(100);

                while (languageDirection.ReindexTranslationUnits(ref iterator))
                {
                    bw.ReportProgress(0, reindexStatus.ToString());
                }

                fileBasedTm.RecomputeFuzzyIndexStatistics();
                fileBasedTm.Save();
                reindexStatus.AppendLine(string.Format("Finish reindex {0} translation memory", tm.Name));
                bw.ReportProgress(0, reindexStatus.ToString());
            });
        }
    }
}
