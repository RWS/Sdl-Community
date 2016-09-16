using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Core.PluginFramework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;


using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using SdlXliff.Toolkit.Integration.Helpers;

namespace SdlXliff.Toolkit.Integration.File
{
    public class FilesAnalyzer
    {
        private List<string> _files;
        private List<FileData> _fileResults;

        private const string _ext = ".sdlxliff";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files">file to analyze (search or replace in)</param>
        public FilesAnalyzer(string file)
        {
            _files = new List<string> {file};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files">files to analyze (search or replace in)</param>
        public FilesAnalyzer(List<string> files)
        {
            _files = files;
        }

        /// <summary>
        /// result of search/replace operation
        /// </summary>
        public List<FileData> FileResults
        {
            get { return _fileResults; }
        }

        public IFileExtractor GetFileExtractor(string filePath)
        {
            var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
            var extensionPoint =
                PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();

            foreach (IExtension extension in extensionPoint.Extensions)
            {
                IFileTypeComponentBuilder extensionFileTypeComponentBuilder =
                    (IFileTypeComponentBuilder) extension.CreateInstance();
                extensionFileTypeComponentBuilder.FileTypeManager = fileTypeManager;
                IFileTypeInformation extensionFileTypeInformation =
                    extensionFileTypeComponentBuilder.BuildFileTypeInformation(string.Empty);
                string extensionFileTypeDefinitionId = extensionFileTypeInformation.FileTypeDefinitionId.Id;


                if (Equals(extensionFileTypeDefinitionId, "SDL XLIFF 1.0 v 1.0.0.0"))
                {
                    var extractor = extensionFileTypeComponentBuilder.BuildFileExtractor(filePath);

                    return extractor;
                }
            }
            return null;
        }

        public void SetFileExtractorProperties(IFileExtractor extractor,IMultiFileConverter converter)
        {
            extractor.ItemFactory = converter.ItemFactory;
            extractor.BilingualParser.ItemFactory = converter.ItemFactory;
        }

        public IFileProperties GetFileProperties(IMultiFileConverter converter,string filePath)
        {
            var fileProperties = converter.ItemFactory.CreateFileProperties();
            fileProperties.FileConversionProperties.InputFilePath = filePath;
            fileProperties.FileConversionProperties.OriginalFilePath = filePath;

            return fileProperties;
            
        }

        /// <summary>
        /// searches for matches in files
        /// </summary>
        /// <param name="_settings">search options</param>
        public void SearchInFiles(SearchSettings _settings)
        {
            validateSetts(_settings, false);

            _fileResults = new List<FileData>();
            
            var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
            foreach (string filePath in _files)
            {
                validateFile(filePath);

                var extractor = GetFileExtractor(filePath);

                if (extractor != null)
                {

                    var converter = fileTypeManager.GetConverter(extractor.BilingualParser);

                    SetFileExtractorProperties(extractor,converter);
                  
                    var parserProperties = extractor.BilingualParser as INativeContentCycleAware;

                    var fileProperties = GetFileProperties(converter, filePath);
                   
                    parserProperties?.SetFileProperties(fileProperties);


                    // create object to process each file text to perform search
                    FileReadProcessor readProcessor = new FileReadProcessor(filePath, _settings);

                    // set extractor and processor

                    converter.SynchronizeDocumentProperties();
                    converter.AddBilingualProcessor(readProcessor);

                    // start parsing the file
                    converter.Parse();

                    // save search results
                    _fileResults.Add(new FileData(filePath, readProcessor.ResultInSource, readProcessor.ResultInTarget));
                }
            }
        }

        /// <summary>
        /// searches for matches in files and performs replace of found ones
        /// </summary>
        /// <param name="_settings"></param>
        public void ReplaceInFiles(SearchSettings _settings)
        {
            validateSetts(_settings, true);

            _fileResults = new List<FileData>();

            foreach (string filePath in _files)
            {
                validateFile(filePath);

                createBackupFile(filePath);


                var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
                var extensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();
                //IFileExtractor parser = null;
                IBilingualDocumentGenerator writer = null;
                foreach (IExtension extension in extensionPoint.Extensions)
                {
                    IFileTypeComponentBuilder extensionFileTypeComponentBuilder = (IFileTypeComponentBuilder)extension.CreateInstance();
                    extensionFileTypeComponentBuilder.FileTypeManager = fileTypeManager;
                    IFileTypeInformation extensionFileTypeInformation = extensionFileTypeComponentBuilder.BuildFileTypeInformation(string.Empty);
                    string extensionFileTypeDefinitionId = extensionFileTypeInformation.FileTypeDefinitionId.Id;
                    FileTypeComponentBuilderAttribute attr = extension.ExtensionAttribute as FileTypeComponentBuilderAttribute;

                    if (Equals(extensionFileTypeDefinitionId, "SDL XLIFF 1.0 v 1.0.0.0"))
                    {

                        //parser = extensionFileTypeComponentBuilder.BuildFileExtractor(filePath);
                         writer = extensionFileTypeComponentBuilder.BuildBilingualGenerator(filePath);
                    }
                }
                //XliffFileReader parser = new XliffFileReader(filePath);
                // var parser = FileReaderHelper.FileReader(filePath);

                var extractor = GetFileExtractor(filePath);
                if (extractor != null)
                {
                    var converter = fileTypeManager.GetConverter(extractor.BilingualParser);

                    SetFileExtractorProperties(extractor, converter);

                    var parserProperties = extractor.BilingualParser as INativeContentCycleAware;

                    var fileProperties = GetFileProperties(converter, filePath);

                    parserProperties?.SetFileProperties(fileProperties);

                    // var extractor = fileTypeManager.BuildFileExtractor(parser.BilingualParser, null);

                    // var writer = FileWriterHelper.FileWriter(filePath);

                    // create object to update file to replace found text
                    FileReplaceProcessor replaceProcessor = new FileReplaceProcessor(filePath, _settings);

                    // set extractor and processors
                  //  extractor.BilingualParser = parser.BilingualParser;
                   // converter.AddExtractor(extractor);

                    converter.SynchronizeDocumentProperties();
                    converter.AddBilingualProcessor(replaceProcessor);

                    if (writer != null) converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(writer.Input));

                    // start parsing the file
                    converter.Parse();

                    // save replace results
                    FileData fData = new FileData(filePath, replaceProcessor.ResultSource, replaceProcessor.ResultTarget);
                    fData.ReplaceResults = replaceProcessor.ResultOfReplace;
                    fData.Warnings = replaceProcessor.Warnings;
                    _fileResults.Add(fData);

                    if (fData.ReplaceResults.Count < 1)
                        removeCreatedFile(filePath);
                    else if (!_settings.MakeBackup)
                        removeBackupFile(filePath);
                }
            }
        }

