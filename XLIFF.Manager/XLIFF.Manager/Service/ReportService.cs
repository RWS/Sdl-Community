using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using AnalysisBand = Sdl.Community.XLIFF.Manager.Model.AnalysisBand;
using ConfirmationStatistics = Sdl.Community.XLIFF.Manager.Model.ConfirmationStatistics;
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class ReportService
	{
		public void CreateReport(WizardContext wizardContext, string reportFile, 
			FileBasedProject selectedProject, string targetLanguageCode)
		{
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};

			var actionName = wizardContext.Action == Enumerators.Action.Export
				? "Export to XLIFF"
				: "Import from XLIFF";

			var projectFiles = wizardContext.ProjectFiles.Where(a => a.Selected &&
				string.Compare(a.TargetLanguage, targetLanguageCode, 
					StringComparison.CurrentCultureIgnoreCase) == 0).ToList();

			using (var writer = XmlWriter.Create(reportFile, settings))
			{
				writer.WriteStartElement("task");
				writer.WriteAttributeString("name", actionName);
				writer.WriteAttributeString("created", wizardContext.DateTimeStampToString);

				WriteReportTaskInfo(writer, wizardContext, selectedProject, targetLanguageCode);

				foreach (var projectFile in projectFiles)
				{
					WriteReportFile(writer, wizardContext, projectFile);
				}

				WriteReportTotal(writer, wizardContext, projectFiles);

				writer.WriteEndElement(); //task
			}
		}

		private void WriteReportTotal(XmlWriter writer, WizardContext wizardContext, IReadOnlyCollection<ProjectFile> projectFiles)
		{
			writer.WriteStartElement("batchTotal");

			var totalTranslationOriginStatistics = GetTotalTranslationOriginStatistics(projectFiles, wizardContext.Action);
			WriteAnalysisXml(writer, totalTranslationOriginStatistics?.WordCounts, wizardContext.AnalysisBands, wizardContext.Action);

			var totalConfirmationStatistics = GetTotalConfirmationStatistics(projectFiles, wizardContext.Action);
			WriteConfirmationXml(writer, totalConfirmationStatistics?.WordCounts, wizardContext.Action);

			writer.WriteEndElement(); //batchTotal
		}

		private void WriteReportFile(XmlWriter writer, WizardContext wizardContext, ProjectFile projectFile)
		{
			writer.WriteStartElement("file");
			writer.WriteAttributeString("name", Path.Combine(projectFile.Path, projectFile.Name));
			writer.WriteAttributeString("guid", projectFile.FileId);

			WriteAnalysisXml(writer, projectFile.TranslationOriginStatistics?.WordCounts, wizardContext.AnalysisBands, wizardContext.Action);
			WriteConfirmationXml(writer, projectFile.ConfirmationStatistics?.WordCounts, wizardContext.Action);

			writer.WriteEndElement(); //file
		}

		private void WriteReportTaskInfo(XmlWriter writer, WizardContext wizardContext, IProject fileBasedProject, string languageCode)
		{
			writer.WriteStartElement("taskInfo");
			writer.WriteAttributeString("action", wizardContext.Action.ToString());
			writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
			writer.WriteAttributeString("runAt", wizardContext.DateTimeStamp.ToShortDateString() + " " + wizardContext.DateTimeStamp.ToShortTimeString());

			WriteReportProject(writer, wizardContext);

			WriteReportLanguage(writer, languageCode);

			WriteReportCustomer(writer, wizardContext);

			WriteReportTranslationProviders(writer, fileBasedProject);

			WriteReportSettings(writer, wizardContext);

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

		private static void WriteReportCustomer(XmlWriter writer, WizardContext wizardContext)
		{
			if (!string.IsNullOrEmpty(wizardContext.Project.Customer?.Name))
			{
				writer.WriteStartElement("customer");
				writer.WriteAttributeString("name", wizardContext.Project.Customer.Name);
				writer.WriteAttributeString("email", wizardContext.Project.Customer.Email);
				writer.WriteEndElement(); //customer												  
			}
		}

		private static void WriteReportProject(XmlWriter writer, WizardContext wizardContext)
		{
			writer.WriteStartElement("project");
			writer.WriteAttributeString("name", wizardContext.Project.Name);
			writer.WriteAttributeString("number", wizardContext.Project.Id);
			if (wizardContext.Project.DueDate != DateTime.MinValue && wizardContext.Project.DueDate != DateTime.MaxValue)
			{
				writer.WriteAttributeString("dueDate",
					wizardContext.Project.DueDate.ToShortDateString() + " " + wizardContext.Project.DueDate.ToShortTimeString());
			}

			writer.WriteEndElement(); //project
		}

		private void WriteReportSettings(XmlWriter writer, WizardContext wizardContext)
		{
			writer.WriteStartElement("settings");
			if (wizardContext.Action == Enumerators.Action.Export)
			{
				writer.WriteAttributeString("xliffSupport", wizardContext.ExportOptions.XliffSupport.ToString());
				writer.WriteAttributeString("includeTranslations", wizardContext.ExportOptions.IncludeTranslations.ToString());
				writer.WriteAttributeString("copySourceToTarget", wizardContext.ExportOptions.CopySourceToTarget.ToString());
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(wizardContext.ExportOptions.ExcludeFilterIds));
			}
			else
			{
				writer.WriteAttributeString("overwriteTranslations", wizardContext.ImportOptions.OverwriteTranslations.ToString());
				writer.WriteAttributeString("originSystem", wizardContext.ImportOptions.OriginSystem);
				writer.WriteAttributeString("statusTranslationUpdatedId", GetSegmentStatus(wizardContext.ImportOptions.StatusTranslationUpdatedId));
				writer.WriteAttributeString("statusTranslationNotUpdatedId", GetSegmentStatus(wizardContext.ImportOptions.StatusTranslationNotUpdatedId));
				writer.WriteAttributeString("statusSegmentNotImportedId", GetSegmentStatus(wizardContext.ImportOptions.StatusSegmentNotImportedId));
				writer.WriteAttributeString("excludeFilterItems", GetFitlerItemsString(wizardContext.ImportOptions.ExcludeFilterIds));
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
			if (action == Enumerators.Action.Import)
			{
				WriteConfirmationWordCountStatistics(writer, wordCounts?.NotProcessed, "notProcessed");
			}

			writer.WriteEndElement(); //confirmation
		}

		private void WriteAnalysisXml(XmlWriter writer, WordCounts wordCounts, IReadOnlyCollection<AnalysisBand> analysisBands, Enumerators.Action action)
		{
			writer.WriteStartElement("analysis");

			WriteAnalysisWordCountStatistics(writer, wordCounts?.Processed, analysisBands, "processed");
			WriteAnalysisWordCountStatistics(writer, wordCounts?.Excluded, analysisBands, "excluded");
			if (action == Enumerators.Action.Import)
			{
				WriteAnalysisWordCountStatistics(writer, wordCounts?.NotProcessed, analysisBands, "notProcessed");
			}

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

					if (action == Enumerators.Action.Import)
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

					if (action == Enumerators.Action.Import)
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
