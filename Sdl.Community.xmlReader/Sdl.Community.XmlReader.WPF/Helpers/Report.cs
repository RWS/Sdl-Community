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
		private static Assembly _automaticTasksAnalysisAssembly;
		private static dynamic _reportDefinition;

		public static dynamic GetReportDefinition()
		{
			_reportAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation,Constants.ProjectApiDll));
			_automaticTasksAnalysisAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation,Constants.AnalysidDll));

			var typeReport = _reportAssembly.GetType(
				"Sdl.ProjectApi.Reporting.ReportDefinition");

			var reportConstructor = typeReport.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (reportConstructor != null)
			{
				_reportDefinition = reportConstructor.Invoke(new object[] { });
			}
			if (_automaticTasksAnalysisAssembly != null)
			{
				var resourceNames = _automaticTasksAnalysisAssembly.GetManifestResourceNames();
				var xslResourceName = resourceNames.FirstOrDefault(resourceName => resourceName.EndsWith(".xsl"));
				if (xslResourceName != null)
				{
					using (var stream = _automaticTasksAnalysisAssembly.GetManifestResourceStream(xslResourceName))
					{
						_reportDefinition.Uri = _automaticTasksAnalysisAssembly.GetName().FullName;
						var len = (int)stream.Length;
						var data = new byte[len];
						stream.Read(data, 0, len);
						_reportDefinition.Data = data;
					}
				}
			}
			return _reportDefinition;
		}

		
	}
}
