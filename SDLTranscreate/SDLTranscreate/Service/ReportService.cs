using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using AnalysisBand = Sdl.Community.Transcreate.Model.AnalysisBand;
using ConfirmationStatistics = Sdl.Community.Transcreate.Model.ConfirmationStatistics;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;

namespace Sdl.Community.Transcreate.Service
{
	public class ReportService
	{
		public void CreateReport(TaskContext taskContext, string reportFile, 
			FileBasedProject selectedProject, string targetLanguageCode)
		{
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};

			var projectFiles = taskContext.ProjectFiles.Where(a => a.Selected &&
				string.Compare(a.TargetLanguage, targetLanguageCode, 
					StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

			var reportName = "";
			switch (taskContext.Action)
			{
				case Enumerators.Action.Convert:
					reportName = "Create Transcreate Project Report";
					break;
				case Enumerators.Action.CreateBackTranslation:
					reportName = "Create Back-Translation Project Report";
					break;
				case Enumerators.Action.Export:
					reportName = "Export Translations Report";
					break;
				case Enumerators.Action.Import:
					reportName = "Import Translations Report";
					break;
				case Enumerators.Action.ExportBackTranslation:
					reportName = "Export Back-Translations Report";
					break;
				case Enumerators.Action.ImportBackTranslation:
					reportName = "Import Back-Translations Report";
					break;
			}

			using (var writer = XmlWriter.Create(reportFile, settings))
			{
				writer.WriteStartElement("task");
				writer.WriteAttributeString("name", reportName);
				writer.WriteAttributeString("created", taskContext.DateTimeStampToString);

				WriteReportTaskInfo(writer, taskContext, selectedProject, targetLanguageCode);

				foreach (var projectFile in projectFiles)
				{
					WriteReportFile(writer, taskContext, projectFile);
				}

				WriteReportTotal(writer, taskContext, projectFiles);

				writer.WriteEndElement(); //task
			}
		}

		private void WriteReportTotal(XmlWriter writer, TaskContext taskContext, IReadOnlyCollection<ProjectFile> projectFiles)
		{
			writer.WriteStartElement("batchTotal");

			var totalTranslationOriginStatistics = GetTotalTranslationOriginStatistics(projectFiles, taskContext.Action);
			WriteAnalysisXml(writer, totalTranslationOriginStatistics?.WordCounts, taskContext.AnalysisBands, taskContext.Action);

			var totalConfirmationStatistics = GetTotalConfirmationStatistics(projectFiles, taskContext.Action);
			WriteConfirmationXml(writer, totalConfirmationStatistics?.WordCounts, taskContext.Action);

			writer.WriteEndElement(); //batchTotal
		}

		private void WriteReportFile(XmlWriter writer, TaskContext taskContext, ProjectFile projectFile)
		{
			writer.WriteStartElement("file");
			writer.WriteAttributeString("name", Path.Combine(projectFile.Path, projectFile.Name));
			writer.WriteAttributeString("guid", projectFile.FileId);

			WriteAnalysisXml(writer, projectFile.TranslationOriginStatistics?.WordCounts, taskContext.AnalysisBands, taskContext.Action);
			WriteConfirmationXml(writer, projectFile.ConfirmationStatistics?.WordCounts, taskContext.Action);

			writer.WriteEndElement(); //file
		}

		private void WriteReportTaskInfo(XmlWriter writer, TaskContext taskContext, IProject fileBasedProject, string languageCode)
		{
			writer.WriteStartElement("taskInfo");
			writer.WriteAttributeString("action", taskContext.Action.ToString());
			writer.WriteAttributeString("workflow", taskContext.WorkFlow.ToString());
			writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
			writer.WriteAttributeString("runAt", taskContext.DateTimeStamp.ToShortDateString() + " " + taskContext.DateTimeStamp.ToShortTimeString());

			WriteReportProject(writer, taskContext);

			WriteReportLanguage(writer, languageCode);

			WriteReportCustomer(writer, taskContext);

			WriteReportTranslationProviders(writer, fileBasedProject);

			WriteReportSettings(writer, taskContext);

			writer.WriteEndElement(); //taskInfo
		}

		private static void WriteReportTranslationProviders(XmlWriter writer, IProject fileBasedProject)
		{
			//var cultureInfo = new CultureInfo(languageDirection.Key.TargetLanguageCode);
			//var language = new Language(cultureInfo);
			var config = fileBasedProject.GetTranslationProviderConfiguration();
			foreach (var cascadeEntry in config.Entries)
			{
				if (!cascadeEntry.MainTranslationProvider.Enabled)
				{
					continue;
				}

				var scheme = cascadeEntry.MainTranslationProvider.Uri.Scheme;
				var segments = cascadeEntry.MainTranslationProvider.Uri.Segments;
				var name = scheme + "://";
				if (segments.Length >= 0)
				{
					name += segments[segments.Length - 1];
				}

				writer.WriteStartElement("tm");
				writer.WriteAttributeString("name", name);
				writer.WriteEndElement(); //tm
			}
		}

		private static void WriteReportLanguage(XmlWriter writer, string languageCode)
		{
			writer.WriteStartElement("language");
			writer.WriteAttributeString("id", languageCode);
			writer.WriteAttributeString("name", new CultureInfo(languageCode).DisplayName);
			writer.WriteEndElement(); //language
		}

		private static void WriteReportCustomer(XmlWriter writer, TaskContext taskContext)
		{
			if (!string.IsNullOrEmpty(taskContext.Project.Customer?.Name))
			{
				writer.WriteStartElement("customer");
				writer.WriteAttributeString("name", taskContext.Project.Customer.Name);
				writer.WriteAttributeString("email", taskContext.Project.Customer.Email);
				writer.WriteEndElement(); //customer												  
			}
		}

		private static void WriteReportProject(XmlWriter writer, TaskContext taskContext)
		{
			writer.WriteStartElement("project");
			writer.WriteAttributeString("name", taskContext.Project.Name);
			writer.WriteAttributeString("number", taskContext.Project.Id);
			if (taskContext.Project.DueDate != DateTime.MinValue && taskContext.Project.DueDate != DateTime.MaxValue)
			{
				writer.WriteAttributeString("dueDate",
					taskContext.Project.DueDate.ToShortDateString() + " " + taskContext.Project.DueDate.ToShortTimeString());
			}

			writer.WriteEndElement(); //project
		}

		private void WriteReportSettings(XmlWriter writer, TaskContext taskContext)
		{
			writer.WriteStartElement("settings");
			if (taskContext.Action == Enumerators.Action.Export || taskContext.Action == Enumerators.Action.ExportBackTranslation)
			{
				writer.WriteAttributeString("xliffSupport", taskContext.ExportOptions.XliffSupport.ToString());
				writer.WriteAttributeString("includeTranslations", taskContext.ExportOptions.IncludeTranslations.ToString());
				writer.WriteAttributeString("copySourceToTarget", taskContext.ExportOptions.CopySourceToTarget.ToString());
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(taskContext.ExportOptions.ExcludeFilterIds));
			}
			else if (taskContext.Action == Enumerators.Action.Import || taskContext.Action == Enumerators.Action.ImportBackTranslation)
			{
				writer.WriteAttributeString("overwriteTranslations", taskContext.ImportOptions.OverwriteTranslations.ToString());
				writer.WriteAttributeString("originSystem", taskContext.ImportOptions.OriginSystem);
				writer.WriteAttributeString("statusTranslationUpdatedId", GetSegmentStatus(taskContext.ImportOptions.StatusTranslationUpdatedId));
				writer.WriteAttributeString("statusTranslationNotUpdatedId", GetSegmentStatus(taskContext.ImportOptions.StatusTranslationNotUpdatedId));
				writer.WriteAttributeString("statusSegmentNotImportedId", GetSegmentStatus(taskContext.ImportOptions.StatusSegmentNotImportedId));
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(taskContext.ImportOptions.ExcludeFilterIds));
			}
			else if (taskContext.Action == Enumerators.Action.Convert)
			{
				writer.WriteAttributeString("maxAlternativeTranslations", taskContext.ConvertOptions.MaxAlternativeTranslations.ToString());
				writer.WriteAttributeString("closeProjectOnComplete", taskContext.ConvertOptions.CloseProjectOnComplete.ToString());
			}

