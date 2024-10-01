using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NLog;
using Sdl.Community.TQA.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
//using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;

namespace Sdl.Community.TQA.Providers
{
	public enum TQAProfileType
	{
		tqsEmpty,
		tqsJ2450,
		tqsMQM,
		tqsOther
	}

    public class ReportProvider
    {
        private const string DefaultReportRelativePath = "Reports\\TQAReports";
        private const string reportingFileExtension = ".xlsm";
        private const string TQS_J2450_ReportFileName = "SDL-5-401-F001 QE Form_XXX_XXXXXX_XXX_XX";
        private const string TQS_J2450_ReportTemplate = "SDL-5-401-F001-QE Form_XXX_XXXXXX_XXX_XX.XLSM";
        private const string TQS_MQM_ReportFileName = "RWS-5-401-F002-MQM QE Form-CLIENT_MMM_YY";
        private const string TQS_MQM_ReportTemplate = "RWS-5-401-F002-MQM QE Form-CLIENT_MMM_YY_LL.XLSM";
        private static readonly string ProtectionPassword = "Thames";
        private readonly Logger _logger;
        private readonly string _reportRelativePath;

		public ReportProvider(Logger logger, string reportRelativePath = null)
		{
			_logger = logger;
			if (string.IsNullOrEmpty(reportRelativePath))
			{
				_reportRelativePath = DefaultReportRelativePath;
			}
		}

		public string GetReportDirectory(string projectRootPath)
		{
			if (!Directory.Exists(projectRootPath))
			{
				throw new DirectoryNotFoundException("Param: ProjectRootPath " + projectRootPath);
			}

			var tqaReportsDirectory = Path.Combine(projectRootPath.TrimEnd('\\'), _reportRelativePath);

			if (!Directory.Exists(tqaReportsDirectory))
			{
				Directory.CreateDirectory(tqaReportsDirectory);
			}

			return tqaReportsDirectory;
		}

		public string GetDefaultReportFileFullPath(string reportsDirectory, string defaultReportFileName, string languageId)
		{
			if (string.IsNullOrEmpty(reportsDirectory) || !Directory.Exists(reportsDirectory))
			{
				throw new DirectoryNotFoundException("Param: ReportsDirectory " + reportsDirectory);
			}

			if (string.IsNullOrEmpty(defaultReportFileName))
			{
				throw new ArgumentNullException(defaultReportFileName);
			}

			if (string.IsNullOrEmpty(languageId))
			{
				throw new ArgumentNullException(defaultReportFileName);
			}

			var reportFileName = string.Concat(defaultReportFileName, "_", languageId, reportingFileExtension);
			var reportFilePath = Path.Combine(reportsDirectory, reportFileName);

			var index = 1;
			while (File.Exists(reportFilePath))
			{
				reportFileName = string.Concat(defaultReportFileName, "_", languageId, " (" + index + ")" + reportingFileExtension);
				reportFilePath = Path.Combine(reportsDirectory, reportFileName);
				index++;
			}

			return reportFilePath;
		}

		public string GetProfileTypeName(TQAProfileType tqaStandardType)
		{
			switch (tqaStandardType)
			{
				case TQAProfileType.tqsEmpty:
					return PluginResources.Label_Empty;
				case TQAProfileType.tqsJ2450:
					return PluginResources.Label_J2450Standard;
				case TQAProfileType.tqsMQM:
					return PluginResources.Label_MQMStandard;
				case TQAProfileType.tqsOther:
					return PluginResources.Label_NotCompatible;
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
			}
		}

		public string GetReportTemplateFileForTQAStandard(TQAProfileType tqaStandardType)
		{
			switch (tqaStandardType)
			{
				case TQAProfileType.tqsJ2450:
					return TQS_J2450_ReportTemplate;
				case TQAProfileType.tqsMQM:
					return TQS_MQM_ReportTemplate;
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
			}
		}

