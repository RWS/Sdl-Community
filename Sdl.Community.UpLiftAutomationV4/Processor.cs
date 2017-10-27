using Sdl.Community.FragmentAlignmentAutomation.Processors;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sdl.Community.FragmentAlignmentAutomation
{


    public class Processor : IDisposable
    {

        public event EventHandler<ProgressEventArgs> OnProgressChanged;

        public string InputTmFullPathFullPath { get; private set; }
        public string OutputTmFullPathFullPath { get; private set; }
        public bool QuickAlign { get; private set; }
        public int TotalUnits { get; private set; }




        private FileBasedTranslationMemory _filebasedTmIn;
        private FileBasedTranslationMemory _fileBasedTmOut;

        private FileBasedTranslationMemory FileBasedTmIn
        {
            get { return _filebasedTmIn ?? (_filebasedTmIn = new FileBasedTranslationMemory(InputTmFullPathFullPath)); }
        }
        private FileBasedTranslationMemory FileBasedTmOut
        {
            get
            {                
                if (FileBasedTmIn.FGASupport != FGASupport.Off)
                {
                    if (_fileBasedTmOut != null)
                        return _fileBasedTmOut;

                    File.Copy(InputTmFullPathFullPath, OutputTmFullPathFullPath, true);
                    _fileBasedTmOut = new FileBasedTranslationMemory(OutputTmFullPathFullPath);
                    return _fileBasedTmOut;
                }
                else
                {
                    return _fileBasedTmOut ?? (_fileBasedTmOut = new FileBasedTranslationMemory(OutputTmFullPathFullPath,
                        FileBasedTmIn.Description ?? "",
                        FileBasedTmIn.LanguageDirection.SourceLanguage,
                        FileBasedTmIn.LanguageDirection.TargetLanguage,
                        FileBasedTmIn.FuzzyIndexes,
                        FileBasedTmIn.Recognizers,
                        FileBasedTmIn.TokenizerFlags,
                        FileBasedTmIn.WordCountFlags,
                        true));
                }
            }
        }

        public Processor()
        {
            InputTmFullPathFullPath = string.Empty;
            OutputTmFullPathFullPath = string.Empty;

            QuickAlign = false;
            TotalUnits = 0;
        }
     
        public void SetProcessingVariables(string inputTmFullPath, string outputTmFullPath, bool quickAlign = false)
        {
            _filebasedTmIn = null;
            _fileBasedTmOut = null;

            if (string.IsNullOrEmpty(inputTmFullPath))
                throw new Exception(StringResources.FragmentAlignment_InputTmPathCannotBeNull);

            if (string.IsNullOrEmpty(outputTmFullPath))
                throw new Exception(StringResources.FragmentAlignment_OutputTmPathCannotBeNull);


            InputTmFullPathFullPath = inputTmFullPath;
            OutputTmFullPathFullPath = outputTmFullPath;

            QuickAlign = quickAlign;

            // TODO
            // this will return the total number of translation units for the all language directions
            // in the TM; 
            // refactor: should reference the language direction being processed!
            // the reason that this is being processed here as it can be a costly operation and only
            // needs to occur once.
            TotalUnits = FileBasedTmIn.GetTranslationUnitCount();
        }

        public Task Run()
        {

            var tmExporter = new TmExporter(FileBasedTmIn, TotalUnits);           
            var tmImporter = new TmImporter(FileBasedTmOut, TotalUnits);           
            var modelBuilder = new ModelBuilder(FileBasedTmOut);           
            var fragmentAligner = new FragmentAligner(FileBasedTmOut, QuickAlign);
           
            if (FileBasedTmIn.FGASupport != FGASupport.Off)
            {
               return Process(modelBuilder,
                 fragmentAligner);                             
            }
            else
            {
                return Process(tmExporter,
                    tmImporter,
                    modelBuilder,
                    fragmentAligner);                              
            }
        }


        private  async Task Process(TmExporter tmExporter, TmImporter tmImporter, ModelBuilder modelBuilder, FragmentAligner fragmentAligner)
        {
            try
            {
                tmExporter.OnProgressChanged += tmExporter_OnProgressChanged;
                tmImporter.OnProgressChanged += tmImporter_OnProgressChanged;
                modelBuilder.OnProgressChanged += modelBuilder_OnProgressChanged;
                fragmentAligner.OnProgressChanged += fragmentAligner_OnProgressChanged;


                var exportFullPath = await tmExporter.Export();
                await tmImporter.Import(exportFullPath);

                File.Delete(exportFullPath);

                await modelBuilder.BuildTranslationModel();
                await fragmentAligner.AlignTranslationUnits();
            }
            finally
            {
                tmExporter.OnProgressChanged -= tmExporter_OnProgressChanged;
                tmImporter.OnProgressChanged -= tmImporter_OnProgressChanged;
                modelBuilder.OnProgressChanged -= modelBuilder_OnProgressChanged;
                fragmentAligner.OnProgressChanged -= fragmentAligner_OnProgressChanged;
            }
          
        }
        private async Task Process(ModelBuilder modelBuilder, FragmentAligner fragmentAligner)
        {
            try
            {                
                modelBuilder.OnProgressChanged += modelBuilder_OnProgressChanged;
                fragmentAligner.OnProgressChanged += fragmentAligner_OnProgressChanged;

                await modelBuilder.BuildTranslationModel();
                await fragmentAligner.AlignTranslationUnits();
            }
            finally
            {             
                modelBuilder.OnProgressChanged -= modelBuilder_OnProgressChanged;
                fragmentAligner.OnProgressChanged -= fragmentAligner_OnProgressChanged;
            }
        }

        private void tmExporter_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            ProcessOnProgressChanged(e);
        }
        private void tmImporter_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            ProcessOnProgressChanged(e);
        }
        private void modelBuilder_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            ProcessOnProgressChanged(e);
        }
        private void fragmentAligner_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            ProcessOnProgressChanged(e);
        }

        private void ProcessOnProgressChanged(ProgressEventArgs e)
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged.Invoke(this, new ProgressEventArgs
                {
                    Type = e.Type,
                    Description = e.Description,
                    CurrentProgress = e.CurrentProgress,
                    TotalUnits = e.TotalUnits
                });
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _filebasedTmIn = null;
                _fileBasedTmOut = null;
            }
        }
    }
}
