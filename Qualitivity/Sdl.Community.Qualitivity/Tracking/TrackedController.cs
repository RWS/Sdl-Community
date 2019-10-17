using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Parser;
using Sdl.Community.Qualitivity.Dialogs;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Panels.QualityMetrics;
using Sdl.Community.Qualitivity.Progress;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.TM.Database;
using Sdl.Community.Toolkit.LanguagePlatform;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Comment = Sdl.Community.Structures.Documents.Records.Comment;
using Document = Sdl.TranslationStudioAutomation.IntegrationApi.Document;
using DocumentActivities = Sdl.Community.Structures.Projects.Activities.DocumentActivities;
using RevisionMarker = Sdl.Community.Structures.Documents.Records.RevisionMarker;
using TranslationOrigin = Sdl.Community.Structures.Documents.Records.TranslationOrigin;

namespace Sdl.Community.Qualitivity.Tracking
{
	public class TrackedController
	{


		private static ContentGenerator _contentProcessor;

		public static QualitivityViewController Controller { get; set; }

		public static ContentGenerator ContentProcessor => _contentProcessor ?? (_contentProcessor = new ContentGenerator());

		public static void ProgressChanged(int maximum, int current, string message)
		{
			var progress = new ProgressObject
			{
				CurrentProcessingMessage = ProgressWindow.ProgressDialog.DialogProcessingMessage,
				CurrentProgressTitle = PluginResources.Record_Progress,
				CurrentProgressValueMessage = message,
				CurrentProgressMaximum = maximum,
				CurrentProgressValue = current,
				CurrentProgressPercentage =
					Convert.ToString(Math.Round(Convert.ToDouble(current) / Convert.ToDouble(maximum) * 100, 0),
						CultureInfo.InvariantCulture) + "%",
				TotalProgressTitle = PluginResources.Total_Progress,
				TotalProgressValueMessage =
					string.Format(ProgressWindow.ProgressDialog.DocumentProgressLabelStringFormat,
						ProgressWindow.ProgressDialog.DocumentCurrentIndex,
						ProgressWindow.ProgressDialog.DocumentsMaximum)
			};

			progress.TotalProgressMaximum = ProgressWindow.ProgressDialog.DocumentsMaximum * progress.CurrentProgressMaximum;
			progress.TotalProgressValue = (ProgressWindow.ProgressDialog.DocumentCurrentIndex - 1) * progress.CurrentProgressMaximum + progress.CurrentProgressValue;
			progress.TotalProgressPercentage = Convert.ToString(Math.Round(Convert.ToDouble(progress.TotalProgressValue) / Convert.ToDouble(progress.TotalProgressMaximum) * 100, 0), CultureInfo.InvariantCulture) + "%";

			ProgressWindow.ProgressDialogWorker.ReportProgress(current, progress);
		}

		public static void TrackNewDocumentEntry(Document doc)
		{
			var projectFile = doc.Files.FirstOrDefault();
			if (projectFile == null || Tracked.DictCacheDocumentItems.ContainsKey(projectFile.Id.ToString()))
				return;

			var trackedDocuments = new TrackedDocuments
			{
				DocumentIdFirstOrDefault = projectFile.Id.ToString(),
				ActivityDescription = string.Empty,
				ActivityType = doc.Mode.ToString()
			};

			#region  |  client information  |

			var project = NewQualitivityProject(doc);
			var cpi = Helper.GetClientFromId(project.CompanyProfileId);
			if (cpi != null)
			{
				trackedDocuments.ClientId = cpi.Id;
				trackedDocuments.ClientName = cpi.Name;
			}
			#endregion

			#region  |  project information  |
			trackedDocuments.ProjectIdStudio = project.StudioProjectId;
			trackedDocuments.ProjectNameStudio = doc.Project.GetProjectInfo().Name;
			trackedDocuments.ProjectPathStudio = doc.Project.GetProjectInfo().LocalProjectFolder;


			trackedDocuments.ProjectId = project.Id;
			trackedDocuments.ProjectName = project.Name;
			trackedDocuments.ProjectPath = trackedDocuments.ProjectPathStudio;
			#endregion

			#region  |  Tracked Document Documents  |

			foreach (var file in doc.Files)
			{
				var trackedDocument = new TrackedDocument
				{
					Id = file.Id.ToString(),
					Name = file.Name,
					SourceLanguage = project.SourceLanguage,
					TargetLanguage = doc.ActiveFile.Language.CultureInfo.Name
				};



				#region  |  get file path  |
				var localPath = file.LocalFilePath;
				var targetLanguageId = file.Language.CultureInfo.Name;
				var projectFilePath = string.Empty;
				try
				{
					projectFilePath = localPath.Substring(localPath.LastIndexOf(targetLanguageId + "\\", StringComparison.Ordinal) + (targetLanguageId + "\\").Length);
				}
				catch
				{
					// ignored
				}

				#endregion

				trackedDocument.Path = projectFilePath;

				trackedDocument.TranslationMatchTypesOriginal = InitializeDocumentStatisticalState(Tracked.ActiveDocument, file.Id.ToString());
				trackedDocument.ConfirmationStatusOriginal = InitalizeDocumentConfirmationStatisticalState(Tracked.ActiveDocument, file.Id.ToString());
				trackedDocument.TotalSegments = doc.SegmentPairs.Count();

				trackedDocument.DatetimeOpened = DateTime.Now;
				trackedDocument.DocumentTimer = new Stopwatch();

				trackedDocument.TrackedRecords = new List<Record>();

				trackedDocuments.Documents.Add(trackedDocument);

			}
			#endregion

			#region  |  add the document object to the database  |


			var query = new Query();

			foreach (var trackedDocument in trackedDocuments.Documents)
			{
				var document = new Structures.Documents.Document
				{
					DocumentId = trackedDocument.Id,
					DocumentName = trackedDocument.Name,
					DocumentPath = trackedDocument.Path,
					SourceLanguage = trackedDocument.SourceLanguage,
					TargetLanguage = trackedDocument.TargetLanguage,
					StudioProjectId = trackedDocuments.ProjectIdStudio
				};



				document.Id = query.SaveDocument(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, document);
			}
			#endregion

			#region  |  get the quality metric items  |

			trackedDocuments.QualityMetrics = new List<QualityMetric>();
			foreach (var trackedDocument in trackedDocuments.Documents)
			{
				trackedDocuments.QualityMetrics.AddRange(Helper.GetAllQualityMetricRecordsFromDocument(trackedDocuments.ProjectId, trackedDocument.Id));
			}

			#endregion

			trackedDocuments.ActiveDocument = trackedDocuments.Documents[0];
			//add the new item to the container
			Tracked.DictCacheDocumentItems.Add(trackedDocuments.DocumentIdFirstOrDefault, trackedDocuments);
		}

