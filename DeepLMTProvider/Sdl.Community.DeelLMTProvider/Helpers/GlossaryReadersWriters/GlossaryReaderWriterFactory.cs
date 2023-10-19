using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Service;
using System;
using System.IO;

namespace Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters
{
    public class GlossaryReaderWriterFactory : IGlossaryReaderWriterFactory
    {
        public ActionResult<IGlossaryReaderWriter> CreateFileReader(string filePath, char delimiter = default)
        {
            var fileExtension = Path.GetExtension(filePath).ToLower();

            return ErrorHandler.WrapTryCatch<IGlossaryReaderWriter>(() => fileExtension switch
            {
                ".tsv" => new TsvGlossaryReaderWriter(),
                ".csv" => new CsvGlossaryReaderWriter(delimiter),
                _ => throw new NotSupportedException("File type not supported.")
            });
        }

        public ActionResult<IGlossaryReaderWriter> CreateGlossaryWriter(GlossaryReaderWriterService.Format format) =>
            ErrorHandler.WrapTryCatch<IGlossaryReaderWriter>(() => format switch
            {
                GlossaryReaderWriterService.Format.TSV => new TsvGlossaryReaderWriter(),
                GlossaryReaderWriterService.Format.CSV => new CsvGlossaryReaderWriter(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            });
    }
}