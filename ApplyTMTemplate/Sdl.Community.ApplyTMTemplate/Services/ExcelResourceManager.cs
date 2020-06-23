using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class ExcelResourceManager : IResourceManager
	{

		public void ExportResourcesToExcel(ILanguageResourcesContainer resourceContainer, string filePathTo, Settings settings)
		{
			using var package = GetExcelPackage(filePathTo);
			foreach (var languageResourceBundle in resourceContainer.LanguageResourceBundles)
			{
				if (languageResourceBundle == null) continue;

				var worksheet = package.Workbook.Worksheets.Add(languageResourceBundle.Language.Name);
				PrepareWorksheet(worksheet);

				var lineNumber = 1;
				if (settings.AbbreviationsChecked && languageResourceBundle.Abbreviations != null)
				{
					foreach (var abbreviation in languageResourceBundle.Abbreviations.Items)
					{
						worksheet.Cells["A" + ++lineNumber].Value = abbreviation;
					}
				}

				lineNumber = 1;
				if (settings.OrdinalFollowersChecked && languageResourceBundle.OrdinalFollowers != null)
				{
					foreach (var ordinalFollower in languageResourceBundle.OrdinalFollowers.Items)
					{
						worksheet.Cells["B" + ++lineNumber].Value = ordinalFollower;
					}
				}

				lineNumber = 1;
				if (settings.VariablesChecked && languageResourceBundle.Variables != null)
				{
					foreach (var variable in languageResourceBundle.Variables.Items)
					{
						worksheet.Cells["C" + ++lineNumber].Value = variable;
					}
				}

				lineNumber = 1;
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

				lineNumber = 1;
				if (settings.DatesChecked && (languageResourceBundle.LongDateFormats != null || languageResourceBundle.ShortDateFormats != null))
				{
					var dates = new List<string>();

					if (languageResourceBundle.LongDateFormats != null)
					{
						dates.AddRange(languageResourceBundle.LongDateFormats);
					}
					if (languageResourceBundle.ShortDateFormats != null)
					{
						dates.AddRange(languageResourceBundle.ShortDateFormats);
					}

					foreach (var date in dates)
					{
						worksheet.Cells["E" + ++lineNumber].Value = date;
					}
				}

				lineNumber = 1;
				if (settings.TimesChecked && (languageResourceBundle.LongTimeFormats != null || languageResourceBundle.ShortTimeFormats != null))
				{
					var times = new List<string>();

					if (languageResourceBundle.LongTimeFormats != null)
					{
						times.AddRange(languageResourceBundle.LongTimeFormats);
					}
					if (languageResourceBundle.ShortTimeFormats != null)
					{
						times.AddRange(languageResourceBundle.ShortTimeFormats);
					}

					foreach (var time in times)
					{
						worksheet.Cells["F" + ++lineNumber].Value = time;
					}
				}

				lineNumber = 1;
				if (settings.NumbersChecked && languageResourceBundle.NumbersSeparators != null)
				{
					foreach (var separator in languageResourceBundle.NumbersSeparators)
					{
						worksheet.Cells["G" + ++lineNumber].Value =
							$"{{{separator.GroupSeparators} {separator.DecimalSeparators}}}";
					}
				}

				lineNumber = 1;
				if (settings.MeasurementsChecked && languageResourceBundle.MeasurementUnits != null)
				{
					foreach (var unit in languageResourceBundle.MeasurementUnits)
					{
						worksheet.Cells["H" + ++lineNumber].Value = unit.Key;
					}
				}

				lineNumber = 1;
				if (settings.CurrenciesChecked && languageResourceBundle.CurrencyFormats != null)
				{
					foreach (var currency in languageResourceBundle.CurrencyFormats)
					{
						worksheet.Cells["I" + ++lineNumber].Value = currency.Symbol;
					}
				}
			}

			package.Save();
		}

		private static void PrepareWorksheet(ExcelWorksheet worksheet)
		{
			worksheet.Column(1).Width = 15;
			worksheet.Column(2).Width = 20;
			worksheet.Column(3).Width = 20;
			worksheet.Column(4).Width = 25;
			worksheet.Column(5).Width = 25;
			worksheet.Column(6).Width = 25;
			worksheet.Column(7).Width = 20;
			worksheet.Column(8).Width = 20;
			worksheet.Column(9).Width = 17;

			//every column that has its Style.Locked set to true will be read-only if IsProtected is also set to true;
			//we need to make just the segmentation rules column read-only
			worksheet.Column(1).Style.Locked = false;
			worksheet.Column(2).Style.Locked = false;
			worksheet.Column(3).Style.Locked = false;
			worksheet.Column(4).Style.Locked = true;
			worksheet.Column(5).Style.Locked = true;

			worksheet.Protection.IsProtected = true;

			SetCellStyle(worksheet, "A", 1, PluginResources.Abbreviations);
			SetCellStyle(worksheet, "B", 1, PluginResources.OrdinalFollowers);
			SetCellStyle(worksheet, "C", 1, PluginResources.Variables);
			SetCellStyle(worksheet, "D", 1, PluginResources.SegmentationRules);
			SetCellStyle(worksheet, "E", 1, PluginResources.Dates);
			SetCellStyle(worksheet, "F", 1, PluginResources.Times);
			SetCellStyle(worksheet, "G", 1, PluginResources.NumberSeparators);
			SetCellStyle(worksheet, "H", 1, PluginResources.Measurements);
			SetCellStyle(worksheet, "I", 1, PluginResources.Currencies);
		}

		private static void SetCellStyle(ExcelWorksheet worksheet, string columnLetter, int lineNumber, string property)
		{
			worksheet.Cells[columnLetter + lineNumber].Value = property;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(37, 189, 89));
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Color.SetColor(Color.White);
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Name = "Sommet Rounded";
		}

		public List<LanguageResourceBundle> GetResourceBundlesFromExcel(string filePathFrom)
		{
			using var package = GetExcelPackage(filePathFrom);
			var newLanguageResourceBundles = new List<LanguageResourceBundle>();
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

			return newLanguageResourceBundles;
		}

		private bool AreColumnsValid(object column01, object column02, object column03, object column04)
		{
			bool areValid;
			try
			{
				areValid = column01.ToString().Equals(PluginResources.Abbreviations) &&
						   column02.ToString().Equals(PluginResources.OrdinalFollowers) &&
						   column03.ToString().Equals(PluginResources.Variables) &&
						   column04.ToString().Equals(PluginResources.SegmentationRules);
			}
			catch
			{
				areValid = false;
			}

			return areValid;
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
	}
}