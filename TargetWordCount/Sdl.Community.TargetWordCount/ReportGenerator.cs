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
		public static void GenerateHelixReport(LanguageDirection languageDirection)
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
				CreateHelixReportFile(projectInfo, helixReportPath);

			}
		}


		/// <summary>
		/// Create the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml structure based on the Studio WordCount.xml report structure
		/// The WordCount.xml structure is needed, because it is compatible with Helix import process,
		/// and the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml will be imported in Helix
		/// </summary>
		/// <param name="projectInfo">project information</param>
		/// <param name="helixReportPath">the new TargetWordCount xml report path which will be used in Helix</param>
		private static void CreateHelixReportFile(ProjectInfo projectInfo, string helixReportPath)
		{
			var doc = new XmlDocument();
			doc.Load(helixReportPath);

			var taskElement = doc.CreateElement(string.Empty, "task", string.Empty);
			taskElement.SetAttribute("name", "wordcount");
			doc.AppendChild(taskElement);

			var taskInfoElement = doc.CreateElement(string.Empty, "taskInfo", string.Empty);
			taskInfoElement.SetAttribute("taskId", Guid.NewGuid().ToString());
			taskInfoElement.SetAttribute("runAt", DateTime.UtcNow.ToString());
			taskInfoElement.SetAttribute("tunTime", "Less than 1 second");
			taskElement.AppendChild(taskInfoElement);

			var projectElement = doc.CreateElement(string.Empty, "project", string.Empty);
			taskInfoElement.SetAttribute("name", projectInfo.Name);
			taskInfoElement.SetAttribute("number", projectInfo.Id.ToString());
			taskInfoElement.AppendChild(projectElement);

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