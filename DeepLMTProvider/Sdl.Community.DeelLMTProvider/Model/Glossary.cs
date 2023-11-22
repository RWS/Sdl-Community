using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class Glossary
	{
		public List<GlossaryEntry> Entries { get; set; } = new();
		public string Name { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
	}
}