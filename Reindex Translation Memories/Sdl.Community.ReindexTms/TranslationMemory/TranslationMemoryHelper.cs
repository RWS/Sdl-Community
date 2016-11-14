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
using System.Xml;
using Sdl.Community.ReindexTms.Processor;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.ReindexTms.TranslationMemory
{

    public class TranslationMemoryHelper
    {
        private readonly string _tmsConfigPath;
        private readonly StringBuilder _reindexStatus;

        public TranslationMemoryHelper()
        {
            _tmsConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SDL\SDL Trados Studio\12.0.0.0\TranslationMemoryRepository.xml");
            _reindexStatus = new StringBuilder();
        }

        public List<TranslationMemoryInfo> LoadLocalUserTms()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(_tmsConfigPath);

            return (from XmlElement tmElement in xmlDocument.SelectNodes("/TranslationMemoryRepository/TranslationMemories/TranslationMemory")
                    select tmElement.GetAttribute("path") into path
                    where !string.IsNullOrEmpty(path) && File.Exists(path)
                    select new TranslationMemoryInfo(path, true)).ToList();
        }
        public List<TranslationMemoryInfo> LoadTmsFromPath(string path)
        {
            return LoadTmsFromPath(new[] { path });
        }


        public List<TranslationMemoryInfo> LoadTmsFromPath(string[] paths)
        {
            var tms = new List<TranslationMemoryInfo>();

            foreach (var path in paths)
            {
                var extension = Path.GetExtension(path);
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*.sdltm", SearchOption.AllDirectories);
                    tms.AddRange(files.Select(file => new TranslationMemoryInfo(file, false)));
                }
                else
                {
                    if (extension != null && (File.Exists(path) && extension.Equals(".sdltm", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        tms.Add(new TranslationMemoryInfo(path, false));
                    }
                }
            }


            return tms;
        }

        
        public void Uplift(TranslationMemoryInfo tm,BackgroundWorker bw)
        {
            var originalTmFilePath = ProcessorUtil.RenameOriginalTm(tm.Name, tm.FilePath);
            var upliftTmFilePath = ProcessorUtil.GetOutputTmFullPath(tm.Name, tm.FilePath);
            _reindexStatus.AppendLine(string.Format("Start uplift {0} translation memory", tm.Name));
            bw.ReportProgress(0, _reindexStatus.ToString());

            File.Copy(originalTmFilePath, upliftTmFilePath, true);
            var tmOut = new FileBasedTranslationMemory(upliftTmFilePath);
            tmOut.FGASupport = FGASupport.Automatic;
            tmOut.Save();
            var tmExporter = new TmExporter(tmOut);
            var tmIn = new FileBasedTranslationMemory(tm.FilePath);
            var tmImporter = new TmImporter(tmIn);

            var modelBuilder = new ModelBuilder(tmOut);
            var fragmentAligner = new FragmentAligner(tmOut);

            if (tmIn.FGASupport != FGASupport.NonAutomatic)
            {
                Process(modelBuilder, fragmentAligner);
            }
            else
            {
                Process(tmExporter,
               tmImporter,
               modelBuilder,
               fragmentAligner);
            }
            ProcessorUtil.UpdateTranslationMemory(tmOut);
            _reindexStatus.AppendLine(string.Format("Finish uplift {0} translation memory", tm.Name));

            bw.ReportProgress(0, _reindexStatus.ToString());
        }

        private async void Process(TmExporter tmExporter, TmImporter tmImporter, ModelBuilder modelBuilder, 
           FragmentAligner fragmentAligner)
        {
            try
            {
                
                var exportFullPath = await tmExporter.Export();
                await tmImporter.Import(exportFullPath);

                File.Delete(exportFullPath);

                 modelBuilder.BuildTranslationModel();
                 fragmentAligner.AlignTranslationUnits();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
          

        }
        private  void Process(ModelBuilder modelBuilder, FragmentAligner fragmentAligner)
        {
            try
            {
                 modelBuilder.BuildTranslationModel();
                 fragmentAligner.AlignTranslationUnits();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void Reindex(List<TranslationMemoryInfo> tms, BackgroundWorker bw, bool reindex,bool upplift)
        {
            //remove possible duplicates based on the URI
            var distinctTms = tms.GroupBy(k => k.Uri)
                 .Where(g => g.Any())
                 .Select(g => g.FirstOrDefault())
                 .ToList();

            Parallel.ForEach(distinctTms, tm =>
            {
                
                if (reindex)
                {
                    ProcessReindex(bw,tm);
                }
                if (upplift)
                {
                    Uplift(tm, bw);
                }
            });
        }

        public void ProcessReindex(BackgroundWorker bw, TranslationMemoryInfo tm)
        {
            _reindexStatus.AppendLine(string.Format("Start reindex {0} translation memory", tm.Name));
            bw.ReportProgress(0, _reindexStatus.ToString());
            var fileBasedTm = new FileBasedTranslationMemory(tm.FilePath);
            if ((fileBasedTm.Recognizers & BuiltinRecognizers.RecognizeAlphaNumeric) == 0)
            {
                fileBasedTm.Recognizers |= BuiltinRecognizers.RecognizeAlphaNumeric;
            }

            var languageDirection = fileBasedTm.LanguageDirection;

            var iterator = new LanguagePlatform.TranslationMemory.RegularIterator(100);

            while (languageDirection.ReindexTranslationUnits(ref iterator))
            {
                bw.ReportProgress(0, _reindexStatus.ToString());
            }

            fileBasedTm.RecomputeFuzzyIndexStatistics();
            fileBasedTm.Save();
            _reindexStatus.AppendLine(string.Format("Finish reindex {0} translation memory", tm.Name));

            bw.ReportProgress(0, _reindexStatus.ToString());
        }
    }
}
