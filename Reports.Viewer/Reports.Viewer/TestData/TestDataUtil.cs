using System.Collections.Generic;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.TestData
{
	public class TestDataUtil
	{
		public List<Report> GetTestReports()
		{
			var reports = new List<Report>();

			var report1 = new Report();
			report1.Group = "Convert Project";
			report1.Name = "Conversion report";
			report1.Description = "Convert Project to Transcreate";
			report1.Language = "de-DE";
			report1.Path = @"C:\Tests\Reports\Import_20200730141001_en-US_de-de.xml.html";

			var report2 = new Report();
			report2.Group = "Convert Project";
			report2.Name = "Some Conversion report";
			report2.Description = "Conver Project to Transcreate";
			report2.Language = "it-IT";
			report2.Path = @"C:\Tests\Reports\Import_20200730141001_en-US_it-IT.xml.html";

			var report3 = new Report();
			report3.Group = "Convert Project";
			report3.Name = "Italian conversion report";
			report3.Description = "Convert Project to Transcreate";
			report3.Language = "it-IT";
			report3.Path = @"C:\Tests\Reports\Import_20200730141001_en-US_it-IT.xml.html";

			var report4 = new Report();
			report4.Group = "Export";
			report4.Name = "Export files report";
			report4.Description = "Export report description";
			report4.Language = "it-IT";
			report4.Path = @"C:\Tests\Reports\Import_20200730141001_en-US_it-IT.xml.html";

			var report5 = new Report();
			report5.Group = "General Tasks";
			report5.Name = "Some custom task";
			report5.Description = "Some custom task description";
			report5.Language = "";
			report5.Path = @"C:\Tests\Reports\Import_20200730141001_en-US_it-IT.xml.html";

			reports.Add(report1);
			reports.Add(report2);
			reports.Add(report3);
			reports.Add(report4);
			reports.Add(report5);
			return reports;
		}
	}


}