        /// <summary>
        /// searches for matches in files and updates segment statuses
        /// </summary>
        /// <param name="_settings"></param>
        //public void UpdateStatus(SearchSettings _settings)
        //{
        //    validateSetts(_settings, false);

        //    _fileResults = new List<FileData>();

        //    foreach (string filePath in _files)
        //    {
        //        validateFile(filePath);

                
        //        //XliffFileReader parser = new XliffFileReader(filePath);
        //      //  var parser = FileReaderHelper.FileReader(filePath);
        //        var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);

        //        var extensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<FileTypeComponentBuilderAttribute>();
        //        IFileExtractor parser = null;
        //        IBilingualDocumentGenerator writer = null;
        //        foreach (IExtension extension in extensionPoint.Extensions)
        //        {
        //            IFileTypeComponentBuilder extensionFileTypeComponentBuilder = (IFileTypeComponentBuilder)extension.CreateInstance();
        //            extensionFileTypeComponentBuilder.FileTypeManager = fileTypeManager;
        //            IFileTypeInformation extensionFileTypeInformation = extensionFileTypeComponentBuilder.BuildFileTypeInformation(string.Empty);
        //            string extensionFileTypeDefinitionId = extensionFileTypeInformation.FileTypeDefinitionId.Id;
        //            FileTypeComponentBuilderAttribute attr = extension.ExtensionAttribute as FileTypeComponentBuilderAttribute;

        //            if (Equals(extensionFileTypeDefinitionId, "SDL XLIFF 1.0 v 1.0.0.0") && attr.IsTemplate)
        //            {

        //                parser = extensionFileTypeComponentBuilder.BuildFileExtractor(filePath);
        //                writer = extensionFileTypeComponentBuilder.BuildBilingualGenerator(filePath);
        //            }
        //        }

        //        if (parser != null)
        //        {
        //            var converter = fileTypeManager.GetConverter(parser.BilingualParser);

        //            var extractor = fileTypeManager.BuildFileExtractor(parser.BilingualParser, null);

        //            // var writer = FileWriterHelper.FileWriter(filePath);

        //            // create object to process each file text to perform search
        //            FileStatusUpdateProcessor updateProcessor = new FileStatusUpdateProcessor(filePath, _settings);

        //            // set extractor and processor
        //            extractor.BilingualParser = parser.BilingualParser;
        //            converter.AddExtractor(extractor);
        //            converter.SynchronizeDocumentProperties();
        //            converter.AddBilingualProcessor(updateProcessor);
        //            converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(writer.Input));

        //            // start parsing the file
        //            converter.Parse();

        //            // save search results
        //            _fileResults.Add(new FileData(filePath, updateProcessor.ResultInSource, updateProcessor.ResultInTarget));
        //        }
        //    }
        //}

        #region private

        private void createBackupFile(string filePath)
        {
            string fname = string.Format("{0}.backup", filePath);

            removeBackupFile(filePath);

            System.IO.File.Copy(filePath, fname);
        }

        private void removeBackupFile(string filePath)
        {
            string fname = string.Format("{0}.backup", filePath);
            if (System.IO.File.Exists(fname))
                System.IO.File.Delete(fname);
        }

        private void removeCreatedFile(string filePath)
        {
            string fname = string.Format("{0}.backup", filePath);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            System.IO.File.Move(fname, filePath);
        }

        private void validateFile(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                throw new DirectoryNotFoundException(string.Format("Directory '{0}' not found.",
                    Path.GetDirectoryName(filePath)));
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException(string.Format("File '{0}' not found.", filePath));
            if (Path.GetExtension(filePath).ToLower() != _ext)
                throw new FileNotSupportedException(
                    string.Format("File extension '{0}' not supported.", Path.GetExtension(filePath)), filePath);
        }

        private void validateSetts(SearchSettings settings, bool isReplace)
        {
            if (settings.SearchText == null || settings.SearchText.Length == 0)
                throw new ArgumentOutOfRangeException("Search Text cannot be empty.");
            //if (isReplace)
            //    if (settings.ReplaceText == null || settings.ReplaceText.Length == 0)
            //        throw new ArgumentOutOfRangeException("Replace Text cannot be empty.");
        }

        #endregion
    }
}
