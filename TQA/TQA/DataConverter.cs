using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.TQA.Model;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;

namespace Sdl.Community.TQA
{
	public static class DataConverter
	{
		private static readonly string ProtectionPassword = "Thames";

		public static ReportResults ExtractFromXml(string path, string qualityLevel)
		{
			var report = XDocument.Load(path);
			var languages = report.Descendants("language");
			var reportResults = new ReportResults
			{
				Entries = new List<Entry>(),
				EvaluationComments = new List<string>(),
				QualityLevel = qualityLevel
			};
			foreach (var language in languages)
			{
				var languageString = language.Attribute("name").Value;
				var files = language.Descendants("file");

				foreach (var file in files)
				{
					var fileString = file.Attribute("name").Value;
					var evaluationComment = file.Attribute("evaluationComment").Value;
					if (!string.IsNullOrEmpty(evaluationComment))
					{
						reportResults.EvaluationComments.Add(evaluationComment);
					}
					var segments = file.Descendants("segment");

					foreach (var segment in segments)
					{
						var segmentId = segment.Attribute("id").Value;
						var sourceContent = segment.Element("sourceContent");
						var sourceContentText = new List<Tuple<string, TextType>>();
						foreach (var group in sourceContent.Descendants("item"))
						{
							switch (group.Attribute("type").Value)
							{
								case "":
									sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
									break;
								case "FeedbackAdded":
									if (sourceContent.Elements().Contains(group))
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Added));
									else
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
									break;
								case "FeedbackDeleted":
									if (sourceContent.Elements().Contains(group))
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Deleted));
									else
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
									break;
								case "FeedbackComment":
									sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Comment));
									break;
							}
						}

						var originalTranslation = segment.Attribute("originalTranslation").Value;
						var revisedTranslations = segment.Element("revisedTranslation").Elements()
							.Where(e => e.Name == "group" && (e.Attribute("category") != null || e.Attribute("severity") != null));

						foreach (var revisedTranslation in revisedTranslations)
						{
							var translation = revisedTranslation.Element("item").Attribute("content").Value;
							var category = revisedTranslation.Attribute("category").Value;
							var severity = revisedTranslation.Attribute("severity").Value;
							var comment = revisedTranslation.Attribute("comment").Value;

							var revisedTranslationText = new List<Tuple<string, TextType>>();
							foreach (var rTrans in segment.Element("revisedTranslation").Descendants().Where(e => e.Name == "item"))
							{
								switch (rTrans.Attribute("type").Value)
								{
									case "":
										revisedTranslationText.Add(
											new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
										break;
									case "FeedbackAdded":
										if (revisedTranslation.Elements().Contains(rTrans))
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Added));
										else
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
										break;
									case "FeedbackDeleted":
										if (revisedTranslation.Elements().Contains(rTrans))
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Deleted));
										else
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
										break;
									case "FeedbackComment":
										revisedTranslationText.Add(
											new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Comment));
										break;
								}
							}
							var entry = new Entry(languageString, fileString, segmentId, originalTranslation, revisedTranslationText,
						  sourceContentText, category, severity, comment, translation);
							reportResults.Entries.Add(entry);
						}
					}
				}
			}
			return reportResults;
		}

		public static void WriteExcel(string path, ReportResults reportResults)
		{
			using (var fs = new FileStream(path, FileMode.Create))
			{
				fs.Write(PluginResources.template, 0, PluginResources.template.Length);
			}
			var rows = reportResults.Entries;
			var rowsArray = rows.ToArray();
			var rowsCollection = rows.Select(r => r.GetArray()).ToArray();
			var wb = new XLWorkbook(path);

			var ws = wb.Worksheet("Evaluation details_input");

			for (var i = 0; i < rows.Count; i++)
			{
				for (var j = 0; j < rowsCollection[i].Length; j++)
				{
					ws.Row(i + 4).Cell(j + 1).Value = rowsCollection[i][j];
				}
				var cell = ws.Cell(i + 4, 5);

				var entry = rowsArray[i].RevisedTranslation;

				for (var k = 0; k < entry.Count; k++)
				{
					cell.RichText.AddText(entry[k].Item1);
					switch (entry[k].Item2)
					{
						case TextType.Added:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.GreenPigment);
							cell.RichText.ToArray()[k].SetUnderline();
							break;
						case TextType.Deleted:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.Red);
							cell.RichText.ToArray()[k].SetStrikethrough(true);
							break;
						case TextType.Regular:
							continue;
						case TextType.Comment:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.Blue);
							cell.RichText.ToArray()[k].SetBold();
							break;
					}
				}

				cell = ws.Cell(i + 4, 3);

				entry = rowsArray[i].SourceContent;
				for (var k = 0; k < entry.Count; k++)
				{
					cell.RichText.AddText(entry[k].Item1);
					switch (entry[k].Item2)
					{
						case TextType.Added:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.GreenPigment);
							cell.RichText.ToArray()[k].SetUnderline();
							break;
						case TextType.Deleted:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.Red);
							cell.RichText.ToArray()[k].SetStrikethrough(true);
							break;
						case TextType.Regular:
							continue;
						case TextType.Comment:
							cell.RichText.ToArray()[k].SetFontColor(XLColor.Blue);
							cell.RichText.ToArray()[k].SetBold();
							break;
					}
				}
			}

			GenerateInitialReportSheet(reportResults, wb);
			GenerateFinalReportSheet(wb);

			wb.CalculateMode = XLCalculateMode.Auto;
			wb.Save();

			var spreadsheet = SpreadsheetDocument.Open(path, true);
			//colors took from webpart stylesheet
			AddGradient(spreadsheet, "B6B6B6", "FFE16E73", "FFBDED0A");
			AddGradient(spreadsheet, "B6B6B7", "FFBDED0A", "FF008080");
			spreadsheet.Save();
			spreadsheet.Close();
		}

		private static void GenerateFinalReportSheet(XLWorkbook wb)
		{
			var wsFinalResult = wb.Worksheet("Evaluation Report_Final Result");
			ChangeStyleForEvaluationReport(wsFinalResult);
			wsFinalResult.Protect(ProtectionPassword);
			var rangeUsed = wsFinalResult.RangeUsed();
			rangeUsed.Style.Protection.SetLocked(true);
		}

		private static void GenerateInitialReportSheet(ReportResults reportResults, XLWorkbook wb)
		{
			var wsReport = wb.Worksheet("Evaluation Report_Initial");

			wsReport.Cell("C10").Value = DateTime.Now.ToString("dd-MMM-yyyy");
			wsReport.Cell("M9").Value = "TQA";
			wsReport.Cell("M12").Value = reportResults.QualityLevel;

			wsReport.Protect(ProtectionPassword);
			wsReport.Range(14, 1, 21, 9).Style.Protection.SetLocked(true); // only second table should be protected

			AjustCommentSize(wsReport);

			ChangeStyleForEvaluationReport(wsReport);

			for (var i = 0; i < reportResults.EvaluationComments.Count; i++)
			{
				wsReport.Row(i + 42).Cell(1).Value = reportResults.EvaluationComments[i];
			}
		}

		private static void AjustCommentSize(IXLWorksheet wsReport)
		{
			wsReport.Cell("C4").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C5").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C6").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C7").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C10").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M8").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M9").Comment?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M10").Comment?.Style.Size.SetAutomaticSize();
		}

		private static void AddGradient(SpreadsheetDocument spreadSheet, string dummyColorCode, string firstColorCode, string secondColorCode)
		{
			var wbPart = spreadSheet.GetPartsOfType<WorkbookPart>().FirstOrDefault();
			var wbStylePart = wbPart?.GetPartsOfType<WorkbookStylesPart>().FirstOrDefault();
			var stylesheet = wbStylePart?.Stylesheet;

			var oldFill =
				stylesheet?.Fills.FirstOrDefault(f => f.OuterXml.Contains(dummyColorCode)); // find the fill that uses your unique color
			if (oldFill == null) return;
			var gradientFill = new GradientFill { Degree = 0 };
			gradientFill.Append(new GradientStop { Position = 0D, Color = new Color { Rgb = firstColorCode } });
			gradientFill.Append(new GradientStop { Position = 1D, Color = new Color { Rgb = secondColorCode } });
			oldFill.ReplaceChild(gradientFill, oldFill.FirstChild); // inside the fill replace the patternFill with your gradientFill
		}

		/// <summary>
		/// Overwrithe the excel styles for "Evaluation Report Initial and Final result" to match the template 
		/// </summary>
		private static void ChangeStyleForEvaluationReport(IXLWorksheet wsReport)
		{
			//Remove border which crosses the logo from first table
			wsReport.Range("A1:Q1").Style.Border.BottomBorder = XLBorderStyleValues.None;
			wsReport.Range("A2:Q2").Style.Border.TopBorder = XLBorderStyleValues.None;
			//Dummy collor used later to replace with gradient
			wsReport.Cell("D33").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#B6B6B6")); // use some unique color
			wsReport.Cell("G33").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#B6B6B7")); // use some unique color

			wsReport.Cell("D33").Style.Border.SetRightBorderColor(XLColor.White);
		}

	}
}
