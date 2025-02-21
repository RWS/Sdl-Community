﻿using Sdl.Community.ProjectTerms.Plugin.Exceptions;
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

				using (Stream stream = new MemoryStream(PluginResources.termbaseDefaultDefinitionFile))
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
		public static void AddLanguages(string termbaseDefinitionPath, Dictionary<string, string> languages)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding languages into .xdt file");
				telemetryTracker.TrackEvent("Adding languages into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var languageTag = doc.GetElementsByTagName("Languages")[0];
				foreach (var language in languages.Keys)
				{
					var nodeItemLocale = doc.CreateNode(XmlNodeType.Element, "ItemLocale", null);
					nodeItemLocale.InnerText = languages[language];
					languageTag.AppendChild(nodeItemLocale);

					var nodeItemText = doc.CreateNode(XmlNodeType.Element, "ItemText", null);
					nodeItemText.InnerText = language;
					languageTag.AppendChild(nodeItemText);
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
		public static void AddLanguageGroups(string termbaseDefinitionPath, Dictionary<string, string> languages, string entryType)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding language groups into .xdt file");
				telemetryTracker.TrackEvent("Adding language groups into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var entryTag = doc.GetElementsByTagName(entryType)[0];
				var conceptGrpTag = entryTag.ChildNodes[0];

				foreach (var language in languages.Keys)
				{
					var nodeLanguageGrp = doc.CreateNode(XmlNodeType.Element, "languageGrp", null);
					var nodeLanguage = doc.CreateElement("language");
					nodeLanguage.SetAttribute("type", language);
					nodeLanguage.SetAttribute("lang", languages[language]);
					nodeLanguageGrp.AppendChild(nodeLanguage);

					var termGrp = doc.CreateElement("termGrp");
					nodeLanguageGrp.AppendChild(termGrp);

					conceptGrpTag.AppendChild(nodeLanguageGrp);
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
		public static void AddSchemaElements(string termbaseDefinitionPath, Dictionary<string, string> languages)
		{
			try
			{
				telemetryTracker.StartTrackRequest("Adding language groups into .xdt file");
				telemetryTracker.TrackEvent("Adding language groups into .xdt file", null);

				var doc = new XmlDocument();
				doc.Load(termbaseDefinitionPath);

				var schemaTag = doc.GetElementsByTagName("Schema")[0];
				var elementTypeNode = doc.GetElementsByTagName("ElementType")[8];

				string longNameLanguages = string.Join("|", languages.Keys);
				string shortNameLanguages = string.Join("|", languages.Values);
				
				XmlAttributeCollection attributeTypeAttributes = doc.GetElementsByTagName("AttributeType")[1].Attributes;

				foreach (XmlAttribute attribute in attributeTypeAttributes)
				{
					if(attribute.Name == "values")
					{
						attribute.Value = longNameLanguages;
					}
				}
				
				XmlAttributeCollection attributeTypeLangAttributes = doc.GetElementsByTagName("AttributeType")[2].Attributes;
				foreach (XmlAttribute attribute in attributeTypeLangAttributes)
				{
					if (attribute.Name == "values")
					{
						attribute.Value = shortNameLanguages;
					}
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
	}
}
