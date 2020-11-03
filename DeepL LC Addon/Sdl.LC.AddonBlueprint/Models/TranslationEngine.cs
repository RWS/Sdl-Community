using System.Collections.Generic;

namespace Sdl.Community.DeeplAddon.Models
{
	public class TranslationEngine
	{
		public string Id { get; set; }
		public string Model { get; set; }
		public string EngineSourceLanguage { get; set; }
		public string EngineTargetLanguage { get; set; }
		public string MatchingSourceLanguage { get; set; }	
		public string Name { get; set; }
		public List<string> MatchingTargetLanguages { get; set; }
	}
}
