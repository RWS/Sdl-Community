using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using System;
using System.IO;

namespace Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters
{
    public class GlossaryReaderWriterFactory : IGlossaryReaderWriterFactory
    {
        public ActionResult<IGlossaryReaderWriter> CreateFileReader(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath).ToLower();

            return ErrorHandler.WrapTryCatch<IGlossaryReaderWriter>(() =>
            {
                return fileExtension switch
                {
                    ".tv" => new TsvGlossaryReaderWriter(),
                    ".csv" => throw new NotSupportedException("File type not supported."),
                    _ => throw new NotSupportedException("File type not supported.")
                };
            });
        }
    }
}