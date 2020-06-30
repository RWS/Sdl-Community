using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.ProjectAutomation.FileBased;
using AutomaticTask = Sdl.Community.XLIFF.Manager.Model.Tasks.AutomaticTask;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class ProjectSettingsService
	{
		private string _automaticTaskXml;

		public void UpdateAnalysisTaskReportInfo(FileBasedProject project, AutomaticTask automaticTask)
		{
			try
			{
				_automaticTaskXml = BuildAutomaticTask(automaticTask);

				string content;
				using (var reader = new StreamReader(project.FilePath, Encoding.UTF8))
				{
					content = reader.ReadToEnd();
					reader.Close();
				}

				var regexTasks = new Regex(@"(?<OpeningTag>\<Tasks\>)(?<Content>(.*|))(?<ClosingTag>\<\/Tasks\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				var regexTasksMatch = regexTasks.Match(content);
				if (regexTasksMatch.Success)
				{
					content = regexTasks.Replace(content, ReplaceReportInfo);
				}

				using (var writer = new StreamWriter(project.FilePath, false, Encoding.UTF8))
				{
					writer.Write(content);
					writer.Flush();
					writer.Close();
				}
			}
			catch
			{
				// ignore catch all
			}
		}

		public List<LanguageDirectionInfo> GetLanguageDirections(string filePath)
		{
			var languageDirections = new List<LanguageDirectionInfo>();

			if (string.IsNullOrEmpty(filePath))
			{
				return languageDirections;
			}

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "LanguageDirections", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						foreach (var languageDirection in element.Elements())
						{
							var languageDirectionInfo = new LanguageDirectionInfo
							{
								Guid = GetSettingsBundleAttributeValue(languageDirection, "Guid"),
								SettingsBundleGuid = GetSettingsBundleAttributeValue(languageDirection, "SettingsBundleGuid"),
								TargetLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "TargetLanguageCode"),
								SourceLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "SourceLanguageCode")
							};
							languageDirections.Add(languageDirectionInfo);
						}
					}
				}
			}

			return languageDirections;
		}

		private static string GetSettingsBundleAttributeValue(XElement settingsBundle, string attributeId)
		{
			var settingsBundleGuid = string.Empty;
			foreach (var attribute in settingsBundle.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, attributeId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					settingsBundleGuid = attribute.Value;
				}
			}

			return settingsBundleGuid;
		}

		public string ReplaceReportInfo(Match match)
		{
			var openingTag = match.Groups["OpeningTag"].Value;
			var content = match.Groups["Content"].Value;
			var closingTag = match.Groups["ClosingTag"].Value;			
			return openingTag + content + _automaticTaskXml + closingTag;
		}

		private static string BuildAutomaticTask(AutomaticTask automaticTask)
		{
			using (var ms = new MemoryStream())
			{
				using (var xml = new XmlTextWriter(ms, new UTF8Encoding(false)))
				{
					xml.Formatting = Formatting.Indented;

					xml.WriteStartElement("AutomaticTask");
					xml.WriteAttributeString("Guid", automaticTask.Guid);
					xml.WriteAttributeString("Status", automaticTask.Status);
					xml.WriteAttributeString("CompletedAt", automaticTask.CompletedAt);
					xml.WriteAttributeString("PercentComplete", automaticTask.PercentComplete);
					xml.WriteAttributeString("CreatedAt", automaticTask.CreatedAt);
					xml.WriteAttributeString("PredecessorTaskGuid", automaticTask.PredecessorTaskGuid);
					xml.WriteAttributeString("StartedAt", automaticTask.StartedAt);
					xml.WriteAttributeString("CreatedBy", automaticTask.CreatedBy);

					xml.WriteStartElement("TaskTemplateIds");
					foreach (var templateId in automaticTask.TemplateIds)
					{
						xml.WriteStartElement("TaskTemplateId");
						xml.WriteString(templateId.Value);
						xml.WriteEndElement(); //TaskTemplateId
					}
					xml.WriteEndElement(); //TaskTemplateIds


					xml.WriteStartElement("Files");
					foreach (var taskFile in automaticTask.TaskFiles)
					{
						xml.WriteStartElement("TaskFile");
						xml.WriteAttributeString("Guid", taskFile.Guid);
						xml.WriteAttributeString("Completed", taskFile.Completed);
						xml.WriteAttributeString("ParentTaskFileGuid", taskFile.ParentTaskFileGuid);
						xml.WriteAttributeString("LanguageFileGuid", taskFile.LanguageFileGuid);
						xml.WriteAttributeString("Purpose", taskFile.Purpose);
						xml.WriteEndElement(); //TaskFile

						xml.WriteStartElement("Result");
						xml.WriteAttributeString("Guid", "00000000-0000-0000-0000-000000000000");
						xml.WriteEndElement(); //Result
					}
					xml.WriteEndElement(); //Files


					xml.WriteStartElement("OutputFiles");
					foreach (var outputFile in automaticTask.OutputFiles)
					{
						xml.WriteStartElement("OutputFile");
						xml.WriteAttributeString("LanguageFileGuid", outputFile.LanguageFileGuid);
						xml.WriteAttributeString("Purpose", outputFile.Purpose);
						xml.WriteEndElement(); //OutputFile						
					}
					xml.WriteEndElement(); //OutputFiles

					xml.WriteStartElement("Reports");
					foreach (var report in automaticTask.Reports)
					{
						xml.WriteStartElement("Report");
						xml.WriteAttributeString("Guid", report.Guid);
						xml.WriteAttributeString("Name", report.Name);
						xml.WriteAttributeString("Description", report.Description);
						xml.WriteAttributeString("TaskTemplateId", report.TaskTemplateId);
						xml.WriteAttributeString("LanguageDirectionGuid", report.LanguageDirectionGuid);
						xml.WriteAttributeString("PhysicalPath", report.PhysicalPath);
						xml.WriteEndElement(); //Report						
					}
					xml.WriteEndElement(); //Reports


					xml.WriteStartElement("Result");
					xml.WriteEndElement(); //Result

					xml.WriteEndElement(); //AutomaticTask
				}

				var text = Encoding.UTF8.GetString(ms.ToArray());
				return text;
			}
		}

		//<AutomaticTask
		//	Guid = "0ca6dba8-22b1-4d5f-b1a0-0c533e7e241d" // new guid
		//	Status="Completed" 
		//	CompletedAt="2020-06-05T13:57:31.5180917Z" 
		//	ComplexTaskTemplateId="Sdl.ProjectApi.AutomaticTask.Prepare.NoProjectTM"  // not required!
		//	PercentComplete="100" 
		//	CreatedAt="2020-06-05T13:57:28.6705818Z" 
		//	PredecessorTaskGuid="df4325e5-625c-46cf-9231-3ac4322d4be7" 
		//	StartedAt="2020-06-05T13:57:29.9660832Z" 
		//	CreatedBy="GLOBAL\phartnett">
		//    <TaskTemplateIds>
		//    <TaskTemplateId>Sdl.ProjectApi.AutomaticTasks.PerfectMatch</TaskTemplateId>
		//    </TaskTemplateIds>
		//    <Files>
		//    <TaskFile
		//		Guid = "a0090b83-3199-4bb9-bc98-a4c96cf3cff7" // new guid
		//		Completed="true" 
		//		ParentTaskFileGuid="00000000-0000-0000-0000-000000000000" 
		//		LanguageFileGuid="0cedee4d-f4e2-46db-bd92-f729e50385da"  // project file id
		//		Purpose="WorkFile">
		//        <Result Guid = "00000000-0000-0000-0000-000000000000" />

		//	</ TaskFile >

		//	< TaskFile
		//		Guid="b30f77b2-0cb4-405d-bb40-66cb35d580cc" // new guid
		//		Completed="true" 
		//		ParentTaskFileGuid="00000000-0000-0000-0000-000000000000" 
		//		LanguageFileGuid="3a463fa5-1498-4c48-9b1c-0a16fd828e82"  // project file id
		//		Purpose="WorkFile">
		//        <Result Guid = "00000000-0000-0000-0000-000000000000" />

		//	</ TaskFile >

		//	< TaskFile
		//		Guid="e6730ba2-f739-4681-ab3f-34ff9eea2288" // new guid
		//		Completed="true" 
		//		ParentTaskFileGuid="00000000-0000-0000-0000-000000000000" 
		//		LanguageFileGuid="c61cc269-c569-4cc5-b692-43265e620053" // project file id
		//		Purpose="WorkFile">
		//        <Result Guid = "00000000-0000-0000-0000-000000000000" />

		//	</ TaskFile >

		//	</ Files >

		//	< OutputFiles >

		//	< OutputFile
		//		LanguageFileGuid="0cedee4d-f4e2-46db-bd92-f729e50385da"  // project file id
		//		Purpose="WorkFile" />
		//    <OutputFile
		//		LanguageFileGuid = "3a463fa5-1498-4c48-9b1c-0a16fd828e82"  // project file id
		//		Purpose="WorkFile" />
		//    <OutputFile
		//		LanguageFileGuid = "c61cc269-c569-4cc5-b692-43265e620053"  // project file id
		//		Purpose="WorkFile" />
		//    </OutputFiles>
		//    <Reports>     
		//    <Report
		//		Guid = "e8b7c3ca-8b82-4a1e-8e05-99eb68adebca"  // new guid
		//		Name="Analyze Files Report" 
		//		Description="Statistics showing the expected leverage from applying the translation memories." 
		//		TaskTemplateId="Sdl.ProjectApi.AutomaticTasks.Analysis" 
		//		LanguageDirectionGuid="08496e81-887b-4482-bc61-d729e3f35a55" // language direction guid
		//		PhysicalPath="Reports\Analyze Files en-US_de-de.xml" />
		//    </Reports>
		//    <Parameters />
		//    <Result
		//	Guid = "00000000-0000-0000-0000-000000000000" >

		//	< Messages
		//		Level="Warning" 
		//		Message="No translation memories found to look up translations for language pair English (United States)-&gt;German (Germany)." 
		//		Source="Pre-translate Files" />
		//    </Result>
		//</AutomaticTask>
	}
}
