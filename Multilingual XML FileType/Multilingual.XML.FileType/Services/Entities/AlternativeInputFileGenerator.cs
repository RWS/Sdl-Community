using System.IO;
using System.Text;
using Sdl.FileTypeSupport.Framework;

namespace Multilingual.XML.FileType.Services.Entities
{
    public class AlternativeInputFileGenerator
    {
        private readonly FileSystemService _fileSystemService;

        public AlternativeInputFileGenerator(FileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public string GenerateTempFileWithHiddenEntities(string filePath, Encoding encoding)
        {
            var tempFile = GetTemporaryFilePath();

            var inputStream = _fileSystemService.OpenRead(filePath);

            using (var outputStream = _fileSystemService.OpenWrite(tempFile))
            {
                using (var streamReader = new StreamReader(inputStream, encoding))
                {
                    string character;
                    while ((character = ReadCharacterOrSurrogatePair(streamReader)) != null)
                    {
                        switch (character)
                        {
                            case EntityConstants.BeginEntityRef:
                                WriteToOutputStream(EntityConstants.BeginSdlEntityRefEscape, outputStream, encoding);
                                break;
                            case EntityConstants.EndEntityRef:
                                WriteToOutputStream(EntityConstants.EndSdlEntityRefEscape, outputStream, encoding);
                                break;
                            default:
                                WriteToOutputStream(character, outputStream, encoding);
                                break;
                        }
                    }
                }
            }

            return tempFile;
        }

        private string ReadCharacterOrSurrogatePair(StreamReader streamReader)
        {
            int i = streamReader.Read();
            if (i == -1) return null;

            char c = (char)i;
            if (char.IsHighSurrogate(c))
            {
                // read the low surrogate as well
                int i2 = streamReader.Read();
                if (i2 == -1) return null;

                return c.ToString() + (char)i2;
            }

            // normal character
            return c.ToString();
        }

        private string GetTemporaryFilePath()
        {
            using (var sdlTempFileManager = new TempFileManager())
            {
                return sdlTempFileManager.FilePath;
            }
        }

        private void WriteToOutputStream(string text, Stream outputStream, Encoding encoding)
        {
            var bytes = encoding.GetBytes(text);
            outputStream.Write(bytes, 0, bytes.Length);
        }
    }
}
