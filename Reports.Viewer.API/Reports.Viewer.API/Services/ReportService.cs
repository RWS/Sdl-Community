using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Sdl.Core.PluginFramework;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Reports.Viewer.API.Services
{
	public class ReportService
	{
		private readonly List<ReportDefinition> _thirdPartyReportDefinitions;
		private readonly ProjectSettingsService _projectSettingsService;

		public ReportService(ProjectSettingsService projectSettingsService)
		{
			_projectSettingsService = projectSettingsService;
			_thirdPartyReportDefinitions = GetThirdPartyReportDefinitions();
		}

		public IEnumerable<Report> GetStudioReports(IProject project, bool overwrite)
		{
			var reportDefinitions = new List<ReportDefinition>();

			var reports = new List<Report>();
			if (project is FileBasedProject fileBasedProject)
			{
				var studioReports = _projectSettingsService.GetProjectTaskReports(fileBasedProject.FilePath);
				foreach (var studioReport in studioReports)
				{
					if (studioReport == null)
					{
						continue;
					}

					var reportDefinition =
						reportDefinitions.FirstOrDefault(a => string.Compare(a.Id, studioReport.TemplateId, StringComparison.CurrentCultureIgnoreCase) == 0) ??
						_thirdPartyReportDefinitions.FirstOrDefault(a => string.Compare(a.Id, studioReport.TemplateId, StringComparison.CurrentCultureIgnoreCase) == 0) ??
						GetReportDefinition(studioReport.TemplateId);

					if (reportDefinition == null)
					{
						continue;
					}

					if (!reportDefinitions.Exists(a => a.Id == reportDefinition.Id))
					{
						reportDefinitions.Add(reportDefinition);
					}

					var projectLocalFolder = fileBasedProject.GetProjectInfo().LocalProjectFolder.Trim('\\');
					var xmlPath = Path.Combine(projectLocalFolder, studioReport.Path);
					if (!File.Exists(xmlPath) || !studioReport.IsStudioReport)
					{
						continue;
					}

					var reportPath = CreateHtmlReport(reportDefinition, xmlPath, overwrite);
					if (reportPath != null)
					{
						studioReport.Path = reportPath;
					}

					reports.Add(studioReport);
				}
			}

			return reports;
		}

		private string CreateHtmlReport(ReportDefinition reportDefinition, string xmlPath, bool replace)
		{
			var reportFilePath = xmlPath + ".html";
			if (File.Exists(reportFilePath))
			{
				if (replace)
				{
					File.Delete(reportFilePath);
				}
				else
				{
					return reportFilePath;
				}
			}

			string xmlText;
			using (var r = new StreamReader(xmlPath, Encoding.UTF8))
			{
				xmlText = r.ReadToEnd();
				r.Close();
			}

			var bytes = Transform(xmlText, reportDefinition, false);
			using (Stream s = File.Create(reportFilePath))
			{
				s.Write(bytes, 0, bytes.Length);
				s.Flush();
				s.Close();
			}

			return reportFilePath;
		}

		private ReportDefinition GetReportDefinition(string templateId)
		{
			try
			{
				var reportDefinition = new ReportDefinition
				{
					Id = templateId,
					Assembly = Assembly.Load(templateId)
				};

				if (reportDefinition.Assembly == null)
				{
					return null;
				}

				reportDefinition.Data = GetXslData(reportDefinition.Assembly);
				return reportDefinition;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private List<ReportDefinition> GetThirdPartyReportDefinitions()
		{
			var reportDefinitions = new List<ReportDefinition>();

			var automationExtensionPoint =
				PluginManager.DefaultPluginRegistry.GetExtensionPoint<ProjectAutomation.AutomaticTasks.AutomaticTaskAttribute>();

			foreach (var extension in automationExtensionPoint.Extensions)
			{
				var id = extension?.ExtensionAttribute?.Id;

				if (id == null)
				{
					continue;
				}

				if (!reportDefinitions.Exists(a => a.Id == id))
				{
					var reportDefinition = new ReportDefinition
					{
						Id = id,
						Assembly = extension.ExtensionType.Assembly,
						Data = GetXslData(extension.ExtensionType.Assembly)
					};
					reportDefinitions.Add(reportDefinition);

					var extensionTypeId = extension.ExtensionType.FullName;
					if (extensionTypeId != id)
					{
						reportDefinitions.Add(new ReportDefinition
						{
							Id = extensionTypeId,
							Assembly = reportDefinition.Assembly,
							Data = reportDefinition.Data
						});
					}
				}
			}

			return reportDefinitions;
		}

		private static byte[] Transform(string xml, ReportDefinition reportDefinition, bool useExternalResources)
		{
			var xsl = Encoding.UTF8.GetString(reportDefinition.Data);

			var resourceAssembly = reportDefinition.Assembly;
			if (resourceAssembly == null)
			{
				return null;
			}

			var transform = new XslCompiledTransform(true);
			var xslDoc = new XmlDocument();

			if (string.IsNullOrEmpty(xsl))
			{
				throw new ApplicationException("Error loading stylesheet: stylesheet is empty.");
			}

			try
			{
				xslDoc.LoadXml(xsl);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error loading stylesheet.", ex);
			}

			var nsm = new XmlNamespaceManager(xslDoc.NameTable);
			nsm.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

			transform.Load(xslDoc, new XsltSettings(true, true), new XmlUrlResolver());

			var sw = new StreamWriter(new MemoryStream(), Encoding.UTF8);

			var args = new XsltArgumentList();

			args.AddParam("UseExternalResources", "", useExternalResources);

			// add default extension object
			args.AddExtensionObject("urn:XmlReporting", new DefaultXslExtension(resourceAssembly));


			// add extra extension objects
			//Hashtable extensions = XmlReportRendererConfiguration.Instance.XslExtensionObjects;
			//foreach (string key in extensions.Keys)
			//{
			//	args.AddExtensionObject(key, extensions[key]);
			//}

			transform.Transform(new XmlTextReader(new StringReader(xml)), args, sw);

			sw.BaseStream.Seek(0, SeekOrigin.Begin);
			var output = new byte[sw.BaseStream.Length];
			var bytesRead = sw.BaseStream.Read(output, 0, output.Length);
			return output;
		}

		private byte[] GetXslData(Assembly automaticTaskAssembly)
		{
			if (automaticTaskAssembly == null)
			{
				return null;
			}

			var resourceNames = automaticTaskAssembly.GetManifestResourceNames();
			var xslResourceName = resourceNames.FirstOrDefault(resourceName => resourceName.ToLower().EndsWith(".xsl"));

			if (xslResourceName != null)
			{
				using (var stream = automaticTaskAssembly.GetManifestResourceStream(xslResourceName))
				{
					if (stream == null)
					{
						return null;
					}

					var len = (int)stream.Length;
					var data = new byte[len];
					stream.Read(data, 0, len);
					return data;
				}
			}

			return null;
		}		
	}
}