		public static void NewProjectActivity(TrackedDocuments trackedDocuments)
		{

			try
			{
				var activity = new Activity
				{
					Id = -1
				};

				var query = new Query();

				var project = Helper.GetProjectFromId(trackedDocuments.ProjectId);

				if (project == null || project.Id == -1)
				{
					project = NewQualitivityProject(Tracked.ActiveDocument);
				}

				activity.ProjectId = project.Id;

				var companyProfile = Helper.GetClientFromId(project.CompanyProfileId);
				if (companyProfile != null)
				{
					activity.CompanyProfileId = companyProfile.Id;
				}
				else
				{
					activity.CompanyProfileId = -1;
				}

				if (trackedDocuments.Documents.Count > 1)
				{
					DateTime? earliestDatetime = null;
					DateTime? latestDatetime = null;
					foreach (var trackedDocument in trackedDocuments.Documents)
					{
						if (!earliestDatetime.HasValue)
						{
							earliestDatetime = trackedDocument.DatetimeOpened;
						}
						else if (trackedDocument.DatetimeOpened != null &&
								 trackedDocument.DatetimeOpened.Value < earliestDatetime.Value)
						{
							earliestDatetime = trackedDocument.DatetimeOpened;
						}

						if (!latestDatetime.HasValue)
						{
							latestDatetime = trackedDocument.DatetimeClosed;
						}
						else if (trackedDocument.DatetimeClosed != null &&
								 trackedDocument.DatetimeClosed.Value > latestDatetime.Value)
						{
							latestDatetime = trackedDocument.DatetimeClosed;
						}
					}

					activity.Started = earliestDatetime;
					activity.Stopped = latestDatetime;
				}
				else
				{
					activity.Name = trackedDocuments.Documents[0].Name;

					var datetimeOpened = trackedDocuments.Documents[0].DatetimeOpened;
					if (datetimeOpened != null)
					{
						activity.Started = datetimeOpened.Value;
					}

					var datetimeClosed = trackedDocuments.Documents[0].DatetimeClosed;
					if (datetimeClosed != null)
					{
						activity.Stopped = datetimeClosed.Value;
					}
				}

				activity.ActivityStatus = Activity.Status.New;
				var documentActivities = new List<DocumentActivity>();
				foreach (var trackedDocument in trackedDocuments.Documents)
				{
					trackedDocuments.ActiveDocument = trackedDocument;

					var documentActivity = GetDocumentActivity(activity, project, trackedDocuments);

					documentActivity.ProjectActivityId = activity.Id;
					documentActivity.TranslatableDocument = query.GetDocument(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, trackedDocument.Id);
					documentActivities.Add(documentActivity);
				}

				var projectDocumentActivitiesList = documentActivities.Select(documentActivity => new DocumentActivities
				{
					DocumentId = documentActivity.DocumentId,
					DocumentActivityIds =
						new List<int> { documentActivity.Id },
					DocumentActivityTicks = documentActivity.TicksActivity,
					DocumentRecordsTicks = documentActivity.TicksRecords,
					TranslatableDocument = documentActivity.TranslatableDocument,
					ProjectActivityId = activity.Id
				}).ToList();

				activity.Activities = projectDocumentActivitiesList;


				var qmrs = new QualityMetricReportSettings
				{
					MetricGroupName = Tracked.Settings.QualityMetricGroup.Name,
					MaxSeverityValue = Tracked.Settings.QualityMetricGroup.MaxSeverityValue,
					MaxSeverityInValue = Tracked.Settings.QualityMetricGroup.MaxSeverityInValue,
					MaxSeverityInType = Tracked.Settings.QualityMetricGroup.MaxSeverityInType
				};
				activity.MetricReportSettings = qmrs;

				var projectActivity = new TrackProjectActivity
				{
					Projects = new List<Project> { project },
					Activity = activity,
					DocumentActivities = documentActivities,
					IsEdit = false
				};

				projectActivity.ShowDialog();
				if (!projectActivity.Saved)
				{
					return;
				}

				activity = projectActivity.Activity;
				activity.Id = query.CreateActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, activity);

				try
				{

					Application.DoEvents();
					Cursor.Current = Cursors.WaitCursor;

					foreach (var documentActivity in documentActivities)
						documentActivity.ProjectActivityId = activity.Id;


					ProgressWindow.ProgressDialog = new ProgressDialog();
					try
					{
						ProgressWindow.ProgressDialogWorker = new BackgroundWorker { WorkerReportsProgress = true };

						var documentActivitiesPost = documentActivities;

						ProgressWindow.ProgressDialogWorker.DoWork += (sender, e) => documentActivitiesPost = CreateDocumentActivity_DoWork(null, new DoWorkEventArgs(documentActivitiesPost));

						ProgressWindow.ProgressDialogWorker.RunWorkerCompleted += ProgressWindowHandlers.worker_RunWorkerCompleted;
						ProgressWindow.ProgressDialogWorker.ProgressChanged += ProgressWindowHandlers.worker_ProgressChanged;
						ProgressWindow.ProgressDialogWorker.RunWorkerAsync();
						ProgressWindow.ProgressDialog.ShowDialog();

					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally
					{
						ProgressWindow.ProgressDialog.Dispose();
					}
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}

				//update document activites
				project.Activities.Add(activity);


				Tracked.TarckerCheckNewActivityId = activity.Id;
				Tracked.TarckerCheckNewProjectId = project.Id;

				if (projectActivity.CompanyProfile != null && projectActivity.CompanyProfile.Id != -1 && activity.CompanyProfileId == -1)
				{
					activity.CompanyProfileId = projectActivity.CompanyProfile.Id;
					project.CompanyProfileId = activity.CompanyProfileId;

					foreach (var tpa in project.Activities)
						tpa.CompanyProfileId = project.CompanyProfileId;

					query.UpdateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, project);

					Tracked.TarckerCheckNewProjectAdded = true;
				}
				else
				{
					Tracked.TarckerCheckNewActivityAdded = true;
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static Project NewQualitivityProject(Document doc)
		{

			Project newProject = null;
			try
			{
				var projectInfo = doc.Project.GetProjectInfo();
				if (projectInfo != null)
				{
					foreach (var project in Tracked.TrackingProjects.TrackerProjects)
					{
						if (project.StudioProjectId != projectInfo.Id.ToString() &&
							string.Compare(project.StudioProjectPath, projectInfo.LocalProjectFolder.Trim(),
								StringComparison.OrdinalIgnoreCase) != 0)
						{
							continue;
						}

						newProject = project;
						break;
					}
					if (newProject == null)
					{
						var query = new Query();

						newProject = new Project
						{
							Id = -1,
							Name = projectInfo.Name,
							Path = projectInfo.LocalProjectFolder.Trim(),
							StudioProjectId = projectInfo.Id.ToString(),
							StudioProjectName = projectInfo.Name.Trim(),
							StudioProjectPath = projectInfo.LocalProjectFolder.Trim(),
							SourceLanguage = projectInfo.SourceLanguage.CultureInfo.Name,
							CompanyProfileId = -1,
							Activities = new List<Activity>(),
							ProjectStatus = projectInfo.IsCompleted ? @"Completed" : @"In progress",
							Description = projectInfo.Description ?? string.Empty,
							Started = projectInfo.CreatedAt,
							Created = DateTime.Now,
							Due = projectInfo.DueDate ?? DateTime.Now.AddDays(7)
						};

						if (newProject.Due < DateTime.Now)
						{
							newProject.Due = DateTime.Now.AddDays(1);
						}

						newProject.Completed = DateTime.Now;

						newProject.Id = query.CreateProject(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, newProject);
						Tracked.TrackingProjects.TrackerProjects.Add(newProject);

						Tracked.TarckerCheckNewProjectId = newProject.Id;
						Tracked.TarckerCheckNewProjectAdded = true;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(PluginResources.ErrorMessage_Get_tracker_project_from_document_0 + ex.Message);
			}


			return newProject;
		}

		public static List<StateCountItem> InitializeDocumentStatisticalState(Document doc, string fileId)
		{
			var stateCountItems = new List<StateCountItem>();

			var pmSegments = 0;
			var cmSegments = 0;
			var atSegments = 0;
			var exactSegments = 0;
			var repsSegments = 0;
			var fuzzySegments = 0;
			var fuzzy99Segments = 0;
			var fuzzy94Segments = 0;
			var fuzzy84Segments = 0;
			var fuzzy74Segments = 0;
			var newSegments = 0;

			var pmWords = 0;
			var cmWords = 0;
			var atWords = 0;
			var exactWords = 0;
			var repsWords = 0;
			var fuzzyWords = 0;
			var fuzzy99Words = 0;
			var fuzzy94Words = 0;
			var fuzzy84Words = 0;
			var fuzzy74Words = 0;
			var newWords = 0;


			var pmCharacters = 0;
			var cmCharacters = 0;
			var atCharacters = 0;
			var exactCharacters = 0;
			var repsCharacters = 0;
			var fuzzyCharacters = 0;
			var fuzzy99Characters = 0;
			var fuzzy94Characters = 0;
			var fuzzy84Characters = 0;
			var fuzzy74Characters = 0;
			var newCharacters = 0;


			var pmTags = 0;
			var cmTags = 0;
			var atTags = 0;
			var exactTags = 0;
			var repsTags = 0;
			var fuzzyTags = 0;
			var fuzzy99Tags = 0;
			var fuzzy94Tags = 0;
			var fuzzy84Tags = 0;
			var fuzzy74Tags = 0;
			var newTags = 0;

			var sourceLanguage = doc.ActiveFileProperties.FileConversionProperties.SourceLanguage.CultureInfo;
			var targetLanguage = doc.ActiveFileProperties.FileConversionProperties.TargetLanguage.CultureInfo;

			var segmentPairProcessor = new SegmentPairProcessor(
				new Toolkit.LanguagePlatform.Models.Settings(sourceLanguage, targetLanguage), new Toolkit.LanguagePlatform.Models.PathInfo());

			var parser = new ContentGenerator();

			foreach (var segPair in doc.SegmentPairs)
			{
				if (segPair.GetProjectFile().Id.ToString() != fileId) continue;
				if (segPair.Properties.TranslationOrigin == null) continue;
				var match = Helper.GetTranslationStatus(segPair.Properties.TranslationOrigin);

				parser.ProcessSegment(segPair.Source, true, null);

				var words = 0;
				var chars = 0;
				var tags = 0;
				var placeholders = 0;
				try
				{
					var results = segmentPairProcessor.GetSegmentPairInfo(segPair);
					if (results != null)
					{
						words = results.SourceWordCounts.Words;
						chars = results.SourceWordCounts.Characters;
						placeholders = results.SourceWordCounts.Placeables;
						tags = results.SourceWordCounts.Tags;
					}
				}
				catch
				{
					// catch all
				}

				switch (match.ToUpper())
				{
					case "PM":
						{
							pmSegments++;
							pmWords += words;
							pmCharacters += chars;
							pmTags += tags + placeholders;
							break;
						}
					case "CM":
						{
							cmSegments++;
							cmWords += words;
							cmCharacters += chars;
							cmTags += tags + placeholders;
							break;
						}
					case "AT":
						{
							atSegments++;
							atWords += words;
							atCharacters += chars;
							atTags += tags + placeholders;
							break;
						}

					case "REPS":
						{
							repsSegments++;
							repsWords += words;
							repsCharacters += chars;
							repsTags += tags + placeholders;
							break;
						}
					case "100%":
						{
							exactSegments++;
							exactWords += words;
							exactCharacters += chars;
							exactTags += tags + placeholders;
							break;
						}
					default:
						{
							var matchPercentage = Helper.GetMatchValue(match);


							if (matchPercentage >= 100)
							{
								exactSegments++;
								exactWords += words;
								exactCharacters += chars;
								exactTags += tags + placeholders;
							}
							else if (matchPercentage >= 95 && matchPercentage < 100)
							{
								fuzzySegments++;
								fuzzyWords += words;
								fuzzyCharacters += chars;
								fuzzyTags += tags + placeholders;

								fuzzy99Segments++;
								fuzzy99Words += words;
								fuzzy99Characters += chars;
								fuzzy99Tags += tags + placeholders;
							}
							else if (matchPercentage >= 85 && matchPercentage < 95)
							{
								fuzzySegments++;
								fuzzyWords += words;
								fuzzyCharacters += chars;
								fuzzyTags += tags + placeholders;

								fuzzy94Segments++;
								fuzzy94Words += words;
								fuzzy94Characters += chars;
								fuzzy94Tags += tags + placeholders;
							}
							else if (matchPercentage >= 75 && matchPercentage < 85)
							{
								fuzzySegments++;
								fuzzyWords += words;
								fuzzyCharacters += chars;
								fuzzyTags += tags + placeholders;

								fuzzy84Segments++;
								fuzzy84Words += words;
								fuzzy84Characters += chars;
								fuzzy84Tags += tags + placeholders;
							}
							else if (matchPercentage >= 50 && matchPercentage < 75)
							{
								fuzzySegments++;
								fuzzyWords += words;
								fuzzyCharacters += chars;
								fuzzyTags += tags + placeholders;

								fuzzy74Segments++;
								fuzzy74Words += words;
								fuzzy74Characters += chars;
								fuzzy74Tags += tags + placeholders;
							}
							else
							{
								newSegments++;
								newWords += words;
								newCharacters += chars;
								newTags += tags + placeholders;
							}
						}
						break;
				}
			}

			stateCountItems.Add(new StateCountItem
			{
				Name = "PM",
				Value = pmSegments
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "CM",
				Value = cmSegments
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "AT",
				Value = atSegments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "REPS",
				Value = repsSegments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Exact",
				Value = exactSegments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy",
				Value = fuzzySegments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy99",
				Value = fuzzy99Segments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy94",
				Value = fuzzy94Segments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy84",
				Value = fuzzy84Segments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy74",
				Value = fuzzy74Segments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "New",
				Value = newSegments
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "PMWords",
				Value = pmWords
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "CMWords",
				Value = cmWords
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "ATWords",
				Value = atWords
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "REPSWords",
				Value = repsWords
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "ExactWords",
				Value = exactWords
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "FuzzyWords",
				Value = fuzzyWords
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy99Words",
				Value = fuzzy99Words
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy94Words",
				Value = fuzzy94Words
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy84Words",
				Value = fuzzy84Words
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy74Words",
				Value = fuzzy74Words
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "NewWords",
				Value = newWords
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "PMCharacters",
				Value = pmCharacters
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "CMCharacters",
				Value = cmCharacters
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "ATCharacters",
				Value = atCharacters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "REPSCharacters",
				Value = repsCharacters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "ExactCharacters",
				Value = exactCharacters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "FuzzyCharacters",
				Value = fuzzyCharacters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy99Characters",
				Value = fuzzy99Characters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy94Characters",
				Value = fuzzy94Characters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy84Characters",
				Value = fuzzy84Characters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy74Characters",
				Value = fuzzy74Characters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "NewCharacters",
				Value = newCharacters
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "PMTags",
				Value = pmTags
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "CMTags",
				Value = cmTags
			});


			stateCountItems.Add(new StateCountItem
			{
				Name = "ATTags",
				Value = atTags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "REPSTags",
				Value = repsTags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "ExactTags",
				Value = exactTags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "FuzzyTags",
				Value = fuzzyTags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy99Tags",
				Value = fuzzy99Tags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy94Tags",
				Value = fuzzy94Tags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy84Tags",
				Value = fuzzy84Tags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "Fuzzy74Tags",
				Value = fuzzy74Tags
			});

			stateCountItems.Add(new StateCountItem
			{
				Name = "NewTags",
				Value = newTags
			});

			return stateCountItems;
		}

		public static List<StateCountItem> InitalizeDocumentConfirmationStatisticalState(Document doc, string fileId)
		{

			var tccsos = new List<StateCountItem>();

			var documentTotalNotTranslatedOriginal = 0;
			var documentTotalDraftOriginal = 0;
			var documentTotalTranslatedOriginal = 0;
			var documentTotalTranslationRejectedOriginal = 0;
			var documentTotalTranslationApprovedOriginal = 0;
			var documentTotalSignOffRejectedOriginal = 0;
			var documentTotalSignedOffOriginal = 0;

			foreach (var segPair in doc.SegmentPairs)
			{
				if (segPair.GetProjectFile().Id.ToString() != fileId) continue;
				switch (segPair.Properties.ConfirmationLevel)
				{
					case ConfirmationLevel.Unspecified: documentTotalNotTranslatedOriginal++; break;
					case ConfirmationLevel.Draft: documentTotalDraftOriginal++; break;
					case ConfirmationLevel.Translated: documentTotalTranslatedOriginal++; break;
					case ConfirmationLevel.RejectedTranslation: documentTotalTranslationRejectedOriginal++; break;
					case ConfirmationLevel.ApprovedTranslation: documentTotalTranslationApprovedOriginal++; break;
					case ConfirmationLevel.RejectedSignOff: documentTotalSignOffRejectedOriginal++; break;
					case ConfirmationLevel.ApprovedSignOff: documentTotalSignedOffOriginal++; break;
					default: documentTotalNotTranslatedOriginal++; break;
				}
			}

			tccsos.Add(new StateCountItem
			{
				Name = "NotTranslated",
				Value = documentTotalNotTranslatedOriginal
			});

			tccsos.Add(new StateCountItem
			{
				Name = "Draft",
				Value = documentTotalDraftOriginal
			});

			tccsos.Add(new StateCountItem
			{
				Name = "Translated",
				Value = documentTotalTranslatedOriginal
			});

			tccsos.Add(new StateCountItem
			{
				Name = "TranslationRejected",
				Value = documentTotalTranslationRejectedOriginal
			});

			tccsos.Add(new StateCountItem
			{
				Name = "TranslationApproved",
				Value = documentTotalTranslationApprovedOriginal
			});


			tccsos.Add(new StateCountItem
			{
				Name = "SignOffRejected",
				Value = documentTotalSignOffRejectedOriginal
			});

			tccsos.Add(new StateCountItem
			{
				Name = "SignedOff",
				Value = documentTotalSignedOffOriginal
			});


			return tccsos;


		}

		public static List<ContentSection> GetRecordContentSections(List<SegmentSection> segmentSections, ContentSection.LanguageType languageType, ref string content)
		{
			var contentSections = new List<ContentSection>();
			foreach (var segmentSection in segmentSections)
			{
				if (segmentSection.GetType() == typeof(Text))
				{
					var objText = segmentSection as Text;

					var section = new ContentSection { LangType = languageType };
					if (objText == null)
					{
						continue;
					}

					section.Content = objText.Value;
					section.CntType = ContentSection.ContentType.Text;
					section.RevisionMarker = null;
					section.HasRevision = false;
					if (objText.Revision != null)
					{
						section.HasRevision = true;
						section.RevisionMarker = new RevisionMarker
						{
							ContentSectionId = section.Id,
							Author = objText.Revision.Author,
							Created = objText.Revision.Date,
							RevType = (RevisionMarker.RevisionType)Enum.Parse(
								typeof(RevisionMarker.RevisionType), objText.Revision.RevType.ToString(), true)
						};
					}
					contentSections.Add(section);

					if (objText.Revision == null || (objText.Revision != null &&
													 objText.Revision.RevType !=
													 Parser.RevisionMarker.RevisionType.Delete))
					{
						content += objText.Value;
					}
				}
				else
				{
					var objTag = segmentSection as Tag;

					if (objTag != null)
					{
						var section = new ContentSection
						{
							LangType = languageType,
							IdRef = objTag.TagId,
							Content = objTag.TextEquivalent,
							CntType = (ContentSection.ContentType)Enum.Parse(
								typeof(ContentSection.ContentType), objTag.SectionType.ToString(), true),
							RevisionMarker = null,
							HasRevision = false
						};

						if (objTag.Revision != null)
						{
							section.HasRevision = true;
							section.RevisionMarker = new RevisionMarker
							{
								ContentSectionId = section.Id,
								Author = objTag.Revision.Author,
								Created = objTag.Revision.Date,
								RevType = (RevisionMarker.RevisionType)Enum.Parse(
									typeof(RevisionMarker.RevisionType), objTag.Revision.RevType.ToString(), true)
							};
						}
						contentSections.Add(section);
					}

					if (objTag != null && (objTag.Revision == null || (objTag.Revision != null && objTag.Revision.RevType != Parser.RevisionMarker.RevisionType.Delete)))
						content += objTag.TextEquivalent;
				}
			}

			return contentSections;
		}

		public static DocumentActivity GetDocumentActivity(Activity activity, Project project, TrackedDocuments trackedDocuments)
		{
			var documentActivity = new DocumentActivity
			{
				Id = -1,
				DocumentId = trackedDocuments.ActiveDocument.Id,
				TranslatableDocument = new Structures.Documents.Document
				{
					DocumentId = trackedDocuments.ActiveDocument.Id,
					DocumentName = trackedDocuments.ActiveDocument.Name,
					DocumentPath = trackedDocuments.ActiveDocument.Path,
					SourceLanguage = project.SourceLanguage,
					TargetLanguage = trackedDocuments.ActiveDocument.TargetLanguage,
					StudioProjectId = project.StudioProjectId
				},
				ProjectId = trackedDocuments.ProjectId,
				ProjectActivityId = activity.Id,
				DocumentActivityType = trackedDocuments.ActivityType
			};

			#region  |  attach the quality metric items to the records  |

			foreach (var qualityMetric in trackedDocuments.QualityMetrics)
			{
				if (!qualityMetric.Updated) continue;
				foreach (var record in trackedDocuments.ActiveDocument.TrackedRecords)
				{
					if (record.ParagraphId != qualityMetric.ParagraphId || record.SegmentId != qualityMetric.SegmentId)
					{
						continue;
					}

					if (record.QualityMetrics.Exists(a => a.Id == qualityMetric.Id))
					{
						continue;
					}

					record.QualityMetrics.Add(qualityMetric);
					break;
				}
			}


			#endregion

			documentActivity.Records = trackedDocuments.ActiveDocument.TrackedRecords;
			documentActivity.Started = trackedDocuments.ActiveDocument.DatetimeOpened;
			documentActivity.Stopped = trackedDocuments.ActiveDocument.DatetimeClosed;
			documentActivity.TicksActivity = trackedDocuments.ActiveDocument.DocumentTimer.Elapsed.Ticks;
			documentActivity.TicksRecords = Helper.GetTotalTicksFromActivityRecords(documentActivity.Records);

			documentActivity.WordCount = Helper.GetTotalWordsFromActivityRecords(documentActivity.Records);

			documentActivity.DocumentStateCounters.TranslationMatchTypes = trackedDocuments.ActiveDocument.TranslationMatchTypesOriginal;
			documentActivity.DocumentStateCounters.ConfirmationStatuses = trackedDocuments.ActiveDocument.ConfirmationStatusOriginal;
			return documentActivity;
		}

		public static void InitializeActiveSegment(TrackedDocuments trackedDocuments)
		{
			try
			{
				trackedDocuments.ActiveSegment = new TrackedSegment
				{
					CurrentSegmentOpened = DateTime.Now,
					CurrentSegmentClosed = DateTime.Now
				};

				trackedDocuments.ActiveSegment.CurrentSegmentTimer.Restart();

				trackedDocuments.ActiveSegment.CurrentSegmentContentHasChanged = false;
				trackedDocuments.ActiveSegment.CurrentSegmentSelected = Tracked.ActiveDocument.GetActiveSegmentPair();
#if DEBUG
				Debug.WriteLine("Initializing active segment");
				if (trackedDocuments.ActiveSegment.CurrentSegmentSelected == null)
					Debug.WriteLine("Current segment is null");
				else
					Debug.WriteLine("Current segment source " + trackedDocuments.ActiveSegment.CurrentSegmentSelected.Source.ToString());
#endif

				try
				{
					var activeFileId = Tracked.ActiveDocument.ActiveFile.Id.ToString();
					if (trackedDocuments.ActiveDocument.Id != activeFileId)
					{
						trackedDocuments.ActiveDocument.DocumentTimer.Stop();
						trackedDocuments.ActiveDocument = trackedDocuments.Documents.Find(a => a.Id == activeFileId);
						trackedDocuments.ActiveDocument.DocumentTimer.Start();
					}
				}
				catch
				{
					// ignored
				}
				if (trackedDocuments.ActiveSegment.CurrentSegmentSelected != null)
				{
					trackedDocuments.ActiveSegment.CurrentISegmentPairProperties = trackedDocuments.ActiveSegment.CurrentSegmentSelected.Properties.Clone() as ISegmentPairProperties;
					trackedDocuments.ActiveSegment.CurrentDocumentId = Tracked.ActiveDocument.ActiveFile.Id.ToString();
					trackedDocuments.ActiveSegment.CurrentSegmentId = trackedDocuments.ActiveSegment.CurrentSegmentSelected.Properties.Id.Id;
					trackedDocuments.ActiveSegment.CurrentParagraphId = trackedDocuments.ActiveSegment.CurrentSegmentSelected.GetParagraphUnitProperties().ParagraphUnitId.Id;
					trackedDocuments.ActiveSegment.CurrentSegmentUniqueId = trackedDocuments.ActiveSegment.CurrentParagraphId + "." + trackedDocuments.ActiveSegment.CurrentSegmentId;
#if DEBUG
					Debug.WriteLine("Confirmation level: " + trackedDocuments.ActiveSegment.CurrentSegmentSelected.Properties.ConfirmationLevel);
#endif

					// check if there has been a change in the record structure; if yes, then add it to the container
					if (!Tracked.DocumentSegmentPairs.ContainsKey(trackedDocuments.ActiveSegment.CurrentSegmentUniqueId))
						TrackSegmentPair(trackedDocuments.ActiveSegment.CurrentSegmentUniqueId);

					QualitivityRevisionController.SetCurrentSelectedSegmentId(trackedDocuments.ActiveSegment.CurrentParagraphId, trackedDocuments.ActiveSegment.CurrentSegmentId);

					trackedDocuments.ActiveSegment.CurrentKeyStrokes = new List<KeyStroke>();

					var trackingSegmentContentTrg = string.Empty;
					var targetSectionsCurrent = GetRecordContentSections(
						Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].TargetSections
							, ContentSection.LanguageType.Target
							, ref trackingSegmentContentTrg);

					trackedDocuments.ActiveSegment.CurrentTargetSections = new List<ContentSection>();
					foreach (var section in targetSectionsCurrent)
						trackedDocuments.ActiveSegment.CurrentTargetSections.Add((ContentSection)section.Clone());

					foreach (var contentSection in trackedDocuments.ActiveSegment.CurrentTargetSections)
					{
						if (contentSection.RevisionMarker == null || contentSection.RevisionMarker.RevType != RevisionMarker.RevisionType.Delete)
						{
							continue;
						}

						contentSection.Content = string.Empty;
						contentSection.RevisionMarker = null;
					}
				}
				else
				{
					QualitivityRevisionController.SetCurrentSelectedSegmentId(string.Empty, string.Empty);
					trackedDocuments.ActiveSegment = new TrackedSegment();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		public static void TrackActiveChanges(TrackedDocuments trackedDocuments)
		{
			try
			{
				if (trackedDocuments.ActiveSegment.CurrentSegmentSelected == null) return;

				#region  |  check the current segment  |

				trackedDocuments.ActiveSegment.CurrentSegmentClosed = DateTime.Now;
				trackedDocuments.ActiveSegment.CurrentSegmentTimer.Stop();

				var targetCommentsChanged = false;

				#region  |  targetCommentsChanged  |

				ContentProcessor.ProcessSegment(trackedDocuments.ActiveSegment.CurrentSegmentSelected.Target, true, new List<string>());

				var trackingTargetComments = new List<Comment>();
				var targetSegmentComments = new List<Parser.Comment>();

				foreach (var comment in ContentProcessor.Comments)
				{
					targetSegmentComments.Add((Parser.Comment)comment.Clone());

					var newComment = new Comment { Author = comment.Author };
					if (comment.Date.HasValue)
					{
						newComment.Created = new DateTime(comment.Date.Value.Ticks);
					}

					newComment.Severity = comment.Severity;
					newComment.Content = comment.Text;
					newComment.Version = comment.Version;

					trackingTargetComments.Add(newComment);
				}
				var trackingTargetCommentsChanged = new List<Comment>();


				foreach (var comment in trackingTargetComments)
				{
					if (!Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Comments.Exists(x =>
						comment.Created != null && x.Date == comment.Created.Value))
					{
						trackingTargetCommentsChanged.Add(comment);
						targetCommentsChanged = true;
					}
					else
					{
						foreach (var trackedComment in Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Comments)
						{
							if (trackedComment.Date != comment.Created.Value)
							{
								continue;
							}

							if (trackedComment.Text.Trim() != comment.Content.Trim())
							{
								trackingTargetCommentsChanged.Add(comment);
								targetCommentsChanged = true;
							}
							break;
						}
					}
				}

				#endregion

				#region  |  qualityMetricChanged  |

				var qualityMetrics = QualitivityRevisionController.GetUpdatedQualityMetricsForCurrentSegment();
				var qualityMetricChanged = qualityMetrics.Count > 0;
				if (qualityMetricChanged)
				{
					foreach (var qualityMetric in qualityMetrics)
					{
						var qmUpdated = trackedDocuments.QualityMetrics.Find(a => a.Id == qualityMetric.Id);
						if (qmUpdated != null)
						{
							qmUpdated.Status = qualityMetric.Status;
							qmUpdated.Content = qualityMetric.Content;
							qmUpdated.Comment = qualityMetric.Comment;
							qmUpdated.Modified = qualityMetric.Modified;
							qmUpdated.Name = qualityMetric.Name;
							qmUpdated.SeverityName = qualityMetric.SeverityName;
							qmUpdated.SeverityValue = qualityMetric.SeverityValue;
							qmUpdated.UserName = qualityMetric.UserName;
							qmUpdated.Updated = qualityMetric.Updated;
							qmUpdated.Removed = qualityMetric.Removed;
						}
						else
						{
							trackedDocuments.QualityMetrics.Add(qualityMetric.Clone() as QualityMetric);
						}
					}
				}


				#endregion

				if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty(@"recordNonUpdatedSegments").Value)
					|| Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].ConfirmationLevel
							!= trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.ConfirmationLevel.ToString()
					|| trackedDocuments.ActiveSegment.CurrentSegmentContentHasChanged
					|| targetCommentsChanged
					|| qualityMetricChanged)
				{
					var record = new Record
					{
						Id = -1,
						Started = trackedDocuments.ActiveSegment.CurrentSegmentOpened,
						Stopped = trackedDocuments.ActiveSegment.CurrentSegmentClosed,
						TicksElapsed = trackedDocuments.ActiveSegment.CurrentSegmentTimer.Elapsed.Ticks,
						ParagraphId = trackedDocuments.ActiveSegment.CurrentParagraphId,
						SegmentId = trackedDocuments.ActiveSegment.CurrentSegmentId
					};

					ContentProcessor.ProcessSegment(trackedDocuments.ActiveSegment.CurrentSegmentSelected.Source, true, new List<string>());
					var trackingSegmentContentSrc = string.Empty;
					record.ContentSections.SourceSections = GetRecordContentSections(
						ContentProcessor.SegmentSections
						, ContentSection.LanguageType.Source
						, ref trackingSegmentContentSrc);

					try
					{
						var results = trackedDocuments.ActiveDocument.SegmentPairProcessor.GetSegmentPairInfo(trackedDocuments.ActiveSegment.CurrentSegmentSelected);
						if (results != null)
						{
							record.WordCount = results.SourceWordCounts.Words;
							record.CharsCount = results.SourceWordCounts.Characters;
							record.PlaceablesCount = results.SourceWordCounts.Placeables;
							record.TagsCount = results.SourceWordCounts.Tags;
						}
					}
					catch
					{
						// catch all
					}

					#region  |  translationOrigins  |

					record.TranslationOrigins.Original.ConfirmationLevel = Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].ConfirmationLevel;
					record.TranslationOrigins.Updated.ConfirmationLevel = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.ConfirmationLevel.ToString();

					record.TranslationOrigins.Original.TranslationStatus = Helper.GetTranslationStatus(Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin);
					record.TranslationOrigins.Updated.TranslationStatus = Helper.GetTranslationStatus(trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin);

					record.TranslationOrigins.Original.OriginType = Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.OriginType;
					record.TranslationOrigins.Updated.OriginType = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginType;

					record.TranslationOrigins.Original.OriginSystem = Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.OriginSystem != null ? Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.OriginSystem : string.Empty;
					record.TranslationOrigins.Updated.OriginSystem = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginSystem != null ? trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginSystem : string.Empty;

					if (trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginBeforeAdaptation != null)
					{
						if (record.TranslationOrigins.UpdatedPrevious == null)
						{
							record.TranslationOrigins.UpdatedPrevious = new TranslationOrigin(TranslationOrigin.LanguageType.UpdatedPrevious);
						}

						record.TranslationOrigins.UpdatedPrevious.TranslationStatus = Helper.GetTranslationStatus(trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginBeforeAdaptation);
						record.TranslationOrigins.UpdatedPrevious.OriginType = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginType;
						record.TranslationOrigins.UpdatedPrevious.OriginSystem = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem != null ? trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem : string.Empty;
					}

					#endregion

					var trackingSegmentContentTrgPrevious = string.Empty;
					record.ContentSections.TargetOriginalSections = GetRecordContentSections(
						Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].TargetSections
						, ContentSection.LanguageType.Target
						, ref trackingSegmentContentTrgPrevious);


					ContentProcessor.ProcessSegment(trackedDocuments.ActiveSegment.CurrentSegmentSelected.Target, true, Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].RevisionMarkerUniqueIds);

					var trackingSegmentContentTrg = string.Empty;
					record.ContentSections.TargetUpdatedSections = GetRecordContentSections(
						ContentProcessor.SegmentSections
						, ContentSection.LanguageType.TargetUpdated
						, ref trackingSegmentContentTrg);

					record.Comments = new List<Comment>();
					record.Comments.AddRange(trackingTargetCommentsChanged);

					//add the item to the segments being tracked
					trackedDocuments.ActiveDocument.TrackedRecords.Add(record);

					#region  |  add key strokes  |

					if (Convert.ToBoolean(Tracked.Settings.GetTrackingProperty("recordKeyStokes").Value))
					{
						record.TargetKeyStrokes = new List<KeyStroke>();

						foreach (var ks in trackedDocuments.ActiveSegment.CurrentKeyStrokes)
						{
							record.TargetKeyStrokes.Add((KeyStroke)ks.Clone());
						}

						#region  |  sanity check against the auto-translation cycle  |

						// might not have been picked up by the key stroke sequence
						// this check ensures that the system and origin information will be correctly 
						// allocated to the last key stroke entry
						if (string.Compare(record.TranslationOrigins.Updated.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) != 0)
						{
							if (record.TargetKeyStrokes.Count > 0)
							{
								var ks = record.TargetKeyStrokes[record.TargetKeyStrokes.Count - 1];
								if (string.Compare(record.TranslationOrigins.Updated.OriginType, ks.OriginType, StringComparison.OrdinalIgnoreCase) != 0)
								{
									if (string.Compare(trackingSegmentContentTrg, ks.Text, StringComparison.OrdinalIgnoreCase) == 0)
									{
										ks.OriginType = record.TranslationOrigins.Updated.OriginType;
										ks.OriginSystem = record.TranslationOrigins.Updated.OriginSystem ?? string.Empty;
										ks.Match = ((int)trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.MatchPercent).ToString();
									}
									else
									{
										//should never get here; but just in case...
										ks = new KeyStroke
										{
											Created = trackedDocuments.ActiveSegment.CurrentSegmentClosed,
											Text = trackingSegmentContentTrg,
											OriginType = record.TranslationOrigins.Updated.OriginType,
											OriginSystem = record.TranslationOrigins.Updated.OriginSystem ?? string.Empty,
											Match = ((int)trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.MatchPercent).ToString(),
											Position = Convert.ToInt32(TrackedDocumentEvents.GetTargetCursorPosition()),
											X = Cursor.Position.X,
											Y = Cursor.Position.Y
									};

										record.TargetKeyStrokes.Add(ks);
									}
								}
							}
							else
							{
								var keyStroke = new KeyStroke
								{
									Created = trackedDocuments.ActiveSegment.CurrentSegmentClosed,
									Text = trackingSegmentContentTrg,
									OriginType = record.TranslationOrigins.Updated.OriginType,
									OriginSystem = record.TranslationOrigins.Updated.OriginSystem ?? string.Empty,
									Match = ((int)trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.MatchPercent).ToString(),
									Position = Convert.ToInt32(TrackedDocumentEvents.GetTargetCursorPosition()),
									X = Cursor.Position.X,
									Y = Cursor.Position.Y
								};

								record.TargetKeyStrokes.Add(keyStroke);
							}
						}

						#endregion

						trackedDocuments.ActiveSegment.CurrentKeyStrokes = new List<KeyStroke>();
					}
					#endregion

					#region  |  update cache  |

					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Target = trackingSegmentContentTrg;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].TargetSections = ContentProcessor.SegmentSections;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].RevisionMarkerUniqueIds = ContentProcessor.RevisionMarkersUniqueIds;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].ConfirmationLevel = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.ConfirmationLevel.ToString();

					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Comments = targetSegmentComments;

					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.IsRepeated = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.IsRepeated;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.IsStructureContextMatch = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.IsStructureContextMatch;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.MatchPercentage = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.MatchPercent;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.OriginSystem = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginSystem != null ? trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginSystem : string.Empty;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.OriginType = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.OriginType;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.RepetitionTableId = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.RepetitionTableId.Id;
					Tracked.DocumentSegmentPairs[trackedDocuments.ActiveSegment.CurrentSegmentUniqueId].Origin.TextContextMatchLevel = trackedDocuments.ActiveSegment.CurrentISegmentPairProperties.TranslationOrigin.TextContextMatchLevel.ToString();


