using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System.IO;
using System.Text;

namespace Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters
{
    public class CsvGlossaryReaderWriter : IGlossaryReaderWriter
    {
        public CsvGlossaryReaderWriter(char delimiter = ',') => Delimiter = delimiter;

        private char Delimiter { get; }

        public ActionResult<Glossary> ReadGlossary(string filePath) =>
                    ErrorHandler.WrapTryCatch(() =>
            {
                using var reader = new StreamReader(filePath, Encoding.Default);
                var glossary = new Glossary();

                while (reader.ReadLine() is { } line)
                {
                    var fields = line.Split(Delimiter);
                    glossary.Entries.Add(new GlossaryEntry { SourceTerm = fields[0], TargetTerm = fields[1] });
                }

                return glossary;
            });

        public ActionResult<Glossary> WriteGlossary(Glossary glossary, string filePath) =>
            ErrorHandler.WrapTryCatch(() =>
            {
                using var writer = new StreamWriter(filePath);
                glossary.Entries.ForEach(ge => writer.WriteLine($"{ge.SourceTerm},{ge.TargetTerm}"));
                return glossary;
            });
    }
}