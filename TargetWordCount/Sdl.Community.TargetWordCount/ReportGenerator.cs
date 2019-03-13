using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TargetWordCount
{
	public static class ReportGenerator
	{
		private const string Characters = "Characters";
		private const string Words = "Words";

		/// <summary>
		/// Get project controller
		/// </summary>
		/// <returns>information for the Project Controller</returns>
		private static ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		public static string Generate(List<ISegmentWordCounter> counters, IWordCountBatchTaskSettings settings)
		{
			var grandTotal = new CountTotal();
			var fileData = new List<CountTotal>();

			CollectFileData(counters, settings, grandTotal, fileData);

			grandTotal.CountMethod = fileData.First().CountMethod;
			grandTotal.FileName = "Total";

			return CreateReport(grandTotal, fileData, settings);
		}

		/// <summary>
		/// Generate new .xml reports( the reports will be imported manually in Helix).
		/// The already reports which are generated through TargetWordCount app are not compatible in Helix
		/// </summary>
		/// <param name="languageDirection"></param>
		public static void GenerateHelixReport(LanguageDirection languageDirection, List<ProjectFile> projectFiles)
		{
			var currentProject = GetProjectController().CurrentProject;
			var projectInfo = currentProject != null ?  currentProject.GetProjectInfo() : null;
			if (projectInfo != null)
			{
				var directoryFolder = $@"{projectInfo.LocalProjectFolder}\Reports\StudioTargetWordCount";
				if (!Directory.Exists(directoryFolder))
				{
					Directory.CreateDirectory(directoryFolder);
				}

				var directoryInfo = new DirectoryInfo($@"{projectInfo.LocalProjectFolder}\Reports\");
				var fileInfo = directoryInfo
					.GetFiles()
					.OrderByDescending(f => f.LastWriteTime)
					.FirstOrDefault(n => n.Name.StartsWith($@"Target Word Count {languageDirection.SourceLanguage.CultureInfo.Name}_{languageDirection.TargetLanguage.CultureInfo.Name}"));

				var helixReportPath = Path.Combine(directoryFolder, Path.GetFileName(fileInfo.FullName));				
				if (File.Exists(helixReportPath))
				{
					File.Delete(helixReportPath);
				}
				File.Create(helixReportPath).Dispose();

				// Create the new helix report xml report structure as the one from the Studio WordCount.xml report
				CreateReportDocument(projectInfo, languageDirection, helixReportPath, fileInfo, projectFiles);

			}
		}


		/// <summary>
		/// Create the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml structure based on the Studio WordCount.xml report structure
		/// The WordCount.xml structure is needed, because it is compatible with Helix import process,
		/// and the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml will be imported in Helix
		/// </summary>
		/// <param name="projectInfo">project information</param>
		/// <param name="helixReportPath">the new TargetWordCount xml report path which will be used in Helix</param>
		private static void CreateReportDocument(
			ProjectInfo projectInfo,
			LanguageDirection languageDirection,
			string helixReportPath,
			FileInfo fileInfo,
			List<ProjectFile> projectFiles)
		{
			var doc = new XmlDocument();

			var taskElement = doc.CreateElement(string.Empty, "task", string.Empty);
			taskElement.SetAttribute("name", "wordcount");
			doc.AppendChild(taskElement);

			var taskInfoElement = doc.CreateElement(string.Empty, "taskInfo", string.Empty);
			taskInfoElement.SetAttribute("taskId", Guid.NewGuid().ToString());
			taskInfoElement.SetAttribute("runAt", DateTime.UtcNow.ToString());
			taskInfoElement.SetAttribute("runTime", "Less than 1 second");
			taskElement.AppendChild(taskInfoElement);

			var projectElement = doc.CreateElement(string.Empty, "project", string.Empty);
			projectElement.SetAttribute("name", projectInfo.Name);
			projectElement.SetAttribute("number", projectInfo.Id.ToString());
			taskInfoElement.AppendChild(projectElement);

			var languageElement = doc.CreateElement(string.Empty, "language", string.Empty);
			languageElement.SetAttribute("lcid", languageDirection.TargetLanguage.CultureInfo.LCID.ToString());
			languageElement.SetAttribute("name", languageDirection.TargetLanguage.DisplayName);
			taskElement.AppendChild(languageElement);

			// take the files values from the Target Word Count {sourceLanguage_languageDirection.TargetLanguage}.xml report
			// and add the files nodes to the doc 
			projectFiles = projectFiles.Where(p => p.Language.DisplayName.Equals(languageDirection.TargetLanguage.DisplayName)).ToList();
			SetTargetFilesNode(doc, taskElement, languageElement, fileInfo, projectFiles);
			doc.Save(helixReportPath);
		}

		private static void SetTargetFilesNode(
			XmlDocument doc,
			XmlElement taskElement,
			XmlElement languageElement,
			FileInfo fileInfo,
			List<ProjectFile> projectFiles)
		{
			var document = new XmlDocument();
			document.Load(fileInfo.FullName);
			int totalSegments = 0;
			int totalWords = 0;
			int number;
			foreach(var projectFile in projectFiles)
			{
				var fileNode = document.SelectSingleNode($"//File[@Name='{projectFile.Name}']");
				var totalNodeAttributes = fileNode.SelectSingleNode("Total") != null ? fileNode.SelectSingleNode("Total").Attributes : null;
				if (totalNodeAttributes.Count > 0)
				{
					var fileElement = doc.CreateElement(string.Empty, "file", string.Empty);
					fileElement.SetAttribute("name", projectFile.Name);
					fileElement.SetAttribute("guid", projectFile.Id.ToString());
					languageElement.AppendChild(fileElement);

					SetTotalElements(doc, totalNodeAttributes["Segments"].Value, totalNodeAttributes["Count"].Value, fileElement);
					totalSegments += int.TryParse(totalNodeAttributes["Segments"].Value, out number) != false ? int.Parse(totalNodeAttributes["Segments"].Value) : 0;
					totalWords += int.TryParse(totalNodeAttributes["Count"].Value, out number) != false ? int.Parse(totalNodeAttributes["Count"].Value) : 0;
				}				
			}
			var batchTotalElement = doc.CreateElement(string.Empty, "batchTotal", string.Empty);
			taskElement.AppendChild(batchTotalElement);
			SetTotalElements(doc, totalSegments.ToString(), totalWords.ToString(), batchTotalElement);
		}

		private static void SetTotalElements(XmlDocument doc, string segmentValue, string wordCountValue, XmlElement parentElement)
		{
			var totalElement = doc.CreateElement(string.Empty, "total", string.Empty);
			totalElement.SetAttribute("segments", segmentValue);
			totalElement.SetAttribute("characters", string.Empty);
			totalElement.SetAttribute("placeables", string.Empty);
			totalElement.SetAttribute("tags", string.Empty);
			totalElement.SetAttribute("words", wordCountValue);
			parentElement.AppendChild(totalElement);
		}

		private static void AccumulateCountData(IWordCountBatchTaskSettings settings, ISegmentWordCounter counter, CountTotal info)
		{
			info.FileName = counter.FileName;

			SetCountMethod(settings, counter, info);

			foreach (var segInfo in counter.FileCountInfo.SegmentCounts)
			{
				var origin = segInfo.TranslationOrigin;

				if (origin == null)
				{
					info.Increment(CountTotal.New, segInfo.CountData);
				}
				else
				{
					if (settings.ReportLockedSeperately && segInfo.IsLocked)
					{
						info.Increment(CountTotal.Locked, segInfo.CountData);
					}
					else if (origin.OriginType == "document-match")
					{
						info.Increment(CountTotal.PerfectMatch, segInfo.CountData);
					}
					else if (origin.IsRepeated)
					{
						info.Increment(CountTotal.Repetitions, segInfo.CountData);
					}
					else if (origin.MatchPercent == 100)
					{
						if (origin.TextContextMatchLevel == Sdl.FileTypeSupport.Framework.NativeApi.TextContextMatchLevel.SourceAndTarget)
						{
							info.Increment(CountTotal.ContextMatch, segInfo.CountData);
						}
						else
						{
							info.Increment(CountTotal.OneHundredPercent, segInfo.CountData);
						}
					}
					else if (origin.MatchPercent >= 95)
					{
						info.Increment(CountTotal.NinetyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 85)
					{
						info.Increment(CountTotal.EightyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 75)
					{
						info.Increment(CountTotal.SeventyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 50)
					{
						info.Increment(CountTotal.FiftyPercent, segInfo.CountData);
					}
					else
					{
						info.Increment(CountTotal.New, segInfo.CountData);
					}
				}

				if (!(settings.ReportLockedSeperately && segInfo.IsLocked))
				{
					info.Increment(CountTotal.Total, segInfo.CountData);
				}

				if (segInfo.IsLocked)
				{
					info.LockedSpaceCountTotal += segInfo.SpaceCount;
				}
				else
				{
					info.UnlockedSpaceCountTotal += segInfo.SpaceCount;
				}
			}
		}

		private static void CollectFileData(List<ISegmentWordCounter> counters, IWordCountBatchTaskSettings settings, CountTotal grandTotal, List<CountTotal> fileData)
		{
			foreach (var counter in counters)
			{
				var info = new CountTotal();

				AccumulateCountData(settings, counter, info);

				fileData.Add(info);
				grandTotal.Increment(info);
				grandTotal.UnlockedSpaceCountTotal += info.UnlockedSpaceCountTotal;
				grandTotal.LockedSpaceCountTotal += info.LockedSpaceCountTotal;
			}
		}

		private static string CreateReport(CountTotal grandTotal, List<CountTotal> fileData, IWordCountBatchTaskSettings settings)
		{
			var builder = new ReportBuilder();

			// Build grand total table
			builder.BuildTotalTable(grandTotal, settings);

			// Build individual file tables
			foreach (var data in fileData)
			{
				builder.BuildFileTable(data, settings);
			}

			return builder.GetReport();
		}

		private static void SetCountMethod(IWordCountBatchTaskSettings settings, ISegmentWordCounter counter, CountTotal info)
		{
			Language language = null;

			if (settings.UseSource)
			{
				language = counter.FileCountInfo.SourceInfo;
			}
			else
			{
				language = counter.FileCountInfo.TargetInfo;
			}

			if (language.UsesCharacterCounts)
			{
				info.CountMethod = CountUnit.Character;
			}
			else
			{
				info.CountMethod = CountUnit.Word;
			}
		}
	}
}