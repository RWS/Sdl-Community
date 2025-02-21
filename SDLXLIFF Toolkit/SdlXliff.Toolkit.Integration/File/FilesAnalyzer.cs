using Sdl.Core.PluginFramework;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace SdlXliff.Toolkit.Integration.File
{
    public class FilesAnalyzer
    {
        private const string _ext = ".sdlxliff";
        private List<FileData> _fileResults;
        private List<string> _files;

        /// <summary>
        ///
        /// </summary>
        /// <param name="files">file to analyze (search or replace in)</param>
        public FilesAnalyzer(string file)
        {
            _files = new List<string> { file };
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
                    (IFileTypeComponentBuilder)extension.CreateInstance();
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

        public IFileProperties GetFileProperties(IMultiFileConverter converter, string filePath)
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

                    SetFileExtractorProperties(extractor, converter);

                    var parserProperties = extractor.BilingualParser as INativeContentCycleAware;

                    var fileProperties = GetFileProperties(converter, filePath);

                    parserProperties?.SetFileProperties(fileProperties);

                    // create object to process each file text to perform search
                    FileReadProcessor readProcessor = new FileReadProcessor(filePath, _settings);

                    // set extractor and processor

                    converter.SynchronizeDocumentProperties();
                    converter.AddBilingualProcessor(readProcessor);

                    try
                    {
                        converter.Parse();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    // save search results
                    _fileResults.Add(new FileData(filePath, readProcessor.ResultInSource, readProcessor.ResultInTarget));
                }
            }
        }

        public void SetFileExtractorProperties(IFileExtractor extractor, IMultiFileConverter converter)
        {
            extractor.ItemFactory = converter.ItemFactory;
            extractor.BilingualParser.ItemFactory = converter.ItemFactory;
        }

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
    }
}