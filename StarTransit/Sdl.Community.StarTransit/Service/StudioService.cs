using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Shared.Models;
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

		public PackageModel GetModelBasedOnStudioTemplate(string templatePath)
		{
			if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath)) return null;

			var projectTemplateDocument = new XmlDocument();
			projectTemplateDocument.Load(templatePath);
			var projectTemplate = projectTemplateDocument.SelectSingleNode("/ProjectTemplate");
			var settingsBundleGuid = string.Empty;

			if (projectTemplate?.Attributes != null)
			{
				foreach (XmlAttribute attribute in projectTemplate.Attributes)
				{
					if (!attribute.Name.Equals("SettingsBundleGuid")) continue;
					settingsBundleGuid = attribute.Value;
					break;
				}

				var projectOrigin = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
					ProjectSettingsGroupId, ProjectOriginId);
				if (string.IsNullOrEmpty(projectOrigin)) return null;

				var projectLocation = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
					ProjectTemplateSettingsGroupId, ProjectLocationId);

				var customerId = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
					GeneralProjectInfoSettingsGroupId, CustomerId);

				var dueDate = GetSettingsByGroupId(projectTemplateDocument, settingsBundleGuid,
					GeneralProjectInfoSettingsGroupId, DueDateGroupId);

				var packageModel = new PackageModel
				{
					Location = projectLocation, Customer = new Customer {Name = customerId}
				};

				if (DateTime.TryParse(dueDate, out var selectedDueDate))
				{
					packageModel.DueDate = selectedDueDate;
				}

				return packageModel;
			}

			return null;
		}

		private string GetSettingsByGroupId(XmlDocument projectTemplateDocument, string settingsBundleGuid,string groupId,string settingId)
		{
			var  settingsNode= projectTemplateDocument.SelectSingleNode(
					$"/ProjectTemplate/SettingsBundles/SettingsBundle[@Guid='{settingsBundleGuid}']/SettingsBundle/SettingsGroup[@Id='{groupId}']/Setting[@Id='{settingId}']");
			
			return settingsNode?.InnerText;
		}

		public void RefreshProjects()
		{
			_projectsController.RefreshProjects();
		}
	}
}