		public byte[] GetReportTemplateForTQAStandard(TQAProfileType tqaStandardType)
		{
			switch (tqaStandardType)
			{
				case TQAProfileType.tqsJ2450:
					return PluginResources.SDL_5_401_F001_QE_Form_XXX_XXXXXX_XXX_XX;
				case TQAProfileType.tqsMQM:
					return PluginResources.RWS_5_401_F002_MQM_QE_Form_XXX_XXXXXX_XXX;
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
			}
		}

		public string GetReportDefaultOutputFilename(TQAProfileType tqaStandardType)
		{
			switch (tqaStandardType)
			{
				case TQAProfileType.tqsJ2450:
					return TQS_J2450_ReportFileName;
				case TQAProfileType.tqsMQM:
					return TQS_MQM_ReportFileName;
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
			}
		}


		public ReportResults ExtractFromXml(string path, string qualityLevel)
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
									{
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Added));
									}
									else
									{
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
									}
									break;
								case "FeedbackDeleted":
									if (sourceContent.Elements().Contains(group))
									{
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Deleted));
									}
									else
									{
										sourceContentText.Add(new Tuple<string, TextType>(group.Attribute("content").Value, TextType.Regular));
									}
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
							var categorySubCategory = revisedTranslation.Attribute("category").Value;
							var subCategoryIdx = categorySubCategory.IndexOf("-");
							var category = subCategoryIdx >= 0 ? categorySubCategory.Substring(0, subCategoryIdx).Trim() : "";
							var subcategory = subCategoryIdx >= 0 ? categorySubCategory.Substring(subCategoryIdx + 1).Trim() : "";

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
										{
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Added));
										}
										else
										{
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
										}
										break;
									case "FeedbackDeleted":
										if (revisedTranslation.Elements().Contains(rTrans))
										{
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Deleted));
										}
										else
										{
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Regular));
										}
										break;
									case "FeedbackComment":
										{
											revisedTranslationText.Add(
												new Tuple<string, TextType>(rTrans.Attribute("content").Value, TextType.Comment));
										}
										break;
								}
							}

							var entry = new Entry(languageString, fileString, segmentId, originalTranslation, revisedTranslationText,
									sourceContentText, category, subcategory, severity, comment, translation);
							reportResults.Entries.Add(entry);
						}
					}
				}
			}
			return reportResults;
		}

		public void WriteExcel(string path, ReportResults reportResults, TQAProfileType tqaStandard)
		{
			using (var fs = new FileStream(path, FileMode.Create))
			{
				var template = GetReportTemplateForTQAStandard(tqaStandard);
				fs.Write(template, 0, template.Length);
			}

			var rows = reportResults.Entries;
			var rowsArray = rows.ToArray();
			var rowsCollection = rows.Select(r => r.GetArray(tqaStandard)).ToArray();
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
					cell.GetRichText().AddText(entry[k].Item1);
					switch (entry[k].Item2)
					{
						case TextType.Added:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.GreenPigment);
							cell.GetRichText().ToArray()[k].SetUnderline();
							break;
						case TextType.Deleted:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.Red);
							cell.GetRichText().ToArray()[k].SetStrikethrough(true);
							break;
						case TextType.Regular:
							continue;
						case TextType.Comment:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.Blue);
							cell.GetRichText().ToArray()[k].SetBold();
							break;
					}
				}

				cell = ws.Cell(i + 4, 3);

				entry = rowsArray[i].SourceContent;
				for (var k = 0; k < entry.Count; k++)
				{
					cell.GetRichText().AddText(entry[k].Item1);
					switch (entry[k].Item2)
					{
						case TextType.Added:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.GreenPigment);
							cell.GetRichText().ToArray()[k].SetUnderline();
							break;
						case TextType.Deleted:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.Red);
							cell.GetRichText().ToArray()[k].SetStrikethrough(true);
							break;
						case TextType.Regular:
							continue;
						case TextType.Comment:
							cell.GetRichText().ToArray()[k].SetFontColor(XLColor.Blue);
							cell.GetRichText().ToArray()[k].SetBold();
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

		public bool GenerateReport(FileBasedProject project, AutomaticTask automaticTask, TQAProfileType tqaProfileType, string reportFilePath, string quality)
		{
			var tempFile = Path.GetTempFileName();
			var tempReportFile = tempFile + ".xml";
			File.Move(tempFile, tempReportFile);

			try
			{
				project.SaveTaskReportAs(automaticTask.Reports[0].Id, tempReportFile, ReportFormat.Xml);
				var extractedData = ExtractFromXml(tempReportFile, quality);

				WriteExcel(reportFilePath, extractedData, tqaProfileType);
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n " + PluginResources.MsgTQATaskNotRunCorrectly);
				return false;
			}
			finally
			{
				//var projectReportsOperations = new ProjectReportsOperations(project);
				//var projectReports = projectReportsOperations.GetProjectReports();
				//var projectReport = projectReports.FirstOrDefault(r => r.Id == automaticTask.Reports[0].Id);
				//if (projectReport != null)
				//{
				//	projectReportsOperations.RemoveReports(new List<Guid> { projectReport.Id });
				//}

				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}

				if (File.Exists(tempReportFile))
				{
					File.Delete(tempReportFile);
				}
			}

			return true;
		}

		private void GenerateFinalReportSheet(XLWorkbook wb)
		{
			var wsFinalResult = wb.Worksheet("Evaluation Report_Final Result");
			ChangeStyleForEvaluationReport(wsFinalResult);

			if (!wsFinalResult.IsProtected)
			{
				wsFinalResult.Protect(ProtectionPassword);
			}

			var rangeUsed = wsFinalResult.RangeUsed();
			rangeUsed.Style.Protection.SetLocked(true);
		}

		private void GenerateInitialReportSheet(ReportResults reportResults, XLWorkbook wb)
		{
			var wsReport = wb.Worksheet("Evaluation Report_Initial");

			wsReport.Cell("C10").Value = DateTime.Now.ToString("dd-MMM-yyyy");
			wsReport.Cell("M9").Value = "TQA";
			wsReport.Cell("M12").Value = reportResults.QualityLevel;

			if (!wsReport.IsProtected)
			{
				wsReport.Protect(ProtectionPassword);
			}

			wsReport.Range(14, 1, 21, 9).Style.Protection.SetLocked(true); // only second table should be protected

			AjustCommentSize(wsReport);

			ChangeStyleForEvaluationReport(wsReport);

			for (var i = 0; i < reportResults.EvaluationComments.Count; i++)
			{
				wsReport.Row(i + 42).Cell(1).Value = reportResults.EvaluationComments[i];
			}
		}

		private void AjustCommentSize(IXLWorksheet wsReport)
		{
			wsReport.Cell("C4").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C5").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C6").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C7").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("C10").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M8").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M9").GetComment()?.Style.Size.SetAutomaticSize();
			wsReport.Cell("M10").GetComment()?.Style.Size.SetAutomaticSize();
		}

		private void AddGradient(SpreadsheetDocument spreadSheet, string dummyColorCode, string firstColorCode, string secondColorCode)
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

		private void ChangeStyleForEvaluationReport(IXLWorksheet wsReport)
		{
			//Remove border which crosses the logo from first table
			wsReport.Range("A1:Q1").Style.Border.BottomBorder = XLBorderStyleValues.None;
			wsReport.Range("A2:Q2").Style.Border.TopBorder = XLBorderStyleValues.None;
			//   var sdlGreen = XLColor.FromHtml("#00A89F");
			//wsReport.Style.Border.SetOutsideBorderColor(sdlGreen);

			//Dummy collor used later to replace with gradient
			wsReport.Cell("D33").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#B6B6B6")); // use some unique color
			wsReport.Cell("G33").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#B6B6B7")); // use some unique color

			wsReport.Cell("D33").Style.Border.SetRightBorderColor(XLColor.White);
		}
	}
}
