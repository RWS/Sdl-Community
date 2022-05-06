using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Helpers
{
	public class RegexImporter
	{
		public List<string> ImportPatterns(List<string> fileNames)
		{
			var patterns = new List<string>();
			foreach (var fileName in fileNames)
			{
				if (!File.Exists(fileName)) continue;

				var text = File.ReadAllText(fileName);
				patterns.AddRange(Regex.Split(text, Environment.NewLine));
			}

			return patterns;
		}
	}
}
