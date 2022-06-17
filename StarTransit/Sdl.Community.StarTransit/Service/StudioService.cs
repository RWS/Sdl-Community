using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.StarTransit.Service
{
	public class StudioService: IStudioService
	{
		private readonly ProjectsController _projectsController;
		private const string ProjectSettingsGroupId = "ProjectSettings";
		private const string GeneralProjectInfoSettingsGroupId = "GeneralProjectInfoSettings";
		private const string ProjectTemplateSettingsGroupId = "ProjectTemplateSettings";
		private const string DueDateGroupId = "DueDate";
		private const string CustomerId = "CustomerId";
		private const string ProjectOriginId = "ProjectOrigin";
		private const string ProjectLocationId = "ProjectLocation";

		public StudioService(ProjectsController projectsController)
		{
			_projectsController = projectsController;
		}

		public List<ProjectTemplateInfo> GetProjectTemplates()
		{
			var templateList = _projectsController?.GetProjectTemplates()?.OrderBy(t => t.Name).ToList();
			return templateList ?? new List<ProjectTemplateInfo>();
		}

		public Task<List<Customer>> GetCustomers(string projectsXmlFilePath)
		{
			var customersList = new List<Customer>();
			return Task.Run(() =>
			{
				try
				{
					var projectsFile = new XmlDocument();
					projectsFile.Load(projectsXmlFilePath);
					var customers = projectsFile.GetElementsByTagName("Customers");
					foreach (XmlNode custom in customers)
					{
						foreach (XmlNode child in custom.ChildNodes)
						{
							var name = child.Attributes?["Name"]?.Value;
							var guid = child.Attributes?["Guid"]?.Value;
							var email = child.Attributes?["Email"]?.Value;
							if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(guid)) continue;
							var customer = new Customer {Name = name, Guid = new Guid(guid), Email = email};
							customersList.Add(customer);
						}
					}
				}
				catch (Exception ex)
				{
					//_logger.Error($"{ex.Message}\n {ex.StackTrace}");
				}

				customersList.Insert(0, new Customer()); // used to clear the selection
				
				return customersList;
			});
		}

		public Task<PackageModel> GetModelBasedOnStudioTemplate(string templatePath, CultureInfo sourceCultureInfo,
			Language[] targetLanguages)
		{
			return Task.Run(() =>
			{
				if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath)) return null;

				var projectTemplateDocument = new XmlDocument();
				projectTemplateDocument.Load(templatePath);
				var projectTemplate = projectTemplateDocument.SelectSingleNode("/ProjectTemplate");
				var settingsBundleGuid = string.Empty;

				if (projectTemplate?.Attributes == null) return null;
				foreach (XmlAttribute attribute in projectTemplate.Attributes)
				{
					if (!attribute.Name.Equals("SettingsBundleGuid")) continue;
					settingsBundleGuid = attribute.Value;
					break;
				}

				if (string.IsNullOrEmpty(settingsBundleGuid)) return null;

				var templateOptions = GetTemplateOptions(projectTemplateDocument, settingsBundleGuid, sourceCultureInfo,
					targetLanguages);

				if (templateOptions is null) return null;

				var packageModel = new PackageModel
				{
					Location = templateOptions.ProjectLocation,
					DueDate = templateOptions.DueDate,
					LanguagePairs = templateOptions.TemplateLanguagePairDetails
				};

				if (!string.IsNullOrEmpty(templateOptions.CustomerId))
				{
					packageModel.Customer = new Customer {Name = templateOptions.CustomerId};
				}

				return packageModel;

			});
		}

		public (bool, Language) IsTmCreatedFromPlugin(string tmName, CultureInfo sourceCultureInfo,
			Language[] targetLanguages)
		{
			if (sourceCultureInfo is null || targetLanguages is null) return (false, null);
			foreach (var targetLanguage in targetLanguages)
			{
				if (tmName.Contains(
					$"{sourceCultureInfo.TwoLetterISOLanguageName}-{targetLanguage.CultureInfo.TwoLetterISOLanguageName}")
				)
				{
					return (true, targetLanguage);
				}
			}
			return (false, null);
		}

		private TemplateOptions GetTemplateOptions(XmlDocument projectTemplateDocument, string settingsBundleGuid,
			CultureInfo sourceCultureInfo, Language[] targetLanguages)
		{
			var projectOrigin = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
				ProjectSettingsGroupId, ProjectOriginId);
			if (string.IsNullOrEmpty(projectOrigin)) return null;

			var templateOptions = new TemplateOptions {TemplateLanguagePairDetails = new List<LanguagePair>()};
			var projectLocation = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
				ProjectTemplateSettingsGroupId, ProjectLocationId);

			var customerId = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
				GeneralProjectInfoSettingsGroupId, CustomerId);

			var dueDate = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
				GeneralProjectInfoSettingsGroupId, DueDateGroupId);
			if (DateTime.TryParse(dueDate, out var selectedDueDate) && selectedDueDate != DateTime.MaxValue)
			{
				templateOptions.DueDate = selectedDueDate;
			}

			var tmTemplateDetails = TmDetailsFromCascadeEntry(projectTemplateDocument, sourceCultureInfo, targetLanguages);
			if (tmTemplateDetails != null)
			{
				foreach (var tmDetails in tmTemplateDetails)
				{
					templateOptions.TemplateLanguagePairDetails.Add(tmDetails.TransitLanguagePairOptions);
				}
			}

			templateOptions.ProjectLocation = projectLocation;
			templateOptions.CustomerId = customerId;
			return templateOptions;
		}

		private string GetSettingsByGroupId(XmlDocument projectTemplateDocument, string settingsBundleGuid,string groupId,string settingId)
		{
			var settingsNode= projectTemplateDocument.SelectSingleNode(
					$"/ProjectTemplate/SettingsBundles/SettingsBundle[@Guid='{settingsBundleGuid}']/SettingsBundle/SettingsGroup[@Id='{groupId}']/Setting[@Id='{settingId}']");
			
			return settingsNode?.InnerText;
		}

		private List<TemplateTmDetails> TmDetailsFromCascadeEntry(XmlDocument projectTemplateDocument, CultureInfo sourceCultureInfo, Language[] targetLanguages)
		{
			if (sourceCultureInfo is null || targetLanguages is null) return null;
			var cascadeEntries = projectTemplateDocument.SelectNodes("/ProjectTemplate/CascadeItem/CascadeEntryItem");
			if (cascadeEntries is null) return null;

			var tmDetails = new List<TemplateTmDetails>();
			foreach (XmlNode cascadeEntryItem in cascadeEntries)
			{
				var details = new TemplateTmDetails();
				if (cascadeEntryItem.Attributes is null) continue;
				foreach (XmlAttribute entryAttribute in cascadeEntryItem.Attributes)
				{
					if (!entryAttribute.Name.Equals("Penalty")) continue;
					int.TryParse(entryAttribute.Value, out var penaltySet);
					details.Penalty = penaltySet;

					foreach (XmlNode mainTranslationProviderItem in cascadeEntryItem.ChildNodes)
					{
						if (mainTranslationProviderItem.Attributes is null) continue;
						foreach (XmlAttribute providerAttribute in mainTranslationProviderItem.Attributes)
						{
							if (!providerAttribute.Name.Equals("Uri")) continue;
							var uri = new Uri(providerAttribute.Value);
							details.LocalPath = FileBasedTranslationMemory.GetFileBasedTranslationMemoryFilePath(uri);
							details.Name = Path.GetFileNameWithoutExtension(details.LocalPath);
							details.TransitLanguagePairOptions = GetLanguagePairTmOptions(details, sourceCultureInfo, targetLanguages);
						}
					}
				}

				if (details.TransitLanguagePairOptions != null)
				{
					tmDetails.Add(details);
				}
			}
			return tmDetails;
		}

		/// <summary>
		/// Create language pairs options based on the TM types found in template. This options will be later used in the UI to enable
		/// following options from Penalty Page: "No tm", "Create new tm", "Choose existing tm"
		/// </summary>
		/// <param name="tmDetails">Path and name for tm found in Studio template</param>
		/// <param name="sourceCultureInfo">Package source language</param>
		/// <param name="targetLanguages">Package target languages</param>
		/// <returns>true and target language supported</returns>
		private LanguagePair GetLanguagePairTmOptions(TemplateTmDetails tmDetails, CultureInfo sourceCultureInfo,
			Language[] targetLanguages)
		{
			var (isCreatedFromPlugin, language) =
				IsTmCreatedFromPlugin(tmDetails.Name, sourceCultureInfo, targetLanguages);
			tmDetails.IsCreatedFromPlugin = isCreatedFromPlugin;
			tmDetails.TargetLanguage = language;

			if (isCreatedFromPlugin)
			{
				return new LanguagePair
				{
					SourceLanguage = sourceCultureInfo,
					TargetLanguage = new CultureInfo(language.CultureInfo.Name),
					CreateNewTm = true,
					TemplatePenalty = tmDetails.Penalty
				};
			}

			var (tmCorrespondsToLanguagePair, targetLanguage) =
				TmSupportsAnyLanguageDirection(new Uri(tmDetails.LocalPath), sourceCultureInfo, targetLanguages);
			if (tmCorrespondsToLanguagePair)
			{
				return new LanguagePair
				{
					SourceLanguage = sourceCultureInfo,
					TargetLanguage = new CultureInfo(targetLanguage.CultureInfo.Name),
					ChoseExistingTm = true,
					TmPath = tmDetails.LocalPath,
					TmName = tmDetails.Name
				};
			}
			return null;
		}

		/// <summary>
		/// Checks if TM found in the template supports any language from Transit Packages
		/// </summary>
		/// <param name="tmLocalPath">Tm local path read from project template</param>
		/// <param name="sourceCultureInfo">Package source language</param>
		/// <param name="targetLanguages">Package target languages</param>
		/// <returns>true and target language supported</returns>
		/// <returns>false if the TM target language is not supported by Transit Package</returns>
		public (bool, Language) TmSupportsAnyLanguageDirection(Uri tmLocalPath, CultureInfo sourceCultureInfo,
			Language[] targetLanguages)
		{
			if (sourceCultureInfo is null || targetLanguages is null) return (false, null);

			var tm = new FileBasedTranslationMemory(tmLocalPath);
			var tmSupportedLanguages = tm.SupportedLanguageDirections;

			foreach (var supportedLanguageDirection in tmSupportedLanguages)
			{
				var isTmSourceLanguageSupported =
					sourceCultureInfo.Name.Equals(supportedLanguageDirection.SourceCultureName);
				if (!isTmSourceLanguageSupported) return (false, null);

				var correspondingTarget = targetLanguages.FirstOrDefault(t =>
					t.CultureInfo.Name.Equals(supportedLanguageDirection.TargetCultureName));
				if (correspondingTarget != null)
				{
					return (true, correspondingTarget);
				}
			}
			return (false, null);
		}
	}
}