			writer.WriteEndElement(); //settings
		}


		private string GetSegmentStatus(string id)
		{
			var confirmationStatuses = Enumerators.GetConfirmationStatuses();
			var status = confirmationStatuses.FirstOrDefault(a => a.Id == id);
			return status != null ? status.Name : "Don't Change";
		}

		private void WriteConfirmationXml(XmlWriter writer, WordCounts wordCounts, Enumerators.Action action)
		{
			writer.WriteStartElement("confirmation");

			WriteConfirmationWordCountStatistics(writer, wordCounts?.Processed, "processed");
			WriteConfirmationWordCountStatistics(writer, wordCounts?.Excluded, "excluded");
			if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
			{
				WriteConfirmationWordCountStatistics(writer, wordCounts?.NotProcessed, "notProcessed");
			}
			WriteConfirmationWordCountStatistics(writer, wordCounts?.Total, "total");

			writer.WriteEndElement(); //confirmation
		}

		private void WriteAnalysisXml(XmlWriter writer, WordCounts wordCounts, IReadOnlyCollection<AnalysisBand> analysisBands, Enumerators.Action action)
		{
			writer.WriteStartElement("analysis");

			WriteAnalysisWordCountStatistics(writer, wordCounts?.Processed, analysisBands, "processed");
			WriteAnalysisWordCountStatistics(writer, wordCounts?.Excluded, analysisBands, "excluded");
			if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
			{
				WriteAnalysisWordCountStatistics(writer, wordCounts?.NotProcessed, analysisBands, "notProcessed");
			}
			WriteAnalysisWordCountStatistics(writer, wordCounts?.Total, analysisBands, "total");

			writer.WriteEndElement(); //analysis
		}

