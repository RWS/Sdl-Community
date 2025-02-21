using System.IO;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public class TsvReaderWriter : ITsvReaderWriter
    {
        public Glossary ReadTsvGlossary(string filename)
        {
            using var reader = new StreamReader(filename);
            var glossary = new Glossary();

            while (reader.ReadLine() is { } line)
            {
                var fields = line.Split('\t');
                glossary.Entries.Add(new GlossaryEntry
                {
                    SourceTerm = fields[0],
                    TargetTerm = fields[1]
                });
            }

            return glossary;
        }

        public void WriteTsvGlossary(Glossary glossary, string filename)
        {
            using var writer = new StreamWriter(filename);
            glossary.Entries.ForEach(ge => writer.WriteLine($"{ge.SourceTerm}\t{ge.TargetTerm}"));
        }
    }
}