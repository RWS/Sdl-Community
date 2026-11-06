using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.Qualitivity.Dialogs.Export;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Configuration;
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
			CreateReport(906, 406);

			using (var workbook = SpreadsheetDocument.Open(_file, false))
			{
				Assert.Equal("Elapsed Seconds", CellText(workbook, "P1"));
				Assert.Equal("131", CellText(workbook, "P2"));
			}
		}

		[Fact]
		public void CreateExcelReport_OneActivity_FilterCoversAllColumnsAndRows()
		{
			CreateReport(10);

			using (var workbook = SpreadsheetDocument.Open(_file, false))
			{
				var filters = workbook.WorkbookPart.WorksheetParts
					.SelectMany(part => part.Worksheet.Descendants<AutoFilter>())
					.Select(filter => filter.Reference.Value);

				Assert.Contains("A1:P2", filters);
			}
		}

		public void Dispose() => File.Delete(_file);

		private void CreateReport(params long[] documentTenthsOfSeconds)
		{
			Tracked.TrackingProjects = new TrackingProjects
			{
				TrackerProjects = new List<Project> { new Project { Id = 1, Name = "Project" } }
			};

			var activity = new Activity
			{
				Id = 1,
				ProjectId = 1,
				Activities = documentTenthsOfSeconds
					.Select(tenths => new DocumentActivities { DocumentActivityTicks = tenths * TicksPerTenthOfSecond })
					.ToList()
			};

			new ExportToFile().create_excel_report(_file, new List<Activity> { activity }, new List<XmlDocumentActivity>());
		}

		private static string CellText(SpreadsheetDocument workbook, string reference)
		{
			var sheet = workbook.WorkbookPart.WorksheetParts
				.First(part => part.Worksheet.Descendants<Cell>().Any(cell => cell.InnerText == "Client Name"));

			return sheet.Worksheet.Descendants<Cell>().FirstOrDefault(cell => cell.CellReference == reference)?.InnerText;
		}
	}
}
