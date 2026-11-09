using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.Qualitivity.Dialogs.Export;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Configuration;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class ExportToFileTests : IDisposable
	{
		private const long TicksPerTenthOfSecond = 1000000;

		private readonly string _file = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");

		[Fact]
		public void CreateExcelReport_ActivityWithTwoDocuments_WritesSummedElapsedSeconds()
		{
			// 90.6 s + 40.6 s = 131.2 s; converting each document first would give 130
			CreateReport(new List<XmlDocumentActivity>(), 906, 406);

			using (var workbook = SpreadsheetDocument.Open(_file, false))
			{
				Assert.Equal("Elapsed Seconds", CellText(workbook, "P1"));
				Assert.Equal("131", CellText(workbook, "P2"));
			}
		}

		[Fact]
		public void CreateExcelReport_OneActivity_FilterCoversAllColumnsAndRows()
		{
			CreateReport(new List<XmlDocumentActivity>(), 10);

			Assert.Contains("A1:P2", Filters());
		}

		[Fact]
		public void CreateExcelReport_FourRecords_DocumentActivitiesFilterCoversAllRows()
		{
			CreateReport(Enumerable.Range(0, 4).Select(i => Record()).ToList(), 10);

			Assert.Contains("A1:AB5", Filters());
		}

		[Fact]
		public void CreateExcelReport_RecordWithThreeMetrics_QualityMetricsFilterCoversAllRows()
		{
			var record = Record();
			record.QualityMetrics = Enumerable.Range(0, 3).Select(i => new QualityMetric { Name = "Metric" }).ToList();

			CreateReport(new List<XmlDocumentActivity> { record }, 10);

			Assert.Contains("A1:R4", Filters());
		}

		[Fact]
		public void CreateExcelReport_RecordWithTwentyKeystrokes_KeyStrokeFilterCoversAllColumnsAndRows()
		{
			var record = Record();
			record.KeyStrokes = Enumerable.Range(0, 20).Select(i => new KeyStroke()).ToList();

			CreateReport(new List<XmlDocumentActivity> { record }, 10);

			// 18 columns, A to R; the last one is Position
			Assert.Contains("A1:R21", Filters());
		}

		public void Dispose() => File.Delete(_file);

		private static XmlDocumentActivity Record()
		{
			return new XmlDocumentActivity { ProjectId = 1, ActivityId = 1 };
		}

		private void CreateReport(List<XmlDocumentActivity> records, params long[] documentTenthsOfSeconds)
		{
			var activity = new Activity
			{
				Id = 1,
				ProjectId = 1,
				Name = "Activity",
				Activities = documentTenthsOfSeconds
					.Select(tenths => new DocumentActivities { DocumentActivityTicks = tenths * TicksPerTenthOfSecond })
					.ToList()
			};

			Tracked.TrackingProjects = new TrackingProjects
			{
				TrackerProjects = new List<Project> { new Project { Id = 1, Name = "Project", Activities = new List<Activity> { activity } } }
			};

			new ExportToFile().create_excel_report(_file, new List<Activity> { activity }, records);
		}

		private List<string> Filters()
		{
			using (var workbook = SpreadsheetDocument.Open(_file, false))
			{
				return workbook.WorkbookPart.WorksheetParts
					.SelectMany(part => part.Worksheet.Descendants<AutoFilter>())
					.Select(filter => filter.Reference.Value)
					.ToList();
			}
		}

		private static string CellText(SpreadsheetDocument workbook, string reference)
		{
			var sheet = workbook.WorkbookPart.WorksheetParts
				.First(part => part.Worksheet.Descendants<Cell>().Any(cell => cell.InnerText == "Client Name"));

			return sheet.Worksheet.Descendants<Cell>().FirstOrDefault(cell => cell.CellReference == reference)?.InnerText;
		}
	}
}
