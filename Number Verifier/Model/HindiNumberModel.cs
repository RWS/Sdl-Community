using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Model
{
	public class HindiNumberModel
	{
		public string[] SourceGroups { get; set; }
		public string[] TargetGroups { get; set; }
		public string[] TextGroups { get; set; }
		public string SourceLanguage { get; set; }
		public Dictionary<string, string> HindiDictionary { get; set; }
		public Dictionary<string, string> TargetDictionary { get; set; }
	}
}