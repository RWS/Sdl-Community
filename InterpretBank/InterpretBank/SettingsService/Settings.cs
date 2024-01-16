using System.Collections.Generic;
using Newtonsoft.Json;

namespace InterpretBank.SettingsService
{
	public class Settings
	{
		public string DatabaseFilepath { get; set; }

		[JsonIgnore]
		public string SettingsId { get; set; }

		public List<string> Glossaries { get; set; }
		public List<string> Tags { get; set; }
	}
}