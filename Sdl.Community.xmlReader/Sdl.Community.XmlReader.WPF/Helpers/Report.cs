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
			_reportAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation,Constants.ProjectApiDll));

			var typeReport = _reportAssembly.GetType(
				"Sdl.ProjectApi.Reporting.ReportDefinition");


			var reportConstructor = typeReport.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (reportConstructor != null)
			{
				var reportInstance = reportConstructor.Invoke(new object[] { });
			}
	
		}
	}
}
