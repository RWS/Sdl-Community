using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XmlReader.WPF.ViewModels;


namespace Sdl.Community.XmlReader.WPF.Helpers
{
	public static class Report
	{
		private static Assembly _reportAssembly;
		private static Assembly _automaticTasksAnalysisAssembly;
		private static Assembly _xmlReportingAssembly;
		private static dynamic _reportDefinition;
		private static dynamic _excelRenderer;


		private static void GetReportDefinition()
		{
			_reportAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation, Constants.ProjectApiDll));
			_automaticTasksAnalysisAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation, Constants.AnalysidDll));

			var typeReport = _reportAssembly.GetType(Constants.ReportDefinitionType);

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
						var len = (int) stream.Length;
						var data = new byte[len];
						stream.Read(data, 0, len);
						_reportDefinition.Data = data;
					}
				}
			}
		}

		public static void GenerateExcelReport(string folderPath,List<TargetLanguageCodeViewModel> selectedItems)
		{
			GetReportDefinition();

			_xmlReportingAssembly = Assembly.LoadFrom(Path.Combine(Constants.StudioLocation, Constants.XmlReportingDll));

			var reportingType = _xmlReportingAssembly.GetType(Constants.ExcelReportRendererType);
			var reportingConstructor = reportingType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, Type.EmptyTypes, null);

			if (reportingConstructor != null)
			{
				_excelRenderer = reportingConstructor.Invoke(new object[] { });

				var reportFormat = _excelRenderer.ReportFormats[0];
				var renderMethod = reportingType.GetMethod(Constants.RenderReportMethod);
				foreach (var item in selectedItems)
				{
					foreach (dynamic report in item.Children)
					{
						
						var content = File.ReadAllText(report.AnalyzeFilePath);
							
						if (renderMethod != null)
						{
							var reportRawData = renderMethod.Invoke(_excelRenderer,
								new[]
								{
									content, _reportDefinition, reportFormat
								});

							if (reportRawData != null)
							{
								var reportName = string.Concat("generatedReport_", report.AnalyzeFileName, ".xlsx");
								using (Stream s = File.Create(Path.Combine(folderPath, reportName)))
								{
									s.Write(reportRawData, 0, reportRawData.Length);
								}
							}
						}
					}
				}
		
			}
		}

	}
}
