using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.TargetWordCount.Helpers;
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
		/// The app should support reports name like this: Target Word Count sv-SE_en-us.xml and Target Word Count sv-SE_en-US.xml
		/// </summary>
		/// <param name="languageDirection">language direction</param>
		/// <param name="projectFiles">project files for the language direction on which the batch task is running</param>
		public static void GenerateHelixReport(LanguageDirection languageDirection, List<ProjectFile> projectFiles)
		{
			var currentProject = GetProjectController()?.CurrentProject;
			var projectInfo = currentProject?.GetProjectInfo();
			if (projectInfo != null)
			{
				var directoryFolder = $@"{projectInfo.LocalProjectFolder}{Constants.ReportFolder}";
				if (!Directory.Exists(directoryFolder))
				{
					Directory.CreateDirectory(directoryFolder);
				}

				var directoryInfo = new DirectoryInfo($@"{projectInfo.LocalProjectFolder}{Constants.Reports}");

				var fileInfo = directoryInfo
					.GetFiles()
					.OrderByDescending(f => f.LastWriteTime)
					.FirstOrDefault(n => n.Name.ToLower().StartsWith($@"{Constants.TargetWordCount.ToLower()} {languageDirection.SourceLanguage.CultureInfo.Name.ToLower()}_{languageDirection.TargetLanguage.CultureInfo.Name.ToLower()}"));

				if (fileInfo != null)
				{
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
		}

		/// <summary>
		/// Create the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml structure based on the Studio WordCount.xml report structure
		/// The WordCount.xml structure is needed, because it is compatible with Helix import process,
		/// and the ..\Reports\StudioTargetWordCount\{targetWordCountReportName}.xml will be imported in Helix
		/// </summary>
		/// <param name="projectInfo">project information</param>
		/// <param name="languageDirection">file language direction</param>
		/// <param name="helixReportPath">the new TargetWordCount xml report path which will be used in Helix</param>
		/// <param name="fileInfo">file info</param>
		/// <param name="projectFiles">list of the project files on which the batch task is running</param>
		private static void CreateReportDocument(
			ProjectInfo projectInfo,
			LanguageDirection languageDirection,
			string helixReportPath,
			FileInfo fileInfo,
			List<ProjectFile> projectFiles)
		{
			var doc = new XmlDocument();
			var totalSegments = 0;
			var totalWords = 0;

			var taskElement = doc.CreateElement(string.Empty, Constants.Task, string.Empty);
			taskElement.SetAttribute(Constants.Name, Constants.WordCount);
			doc.AppendChild(taskElement);

			var taskInfoElement = doc.CreateElement(string.Empty, Constants.TaskInfo, string.Empty);
			taskInfoElement.SetAttribute(Constants.TaskId, Guid.NewGuid().ToString());
			taskInfoElement.SetAttribute(Constants.RunAt, DateTime.UtcNow.ToString());
			taskInfoElement.SetAttribute(Constants.RunTime, Constants.OneSecondLess);
			taskElement.AppendChild(taskInfoElement);

			var projectElement = doc.CreateElement(string.Empty, Constants.Project, string.Empty);
			projectElement.SetAttribute(Constants.Name, projectInfo.Name);
			projectElement.SetAttribute(Constants.Number, projectInfo.Id != null ? projectInfo.Id.ToString() : Guid.NewGuid().ToString());
			taskInfoElement.AppendChild(projectElement);

			var languageElement = doc.CreateElement(string.Empty, Constants.Language, string.Empty);
			languageElement.SetAttribute(Constants.Lcid, languageDirection.TargetLanguage.CultureInfo.LCID.ToString());
			languageElement.SetAttribute(Constants.Name, languageDirection.TargetLanguage.DisplayName);
			taskInfoElement.AppendChild(languageElement);

			// take the files values from the Target Word Count {sourceLanguage_languageDirection.TargetLanguage}.xml report
			// and add the files nodes to the doc for each projectFiles on which the TargetWordCount batch task had been run.
			var document = new XmlDocument();
			document.Load(fileInfo.FullName);

			foreach (var projectFile in projectFiles)
			{
				var fileNode = document.SelectSingleNode($"{Constants.FilePath}'{projectFile.Name}']");
				if (fileNode != null)
				{
					var totalNodeAttributes = fileNode.SelectSingleNode(Constants.Total) != null ? fileNode.SelectSingleNode(Constants.Total).Attributes : null;
					if (totalNodeAttributes?.Count > 0)
					{
						var fileElement = doc.CreateElement(string.Empty, Constants.File, string.Empty);
						fileElement.SetAttribute(Constants.Name, projectFile.Name);
						fileElement.SetAttribute(Constants.Guid, projectFile.Id.ToString());
						taskElement.AppendChild(fileElement);

						var analyseElement = doc.CreateElement(string.Empty, Constants.Analyse, string.Empty);
						fileElement.AppendChild(analyseElement);

						SetAnalyseElement(doc, Constants.Perfect, Constants.Zero, Constants.Zero, analyseElement);
						SetAnalyseElement(doc, Constants.InContextExact, Constants.Zero, Constants.Zero, analyseElement);
						SetAnalyseElement(doc, Constants.Repeated, Constants.Zero, Constants.Zero, analyseElement);
						SetAnalyseElement(doc, Constants.LowTotal, totalNodeAttributes[Constants.Segments].Value, totalNodeAttributes[Constants.Count].Value, analyseElement);
						SetAnalyseElement(doc, Constants.New, totalNodeAttributes[Constants.Segments].Value, totalNodeAttributes[Constants.Count].Value, analyseElement);
						totalSegments += int.TryParse(totalNodeAttributes[Constants.Segments].Value, out _) ? int.Parse(totalNodeAttributes[Constants.Segments].Value) : 0;
						totalWords += int.TryParse(totalNodeAttributes[Constants.Count].Value, out _) ? int.Parse(totalNodeAttributes[Constants.Count].Value) : 0;
					}
				}
			}
			var batchTotalElement = doc.CreateElement(string.Empty, Constants.BatchTotal, string.Empty);
			taskElement.AppendChild(batchTotalElement);
			var batchAnalyseElement = doc.CreateElement(string.Empty, Constants.Analyse, string.Empty);
			batchTotalElement.AppendChild(batchAnalyseElement);

			SetAnalyseElement(doc, Constants.LowTotal, totalSegments.ToString(), totalWords.ToString(), batchAnalyseElement);

			doc.Save(helixReportPath);
		}

		// Create the elements in report for the following xml elements: 'Total' and 'BatchTotal' 
		private static void SetAnalyseElement(XmlDocument doc, string elementName, string segmentsValue, string wordCountValue, XmlElement parentElement)
		{
			segmentsValue = !segmentsValue.Equals(Constants.Zero) ? segmentsValue : Constants.Zero;
			wordCountValue = !wordCountValue.Equals(Constants.Zero) ? wordCountValue : Constants.Zero;
			var totalElement = doc.CreateElement(string.Empty, elementName, string.Empty);
			totalElement.SetAttribute(Constants.LowSegments, segmentsValue);
			totalElement.SetAttribute(Constants.Characters, string.Empty);
			totalElement.SetAttribute(Constants.Placeables, string.Empty);
			totalElement.SetAttribute(Constants.Tags, string.Empty);
			totalElement.SetAttribute(Constants.Words, wordCountValue);
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
						if (origin.TextContextMatchLevel == FileTypeSupport.Framework.NativeApi.TextContextMatchLevel.SourceAndTarget)
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
			Language language;

			language = settings.UseSource ? counter.FileCountInfo.SourceInfo : counter.FileCountInfo.TargetInfo;

			info.CountMethod = language.UsesCharacterCounts ? CountUnit.Character : CountUnit.Word;
		}
	}
}