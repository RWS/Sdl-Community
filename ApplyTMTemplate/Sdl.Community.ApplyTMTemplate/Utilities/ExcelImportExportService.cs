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

			//template to use and compare the new one with
			var defaultOrOldTemplate = deserializedTemplate ?? defaultLanguageResourceProvider;

			var node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup/LanguageResource[@Type='Abbreviations' and @Lcid='{currentCultureLcid.ToString()}']");
			AddItemsToWordlist(newLanguageResourceBundle, defaultOrOldTemplate, "Abbreviations");
			node?.ParentNode?.RemoveChild(node);

			var xmlElement = CreateLanguageResourceNode(serializedTemplate, "Abbreviations", currentCultureLcid.ToString());
			xmlElement.InnerText = ConvertWordlistToBase64(defaultOrOldTemplate, "Abbreviations");
			serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);

			node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup/LanguageResource[@Type='OrdinalFollowers' and @Lcid='{currentCultureLcid.ToString()}']");
			AddItemsToWordlist(newLanguageResourceBundle, defaultOrOldTemplate, "OrdinalFollowers");
			node?.ParentNode?.RemoveChild(node);

			xmlElement = CreateLanguageResourceNode(serializedTemplate, "OrdinalFollowers", currentCultureLcid.ToString());
			xmlElement.InnerText = ConvertWordlistToBase64(defaultOrOldTemplate, "OrdinalFollowers");
			serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);

			node = serializedTemplate.SelectSingleNode($"/LanguageResourceGroup/LanguageResource[@Type='Variables' and @Lcid='{currentCultureLcid.ToString()}']");
			AddItemsToWordlist(newLanguageResourceBundle, defaultOrOldTemplate, "Variables");
			node?.ParentNode?.RemoveChild(node);

			xmlElement = CreateLanguageResourceNode(serializedTemplate, "Variables", currentCultureLcid.ToString());
			xmlElement.InnerText = ConvertWordlistToBase64(defaultOrOldTemplate, "Variables");
			serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);

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

				xmlElement = CreateLanguageResourceNode(serializedTemplate, "SegmentationRules", currentCultureLcid.ToString());
				xmlElement.InnerText = ConvertSegmentationRulesToBase64(defaultOrOldTemplate, segmentationRulesToBeAdded);
				serializedTemplate.SelectSingleNode("/LanguageResourceGroup")?.AppendChild(xmlElement);
			}

			var xmlTextWriter = new XmlTextWriter(filePathTo, Encoding.UTF8) { Formatting = Formatting.None };
			serializedTemplate.Save(xmlTextWriter);
		}

		private static void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle defaultOrOldTemplate, string property)
		{
			var getterOfDefaultOrOldTemplate = (typeof(LanguageResourceBundle).GetProperty(property).GetMethod.Invoke(defaultOrOldTemplate, null) as Wordlist);
			var getterOfNewLanguageResourceBundle = (typeof(LanguageResourceBundle).GetProperty(property).GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist);

			foreach (var abbrev in getterOfNewLanguageResourceBundle.Items)
			{
				getterOfDefaultOrOldTemplate.Add(abbrev);
			}
		}

		private static string ConvertWordlistToBase64(LanguageResourceBundle defaultOrOldTemplate, string property)
		{
			var stringWriter = new Utf8StringWriter();
			(typeof(LanguageResourceBundle).GetProperty(property).GetMethod.Invoke(defaultOrOldTemplate, null) as Wordlist).Save(stringWriter);
			var bytesAbbreviations = Encoding.UTF8.GetBytes(stringWriter.ToString());
			var base64Abbreviations = Convert.ToBase64String(bytesAbbreviations);
			return base64Abbreviations;
		}

		private static string ConvertSegmentationRulesToBase64(LanguageResourceBundle defaultOrOldTemplate, SegmentationRules segmentationRulesToBeAdded)
		{
			var memoryStream = new MemoryStream();
			segmentationRulesToBeAdded.Save(memoryStream);
			var segmentationRulesBase64 = Convert.ToBase64String(memoryStream.ToArray());
			return segmentationRulesBase64;
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
