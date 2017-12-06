using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
	public class TermbaseDefinitionFile
	{
		private static ITelemetryTracker telemetryTracker = new TelemetryTracker();

		/// <summary>
		/// Extract the content from embedded resource file 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string GetResourceTextFile(string fileName)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Reading embedded .xdt file");
				telemetryTracker.TrackEvent("Reading embedded .xdt file", null);

				var result = string.Empty;

				using (Stream stream = typeof(TermbaseDefinitionFile).Assembly.GetManifestResourceStream("Sdl.Community.ProjectTerms.Plugin.Resources." + fileName))
				{
					using (StreamReader sr = new StreamReader(stream))
					{
						result = sr.ReadToEnd();
					}
				}

				return result;
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseDefinitionException(PluginResources.Error_GetResourceTextFile + e.Message));
				telemetryTracker.TrackTrace((new TermbaseDefinitionException(PluginResources.Error_GetResourceTextFile + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseDefinitionException(PluginResources.Error_GetResourceTextFile + e.Message);
			}
		}

		/// <summary>
		/// Save the embedded resource content locally in the Temp folder
		/// </summary>
		/// <param name="content"></param>
		public static string SaveTermbaseDefinitionToTempLocation(string content)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Saving embedded .xdt file locally");
				telemetryTracker.TrackEvent("Saving embedded .xdt file locally", null);

				var tempLocationPath = Path.GetTempPath();
				var termbaseDefinitionPath = Path.Combine(tempLocationPath + "Termbases", "termbaseDefinition.xdt");
				Utils.Utils.CreateDirectory(Path.GetDirectoryName(termbaseDefinitionPath));

				var doc = new XmlDocument();
				doc.LoadXml(content);
				doc.Save(termbaseDefinitionPath);

				return termbaseDefinitionPath;
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseDefinitionException(PluginResources.Error_SaveTermbaseDefinitionToTempLocation + e.Message));
				telemetryTracker.TrackTrace((new TermbaseDefinitionException(PluginResources.Error_SaveTermbaseDefinitionToTempLocation + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseDefinitionException(PluginResources.Error_SaveTermbaseDefinitionToTempLocation + e.Message);
			}
		}

		/// <summary>
		/// Add languages to termbase definition file.
		/// </summary>
		/// <param name="termbaseDefinitionPath"></param>
		/// <param name="langs"></param>
		public static void AddLanguages(string termbaseDefinitionPath, Dictionary<string, string> langs)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding languages into .xdt file");
				telemetryTracker.TrackEvent("Adding languages into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var langTag = doc.GetElementsByTagName("Languages")[0];
				foreach (var lang in langs.Keys)
				{
					var nodeItemLocale = doc.CreateNode(XmlNodeType.Element, "ItemLocale", null);
					nodeItemLocale.InnerText = langs[lang];
					langTag.AppendChild(nodeItemLocale);

					var nodeItemText = doc.CreateNode(XmlNodeType.Element, "ItemText", null);
					nodeItemText.InnerText = lang;
					langTag.AppendChild(nodeItemText);
				}

				doc.Save(termbaseDefinitionPath);
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message));
				telemetryTracker.TrackTrace((new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message);
			}
		}

		/// <summary>
		/// Add language groups to termbase definition file.
		/// </summary>
		/// <param name="termbaseDefinitionPath">termbase definition path</param>
		/// <param name="langs">project languages</param>
		public static void AddLanguageGroups(string termbaseDefinitionPath, Dictionary<string, string> langs, string entryType)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding language groups into .xdt file");
				telemetryTracker.TrackEvent("Adding language groups into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var entryTag = doc.GetElementsByTagName(entryType)[0];
				var conceptGrpTag = entryTag.ChildNodes[0];

				foreach (var lang in langs.Keys)
				{
					var nodeLanguageGrp = doc.CreateNode(XmlNodeType.Element, "languageGrp", null);
					var nodeLanguage = doc.CreateElement("language");
					nodeLanguage.SetAttribute("type", langs[lang]);
					nodeLanguage.SetAttribute("lang", lang);
					nodeLanguageGrp.AppendChild(nodeLanguage);
				}

				doc.Save(termbaseDefinitionPath);
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message));
				telemetryTracker.TrackTrace((new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message);
			}
		}

		/// <summary>
		/// Add schema attributes for the languages to termbase definition file.
		/// </summary>
		/// <param name="termbaseDefinitionPath">termbase defitinion path</param>
		/// <param name="langs">project languages</param>
		public static void AddSchemaElements(string termbaseDefinitionPath, Dictionary<string, string> langs)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding language groups into .xdt file");
				telemetryTracker.TrackEvent("Adding language groups into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var schemaTag = doc.GetElementsByTagName("Schema")[0];
				var elementTypeNode = doc.GetElementsByTagName("ElementType")[3];

				string longNameLanguages = string.Join("|", langs.Keys);
				string shortNameLanguages = string.Join("|", langs.Values);

				var attributeTypeTag = doc.CreateElement("AttributeType");
				attributeTypeTag.SetAttribute("name", "type");
				attributeTypeTag.SetAttribute("type", "languages");
				attributeTypeTag.SetAttribute("values", longNameLanguages);
				elementTypeNode.AppendChild(attributeTypeTag);
				doc.Save(termbaseDefinitionPath);

				var attributeTypeLangTag = doc.CreateElement("AttributeType");
				attributeTypeTag.SetAttribute("name", "lang");
				attributeTypeTag.SetAttribute("type", "locales");
				attributeTypeTag.SetAttribute("values", shortNameLanguages);
				elementTypeNode.AppendChild(attributeTypeLangTag);

				doc.Save(termbaseDefinitionPath);
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message));
				telemetryTracker.TrackTrace((new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseDefinitionException(PluginResources.Error_AddLanguages + e.Message);
			}
		}
	}
}
