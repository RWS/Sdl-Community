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
		private static Assembly _xmlReportingAssembly;
		private static dynamic _reportDefinition;
		private static dynamic _excelRenderer;
		

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

		public static void GetExcelRenderer()
		{
			_xmlReportingAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation,Constants.XmlReportingDll));

			var reportingType = _xmlReportingAssembly.GetType("Sdl.ProjectApi.Reporting.XmlReporting.MhtReportRenderer");
			var reportingConstructor = reportingType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (reportingConstructor != null)
			{
				_excelRenderer = reportingConstructor.Invoke(new object[] { });

				var reportFormat = _excelRenderer.ReportFormats[0];
				var renderMethod = reportingType.GetMethod("RenderReport");
				var content =
					File.ReadAllText(
						@"C:\Users\aghisa\Documents\Studio 2017\Projects\BedAndBreakfast\Reports\Analyze Files en-GB_en-AU.xml");
				if (renderMethod != null)
				{
					dynamic report = renderMethod.Invoke(_excelRenderer,
						new[]
						{content
							,
							_reportDefinition, reportFormat
						});

					if (report != null)
					{
						using (Stream s = File.Create(@"C:\Users\aghisa\Desktop\andreaReport.mht"))
						{
							s.Write(report, 0, report.Length);
						}
					}
				}
			}
		}
		
	}
}
