using Sdl.FileTypeSupport.Framework.IntegrationApi;


namespace ExportToExcel
{
    public class Worker
    {
        GeneratorSettings _settings;
        private DataExtractor _dataExtractor;

        internal DataExtractor DataExtractorObj
        {
            get
            {
                if (_dataExtractor == null)
                {
                    _dataExtractor = new DataExtractor();
                }
                _dataExtractor.Settings = _settings;
                return _dataExtractor;
            }
            set { _dataExtractor = value; }
        }

        public void UpdateSettings(GeneratorSettings settings)
        {
            _settings = settings;
        }

        public Worker(GeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Run export on SDLXLIFF path
        /// </summary>
        /// <param name="filePath">SDLXLIFF file for export</param>
        public bool GeneratePreviewFiles(string filePath, IMultiFileConverter multiFileConverter)
        {

            multiFileConverter.SynchronizeDocumentProperties();
            multiFileConverter.AddBilingualProcessor(new FileReader(DataExtractorObj, _settings, filePath));
            multiFileConverter.Parse();

            return true;
        }




       
    }
}

