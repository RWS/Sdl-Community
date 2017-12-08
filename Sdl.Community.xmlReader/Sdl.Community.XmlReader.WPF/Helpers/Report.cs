using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;



namespace Sdl.Community.XmlReader.WPF.Helpers
{
	public  static class Report
	{
		private  static Assembly _reportAssembly;
		public static void GetReportDefinition()
		{
			var reportAssembly = Assembly.GetExecutingAssembly()
				.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith("Sdl.Community.XmlReader.WPF._3rd_party.Sdl.ProjectApi.Reporting.dll"));
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(reportAssembly))
			{
				if (stream.CanSeek)
				{
					stream.Seek(0, SeekOrigin.Begin);
				}

				var block = new byte[stream.Length];
				try
				{
					stream.Read(block, 0, block.Length);

					_reportAssembly = Assembly.Load(block);
				}
				catch (Exception e)
				{
				}
			}
			Type typeReport = _reportAssembly.GetType(
				"Sdl.ProjectApi.Reporting.ReportDefinition");

		
			var reportConstructor = typeReport.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (reportConstructor != null)
			{
				dynamic reportInstance = reportConstructor.Invoke(new object[] { });
			}
	
		}
	}
}
