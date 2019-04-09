using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
		public void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories, Settings settings, string templatePath, FileBasedLanguageResourcesTemplate deserializedTemplate)
		{
			var newLanguageResourceBundles = new List<LanguageResourceBundle>();
			foreach (var tm in translationMemories)
			{
				foreach (var bundle in tm.Tm.LanguageResourceBundles)
				{
					newLanguageResourceBundles.Add(bundle);
				}
			}

			ExcludeWhatIsNotNeeded(newLanguageResourceBundles, settings);

			AddNewBundles(deserializedTemplate, newLanguageResourceBundles);

			deserializedTemplate.SaveAs(templatePath);
		}

		public void ImportResourcesFromExcel(string filePathFrom, string filePathTo, Settings settings, FileBasedLanguageResourcesTemplate deserializedTemplate)
		{
			using (var package = GetExcelPackage(filePathFrom))
			{
				var newLanguageResourceBundles = new List<LanguageResourceBundle>();

				ReadFromExcel(settings, deserializedTemplate, package, newLanguageResourceBundles);

				deserializedTemplate.SaveAs(filePathTo);
			}
		}

		private void ReadFromExcel(Settings settings, FileBasedLanguageResourcesTemplate deserializedTemplate, ExcelPackage package, List<LanguageResourceBundle> newLanguageResourceBundles)
		{
			foreach (var workSheet in package.Workbook.Worksheets)
			{
				var column01 = workSheet.Cells[1, 1].Value;
				var column02 = workSheet.Cells[1, 2].Value;
				var column03 = workSheet.Cells[1, 3].Value;
				var column04 = workSheet.Cells[1, 4].Value;

				if (!AreColumnsValid(column01, column02, column03, column04))
				{
					throw new Exception(PluginResources.Excel_spreadsheet_not_in_correct_format);
				}

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

				newLanguageResourceBundles.Add(newLanguageResourceBundle);
			}
			ExcludeWhatIsNotNeeded(newLanguageResourceBundles, settings);
			AddNewBundles(deserializedTemplate, newLanguageResourceBundles);
		}

		public void ExcludeWhatIsNotNeeded(List<LanguageResourceBundle> languageResourceBundles, Settings settings)
		{
			foreach (var bundle in languageResourceBundles)
			{
				if (bundle.Abbreviations != null && (!settings.AbbreviationsChecked || bundle.Abbreviations.Count == 0))
				{
					bundle.Abbreviations = null;
				}

				if (bundle.OrdinalFollowers != null && (!settings.OrdinalFollowersChecked || bundle.OrdinalFollowers.Count == 0))
				{
					bundle.OrdinalFollowers = null;
				}

				if (bundle.Variables != null && (!settings.VariablesChecked || bundle.Variables.Count == 0))
				{
					bundle.Variables = null;
				}

				if (bundle.SegmentationRules != null && (!settings.SegmentationRulesChecked || bundle.SegmentationRules.Count == 0))
				{
					bundle.SegmentationRules = null;
				}
			}
		}

		public void ExportResources(FileBasedLanguageResourcesTemplate template, string filePathTo, Settings settings)
		{
			using (var package = GetExcelPackage(filePathTo))
			{
				foreach (var languageResourceBundle in template.LanguageResourceBundles)
				{
					var worksheet = package.Workbook.Worksheets.Add(languageResourceBundle.Language.Name);

					worksheet.Column(1).Width = 20;
					worksheet.Column(2).Width = 20;
					worksheet.Column(3).Width = 20;
					worksheet.Column(4).Width = 100;

					var lineNumber = 1;

					SetCellStyle(worksheet, "A", lineNumber, "Abbreviations");

					if (settings.AbbreviationsChecked && languageResourceBundle.Abbreviations != null)
					{
						foreach (var abbreviation in languageResourceBundle.Abbreviations.Items)
						{
							worksheet.Cells["A" + ++lineNumber].Value = abbreviation;
						}
					}

					lineNumber = 1;

					SetCellStyle(worksheet, "B", lineNumber, "OrdinalFollowers");

					if (settings.OrdinalFollowersChecked && languageResourceBundle.OrdinalFollowers != null)
					{
						foreach (var ordinalFollower in languageResourceBundle.OrdinalFollowers.Items)
						{
							worksheet.Cells["B" + ++lineNumber].Value = ordinalFollower;
						}
					}

					lineNumber = 1;

					SetCellStyle(worksheet, "C", lineNumber, "Variables");

					if (settings.VariablesChecked && languageResourceBundle.Variables != null)
					{
						foreach (var variable in languageResourceBundle.Variables.Items)
						{
							worksheet.Cells["C" + ++lineNumber].Value = variable;
						}
					}

					lineNumber = 1;

					SetCellStyle(worksheet, "D", lineNumber, "SegmentationRules");

					if (settings.SegmentationRulesChecked && languageResourceBundle.SegmentationRules != null)
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

		private static void SetCellStyle(ExcelWorksheet worksheet, string columnLetter, int lineNumber, string property)
		{
			worksheet.Cells[columnLetter + lineNumber].Value = property;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Color.SetColor(Color.White);
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Name = "Sommet Rounded";
		}

		private void AddNewBundles(FileBasedLanguageResourcesTemplate deserializedTemplate, List<LanguageResourceBundle> newLanguageResourceBundles)
		{
			foreach (var newBundle in newLanguageResourceBundles)
			{
				var correspondingBundleInTemplate = deserializedTemplate.LanguageResourceBundles[newBundle.Language];

				//in case there is already a bundle for that culture, we need to go into more detail and see what to add and what is already there
				if (correspondingBundleInTemplate != null)
				{
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "Abbreviations");
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "OrdinalFollowers");
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "Variables");
					AddSegmentationRulesToBundle(newBundle, correspondingBundleInTemplate);
				}
				//otherwise, just add the newBundle
				else
				{
					deserializedTemplate.LanguageResourceBundles.Add(newBundle);
				}
			}
		}

		private static void AddSegmentationRulesToBundle(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.SegmentationRules == null) return;
			if (correspondingBundleInTemplate.SegmentationRules != null)
			{
				var newSegmentationRules = new SegmentationRules();
				foreach (var newRule in newBundle.SegmentationRules.Rules)
				{
					if (correspondingBundleInTemplate.SegmentationRules.Rules.All(oldRule =>
						string.Equals(newRule.Description.Text, oldRule.Description.Text,
							StringComparison.OrdinalIgnoreCase)))
					{
						newSegmentationRules.AddRule(newRule);
					}
				}

				correspondingBundleInTemplate.SegmentationRules.Rules.AddRange(newSegmentationRules.Rules);
			}
			else
			{
				correspondingBundleInTemplate.SegmentationRules = new SegmentationRules(newBundle.SegmentationRules);
			}
		}

		private static void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle template, string property)
		{
			var templateBundleGetter = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(template, null) as Wordlist);
			var templateBundleSetter = typeof(LanguageResourceBundle).GetProperty(property)?.SetMethod;
			var newBundleGetter = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist);

			if (newBundleGetter == null || !newBundleGetter.Items.Any()) return;

			if (templateBundleGetter != null && templateBundleGetter.Items.Any())
			{
				foreach (var abbrev in newBundleGetter.Items)
				{
					templateBundleGetter.Add(abbrev);
				}
			}
			else
			{
				templateBundleSetter?.Invoke(template, new [] {new Wordlist(newBundleGetter)});
			}
		}

		private ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		private void ReadFromExcel(ExcelWorksheet workSheet, Wordlist abbreviations, Wordlist ordinalFollowers, Wordlist variables, SegmentationRules segmentationRules)
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

				var segmentationRule = (SegmentationRule)xmlSerializer.Deserialize(stringReader);

				segmentationRules.AddRule(segmentationRule);
			}
		}

		private bool AreColumnsValid(object column01, object column02, object column03, object column04)
		{
			bool areValid;
			try
			{
				areValid = !column01.ToString().Equals("Abbreviations") ||
				           !column02.ToString().Equals("OrdinalFollowers") ||
				           !column03.ToString().Equals("Variables") ||
				           !column04.ToString().Equals("SegmentationRules");
			}
			catch
			{
				areValid = false;
			}

			return areValid;
		}
	}
}