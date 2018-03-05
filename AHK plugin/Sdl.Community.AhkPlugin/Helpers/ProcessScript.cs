using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.Helpers
{
	public static class ProcessScript
	{
		public static List<KeyValuePair<string, Script>> ReadImportedScript(string path)
		{
			var scripts = new List<KeyValuePair<string, Script>>();
			var lines =
				File.ReadAllLines(path).ToList();
			var fileName = Path.GetFileNameWithoutExtension(path);
			while (lines.Count>0)
			{
				var firstEndScriptPosition = lines.IndexOf(";endScript");
				var startScriptPosition = lines.IndexOf(";Name");
				
				var script = GetScript(lines);
				var pair = new KeyValuePair<string, Script>(fileName, script);
				scripts.Add(pair);
				if (firstEndScriptPosition < lines.Count-1)
				{
					//remove index +2 to remove the empty line between scripts
					lines.RemoveRange(startScriptPosition, firstEndScriptPosition + 2);
				}
				else
				{
					lines.Clear();
				}
				
			}
			return scripts;
		}

		private static Script GetScript(List<string>scriptLines)
		{
			var counter = 0;
			var endScriptPosition = scriptLines.IndexOf(";endScript");
			var script = new Script();
			while (counter < endScriptPosition)
			{
				if (scriptLines[counter].Equals(";Name"))
				{
					//increase with 2 because after name we have ===
					//read the name without ";" character which is used for comment
					script.Name = scriptLines[counter + 1].Substring(1);
					counter = counter + 2;
				}
				if (scriptLines[counter].Equals(";Description"))
				{
					var descriptionLinesCounter = 1;
					var descriptionBuilder = new StringBuilder();

					while (!scriptLines[counter + descriptionLinesCounter].Contains(";=="))
					{
						descriptionBuilder.AppendLine(scriptLines[counter + descriptionLinesCounter].Substring(1));
						descriptionLinesCounter++;
					}
					script.Description = descriptionBuilder.ToString();
				}
				if (scriptLines[counter].Equals(";Content"))
				{
					var contentLinesCounter = 1;
					var contentBuilder = new StringBuilder();
					while (!scriptLines[counter + contentLinesCounter].Contains(";endScript"))
					{
						contentBuilder.AppendLine(scriptLines[counter + contentLinesCounter]);
						contentLinesCounter++;
					}
					script.Text = contentBuilder.ToString();
				}
				if (scriptLines[counter].Equals(";Active"))
				{
					script.Active = true;
				}
				if (scriptLines[counter].Equals(";Disabled"))
				{
					script.Active = false;
				}
				counter++;
			}
			return script;
		}
	
	}
}