using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class ExcelImportExportService
	{
		private ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		public void ImportResources(string filePathFrom, string filePathTo, Settings settings, List<LanguageResourceBundle> languageResourceBundles)
		{
			using (var package = GetExcelPackage(filePathFrom))
			{
				foreach (var workSheet in package.Workbook.Worksheets)
				{
					var column01 = workSheet.Cells[1, 1].Value;
					var column02 = workSheet.Cells[1, 2].Value;
					var column03 = workSheet.Cells[1, 3].Value;
					var column04 = workSheet.Cells[1, 4].Value;

					//more validation is necessary
					if (AreColumnsValid(column01, column02, column03, column04)) return;

					var abbreviations = new Wordlist();
					var ordinalFollowers = new Wordlist();
					var variables = new Wordlist();
					var segmentationRules = new SegmentationRules();

					ReadFromExcel(workSheet, abbreviations, ordinalFollowers, variables, segmentationRules);

					var newLanguageResourceBundle =
						new LanguageResourceBundle(CultureInfoExtensions.GetCultureInfo(workSheet.Name))
						{
							Abbreviations = abbreviations,
							OrdinalFollowers = ordinalFollowers,
							Variables = variables,
							SegmentationRules = segmentationRules
						};

					var relevantResourceBundleFromTheTemplateSerialized = languageResourceBundles.FirstOrDefault(lrb => lrb.Language.LCID == newLanguageResourceBundle.Language.LCID);

					SaveToXml(filePathTo, newLanguageResourceBundle, relevantResourceBundleFromTheTemplateSerialized);
				}
			}
		}

		private void SaveToXml(string filePathTo, LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle deserializedTemplate)
		{
			var currentCultureLcid = deserializedTemplate?.Language.LCID ?? newLanguageResourceBundle.Language.LCID;
			var defaultLanguageResourceProvider = GetDefaultResourceBundles(currentCultureLcid);
			var serializedTemplate = LoadDataFromFile(filePathTo);
			XmlNode node;

			//check if there are any abbreviations to be added (besides the default ones or the ones already in the template)

			//template to compare with
			var defaultOrOldTemplate = deserializedTemplate ?? defaultLanguageResourceProvider;
			var abrrevsToBeAdded = new Wordlist();
			foreach (var itemA in newLanguageResourceBundle.Abbreviations.Items)
			{
				if (defaultOrOldTemplate.Abbreviations.Items.All(itemB => !itemA.Equals(itemB, StringComparison.OrdinalIgnoreCase)))
				{
					abrrevsToBeAdded.Add(itemA);
				}
			}

			//add them if there are any
			if (abrrevsToBeAdded.Count > 0)
			{
				node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup[@Type='Abbreviations' and @Lcid='{currentCultureLcid.ToString()}']");

				foreach (var abbrev in abrrevsToBeAdded.Items)
				{
					defaultOrOldTemplate.Abbreviations.Add(abbrev);
				}

				node?.ParentNode?.RemoveChild(node);

				var xmlElement = CreateLanguageResourceNode(serializedTemplate, "Abbreviations", currentCultureLcid.ToString());

				var stringWriter = new Utf8StringWriter();
				defaultOrOldTemplate.Abbreviations.Save(stringWriter);
				var bytesAbbreviations = Encoding.UTF8.GetBytes(stringWriter.ToString());
				var base64Abbreviations = Convert.ToBase64String(bytesAbbreviations);
				xmlElement.InnerText = base64Abbreviations;

				serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);
			}

			//check if there are any ordinalFollowers to be added (besides the default ones
			var ordinalFollowersToBeAdded = new Wordlist();
			foreach (var itemA in newLanguageResourceBundle.OrdinalFollowers.Items)
			{
				if (defaultOrOldTemplate.OrdinalFollowers.Items.All(itemB => !itemA.Equals(itemB, StringComparison.OrdinalIgnoreCase)))
				{
					ordinalFollowersToBeAdded.Add(itemA);
				}
			}

			//add them if there are any
			if (ordinalFollowersToBeAdded.Count > 0)
			{
				node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup[@Type='OrdinalFollowers' and @Lcid='{currentCultureLcid.ToString()}']");

				foreach (var abbrev in ordinalFollowersToBeAdded.Items)
				{
					defaultOrOldTemplate.OrdinalFollowers.Add(abbrev);
				}

				node?.ParentNode?.RemoveChild(node);

				var xmlElement = CreateLanguageResourceNode(serializedTemplate, "OrdinalFollowers", currentCultureLcid.ToString());

				var stringWriter = new Utf8StringWriter();
				defaultOrOldTemplate.OrdinalFollowers.Save(stringWriter);
				var bytesOrdinalFollowers = Encoding.UTF8.GetBytes(stringWriter.ToString());
				var base64OrdinalFollowers = Convert.ToBase64String(bytesOrdinalFollowers);
				xmlElement.InnerText = base64OrdinalFollowers;

				serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);
			}

			//check if there are any variables to be added (besides the default ones
			var variablesToBeAdded = new Wordlist();
			foreach (var itemA in newLanguageResourceBundle.Variables.Items)
			{
				if (defaultOrOldTemplate.Variables.Items.All(itemB => !itemA.Equals(itemB, StringComparison.OrdinalIgnoreCase)))
				{
					variablesToBeAdded.Add(itemA);
				}
			}

			//add them if there are any
			if (variablesToBeAdded.Count > 0)
			{
				node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup[@Type='Variables' and @Lcid='{currentCultureLcid.ToString()}']");
				foreach (var abbrev in variablesToBeAdded.Items)
				{
					defaultOrOldTemplate.Variables.Add(abbrev);
				}

				node?.ParentNode?.RemoveChild(node);

				var xmlElement = CreateLanguageResourceNode(serializedTemplate, "Variables", currentCultureLcid.ToString());

				var stringWriter = new Utf8StringWriter();
				defaultOrOldTemplate.Variables.Save(stringWriter);
				var bytesVariables = Encoding.UTF8.GetBytes(stringWriter.ToString());
				var base64Variables = Convert.ToBase64String(bytesVariables);
				xmlElement.InnerText = base64Variables;

				serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);
			}

			//check if there are any segmentation rules to add (besides the default ones)
			var segmentationRulesToBeAdded = new SegmentationRules();
			foreach (var ruleA in newLanguageResourceBundle.SegmentationRules.Rules)
			{
				if (defaultOrOldTemplate.SegmentationRules.Rules.All(ruleB => !ruleA.Description.Text.Equals(ruleB.Description.Text, StringComparison.OrdinalIgnoreCase)))
				{
					segmentationRulesToBeAdded.AddRule(ruleA);
				}
			}

			//add them if there are any
			if (segmentationRulesToBeAdded.Count > 0)
			{
				node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup/LanguageResource[@Type='SegmentationRules' and @Lcid='{currentCultureLcid}']");

				segmentationRulesToBeAdded.Rules.AddRange(defaultOrOldTemplate.SegmentationRules.Rules);
				node?.ParentNode?.RemoveChild(node);

				var xmlElement = CreateLanguageResourceNode(serializedTemplate, "SegmentationRules", currentCultureLcid.ToString());
				var memoryStream = new MemoryStream();
				segmentationRulesToBeAdded.Save(memoryStream);
				var segmentationRulesBase64 = Convert.ToBase64String(memoryStream.ToArray());
				xmlElement.InnerText = segmentationRulesBase64;
				serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);
			}

			var xmlTextWriter = new XmlTextWriter(filePathTo, Encoding.UTF8) { Formatting = Formatting.None };
			serializedTemplate.Save(xmlTextWriter);
		}

		private LanguageResourceBundle GetDefaultResourceBundles(int currentCultureLcid)
		{
			var defaultLanguageResourceProvider = new DefaultLanguageResourceProvider().GetDefaultLanguageResources(CultureInfoExtensions.GetCultureInfo(currentCultureLcid));

			if (defaultLanguageResourceProvider.Abbreviations == null)
			{
				defaultLanguageResourceProvider.Abbreviations = new Wordlist();
			}

			if (defaultLanguageResourceProvider.OrdinalFollowers == null)
			{
				defaultLanguageResourceProvider.OrdinalFollowers = new Wordlist();
			}

			if (defaultLanguageResourceProvider.Variables == null)
			{
				defaultLanguageResourceProvider.Variables = new Wordlist();
			}

			if (defaultLanguageResourceProvider.SegmentationRules == null)
			{
				defaultLanguageResourceProvider.SegmentationRules = new SegmentationRules();
			}

			return defaultLanguageResourceProvider;
		}

		//private string SerializeConvertToBase64(LanguageResourceBundle languageResourceBundle, string property)
		//{
		//	var stringWriter = new Utf8StringWriter();
		//	var type = DetermineTypeOfProperty(property);
		//	var xmlSerializer = new XmlSerializer(type);
		//	xmlSerializer.Serialize(stringWriter, languageResourceBundle.GetType().GetProperty(property).GetMethod.Invoke(languageResourceBundle, null));
		//	var bytes = Encoding.UTF8.GetBytes(stringWriter.ToString());
		//	var base64 = Convert.ToBase64String(bytes);
		//	return base64;
		//}

		//private Type DetermineTypeOfProperty(string property)
		//{
		//	switch (property)
		//	{
		//		case "Abbreviations":
		//		case "Variables":
		//		case "OrdinalFollowers":
		//			return typeof(Wordlist);
		//		default:
		//			return typeof(SegmentationRules);
		//	}

		//}

		private void ReadFromExcel(ExcelWorksheet workSheet, Wordlist abbreviations, Wordlist ordinalFollowers,
			Wordlist variables, SegmentationRules segmentationRules)
		{
			for (var i = 2; i <= workSheet.Dimension.End.Row; i++)
			{
				abbreviations.Add(workSheet.Cells[i, 1]?.Value?.ToString());
				ordinalFollowers.Add(workSheet.Cells[i, 2]?.Value?.ToString());
				variables.Add(workSheet.Cells[i, 3]?.Value?.ToString());

				var serializedSegmentationRule = workSheet.Cells[i, 4]?.Value?.ToString();

				if (serializedSegmentationRule == null) continue;

				var stringReader = new StringReader(serializedSegmentationRule);
				var xmlSerializer = new XmlSerializer(typeof(SegmentationRule));

				var segmentationRule = (SegmentationRule) xmlSerializer.Deserialize(stringReader);

				segmentationRules.AddRule(segmentationRule);
			}
		}

		private bool AreColumnsValid(object column01, object column02, object column03, object column04)
		{
			return !column01.ToString().Equals("Abbreviations") &&
			       !column02.ToString().Equals("OrdinalFollowers") &&
			       !column03.ToString().Equals("Variables") &&
			       !column04.ToString().Equals("SegmentationRules");
		}

		private XmlElement CreateLanguageResourceNode(XmlDocument langResFile, string type, string lcid)
		{
			var langResNode = langResFile.CreateElement("LanguageResource");
			langResNode.SetAttribute("Type", type);
			langResNode.SetAttribute("Lcid", lcid);

			return langResNode;
		}

		public void ExportResources(List<LanguageResourceBundle> template, string filePathTo)
		{
			using (var package = GetExcelPackage(filePathTo))
			{
				foreach (var languageResourceBundle in template)
				{
					var worksheet = package.Workbook.Worksheets.Add(languageResourceBundle.Language.Name);

					worksheet.Column(1).Width = 20;
					worksheet.Column(2).Width = 20;
					worksheet.Column(3).Width = 20;
					worksheet.Column(4).Width = 100;

					var lineNumber = 1;

					worksheet.Cells["A" + lineNumber].Value = "Abbreviations";
					worksheet.Cells["A" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["A" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["A" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["A" + lineNumber].Style.Font.Name = "Sommet Rounded";

					if (languageResourceBundle.Abbreviations != null)
					{
						foreach (var abbreviation in languageResourceBundle.Abbreviations.Items)
						{
							worksheet.Cells["A" + ++lineNumber].Value = abbreviation;
						}
					}

					lineNumber = 1;

					worksheet.Cells["B" + lineNumber].Value = "OrdinalFollowers";
					worksheet.Cells["B" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["B" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["B" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["B" + lineNumber].Style.Font.Name = "Sommet Rounded";

					if (languageResourceBundle.OrdinalFollowers != null)
					{
						foreach (var ordinalFollower in languageResourceBundle.OrdinalFollowers.Items)
						{
							worksheet.Cells["B" + ++lineNumber].Value = ordinalFollower;
						}
					}

					lineNumber = 1;

					worksheet.Cells["C" + lineNumber].Value = "Variables";
					worksheet.Cells["C" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["C" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["C" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["C" + lineNumber].Style.Font.Name = "Sommet Rounded";

					if (languageResourceBundle.Variables != null)
					{
						foreach (var variable in languageResourceBundle.Variables.Items)
						{
							worksheet.Cells["C" + ++lineNumber].Value = variable;
						}
					}

					lineNumber = 1;

					worksheet.Cells["D" + lineNumber].Value = "SegmentationRules";
					worksheet.Cells["D" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["D" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["D" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["D" + lineNumber].Style.Font.Name = "Sommet Rounded";

					if (languageResourceBundle.SegmentationRules != null)
					{
						foreach (var segmentationRule in languageResourceBundle.SegmentationRules.Rules)
						{
							var stringWriter = new Utf8StringWriter();
							var xmlSerializer = new XmlSerializer(typeof(SegmentationRule));
							xmlSerializer.Serialize(stringWriter, segmentationRule);
							worksheet.Cells["D" + ++lineNumber].Value = stringWriter.ToString();
						}
					}
				}
				package.Save();
			}
		}

		public XmlDocument LoadDataFromFile(string filePath)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);

			return doc;
		}
	}
}
