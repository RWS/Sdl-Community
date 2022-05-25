﻿using System;
using System.Reflection;

namespace Sdl.Community.XmlReader.WPF.Helpers
{
	public static class Constants
	{
		public static  string StudioLocation => ExecutingStudioLocation();
		public static string ProjectApiDll = "Sdl.ProjectApi.Reporting.dll";
		public static string AnalysidDll = "Sdl.ProjectApi.AutomaticTasks.dll";
		public static string  XmlReportingDll="Sdl.ProjectApi.Reporting.XmlReporting.dll";
		public static string ExcelReportRendererType = "Sdl.ProjectApi.Reporting.XmlReporting.ExcelReportRenderer";
		public  static string ReportDefinitionType= "Sdl.ProjectApi.Reporting.ReportDefinition";
		public static string RenderReportMethod = "RenderReport";

		private static string ExecutingStudioLocation()
		{
			var entryAssembly = Assembly.GetEntryAssembly().Location;
			var location = entryAssembly.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

			return location;
		}
	}
}
