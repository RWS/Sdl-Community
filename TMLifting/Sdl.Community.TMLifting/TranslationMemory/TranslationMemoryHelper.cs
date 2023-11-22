using Sdl.Community.TMLifting.Processor;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sdl.Community.TMLifting.TranslationMemory
{
    public class TranslationMemoryHelper
    {
        private readonly string _tmsConfigPath;
        private readonly StringBuilder _reindexStatus;

        public TranslationMemoryHelper()
        {
            var studioService = new StudioVersionService();
            var publicVersion = studioService.GetStudioVersion().ExecutableVersion.Major;
             _tmsConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Trados\Trados Studio\Studio17\TranslationMemoryRepository.xml");
            _reindexStatus = new StringBuilder();
        }

        public List<TranslationMemoryInfo> LoadLocalUserTms()
        {
            var xmlDocument = new XmlDocument();
			if (File.Exists(_tmsConfigPath))
			{
				xmlDocument.Load(_tmsConfigPath);

				return (from XmlElement tmElement in xmlDocument.SelectNodes("/TranslationMemoryRepository/TranslationMemories/TranslationMemory")
						select tmElement.GetAttribute("path") into path
						where !string.IsNullOrEmpty(path) && File.Exists(path)
						select new TranslationMemoryInfo(path, true)).ToList();
			}
			return new List<TranslationMemoryInfo>();
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
                Process(modelBuilder, fragmentAligner,bw);
            }
            else
            {
                Process(tmExporter,
               tmImporter,
               modelBuilder,
               fragmentAligner,bw);
            }
            if (!bw.CancellationPending)
            {
                ProcessorUtil.UpdateTranslationMemory(tmOut);
                _reindexStatus.AppendLine(string.Format("Finish uplift {0} translation memory", tm.Name));

                bw.ReportProgress(0, _reindexStatus.ToString());
            }
            else
            {
                bw.ReportProgress(100,"");
            }                
        }

        private async void Process(
           TmExporter tmExporter,
           TmImporter tmImporter,
           ModelBuilder modelBuilder,             
           FragmentAligner fragmentAligner,
           BackgroundWorker bw)
        {
            try
            {
                
                var exportFullPath = await tmExporter.Export();
                await tmImporter.Import(exportFullPath);

                File.Delete(exportFullPath);

                 modelBuilder.BuildTranslationModel();
                if (!bw.CancellationPending)
                {
                    fragmentAligner.AlignTranslationUnits();
                }
                else
                {
                    bw.ReportProgress(100, "");
                }
               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }         
        }

        private  void Process(ModelBuilder modelBuilder, FragmentAligner fragmentAligner,BackgroundWorker bw)
        {
            try
            {
                 modelBuilder.BuildTranslationModel();
                if (!bw.CancellationPending)
                {
                    fragmentAligner.AlignTranslationUnits();
                }else
                {
                    bw.ReportProgress(100,"");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Process(List<TranslationMemoryInfo> tms, BackgroundWorker bw, bool reindex,bool upplift)
        {
            _reindexStatus.Remove(0, _reindexStatus.Length);
            //remove possible duplicates based on the URI
            var distinctTms = tms.GroupBy(k => k.Uri)
                 .Where(g => g.Any())
                 .Select(g => g.FirstOrDefault())
                 .ToList();

            Parallel.ForEach(distinctTms, tm =>
            {
                if (reindex)
                {
                    ProcessReindexFileBasedTm(bw,tm);
                }
                if (upplift)
                {
                    Uplift(tm, bw);
                }
            });
        }

        public void ProcessReindexFileBasedTm(BackgroundWorker bw, TranslationMemoryInfo tm)
        {
            _reindexStatus.AppendLine(string.Format("Start reindex {0} translation memory", tm.Name));
            bw.ReportProgress(0, _reindexStatus.ToString());
            var fileBasedTm = new FileBasedTranslationMemory(tm.FilePath);
            if ((fileBasedTm.Recognizers & BuiltinRecognizers.RecognizeAlphaNumeric) == 0)
            {
                fileBasedTm.Recognizers |= BuiltinRecognizers.RecognizeAlphaNumeric;
            }

            var languageDirection = fileBasedTm.LanguageDirection;

            var iterator = new RegularIterator(100);

            while (languageDirection.ReindexTranslationUnits(ref iterator))
            {
				if (!bw.CancellationPending)
				{
					bw.ReportProgress(0, _reindexStatus.ToString());

				}
				else
				{
					bw.ReportProgress(100, "");
				}
			}

            fileBasedTm.RecomputeFuzzyIndexStatistics();
            fileBasedTm.Save();
            _reindexStatus.AppendLine(string.Format("Finish reindex {0} translation memory", tm.Name));

            bw.ReportProgress(0, _reindexStatus.ToString());
        }
    }
}