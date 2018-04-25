using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.Anonymizer.Models;

namespace Sdl.Community.Anonymizer.Helpers
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
	}
}
