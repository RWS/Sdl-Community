using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Sdl.Core.Globalization;
using Sdl.Core.PluginFramework;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Verification.Api;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
	/// <summary>
	/// The action for applying a studio project template
	/// </summary>
	[Action("ApplyStudioProjectTemplateAction", Icon = "ASPT", Name = "Apply Studio Project Template", Description = "Applies settings from a project template to the current project")]
	[ActionLayout(typeof(ApplyStudioProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
	[Shortcut(Keys.Control | Keys.Alt | Keys.T)]
	public class ApplyStudioProjectTemplateAction : AbstractViewControllerAction<ProjectsController>
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		/// <summary>
		/// Executes this instance.
		/// </summary>
		protected override void Execute()
		{
			string methodName = "Execute";
			var grammarCheckerSettingsId = "GrammarCheckerSettings";
			var numberVerifierSettingsId = "NumberVerifierSettings";
			var globalVerifiersExtensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<GlobalVerifierAttribute>();
			foreach (var extension in globalVerifiersExtensionPoint.Extensions)
			{
				var globalVerifier = (IGlobalVerifier)extension.CreateInstance();
				var settingsGroupId = globalVerifier.SettingsId;
				if (settingsGroupId.Contains("Grammar"))
				{
					grammarCheckerSettingsId = settingsGroupId;
				}
			}

			// Show the dialog
			var applyTemplateForm = new ApplyTemplateForm(Controller);
			if (applyTemplateForm.ShowDialog() == DialogResult.OK)
			{
				// This is the template that the user selected
				var selectedTemplate = applyTemplateForm.ActiveTemplate;

				// Create list of projects
				var selectedProjects = new List<FileBasedProject>();
				if (applyTemplateForm.ApplyToSelected)
				{
					selectedProjects.AddRange(Controller.SelectedProjects);
				}
				else
				{
					if (Controller.CurrentProject != null)
					{
						selectedProjects.Add(Controller.CurrentProject);
					}
				}

				// Check if we have any projects
				if (selectedProjects.Count == 0)
				{
					MessageBox.Show(applyTemplateForm.ApplyToSelected ? PluginResources.No_Projects_Selected : PluginResources.No_Current_Project, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Work through all projects
				var projectsList = new StringBuilder();
				projectsList.AppendLine(PluginResources.Settings_Applied);
				
				foreach (var targetProject in selectedProjects)
				{
					// Temporary folder path
					var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

					// This is the current project and it's related information
					var targetProjectInfo = targetProject.GetProjectInfo();
					projectsList.AppendLine(targetProjectInfo.Name);
					var targetSettingsBundle = targetProject.GetSettings();

					// This is the source project - check if it's a loaded one
					var sourceProject = Controller.GetAllProjects().FirstOrDefault(loadedProject => string.Compare(loadedProject.FilePath, selectedTemplate.FileLocation, StringComparison.OrdinalIgnoreCase) == 0);

					// Not found so load it from the filing system
					if (sourceProject == null)
					{
						if (string.IsNullOrEmpty(selectedTemplate.FileLocation))
						{
							if (selectedTemplate.Id != Guid.Empty)
							{
								// Create a new project based on a template configured in Studio
								var projectTemplate = new ProjectTemplateReference(selectedTemplate.Uri);

								var savedFolder = targetProjectInfo.LocalProjectFolder;
								targetProjectInfo.LocalProjectFolder = tempFolder;
								sourceProject = new FileBasedProject(targetProjectInfo, projectTemplate);
								targetProjectInfo.LocalProjectFolder = savedFolder;
							}
						}
						else
						{
							if (Path.GetExtension(selectedTemplate.FileLocation).ToLowerInvariant() == ".sdltpl")
							{
								// Create a new project based on a file-based template
								var projectTemplate = new ProjectTemplateReference(selectedTemplate.FileLocation);
								var savedFolder = targetProjectInfo.LocalProjectFolder;
								targetProjectInfo.LocalProjectFolder = tempFolder;
								sourceProject = new FileBasedProject(targetProjectInfo, projectTemplate);
								targetProjectInfo.LocalProjectFolder = savedFolder;
							}
							else
							{
								// Load a project from the filing system
								sourceProject = new FileBasedProject(selectedTemplate.FileLocation);
							}
						}
					}

					// Get the information from the source project
					var sourceProjectInfo = sourceProject.GetProjectInfo();
					var sourceSettingsBundle = sourceProject.GetSettings();

					if (selectedTemplate.VerificationSpecificLanguages == ApplyTemplateOptions.Overwrite)
					{
						// Copy verification settings
						if (sourceProjectInfo.SourceLanguage.Equals(targetProjectInfo.SourceLanguage))
						{
							foreach (Language sourceTargetLanguage in sourceProjectInfo.TargetLanguages)
							{
								foreach (Language targetTargetLanguage in targetProjectInfo.TargetLanguages)
								{
									if (sourceTargetLanguage.Equals(targetTargetLanguage))
									{
										try
										{
											var sourceTmSettings = sourceProject.GetSettings(sourceTargetLanguage);
											var targetTmSettings = targetProject.GetSettings(targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "QAVerificationSettings", targetProject, targetTargetLanguage);
										}
										catch (Exception e)
										{
											MessageBox.Show(e.Message, string.Format(PluginResources.TMSL_Failed, targetTargetLanguage.DisplayName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										}
									}
								}
							}
						}
					}

					// Copy all languages translation providers
					if (selectedTemplate.TranslationProvidersAllLanguages != ApplyTemplateOptions.Keep)
					{
						try
						{
							// Update the "all languages" node
							TranslationProviderConfiguration sourceProviderConfig = sourceProject.GetTranslationProviderConfiguration();
							if (selectedTemplate.TranslationProvidersAllLanguages == ApplyTemplateOptions.Merge)
							{
								TranslationProviderConfiguration targetProviderConfig = targetProject.GetTranslationProviderConfiguration();
								MergeTranslationProviders(sourceProviderConfig, targetProviderConfig);
								ValidateTranslationProviderConfiguration(targetProviderConfig);
								targetProject.UpdateTranslationProviderConfiguration(targetProviderConfig);
							}
							else
							{
								ValidateTranslationProviderConfiguration(sourceProviderConfig);
								targetProject.UpdateTranslationProviderConfiguration(sourceProviderConfig);
							}
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.TPAL_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy language-specific translation providers
					if (selectedTemplate.TranslationProvidersSpecificLanguages != ApplyTemplateOptions.Keep)
					{
						if (sourceProjectInfo.SourceLanguage.Equals(targetProjectInfo.SourceLanguage))
						{
							foreach (var sourceTargetLanguage in sourceProjectInfo.TargetLanguages)
							{
								foreach (var targetTargetLanguage in targetProjectInfo.TargetLanguages)
								{
									if (sourceTargetLanguage.Equals(targetTargetLanguage))
									{
										try
										{
											// Copy translation providers
											TranslationProviderConfiguration sourceProviderConfig = sourceProject.GetTranslationProviderConfiguration(sourceTargetLanguage);
											if (selectedTemplate.TranslationProvidersSpecificLanguages == ApplyTemplateOptions.Merge)
											{
												TranslationProviderConfiguration targetProviderConfig = targetProject.GetTranslationProviderConfiguration(targetTargetLanguage);
												MergeTranslationProviders(sourceProviderConfig, targetProviderConfig);
												ValidateTranslationProviderConfiguration(targetProviderConfig);
												targetProject.UpdateTranslationProviderConfiguration(targetTargetLanguage, targetProviderConfig);
											}
											else
											{
												ValidateTranslationProviderConfiguration(sourceProviderConfig);
												targetProject.UpdateTranslationProviderConfiguration(targetTargetLanguage, sourceProviderConfig);
											}
										}
										catch (Exception e)
										{
											MessageBox.Show(e.Message, string.Format(PluginResources.TPSL_Failed, targetTargetLanguage.DisplayName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										}
									}
								}
							}
						}
					}

					// Copy TM settings for all languages
					if (selectedTemplate.TranslationMemoriesAllLanguages == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							// Update the translation memory filter settings
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TranslationMemorySettings", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.TMAL_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy TM settings for specific languages
					if (selectedTemplate.TranslationMemoriesSpecificLanguages == ApplyTemplateOptions.Overwrite)
					{
						if (sourceProjectInfo.SourceLanguage.Equals(targetProjectInfo.SourceLanguage))
						{
							foreach (Language sourceTargetLanguage in sourceProjectInfo.TargetLanguages)
							{
								foreach (Language targetTargetLanguage in targetProjectInfo.TargetLanguages)
								{
									if (sourceTargetLanguage.Equals(targetTargetLanguage))
									{
										try
										{
											// Now copy translation memory settings - different section
											var sourceTmSettings = sourceProject.GetSettings(sourceTargetLanguage);
											var targetTmSettings = targetProject.GetSettings(targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "TranslationMemorySettings", targetProject, targetTargetLanguage);
										}
										catch (Exception e)
										{
											MessageBox.Show(e.Message, string.Format(PluginResources.TMSL_Failed, targetTargetLanguage.DisplayName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										}
									}
								}
							}
						}
					}

					// Copy terminology termbases
					if (selectedTemplate.TerminologyTermbases != ApplyTemplateOptions.Keep)
					{
						var sourceTermbaseConfig = sourceProject.GetTermbaseConfiguration();
						var targetTermbaseConfig = targetProject.GetTermbaseConfiguration();

						if (selectedTemplate.TerminologyTermbases == ApplyTemplateOptions.Merge)
						{
							if (targetTermbaseConfig.TermbaseServerUri is null)
							{
								targetTermbaseConfig.TermbaseServerUri = sourceTermbaseConfig.TermbaseServerUri;
							}
							if (!targetTermbaseConfig.Termbases.Any())
							{
								targetTermbaseConfig.Termbases?.AddRange(sourceTermbaseConfig.Termbases);
							}
							MergeTermbases(sourceTermbaseConfig, targetTermbaseConfig);
						}
						else
						{
							targetTermbaseConfig.TermbaseServerUri = sourceTermbaseConfig.TermbaseServerUri;
							targetTermbaseConfig.Termbases.Clear();
							targetTermbaseConfig.LanguageIndexes.Clear();
							MergeTermbases(sourceTermbaseConfig, targetTermbaseConfig);
						}

						// Updating with zero termbases throws an exception
						if (targetTermbaseConfig.Termbases.Count > 0)
						{
							try
							{
								targetProject.UpdateTermbaseConfiguration(targetTermbaseConfig);
							}
							catch (Exception e)
							{
								MessageBox.Show(e.Message, PluginResources.TBTB_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						}
					}

					// Copy terminology settings
					if (selectedTemplate.TerminologySearchSettings == ApplyTemplateOptions.Overwrite)
					{
						var sourceTermbaseConfig = sourceProject.GetTermbaseConfiguration();
						var targetTermbaseConfig = targetProject.GetTermbaseConfiguration();
						targetTermbaseConfig.TermRecognitionOptions = sourceTermbaseConfig.TermRecognitionOptions;
						CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TermRecognitionSettings", targetProject, null);

						// Updating with zero termbases throws an exception
						if (targetTermbaseConfig.Termbases.Count > 0)
						{
							try
							{
								targetProject.UpdateTermbaseConfiguration(targetTermbaseConfig);
							}
							catch (Exception e)
							{
								MessageBox.Show(e.Message, PluginResources.TBSS_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
						}
					}

					// Copy TQA settings where applicable
					if (selectedTemplate.TranslationQualityAssessment == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TranslationQualityAssessmentSettings", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.TQA_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy QA verification settings where applicable
					if (selectedTemplate.VerificationQaChecker30 == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "QAVerificationSettings", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.QAQA_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy tag verification settings where applicable
					if (selectedTemplate.VerificationTagVerifier == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "SettingsTagVerifier", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "Settings", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.QATG_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy term verification settings where applicable
					if (selectedTemplate.VerificationTerminologyVerifier == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "SettingsTermVerifier", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.QATV_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy number verification settings where applicable
					if (selectedTemplate.VerificationNumberVerifier == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, numberVerifierSettingsId, targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.QANV_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					if (selectedTemplate.MatchRepairSettings.Equals(ApplyTemplateOptions.Overwrite))
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "FuzzyMatchRepairSettings",
								targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.MRS_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy grammar checking settings where applicable
					if (selectedTemplate.VerificationGrammarChecker == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, grammarCheckerSettingsId, targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.QAGC_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy file type settings where applicable
					if (selectedTemplate.FileTypes == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "FileTypeManagerConfigurationSettingsGroup", targetProject, null);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.FTTS_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					// Copy batch tasks settings where applicable
					if (selectedTemplate.BatchTasksAllLanguages == ApplyTemplateOptions.Overwrite)
					{
						try
						{
							// Update the translation memory filter settings
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "PseudoTranslateSettings", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "AnalysisTaskSettings", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "VerificationSettings", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "FuzzyMatchRepairSettings", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TranslateTaskSettings", targetProject, null);
							CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TranslationMemoryUpdateTaskSettings", targetProject, null);
							CopySettingsFuzzyBands(sourceSettingsBundle, targetSettingsBundle, "FuzzyBandsSettings", sourceProject, targetProject);
						}
						catch (Exception e)
						{
							MessageBox.Show(e.Message, PluginResources.BTAL_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}

					if (selectedTemplate.BatchTasksSpecificLanguages == ApplyTemplateOptions.Overwrite)
					{
						// Update specific language pairs where possible
						if (sourceProjectInfo.SourceLanguage.Equals(targetProjectInfo.SourceLanguage))
						{
							foreach (var sourceTargetLanguage in sourceProjectInfo.TargetLanguages)
							{
								foreach (var targetTargetLanguage in targetProjectInfo.TargetLanguages)
								{
									if (sourceTargetLanguage.Equals(targetTargetLanguage))
									{
										try
										{
											// Now copy translation memory settings - different section
											var sourceTmSettings = sourceProject.GetSettings(sourceTargetLanguage);
											var targetTmSettings = targetProject.GetSettings(targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "PseudoTranslateSettings", targetProject, targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "AnalysisTaskSettings", targetProject, targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "VerificationSettings", targetProject, targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "TranslateTaskSettings", targetProject, targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "FuzzyMatchRepairSettings", targetProject, targetTargetLanguage);
											CopySettingsGroup(sourceTmSettings, targetTmSettings, "TranslationMemoryUpdateTaskSettings", targetProject, targetTargetLanguage);
										}
										catch (Exception e)
										{
											MessageBox.Show(e.Message, string.Format(PluginResources.BTSL_Failed, targetTargetLanguage.DisplayName), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
										}
									}
								}
							}
						}
					}

					// Remove the temporary folder if it exists
					if (Directory.Exists(tempFolder))
					{
						Directory.Delete(tempFolder, true);
					}

					// Use reflection to synch the project to the server
					try
					{
						_logger.Info("Method: {methodName}, Target Project: {@TargetProject}",
								methodName, targetProject);
						// re-analyze files, otherwise, everything would be "n/a" (SDLCOM-4268)
						/*
						 * var targetIds = targetProject.GetTargetLanguageFiles().Select(f => f.Id).ToArray();
						 * targetProject.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.AnalyzeFiles);
						 * targetProject.Save();
						*/
						// run analyze and pre-translating batch tasks (SDLCOM-4714)
						if (applyTemplateForm.RunAnalysisBatchTaskFlag || applyTemplateForm.RunPreTranslateBatchTaskFlag)
						{
							_logger.Info("Method: {methodName}, RunAnalysisBatchTaskFlag: {RunAnalysisBatchTaskFlag}, RunPreTranslateBatchTaskFlag: {RunPreTranslateBatchTaskFlag}",
								methodName, applyTemplateForm.RunAnalysisBatchTaskFlag, applyTemplateForm.RunPreTranslateBatchTaskFlag);
							var targetIds = targetProject.GetTargetLanguageFiles().Select(f => f.Id).ToArray();
							// run automatic task for analyze files
							if (applyTemplateForm.RunAnalysisBatchTaskFlag)
							{								
								var objAnalysisAutomaticTask = targetProject.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.AnalyzeFiles);
								if (objAnalysisAutomaticTask != null)
								{
									if (objAnalysisAutomaticTask.Status == TaskStatus.Completed)
									{
										targetProject.Save();
									}
									_logger.Info("Method: {methodName}, Analysis Automatic Task Status: {Status}",
										methodName, objAnalysisAutomaticTask.Status);
								}
								_logger.Info("Method: {methodName}, Analysis Automatic Task: {@objAnalysisAutomaticTask}",
											methodName, objAnalysisAutomaticTask);
								// Copy automation analysis batch task
								if (selectedTemplate.AnalysisBatchTask != ApplyTemplateOptions.Keep)
								{
									try
									{
										CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "AnalysisTaskSettings", targetProject);
									}
									catch (Exception e)
									{
										MessageBox.Show(e.Message, PluginResources.AABT_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
								}
							}
							// run automatic task for pre translate files
							if (applyTemplateForm.RunPreTranslateBatchTaskFlag)
							{
								var objPreTranslateAutomaticTask = targetProject.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.PreTranslateFiles);
								if (objPreTranslateAutomaticTask != null)
								{
									if (objPreTranslateAutomaticTask.Status == TaskStatus.Completed)
									{
										targetProject.Save();
									}
									_logger.Info("Method: {methodName}, Pre-Translate Automatic Task Status: {Status}",
									methodName, objPreTranslateAutomaticTask.Status);
								}
								_logger.Info("Method: {methodName}, Pre-Translate Automatic Task: {@objPreTranslateAutomaticTask}",
										methodName, objPreTranslateAutomaticTask);
								// Copy automation pre translate batch task
								if (selectedTemplate.PreTranslateBatchTask != ApplyTemplateOptions.Keep)
								{
									try
									{
										CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, "TranslateTaskSettings", targetProject);
									}
									catch (Exception e)
									{
										MessageBox.Show(e.Message, PluginResources.APTBT_Failed, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
								}
							}

						}


						var type = typeof(FileBasedProject);
						var fieldInfo = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
						if (fieldInfo is null)
						{
							Controller.RefreshProjects();
							applyTemplateForm.SaveProjectTemplates();
							Controller.RefreshProjects();
							MessageBox.Show(projectsList.ToString(), PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
							return;
						}

						var project = fieldInfo.GetValue(targetProject);
						var updateServerMethod = project.GetType().GetMethod("ExecuteOperation");
						//For GS projects
						updateServerMethod?.Invoke(targetProject, new object[] { "UpdateServerProjectSettingsOperation", new object[] { true } });
						//For LC projects
						updateServerMethod?.Invoke(targetProject, new object[] { "SynchronizeServerProjectDataOperation", new object[] { null } });
					}
					catch (Exception e)
					{
						Console.Write(e);
						_logger.Error("Method: {methodName}, Exception: {@ex}", methodName, e);
						throw;
					}
				}

				Controller.RefreshProjects();

				// Tell the user we're done
				MessageBox.Show(projectsList.ToString(), PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			// Save the project templates anyway
			applyTemplateForm.SaveProjectTemplates();
			Controller.RefreshProjects();
		}

		#region FuzzyBands 
		private void CopySettingsFuzzyBands(ISettingsBundle sourceSettingsBundle, ISettingsBundle targetSettingsBundle, string settingsGroupId, FileBasedProject sourceProject, FileBasedProject targetProject)
		{
			//Valentin -> code already prepared for the "long term" Studio API solution change, if the "Studio team" will add this AnalysisBands settings in the BundleSettings where it should be.
			//I hope that they will respect the naming convention and the section will be named "FuzzyBandsSettings"
			if (!CopySettingsGroup(sourceSettingsBundle, targetSettingsBundle, settingsGroupId, targetProject))
			{
				CopySettingsFuzzyBands(sourceProject, targetProject);
			}
		}


		private void CopySettingsFuzzyBands(FileBasedProject sourceProject, FileBasedProject targetProject)
		{

			var sourceAnalysisBandsAsIntsx = GetAnalysisBandsAsIntArray(GetProjectUsingReflection(sourceProject));
			var internalTargetProject = GetProjectUsingReflection(targetProject);
			if (internalTargetProject is null)
			{
				return;
			}

			var setAnalysisBandsMethod = internalTargetProject.GetType().GetMethod("SetAnalysisBands");
			//update the FuzzyBands
			setAnalysisBandsMethod?.Invoke(internalTargetProject, new object[] { sourceAnalysisBandsAsIntsx });
		}


		private int[] GetAnalysisBandsAsIntArray(dynamic internalDynamicProject)
		{
			var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);
			var arrayLength = internalDynamicProject?.AnalysisBands?.Length ?? 0;
			var analysisBandsMinsValues = new int[arrayLength];
			var analysisBands = internalDynamicProject?.AnalysisBands;
			if (analysisBands is null)
			{
				return analysisBandsMinsValues;
			}
			var i = 0;
			foreach (var analysisBand in analysisBands)
			{
				Match match = regex.Match(analysisBand.ToString());
				if (match.Success)
				{
					var min = match.Groups["min"].Value;
					int.TryParse(min, out analysisBandsMinsValues[i]);
					i++;
				}
			}

			return analysisBandsMinsValues;
		}


		private dynamic GetProjectUsingReflection(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			dynamic internalDynamicProject = internalProjectField?.GetValue(project);
			return internalDynamicProject;
		}


		//should be AnalysisBand from Sdl.ProjectApi.Implementation but the class is internal and this  it's not accessible here. Also the IAnalysisBand that it's the base of AnalysisBand it's in SDL.ProjectAPI which can be referenced as assembly because it's restricted be the Studio rules. 
		//To use IAnalysisBand we can't because the SDL.ProjectAPI is restricted to be referenced. It's marked as non public.
		//so basically can't handle the AnalysisBand type that is used in Project.
		private AnalysisBand[] GetAnalysisBandsValues(dynamic internalDynamicProject)
		{
			var analysisList = internalDynamicProject?.AnalysisBands;

			if (analysisList != null)
			{
				var result = new AnalysisBand[analysisList.Length];

				for (var i = 0; i < analysisList.Length - 1; i++)
				{
					result[i] = analysisList[i] as AnalysisBand;

				}
			}


			return null;
		}


		#endregion FuzzyBands 

		/// <summary>
		/// Validates the translation provider configuration.
		/// </summary>
		/// <param name="providerConfig">The provider configuration.</param>
		private void ValidateTranslationProviderConfiguration(TranslationProviderConfiguration providerConfig)
		{
			foreach (var entry in providerConfig.Entries)
			{
				if (entry.PerformUpdate && !entry.PerformNormalSearch)
				{
					entry.PerformNormalSearch = true;
				}
			}
		}

		/// <summary>
		/// Copies the settings group.
		/// </summary>
		/// <param name="sourceSettings">The source settings.</param>
		/// <param name="targetSettings">The target settings.</param>
		/// <param name="settingsGroupId">The settings group identifier.</param>
		/// <param name="targetProject">The target project.</param>
		/// <param name="targetLanguage">The target language.</param>
		private void CopySettingsGroup(ISettingsBundle sourceSettings, ISettingsBundle targetSettings, string settingsGroupId, FileBasedProject targetProject, Language targetLanguage)
		{
			if (!string.IsNullOrEmpty(settingsGroupId))
			{
				if (targetSettings.ContainsSettingsGroup(settingsGroupId))
				{
					targetSettings.RemoveSettingsGroup(settingsGroupId);
				}

				if (sourceSettings.ContainsSettingsGroup(settingsGroupId))
				{
					var sourceSettingsGroup = sourceSettings.GetSettingsGroup(settingsGroupId);
					var targetSettingsGroup = targetSettings.GetSettingsGroup(settingsGroupId);
					targetSettingsGroup.ImportSettings(sourceSettingsGroup);
					if (targetLanguage == null)
					{
						targetProject.UpdateSettings(targetSettings);
					}
					else
					{
						targetProject.UpdateSettings(targetLanguage, targetSettings);
					}
				}
			}
			else
			{
				targetSettings.GetSettingsGroup(settingsGroupId).Reset();
				if (targetLanguage == null)
				{
					targetProject.UpdateSettings(targetSettings);
				}
				else
				{
					targetProject.UpdateSettings(targetLanguage, targetSettings);
				}
			}
		}

		private bool CopySettingsGroup(ISettingsBundle sourceSettings, ISettingsBundle targetSettings,
			string settingsGroupId, FileBasedProject targetProject)
		{
			if (string.IsNullOrEmpty(settingsGroupId))
				return false;

			if (!sourceSettings.ContainsSettingsGroup(settingsGroupId))
				return false;

			if (targetSettings.ContainsSettingsGroup(settingsGroupId))
			{
				targetSettings.RemoveSettingsGroup(settingsGroupId);
			}
			var sourceSettingsGroup = sourceSettings.GetSettingsGroup(settingsGroupId);
			var targetSettingsGroup = targetSettings.GetSettingsGroup(settingsGroupId);
			targetSettingsGroup.ImportSettings(sourceSettingsGroup);
			targetProject.UpdateSettings(targetSettings);
			return true;
		}

		/// <summary>
		/// Merges the translation providers.
		/// </summary>
		/// <param name="sourceProviderConfig">The source provider configuration.</param>
		/// <param name="targetProviderConfig">The target provider configuration.</param>
		private void MergeTranslationProviders(TranslationProviderConfiguration sourceProviderConfig, TranslationProviderConfiguration targetProviderConfig)
		{
			// Remember where we're going to insert the translation providers
			var indexToInsert = 0;

			// Look at each translation provider in the source project
			foreach (var sourceCascadeEntry in sourceProviderConfig.Entries)
			{
				// See if we can match the translation provider to one already there
				var foundEntry = targetProviderConfig.Entries.Any(targetCascadeEntry => sourceCascadeEntry.MainTranslationProvider.Uri == targetCascadeEntry.MainTranslationProvider.Uri);

				// If we didn't find the translation provider then add a new one to the target project
				if (!foundEntry)
				{
					targetProviderConfig.Entries.Insert(indexToInsert++, sourceCascadeEntry);
				}
			}
		}

		/// <summary>
		/// Merges the terminology databases.
		/// </summary>
		/// <param name="sourceTermbaseConfig">The source terminology database configuration.</param>
		/// <param name="targetTermbaseConfig">The target terminology database configuration.</param>
		private void MergeTermbases(TermbaseConfiguration sourceTermbaseConfig, TermbaseConfiguration targetTermbaseConfig)
		{
			// Look at each termbase in the source project
			foreach (var sourceTermbase in sourceTermbaseConfig.Termbases)
			{
				// Look for a matching termbase in the target project
				var foundEntry = false;
				foreach (var targetTermbase in targetTermbaseConfig.Termbases)
				{
					// Detect local or server termbases
					if (sourceTermbase is LocalTermbase)
					{
						// Skip if the file doesn't exist
						if (!File.Exists((sourceTermbase as LocalTermbase).FilePath))
						{
							foundEntry = true;
							break;
						}

						// Must be the same type to match
						if (targetTermbase is LocalTermbase)
						{
							// Match based on file path
							if ((sourceTermbase as LocalTermbase).FilePath.Equals((targetTermbase as LocalTermbase).FilePath, StringComparison.InvariantCultureIgnoreCase))
							{
								foundEntry = true;
								break;
							}
						}
					}
					else
					{
						// If we're dealing with server termbase then we need the same server!
						if (sourceTermbaseConfig.TermbaseServerUri == targetTermbaseConfig.TermbaseServerUri)
						{
							// Must be the same type to match
							if (targetTermbase is ServerTermbase)
							{
								// Match based on name
								var serverTermbase = sourceTermbase as ServerTermbase;
								if (serverTermbase != null && serverTermbase.Name.Equals((targetTermbase as ServerTermbase).Name, StringComparison.InvariantCultureIgnoreCase))
								{
									foundEntry = true;
									break;
								}
							}
						}
						else
						{
							// We can't add this one so "pretend" we found it
							foundEntry = true;
						}
					}
				}

				// If we didn't find the current termbase then add it to the target project
				if (!foundEntry)
				{
					targetTermbaseConfig.Termbases.Add(sourceTermbase);
				}
			}

			// Merge the language indexes
			foreach (var sourceLanguageIndex in sourceTermbaseConfig.LanguageIndexes)
			{
				var foundIndex = targetTermbaseConfig.LanguageIndexes.Any(targetLanguageIndex => sourceLanguageIndex.ProjectLanguage.Equals(targetLanguageIndex.ProjectLanguage));

				// If we didn't find an entry for this language then add it to the target project
				if (!foundIndex)
				{
					targetTermbaseConfig.LanguageIndexes.Add(sourceLanguageIndex);
				}
			}
		}
	}

	//[Action("ApplyStudioProjectTemplateHelpAction", Icon = "question", Name = "Apply Studio Project Template Help", Description = "An wiki page will be opened in browser uith user documentation")]
	//[ActionLayout(typeof(ApplyStudioProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	//[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
	//public class ApplyStudioProjectTemplateHelpAction : AbstractViewControllerAction<ProjectsController>
	//{
	//	protected override void Execute()
	//	{
	//		System.Diagnostics.Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3157/apply-studio-project-template");
	//	}
	//}
}