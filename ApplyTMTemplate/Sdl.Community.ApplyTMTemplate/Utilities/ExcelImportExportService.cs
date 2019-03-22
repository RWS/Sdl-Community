using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
		private XmlSerializer _xmlSerializer;

		public ExcelImportExportService()
		{
			_xmlSerializer = new XmlSerializer(typeof(SegmentationRule));
		}

		private ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		public void ImportResources(string filePathFrom, string filePathTo, Settings settings)
		{
			using (var package = GetExcelPackage(filePathFrom))
			{
				foreach (var workSheet in package.Workbook.Worksheets)
				{
					var column01 = workSheet.Cells[1, 1].Value;
					var column02 = workSheet.Cells[1, 2].Value;
					var column03 = workSheet.Cells[1, 3].Value;
					var column04 = workSheet.Cells[1, 4].Value;

					if (AreColumnsValid(column01, column02, column03, column04)) return;

					var abbreviations = new Wordlist();
					var ordinalFollowers = new Wordlist();
					var variables = new Wordlist();
					var segmentationRules = new SegmentationRules();

					ReadFromExcel(workSheet, abbreviations, ordinalFollowers, variables, segmentationRules);

					var languageResourceBundle =
						new LanguageResourceBundle(CultureInfoExtensions.GetCultureInfo(workSheet.Name))
						{
							Abbreviations = abbreviations,
							OrdinalFollowers = ordinalFollowers,
							Variables = variables,
							SegmentationRules = segmentationRules
						};

					var languageResourceFile = LoadDataFromFile(filePathTo);

					SaveToXml(filePathTo, languageResourceFile, workSheet, languageResourceBundle);
				}
			}
		}

		private static void SaveToXml(string filePathTo, XmlDocument languageResourceFile, ExcelWorksheet workSheet,
			LanguageResourceBundle languageResourceBundle)
		{
			var xmlElement = languageResourceFile.CreateElement("LanguageResource");
			xmlElement.SetAttribute("Type", "SegmentationRules");
			xmlElement.SetAttribute("Lcid", CultureInfoExtensions.GetCultureInfo(workSheet.Name).LCID.ToString());

			var xmlSerializer = new XmlSerializer(typeof(SegmentationRules));
			var stringWriter = new Utf8StringWriter();

			xmlSerializer.Serialize(stringWriter, languageResourceBundle.SegmentationRules);

			var segmentationRulesBytes = Encoding.UTF8.GetBytes(stringWriter.ToString());

			var segmentationRulesBase64 = Convert.ToBase64String(segmentationRulesBytes);

			xmlElement.InnerText = segmentationRulesBase64;

			languageResourceFile?.GetElementsByTagName("LanguageResourceGroup")[0].AppendChild(xmlElement);

			var xmlTextWriter = new XmlTextWriter(filePathTo, Encoding.UTF8) {Formatting = Formatting.None};

			languageResourceFile.Save(xmlTextWriter);
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

				var segmentationRule = (SegmentationRule) _xmlSerializer.Deserialize(stringReader);

				segmentationRules.AddRule(segmentationRule);
			}
		}

		private static bool AreColumnsValid(object column01, object column02, object column03, object column04)
		{
			return !column01.ToString().Equals("Abbreviations") &&
			       !column02.ToString().Equals("OrdinalFollowers") &&
			       !column03.ToString().Equals("Variables") &&
			       !column04.ToString().Equals("SegmentationRules");
		}

		public void ExportResources(List<LanguageResourceBundle> template, string filePath)
		{
			using (var package = GetExcelPackage(filePath))
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

					foreach (var abbreviation in languageResourceBundle.Abbreviations.Items)
					{
						worksheet.Cells["A" + ++lineNumber].Value = abbreviation;
					}

					lineNumber = 1;

					worksheet.Cells["B" + lineNumber].Value = "OrdinalFollowers";
					worksheet.Cells["B" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["B" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["B" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["B" + lineNumber].Style.Font.Name = "Sommet Rounded";

					foreach (var ordinalFollower in languageResourceBundle.OrdinalFollowers.Items)
					{
						worksheet.Cells["B" + ++lineNumber].Value = ordinalFollower;
					}

					lineNumber = 1;

					worksheet.Cells["C" + lineNumber].Value = "Variables";
					worksheet.Cells["C" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["C" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["C" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["C" + lineNumber].Style.Font.Name = "Sommet Rounded";
					foreach (var variable in languageResourceBundle.Variables.Items)
					{
						worksheet.Cells["C" + ++lineNumber].Value = variable;
					}

					lineNumber = 1;

					worksheet.Cells["D" + lineNumber].Value = "SegmentationRules";
					worksheet.Cells["D" + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells["D" + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
					worksheet.Cells["D" + lineNumber].Style.Font.Color.SetColor(Color.White);
					worksheet.Cells["D" + lineNumber].Style.Font.Name = "Sommet Rounded";
					foreach (var segmentationRule in languageResourceBundle.SegmentationRules.Rules)
					{
						var stringWriter = new Utf8StringWriter();
						_xmlSerializer.Serialize(stringWriter, segmentationRule);
						worksheet.Cells["D" + ++lineNumber].Value = stringWriter.ToString();
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