					#endregion
				}

				#endregion

				trackedDocuments.ActiveSegment.CurrentSegmentSelected = null;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public static void TrackSegmentPairs(Document doc)
		{
			Tracked.DocumentSegmentPairs = new Dictionary<string, SegmentPair>();

			foreach (var segPair in doc.SegmentPairs)
			{
				var segmentPair = GetParsedSegmentPair(segPair);
				var uniqueId = segmentPair.ParagraphId + "." + segmentPair.Id;

				// this should never happen, but check that the segment is not already in the dictionary
				if (Tracked.DocumentSegmentPairs.ContainsKey(uniqueId))
				{
					continue;
				}

				Tracked.DocumentSegmentPairs.Add(uniqueId, segmentPair);
			}
		}

		public static void InitializeDocumentTracking(Document doc)
		{
			#region  |  initialize document cache item  |

			var projectFile = doc?.Files.FirstOrDefault();
			if (projectFile == null)
			{
				return;
			}

			var trackedDocuments = Tracked.DictCacheDocumentItems[projectFile.Id.ToString()];

			//get active document
			trackedDocuments.ActiveDocument = trackedDocuments.Documents.Find(a => a.Id == doc.ActiveFile.Id.ToString());


			trackedDocuments.ActiveSegment.CurrentSegmentContentHasChanged = false;
			trackedDocuments.ActiveSegment.CurrentSegmentSelected = null;

			#endregion

			#region  |  add handlers  |

			doc.SegmentsConfirmationLevelChanged -= TrackedDocumentEvents.ConfirmationLevelChanged;
			doc.SegmentsTranslationOriginChanged -= TrackedDocumentEvents.TranslationOriginChanged;
			doc.ActiveSegmentChanged -= TrackedDocumentEvents.ActiveSegmentChanged;
			doc.ContentChanged -= TrackedDocumentEvents.ContentChanged;
			doc.Selection.Changed -= TrackedDocumentEvents.SelectionChanged;
			doc.Selection.Source.Changed -= TrackedDocumentEvents.SourceChanged;
			doc.Selection.Target.Changed -= TrackedDocumentEvents.TargetChanged;

			doc.SegmentsConfirmationLevelChanged += TrackedDocumentEvents.ConfirmationLevelChanged;
			doc.SegmentsTranslationOriginChanged += TrackedDocumentEvents.TranslationOriginChanged;
			doc.ActiveSegmentChanged += TrackedDocumentEvents.ActiveSegmentChanged;
			doc.ContentChanged += TrackedDocumentEvents.ContentChanged;
			doc.Selection.Changed += TrackedDocumentEvents.SelectionChanged;
			doc.Selection.Source.Changed += TrackedDocumentEvents.SourceChanged;
			doc.Selection.Target.Changed += TrackedDocumentEvents.TargetChanged;

			#endregion
			try
			{
				TrackSegmentPairs(doc);

				if ((Tracked.TrackingState == Tracked.TimerState.Started || Tracked.TrackingState == Tracked.TimerState.Paused)
					&& trackedDocuments.ActiveDocument.Id != string.Empty)
				{
					InitializeActiveSegment(trackedDocuments);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public static List<Activity> DuplicateProjectActivity_DoWork(object sender, DoWorkEventArgs e)
		{

			var projectActivities = DuplicateProjectActivity_DoWork(e.Argument as List<Activity>);
			e.Result = projectActivities;

			return projectActivities;
		}

		public static List<DocumentActivity> CreateDocumentActivity_DoWork(object sender, DoWorkEventArgs e)
		{
			var documentActivities = CreateDocumentActivity_DoWork(e.Argument as List<DocumentActivity>);
			e.Result = documentActivities;

			return documentActivities;
		}

		public static List<DocumentActivity> UpdateDocumentActivity_DoWork(object sender, DoWorkEventArgs e)
		{
			var documentActivities = UpdateDocumentActivity_DoWork(e.Argument as List<DocumentActivity>);
			e.Result = documentActivities;

			return documentActivities;
		}

		private static List<Activity> DuplicateProjectActivity_DoWork(List<Activity> projectActivities)
		{
			var query = new Query();
			var activitiesCloned = new List<Activity>();

			try
			{
				query.ProgressChanged += ProgressChanged;

				var totalDocumentActivities = projectActivities.Sum(activity => activity.Activities.Count);

				ProgressWindow.ProgressDialog.DocumentCurrentIndex = 0;
				ProgressWindow.ProgressDialog.DocumentsMaximum = totalDocumentActivities;
				ProgressWindow.ProgressDialog.DocumentProgressLabelStringFormat = PluginResources.Updating_0_of_1_documents;
				ProgressWindow.ProgressDialog.DialogProcessingMessage = PluginResources.Create_New_Document_Activity_Message;

				foreach (var activity in projectActivities)
				{
					var activityClone = (Activity)activity.Clone();
					activityClone.Id = -1;
					activityClone.Name = activityClone.Name + "_copy";

					activityClone.Id = query.CreateActivity(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, activityClone);
					activitiesCloned.Add(activityClone);

					var documentActivities = query.GetDocumentActivities(
						Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + activity.ProjectId.ToString().PadLeft(6, '0')
						, Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
						, activity.Id, null);

					foreach (var documentActivity in documentActivities)
					{
						if (!(documentActivity.Clone() is DocumentActivity documentActivityClone))
						{
							continue;
						}

						documentActivityClone.ProjectActivityId = activityClone.Id;

						ProgressWindow.ProgressDialog.DocumentCurrentIndex++;
						documentActivity.Id = query.CreateDocumentActivity(
							Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + documentActivity.ProjectId.ToString().PadLeft(6, '0')
							, documentActivityClone);
					}
				}
			}
			finally
			{
				query.ProgressChanged -= ProgressChanged;
			}

			return activitiesCloned;
		}

		private static List<DocumentActivity> CreateDocumentActivity_DoWork(List<DocumentActivity> documentActivities)
		{
			var query = new Query();

			try
			{
				query.ProgressChanged += ProgressChanged;

				ProgressWindow.ProgressDialog.DocumentCurrentIndex = 0;
				ProgressWindow.ProgressDialog.DocumentsMaximum = documentActivities.Count;
				ProgressWindow.ProgressDialog.DocumentProgressLabelStringFormat = PluginResources.Updating_0_of_1_documents;
				ProgressWindow.ProgressDialog.DialogProcessingMessage = PluginResources.Create_New_Document_Activity_Message;

				foreach (var documentActivity in documentActivities)
				{
					ProgressWindow.ProgressDialog.DocumentCurrentIndex++;
					documentActivity.Id = query.CreateDocumentActivity(
						Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + documentActivity.ProjectId.ToString().PadLeft(6, '0')
						, documentActivity);
				}
			}
			finally
			{
				query.ProgressChanged -= ProgressChanged;
			}

			return documentActivities;
		}

		private static List<DocumentActivity> UpdateDocumentActivity_DoWork(List<DocumentActivity> documentActivities)
		{
			var query = new Query();

			try
			{
				query.ProgressChanged += ProgressChanged;

				ProgressWindow.ProgressDialog.DocumentCurrentIndex = 0;
				ProgressWindow.ProgressDialog.DocumentsMaximum = documentActivities.Count;
				ProgressWindow.ProgressDialog.DocumentProgressLabelStringFormat = PluginResources.Updating_0_of_1_documents;
				ProgressWindow.ProgressDialog.DialogProcessingMessage = PluginResources.Update_Document_Activity_Message;

				foreach (var documentActivity in documentActivities)
				{
					ProgressWindow.ProgressDialog.DocumentCurrentIndex++;
					query.UpdateDocumentActivity(
						Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + documentActivity.ProjectId.ToString().PadLeft(6, '0')
						, documentActivity);
				}
			}
			finally
			{
				query.ProgressChanged -= ProgressChanged;
			}

			return documentActivities;
		}

		private static void TrackSegmentPair(string currentSegmentUniqueId)
		{
			if (Tracked.DocumentSegmentPairs.ContainsKey(currentSegmentUniqueId))
			{
				return;
			}

			var segmentPair = GetParsedSegmentPair(Tracked.ActiveDocument.ActiveSegmentPair);
			var uniqueId = segmentPair.ParagraphId + "." + segmentPair.Id;

			// this should never happen, check!
			Debug.Assert(uniqueId != currentSegmentUniqueId, "Misalignment between active segment and segment being processed!");

			Tracked.DocumentSegmentPairs.Add(uniqueId, segmentPair);
		}

		private static SegmentPair GetParsedSegmentPair(ISegmentPair segPair)
		{
			ContentProcessor.ProcessSegment(segPair.Target, true, new List<string>());

			var segmentPair = new SegmentPair
			{
				Id = segPair.Properties.Id.Id,
				ParagraphId = segPair.GetParagraphUnitProperties().ParagraphUnitId.Id,
				IsLocked = segPair.Properties.IsLocked,
				ConfirmationLevel = segPair.Properties.ConfirmationLevel.ToString(),
				Source = segPair.Source.ToString(),
				SourceSections = new List<SegmentSection>(),
				Target = segPair.Target.ToString(),
				TargetSections = ContentProcessor.SegmentSections,
				RevisionMarkerUniqueIds = ContentProcessor.RevisionMarkersUniqueIds
			};

			foreach (var comment in ContentProcessor.Comments)
			{
				segmentPair.Comments.Add((Parser.Comment)comment.Clone());
			}

			segmentPair.Origin = new Parser.TranslationOrigin();
			if (segPair.Properties.TranslationOrigin == null)
			{
				return segmentPair;
			}

			segmentPair.Origin.IsRepeated = segPair.Properties.TranslationOrigin.IsRepeated;
			segmentPair.Origin.IsStructureContextMatch = segPair.Properties.TranslationOrigin.IsStructureContextMatch;
			segmentPair.Origin.MatchPercentage = segPair.Properties.TranslationOrigin.MatchPercent;
			segmentPair.Origin.OriginSystem = segPair.Properties.TranslationOrigin.OriginSystem ?? string.Empty;
			segmentPair.Origin.OriginType = segPair.Properties.TranslationOrigin.OriginType;
			segmentPair.Origin.RepetitionTableId = segPair.Properties.TranslationOrigin.RepetitionTableId.Id;
			segmentPair.Origin.TextContextMatchLevel = segPair.Properties.TranslationOrigin.TextContextMatchLevel.ToString();

			return segmentPair;
		}
	}
}
