using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class ExcelResourceManager : IExcelResourceManager
	{
		public void ExportResourcesToExcel(ILanguageResourcesContainer resourceContainer, string filePathTo, Settings settings)
		{
			using var package = GetExcelPackage(filePathTo);
			int lineNumber;
			foreach (var languageResourceBundle in resourceContainer.LanguageResourceBundles)
			{
				if (languageResourceBundle == null) continue;

				var worksheet = package.Workbook.Worksheets.Add(languageResourceBundle.Language.Name);
				PrepareWorksheet(worksheet);

				lineNumber = 1;
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
				if (settings.DatesChecked && languageResourceBundle.LongDateFormats != null)
				{
					var longDates = new List<string>();

					if (languageResourceBundle.LongDateFormats != null)
					{
						longDates.AddRange(languageResourceBundle.LongDateFormats);
					}

					foreach (var date in longDates)
					{
						worksheet.Cells["E" + ++lineNumber].Value = date;
					}
				}

				lineNumber = 1;
				if (settings.DatesChecked && languageResourceBundle.ShortDateFormats != null)
				{
					var shortDates = new List<string>();

					if (languageResourceBundle.ShortDateFormats != null)
					{
						shortDates.AddRange(languageResourceBundle.ShortDateFormats);
					}

					foreach (var date in shortDates)
					{
						worksheet.Cells["F" + ++lineNumber].Value = date;
					}
				}

				lineNumber = 1;
				if (settings.TimesChecked && languageResourceBundle.LongTimeFormats != null)
				{
					var longTimes = new List<string>();

					if (languageResourceBundle.LongTimeFormats != null)
					{
						longTimes.AddRange(languageResourceBundle.LongTimeFormats);
					}

					foreach (var time in longTimes)
					{
						worksheet.Cells["G" + ++lineNumber].Value = time;
					}
				}

				lineNumber = 1;
				if (settings.TimesChecked && languageResourceBundle.ShortTimeFormats != null)
				{
					var shortTimes = new List<string>();

					if (languageResourceBundle.ShortTimeFormats != null)
					{
						shortTimes.AddRange(languageResourceBundle.ShortTimeFormats);
					}

					foreach (var time in shortTimes)
					{
						worksheet.Cells["H" + ++lineNumber].Value = time;
					}
				}

				lineNumber = 1;
				if (settings.NumbersChecked && languageResourceBundle.NumbersSeparators != null)
				{
					foreach (var separator in languageResourceBundle.NumbersSeparators)
					{
						worksheet.Cells["I" + ++lineNumber].Value =
							$"{{{separator.GroupSeparators} {separator.DecimalSeparators}}}";
					}
				}

				lineNumber = 1;
				if (settings.MeasurementsChecked && languageResourceBundle.MeasurementUnits != null)
				{
					foreach (var unit in languageResourceBundle.MeasurementUnits)
					{
						worksheet.Cells["J" + ++lineNumber].Value = unit.Key;
					}
				}

				lineNumber = 1;
				if (settings.CurrenciesChecked && languageResourceBundle.CurrencyFormats != null)
				{
					foreach (var currency in languageResourceBundle.CurrencyFormats)
					{
						worksheet.Cells["K" + ++lineNumber].Value = $"{currency.Symbol} - {currency.CurrencySymbolPositions[0]}";
					}
				}
			}

			var globalSettingsWorksheet = package.Workbook.Worksheets.Add(PluginResources.GlobalSettings);
			PrepareGlobalSettingsWorksheet(globalSettingsWorksheet);
			if (settings.RecognizersChecked && resourceContainer.Recognizers != null)
			{
				var recognizers = resourceContainer.Recognizers.ToString().Split(',');
				lineNumber = 1;
				foreach (var recognizer in recognizers)
				{
					globalSettingsWorksheet.Cells[$"A{++lineNumber}"].Value = recognizer;
				}
			}

			if (settings.WordCountFlagsChecked && resourceContainer.WordCountFlags != null)
			{
				var wordCountFlags = resourceContainer.WordCountFlags.ToString().Split(',');
				lineNumber = 1;
				foreach (var wordCountFlag in wordCountFlags)
				{
					globalSettingsWorksheet.Cells[$"B{++lineNumber}"].Value = wordCountFlag;
				}
			}

			package.Save();
		}

		private void PrepareGlobalSettingsWorksheet(ExcelWorksheet worksheet)
		{
			worksheet.Column(1).Width = 25;
			worksheet.Column(2).Width = 20;

			SetCellStyle(worksheet, "A", 1, PluginResources.Recognizers);
			SetCellStyle(worksheet, "B", 1, PluginResources.WordCountFlags);
		}

		private void PrepareWorksheet(ExcelWorksheet worksheet)
		{
			worksheet.Column(1).Width = 15;
			worksheet.Column(2).Width = 20;
			worksheet.Column(3).Width = 20;
			worksheet.Column(4).Width = 25;
			worksheet.Column(5).Width = 25;
			worksheet.Column(6).Width = 25;
			worksheet.Column(7).Width = 25;
			worksheet.Column(8).Width = 20;
			worksheet.Column(9).Width = 20;
			worksheet.Column(10).Width = 20;
			worksheet.Column(11).Width = 17;

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
			SetCellStyle(worksheet, "E", 1, PluginResources.LongDates);
			SetCellStyle(worksheet, "F", 1, PluginResources.ShortDates);
			SetCellStyle(worksheet, "G", 1, PluginResources.LongTimes);
			SetCellStyle(worksheet, "H", 1, PluginResources.ShortTimes);
			SetCellStyle(worksheet, "I", 1, PluginResources.NumberSeparators);
			SetCellStyle(worksheet, "J", 1, PluginResources.Measurements);
			SetCellStyle(worksheet, "K", 1, PluginResources.Currencies);
		}

		private static void SetCellStyle(ExcelWorksheet worksheet, string columnLetter, int lineNumber, string property)
		{
			worksheet.Cells[columnLetter + lineNumber].Value = property;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
			worksheet.Cells[columnLetter + lineNumber].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 0, 128, 128));
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Color.SetColor(Color.White);
			worksheet.Cells[columnLetter + lineNumber].Style.Font.Name = "Sommet Rounded";
		}

		public List<LanguageResourceBundle> GetResourceBundlesFromExcel(string filePathFrom)
		{
			using var package = GetExcelPackage(filePathFrom);
			var newLanguageResourceBundles = new List<LanguageResourceBundle>();

			if (!IsExcelDocumentValid(package)) return null;
			foreach (var workSheet in package.Workbook.Worksheets)
			{
				if (workSheet.Name == PluginResources.GlobalSettings) continue;
				newLanguageResourceBundles.Add(GetFromExcel(workSheet, CultureInfoExtensions.GetCultureInfo(workSheet.Name)));
			}

			return newLanguageResourceBundles;
		}

		public (BuiltinRecognizers, WordCountFlags) GetTemplateGlobalSettings(string filePathFrom, Settings settings)
		{
			var excelDoc = GetExcelPackage(filePathFrom);
			var settingsWorksheet = excelDoc.Workbook.Worksheets["Global settings"];
			var cells = settingsWorksheet.Cells;

			var recognizers = BuiltinRecognizers.RecognizeNone;
			var wordCountFlags = WordCountFlags.NoFlags;
			for (var i = 2; i <= settingsWorksheet.Dimension.End.Row; i++)
			{
				if (settings.RecognizersChecked)
				{
					AddRecognizers(cells[i, 1].Value?.ToString(), ref recognizers);
				}
				if (settings.WordCountFlagsChecked)
				{
					AddWordCountFlags(cells[i, 2].Value?.ToString(), ref wordCountFlags);
				}
			}

			return (recognizers, wordCountFlags);
		}

		private void AddWordCountFlags(string wordCountFlagSerialized, ref WordCountFlags wordCountFlags)
		{
			if (!string.IsNullOrWhiteSpace(wordCountFlagSerialized))
			{
				if (Enum.TryParse(wordCountFlagSerialized, out WordCountFlags recognizer))
				{
					wordCountFlags |= recognizer;
				}
			}
		}

		private void AddRecognizers(string recognizerSerialized, ref BuiltinRecognizers recognizers)
		{
			if (!string.IsNullOrWhiteSpace(recognizerSerialized))
			{
				if (Enum.TryParse(recognizerSerialized, out BuiltinRecognizers recognizer))
				{
					recognizers |= recognizer;
				}
			}
		}

		private bool IsExcelDocumentValid(ExcelPackage excelPackage)
		{
			foreach (var worksheet in excelPackage.Workbook.Worksheets)
			{

				var cells = worksheet.Cells;

				var column01 = cells[1, 1].Value;
				var column02 = cells[1, 2].Value;
				if (worksheet.Name.Equals(PluginResources.GlobalSettings))
				{
					return column01.ToString().Equals(PluginResources.Recognizers) &&
						   column02.ToString().Equals(PluginResources.WordCountFlags);
				}

				var column03 = cells[1, 3].Value;
				var column04 = cells[1, 4].Value;
				var column05 = cells[1, 5].Value;
				var column06 = cells[1, 6].Value;
				var column07 = cells[1, 7].Value;
				var column08 = cells[1, 8].Value;
				var column09 = cells[1, 9].Value;
				var column10 = cells[1, 10].Value;
				var column11 = cells[1, 11].Value;

				return column01.ToString().Equals(PluginResources.Abbreviations) &&
					   column02.ToString().Equals(PluginResources.OrdinalFollowers) &&
					   column03.ToString().Equals(PluginResources.Variables) &&
					   column04.ToString().Equals(PluginResources.SegmentationRules) &&
					   column05.ToString().Equals(PluginResources.LongDates) &&
					   column06.ToString().Equals(PluginResources.ShortDates) &&
					   column07.ToString().Equals(PluginResources.LongTimes) &&
					   column08.ToString().Equals(PluginResources.ShortTimes) &&
					   column09.ToString().Equals(PluginResources.NumberSeparators) &&
					   column10.ToString().Equals(PluginResources.Measurements) &&
					   column11.ToString().Equals(PluginResources.Currencies);
			}

			return false;
		}

		private ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		//public string GetValueFromCell();

		private LanguageResourceBundle GetFromExcel(ExcelWorksheet workSheet, CultureInfo cultureInfo)
		{
			var cells = workSheet.Cells;

			var languageResourceBundle =
				new LanguageResourceBundle(cultureInfo)
				{
					Abbreviations = new Wordlist(),
					OrdinalFollowers = new Wordlist(),
					Variables = new Wordlist(),
					SegmentationRules = new SegmentationRules(),
					LongDateFormats = new List<string>(),
					ShortDateFormats = new List<string>(),
					LongTimeFormats = new List<string>(),
					ShortTimeFormats = new List<string>(),
					NumbersSeparators = new List<SeparatorCombination>(),
					MeasurementUnits = new Dictionary<string, CustomUnitDefinition>(),
					CurrencyFormats = new List<CurrencyFormat>()
				};

			for (var i = 2; i <= workSheet.Dimension.End.Row; i++)
			{
				var abbreviation = cells[i, 1]?.Value?.ToString();
				AddToWordlist(abbreviation, languageResourceBundle.Abbreviations);

				var ordinalFollower = cells[i, 2]?.Value?.ToString();
				AddToWordlist(ordinalFollower, languageResourceBundle.OrdinalFollowers);

				var variable = cells[i, 3]?.Value?.ToString();
				AddToWordlist(variable, languageResourceBundle.Variables);

				var serializedData = cells[i, 4]?.Value?.ToString();
				AddSegmentationRule(serializedData, languageResourceBundle.SegmentationRules.Rules);

				var longDate = cells[i, 5]?.Value?.ToString();
				AddDateFormat(longDate, languageResourceBundle.LongDateFormats);

				var shortDate = cells[i, 6]?.Value?.ToString();
				AddDateFormat(shortDate, languageResourceBundle.ShortDateFormats);

				var longTime = cells[i, 7]?.Value?.ToString();
				AddDateFormat(longTime, languageResourceBundle.LongTimeFormats);

				var shortTime = cells[i, 8]?.Value?.ToString();
				AddDateFormat(shortTime, languageResourceBundle.ShortTimeFormats);

				serializedData = cells[i, 9]?.Value?.ToString();
				AddNumberSeparator(serializedData, languageResourceBundle.NumbersSeparators);

				var unit = cells[i, 10]?.Value?.ToString();
				AddMeasurementUnits(unit, languageResourceBundle.MeasurementUnits);

				serializedData = cells[i, 11]?.Value?.ToString();
				AddCurrencies(serializedData, languageResourceBundle.CurrencyFormats);
			}

			return languageResourceBundle;
		}

		private static void AddCurrencies(string serializedData, List<CurrencyFormat> currencyFormats)
		{
			if (!string.IsNullOrWhiteSpace(serializedData))
			{
				var currencyData = serializedData.Split('-');
				var parseSucceeded = Enum.TryParse(currencyData[1], out CurrencySymbolPosition currencySymbolPosition);
				if (parseSucceeded)
				{
					var currencySymbolPositions = new List<CurrencySymbolPosition>
					{
						currencySymbolPosition,
						currencySymbolPosition == CurrencySymbolPosition.afterAmount
							? CurrencySymbolPosition.beforeAmount
							: CurrencySymbolPosition.afterAmount
					};
					var currency = new CurrencyFormat
					{
						Symbol = currencyData[0].Trim(),
						CurrencySymbolPositions = currencySymbolPositions
					};
					currencyFormats?.Add(currency);
				}
			}
		}

		private static void AddMeasurementUnits(string unit, Dictionary<string, CustomUnitDefinition> measurementUnits)
		{
			if (!string.IsNullOrWhiteSpace(unit))
			{
				measurementUnits?.Add(unit, new CustomUnitDefinition());
			}
		}

		private void AddNumberSeparator(string serializedData, List<SeparatorCombination> numberSeparators)
		{
			if (!string.IsNullOrWhiteSpace(serializedData))
			{
				var separatorCombinationsStrings = serializedData.Trim('{', '}').Split(' ');

				if (separatorCombinationsStrings.Length > 1)
				{
					var separatorCombination = new SeparatorCombination(separatorCombinationsStrings[0], separatorCombinationsStrings[1], false);
					numberSeparators?.Add(separatorCombination);
				}
			}
		}

		private void AddDateFormat(string date, List<string> list)
		{
			if (!string.IsNullOrWhiteSpace(date))
			{
				list?.Add(date);
			}
		}

		private void AddToWordlist(string word, Wordlist list)
		{
			if (!string.IsNullOrWhiteSpace(word))
			{
				list?.Add(word);
			}
		}

		private void AddSegmentationRule(string serializedData, List<SegmentationRule> list)
		{
			if (!string.IsNullOrWhiteSpace(serializedData))
			{
				var stringReader = new StringReader(serializedData);
				var xmlSerializer = new XmlSerializer(typeof(SegmentationRule));

				var segmentationRule = (SegmentationRule)xmlSerializer.Deserialize(stringReader);

				if (segmentationRule != null)
				{
					list?.Add(segmentationRule);
				}
			}
		}
	}
}