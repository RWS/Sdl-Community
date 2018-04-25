using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExporessions(string  filePath,List<RegexPattern> patterns)
		{
			using (var file = File.CreateText(filePath))
			{
				var serializer = new JsonSerializer();
				serializer.Serialize(file, patterns);
			}
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
			foreach (var file in files)
			{
				var expressions = JsonConvert.DeserializeObject<List<RegexPattern>>(File.ReadAllText(file));
				patterns.AddRange(expressions);
			}

			return patterns;
		}
	}
}
