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

		public void RefreshProjects()
		{
			_projectsController.RefreshProjects();
		}
	}
}