		private TranslationOriginStatistics GetTotalTranslationOriginStatistics(IEnumerable<ProjectFile> projectFiles, Enumerators.Action action)
		{
			var statistics = new TranslationOriginStatistics();

			foreach (var projectFile in projectFiles)
			{
				if (projectFile.TranslationOriginStatistics?.WordCounts != null)
				{
					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Processed)
					{
						var totalWordCount = statistics.WordCounts.Processed.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Processed.Add(wordCount);
						}
					}

					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Excluded)
					{
						var totalWordCount = statistics.WordCounts.Excluded.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Excluded.Add(wordCount);
						}
					}

					if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
					{
						foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.NotProcessed)
						{
							var totalWordCount = statistics.WordCounts.NotProcessed.FirstOrDefault(a =>
								a.Category == wordCount.Category);
							if (totalWordCount != null)
							{
								UpdateTotalWordCount(wordCount, totalWordCount);
							}
							else
							{
								statistics.WordCounts.NotProcessed.Add(wordCount);
							}
						}
					}

					foreach (var wordCount in projectFile.TranslationOriginStatistics?.WordCounts?.Total)
					{
						var totalWordCount = statistics.WordCounts.Total.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Total.Add(wordCount);
						}
					}
				}
			}

			return statistics;
		}

		private ConfirmationStatistics GetTotalConfirmationStatistics(IEnumerable<ProjectFile> projectFiles, Enumerators.Action action)
		{
			var statistics = new ConfirmationStatistics();

			foreach (var projectFile in projectFiles)
			{
				if (projectFile.ConfirmationStatistics?.WordCounts != null)
				{
					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Processed)
					{
						var totalWordCount = statistics.WordCounts.Processed.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Processed.Add(wordCount);
						}
					}

					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Excluded)
					{
						var totalWordCount = statistics.WordCounts.Excluded.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Excluded.Add(wordCount);
						}
					}

					if (action == Enumerators.Action.Import || action == Enumerators.Action.ImportBackTranslation)
					{
						foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.NotProcessed)
						{
							var totalWordCount = statistics.WordCounts.NotProcessed.FirstOrDefault(a =>
								a.Category == wordCount.Category);
							if (totalWordCount != null)
							{
								UpdateTotalWordCount(wordCount, totalWordCount);
							}
							else
							{
								statistics.WordCounts.NotProcessed.Add(wordCount);
							}
						}
					}

					foreach (var wordCount in projectFile.ConfirmationStatistics?.WordCounts?.Total)
					{
						var totalWordCount = statistics.WordCounts.Total.FirstOrDefault(a =>
							a.Category == wordCount.Category);
						if (totalWordCount != null)
						{
							UpdateTotalWordCount(wordCount, totalWordCount);
						}
						else
						{
							statistics.WordCounts.Total.Add(wordCount);
						}
					}
				}
			}

			return statistics;
		}

		private string GetFitlerItemsString(IEnumerable<string> ids)
		{
			var allFilterItems = Enumerators.GetFilterItems();
			var filterItems = Enumerators.GetFilterItems(allFilterItems, ids);
			var items = string.Empty;
			foreach (var filterItem in filterItems)
			{
				items += (string.IsNullOrEmpty(items) ? string.Empty : ", ") +
				         filterItem.Name;
			}

			if (string.IsNullOrEmpty(items))
			{
				items = "[none]";
			}

			return items;
		}

		private void WriteAnalysisWordCountStatistics(XmlWriter writer,
		IReadOnlyCollection<WordCount> wordCounts, IEnumerable<AnalysisBand> analysisBands, string name)
		{
			writer.WriteStartElement(name);

			var totalWordCount = new WordCount
			{
				Category = "Total"
			};

			var perfectMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.PM.ToString()) ?? new WordCount { Category = Enumerators.MatchType.PM.ToString() };
			WriteWordCount(writer, perfectMatch, "perfect");
			UpdateTotalWordCount(perfectMatch, totalWordCount);

			var contextMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.CM.ToString()) ?? new WordCount { Category = Enumerators.MatchType.CM.ToString() };
			WriteWordCount(writer, contextMatch, "context");
			UpdateTotalWordCount(contextMatch, totalWordCount);

			var repetitionMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.Repetition.ToString()) ?? new WordCount { Category = Enumerators.MatchType.Repetition.ToString() };
			WriteWordCount(writer, repetitionMatch, "repetition");
			UpdateTotalWordCount(repetitionMatch, totalWordCount);

			var exactMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == Enumerators.MatchType.Exact.ToString()) ?? new WordCount { Category = Enumerators.MatchType.Exact.ToString() };
			WriteWordCount(writer, exactMatch, "exact");
			UpdateTotalWordCount(exactMatch, totalWordCount);

			foreach (var analysisBand in analysisBands)
			{
				var fuzzyMatchKey = string.Format("{0} {1} - {2}", Enumerators.MatchType.Fuzzy.ToString(),
					analysisBand.MinimumMatchValue + "%", analysisBand.MaximumMatchValue + "%");

				var fuzzyMatch = wordCounts?.FirstOrDefault(a =>
					a.Category == fuzzyMatchKey) ?? new WordCount { Category = fuzzyMatchKey };

				WriteWordCount(writer, fuzzyMatch, "fuzzy",
					analysisBand.MinimumMatchValue, analysisBand.MaximumMatchValue);

				UpdateTotalWordCount(fuzzyMatch, totalWordCount);
			}

			var newMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.New.ToString()) ?? new WordCount { Category = Enumerators.MatchType.New.ToString() };
			WriteWordCount(writer, newMatch, "new");
			UpdateTotalWordCount(newMatch, totalWordCount);

			var nmtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.NMT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.NMT.ToString() };
			WriteWordCount(writer, nmtMatch, "nmt");
			UpdateTotalWordCount(nmtMatch, totalWordCount);

			var amtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.AMT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.AMT.ToString() };
			WriteWordCount(writer, amtMatch, "amt");
			UpdateTotalWordCount(amtMatch, totalWordCount);

			var mtMatch = wordCounts?.FirstOrDefault(a =>
				 a.Category == Enumerators.MatchType.MT.ToString()) ?? new WordCount { Category = Enumerators.MatchType.MT.ToString() };
			WriteWordCount(writer, mtMatch, "mt");
			UpdateTotalWordCount(mtMatch, totalWordCount);

			WriteWordCount(writer, totalWordCount, "total");

			writer.WriteEndElement();
		}

		private void WriteConfirmationWordCountStatistics(XmlWriter writer, IReadOnlyCollection<WordCount> wordCounts, string name)
		{
			writer.WriteStartElement(name);

			var totalWordCount = new WordCount
			{
				Category = "Total"
			};

			var approvedSignOff = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.ApprovedSignOff.ToString()) ?? new WordCount { Category = ConfirmationLevel.ApprovedSignOff.ToString() };
			WriteWordCount(writer, approvedSignOff, "approvedSignOff");
			UpdateTotalWordCount(approvedSignOff, totalWordCount);

			var rejectedSignOff = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.RejectedSignOff.ToString()) ?? new WordCount { Category = ConfirmationLevel.RejectedSignOff.ToString() };
			WriteWordCount(writer, rejectedSignOff, "rejectedSignOff");
			UpdateTotalWordCount(rejectedSignOff, totalWordCount);

			var approvedTranslation = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.ApprovedTranslation.ToString()) ?? new WordCount { Category = ConfirmationLevel.ApprovedTranslation.ToString() };
			WriteWordCount(writer, approvedTranslation, "approvedTranslation");
			UpdateTotalWordCount(approvedTranslation, totalWordCount);

			var rejectedTranslation = wordCounts?.FirstOrDefault(a =>
					a.Category == ConfirmationLevel.RejectedTranslation.ToString()) ?? new WordCount { Category = ConfirmationLevel.RejectedTranslation.ToString() };
			WriteWordCount(writer, rejectedTranslation, "rejectedTranslation");
			UpdateTotalWordCount(rejectedTranslation, totalWordCount);

			var translated = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Translated.ToString()) ?? new WordCount { Category = ConfirmationLevel.Translated.ToString() };
			WriteWordCount(writer, translated, "translated");
			UpdateTotalWordCount(translated, totalWordCount);

			var draft = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Draft.ToString()) ?? new WordCount { Category = ConfirmationLevel.Draft.ToString() };
			WriteWordCount(writer, draft, "draft");
			UpdateTotalWordCount(draft, totalWordCount);

			var unspecified = wordCounts?.FirstOrDefault(a =>
				 a.Category == ConfirmationLevel.Unspecified.ToString()) ?? new WordCount { Category = ConfirmationLevel.Unspecified.ToString() };
			WriteWordCount(writer, unspecified, "unspecified");
			UpdateTotalWordCount(unspecified, totalWordCount);

			WriteWordCount(writer, totalWordCount, "total");

			writer.WriteEndElement();
		}

		private static void WriteWordCount(XmlWriter writer, WordCount wordCount, string name)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("segments", wordCount.Segments.ToString());
			writer.WriteAttributeString("words", wordCount.Words.ToString());
			writer.WriteAttributeString("characters", wordCount.Characters.ToString());
			writer.WriteAttributeString("placeables", wordCount.Placeables.ToString());
			writer.WriteAttributeString("tags", wordCount.Tags.ToString());
			writer.WriteEndElement(); //name			
		}

		private void UpdateTotalWordCount(WordCount wordCount, WordCount totalWordCount)
		{
			totalWordCount.Segments += wordCount.Segments;
			totalWordCount.Words += wordCount.Words;
			totalWordCount.Characters += wordCount.Characters;
			totalWordCount.Placeables += wordCount.Placeables;
			totalWordCount.Tags += wordCount.Tags;
		}

		private static void WriteWordCount(XmlWriter writer, WordCount wordCount, string name, int min, int max)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("min", min.ToString());
			writer.WriteAttributeString("max", max.ToString());
			writer.WriteAttributeString("segments", wordCount.Segments.ToString());
			writer.WriteAttributeString("words", wordCount.Words.ToString());
			writer.WriteAttributeString("characters", wordCount.Characters.ToString());
			writer.WriteAttributeString("placeables", wordCount.Placeables.ToString());
			writer.WriteAttributeString("tags", wordCount.Tags.ToString());
			writer.WriteEndElement(); //name
		}
	}
}
