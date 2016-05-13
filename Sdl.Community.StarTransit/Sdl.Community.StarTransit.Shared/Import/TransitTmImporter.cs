using System;
using System.IO;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
    public class TransitTmImporter
    {
        private readonly IFileTypeManager _fileTypeManager;

        public TransitTmImporter():this(DefaultFileTypeManager.CreateInstance(true))
        {
            
        }

        public TransitTmImporter(IFileTypeManager fileTypeManager)
        {
            _fileTypeManager = fileTypeManager;
        }

        public void CreateSdlXliffFile(string pathToTargetTmFiles,string selectedTmName)
        {
            var pathToExtractFolder = CreateFolderToExtract(pathToTargetTmFiles.Substring(0,pathToTargetTmFiles.LastIndexOf(@"\", StringComparison.Ordinal)));

            var generatedXliffName = GetTmName(pathToTargetTmFiles)+ ".sdlxliff";

            var converter = _fileTypeManager.GetConverterToDefaultBilingual(pathToTargetTmFiles,
                Path.Combine(pathToExtractFolder, generatedXliffName), null);
            converter.Parse();

           
            var pathToExtractedSdlxliff = Path.Combine(pathToExtractFolder, generatedXliffName);
            var fileBase = new FileBasedTranslationMemory(selectedTmName);
            var tmImporter = new TranslationMemoryImporter(fileBase.LanguageDirection);
           
            tmImporter.Import(pathToExtractedSdlxliff);
        }

        private string CreateFolderToExtract(string pathToTemp)
        {
            var pathToExtractFolder = Path.Combine(pathToTemp, "TmExtract");
            if (!Directory.Exists(pathToExtractFolder))
            {
                Directory.CreateDirectory(pathToExtractFolder);
            }

            return pathToExtractFolder;
        }

        private string GetTmName(string tmFilePath)
        {
            var intermediateName =
               tmFilePath.Substring(tmFilePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

            var tmName = intermediateName.Substring(0, intermediateName.LastIndexOf(".", StringComparison.Ordinal));

            return tmName;
        }
    }
}
