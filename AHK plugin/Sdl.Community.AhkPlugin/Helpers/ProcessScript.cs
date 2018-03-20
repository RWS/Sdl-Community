﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSOFile;
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
				if (firstEndScriptPosition > -1 && startScriptPosition > -1)
				{
					var script = GetScript(lines);
					var pair = new KeyValuePair<string, Script>(fileName, script);
					scripts.Add(pair);
					if (firstEndScriptPosition < lines.Count - 1)
					{
						var countToBeRemoved = (firstEndScriptPosition + 2) - startScriptPosition;
						//remove index +2 to remove the empty line between scripts
						lines.RemoveRange(startScriptPosition, countToBeRemoved);
					}
					else
					{
						lines.Clear();
					}
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
			var script = new Script
			{
				ScriptId =  Guid.NewGuid().ToString()
			};
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
					while (!scriptLines[counter + contentLinesCounter].Contains(";=="))
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

		public static void ExportScript(string filePath, List<Script>scripts)
		{
			var scriptLines = new List<string>();
			var file = File.Create(filePath);
			
			file.Close();
			//write reload script
			//File.WriteAllText(filePath,PluginResources.reload);
			//var reloadScriptLines = File.ReadAllLines(filePath).ToList();
			//scriptLines.AddRange(reloadScriptLines);
			foreach (var script in scripts)
			{
				var scriptLinesContent = CreateScriptLinesContent(script);
				scriptLines.AddRange(scriptLinesContent);
			}

			File.WriteAllLines(filePath, scriptLines,Encoding.UTF8);
			//set custom property
			var fileProperties = new OleDocumentProperties();
			fileProperties.Open(filePath);
			object customProperty = "GeneratedByAhkPlugin";
			fileProperties.CustomProperties.Add("SdlCommunity", ref customProperty);
			fileProperties.Close(true);
		}

		private static List<string> CreateScriptLinesContent(Script script)
		{
			var separator = ";===";
			var scriptLines = new List<string>();
			scriptLines.Add(";Name");
			scriptLines.Add(";"+script.Name);
			scriptLines.Add(separator);
			scriptLines.Add(";Description");
			var descriptionLines = script.Description.Split(Environment.NewLine.ToCharArray());
			foreach (var description in descriptionLines)
			{
				if (!string.IsNullOrWhiteSpace(description))
				{
					scriptLines.Add("; "+description);
				}
			}
			scriptLines.Add(separator);
			scriptLines.Add(";Content");
			var contentLines = script.Text.Split(Environment.NewLine.ToCharArray());
			foreach (var content in contentLines)
			{
				if (!string.IsNullOrWhiteSpace(content))
				{
					//if (script.Active)
					//{
					//	scriptLines.Add(content);
					//}
					//else
					//{
					//	scriptLines.Add("; " + content);
					//}
					scriptLines.Add(content);
				}
			}
			scriptLines.Add(separator);
			scriptLines.Add(script.Active ? ";Active" : ";Disabled");
			scriptLines.Add(";endScript");
			scriptLines.Add(Environment.NewLine);
			return scriptLines;
		}
		
		public static bool IsGeneratedByAhkPlugin(string filePath)
		{
			var fileProperties = new OleDocumentProperties();
			fileProperties.Open(filePath);
			foreach (CustomProperty property in fileProperties.CustomProperties)
			{
				if (property.Name == "SdlCommunity")
				{
					return true;
				}
			}
			return false;
		}
	
	}
}