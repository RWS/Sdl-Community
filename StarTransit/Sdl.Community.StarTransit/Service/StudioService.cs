using System;
using System.CodeDom;
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
using Sdl.Versioning;
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

		//TODO: Refactor
		public Task<List<Customer>> GetCustomers()
		{
			var customersList = new List<Customer>();
			return Task.Run(() =>
			{
				try
				{
					var studioVersionService = new StudioVersionService();
					var shortStudioVersion = studioVersionService.GetStudioVersion()?.ShortVersion;
					if (string.IsNullOrEmpty(shortStudioVersion)) return customersList;
					var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
						$@"Studio {shortStudioVersion}\Projects\projects.xml");

					var projectsFile = new XmlDocument();
					projectsFile.Load(projectsPath);
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

		public PackageModel GetModelBasedOnStudioTemplate(string templatePath,CultureInfo sourceCultureInfo, Language[] targetLanguages)
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

			var templateOptions = GetTemplateOptions(projectTemplateDocument, settingsBundleGuid,sourceCultureInfo,targetLanguages);

			if (templateOptions is null) return null;

			var packageModel = new PackageModel
			{
				Location = templateOptions.ProjectLocation,
				DueDate = templateOptions.DueDate
			};

			if (!string.IsNullOrEmpty(templateOptions.CustomerId))
			{
				packageModel.Customer = new Customer {Name = templateOptions.CustomerId};
			}

			return packageModel;
		}

		public (bool, Language) IsTmCreatedFromPlugin(string tmName, CultureInfo sourceCultureInfo,
			Language[] targetLanguages)
		{
			foreach (var targetLanguage in targetLanguages)
			{
				return tmName.Contains(
					$"{sourceCultureInfo.TwoLetterISOLanguageName}-{targetLanguage.CultureInfo.TwoLetterISOLanguageName}") ? (true, targetLanguage) : (false, null);
			}

			return (false, null);
		}

		private TemplateOptions GetTemplateOptions(XmlDocument projectTemplateDocument, string settingsBundleGuid, CultureInfo sourceCultureInfo, Language[] targetLanguages)
		{
			var projectOrigin = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
				ProjectSettingsGroupId, ProjectOriginId);
			if (string.IsNullOrEmpty(projectOrigin)) return null;

			var templateOptions = new TemplateOptions();
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

			var entryItems = GetCascadeEntryItems(projectTemplateDocument,sourceCultureInfo,targetLanguages);
			var languagePairsOptions = GetLanguagePairTmOptions(entryItems, sourceCultureInfo, targetLanguages);
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

		private List<TemplateTmDetails> GetCascadeEntryItems(XmlDocument projectTemplateDocument, CultureInfo sourceCultureInfo, Language[] targetLanguages)
		{
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
							details.Name = FileBasedTranslationMemory.GetFileBasedTranslationMemoryName(uri);

							var isCreatedFromPlugin =
								IsTmCreatedFromPlugin(details.Name, sourceCultureInfo, targetLanguages);
							details.IsCreatedFromPlugin = isCreatedFromPlugin.Item1;
							details.TargetLanguage = isCreatedFromPlugin.Item2;
						}
					}
				}

				tmDetails.Add(details);
			}
			return tmDetails;
		}

		private List<LanguagePair> GetLanguagePairTmOptions(List<TemplateTmDetails> entryItems, CultureInfo sourceCultureInfo, Language[] targetLanguages)
		{
			var langPairOptions = new List<LanguagePair>();
			//var groupedEntries
			return langPairOptions;
		}

		public bool GetTranslationMemoryLanguage(string uri)
		{
			//var tmLocalPath = new Uri(uri);
			//var filePath = FileBasedTranslationMemory.GetFileBasedTranslationMemoryFilePath(tmLocalPath);
			//var scheme = FileBasedTranslationMemory.GetFileBasedTranslationMemoryScheme();
			//var tm =  new FileBasedTranslationMemory(tmLocalPath);.
			return true;
		}

		public void RefreshProjects()
		{
			_projectsController.RefreshProjects();
		}
	}
}
