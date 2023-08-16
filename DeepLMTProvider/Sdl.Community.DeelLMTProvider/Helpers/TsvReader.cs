using System.IO;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
	public class TsvReader
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
	}
}