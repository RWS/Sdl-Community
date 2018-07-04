using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Sdl.Community.StudioMigrationUtility.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioMigrationUtility.Services
{
    public class MigrateProjectsService 
    {
        private readonly StudioVersion _sourceStudioVersion;
        private readonly StudioVersion _destinationStudioVersion;
         
        public MigrateProjectsService(StudioVersion sourceVersion,StudioVersion destinationVersion)
        {
            _sourceStudioVersion = sourceVersion;
            _destinationStudioVersion = destinationVersion;
        }

        public ProjectsController GetProjectsController()
        {
            return SdlTradosStudio.Application.GetController<ProjectsController>();
        }

        public List<Project> GetProjectsToBeMigrated()
        {
            var projects = new List<Project>();
            var sourceProjectsPath = GetProjectsPath(_sourceStudioVersion);
            var destinationProjectsPath = GetProjectsPath(_destinationStudioVersion);

            var projectsXml = XElement.Load(sourceProjectsPath);
            var projectTagItem = XName.Get("ProjectListItem");
            var projectInfoTagItem = XName.Get("ProjectInfo");

            var projectCustomerItem = XName.Get("Customer");

            foreach (var projectTagItems in projectsXml.Descendants(projectTagItem))
            {
	            var guidAttribute = projectTagItems.Attribute("Guid");
	            if (guidAttribute != null)
	            {
		            var filePathAttribute = projectTagItems.Attribute("ProjectFilePath");
		            if (filePathAttribute != null)
		            {
			            var project = new Project
			            {
				            Guid = new Guid(guidAttribute.Value),
				            ProjectFilePath = filePathAttribute.Value
			            };
			            foreach (var projectInfoTagItems in projectTagItems.Descendants(projectInfoTagItem))
			            {
                    
				            if (projectInfoTagItems.Attribute("StartedAt") != null)
				            {
					            var startedAtAttribute = projectInfoTagItems.Attribute("StartedAt");
					            if (startedAtAttribute != null)
					            {
									project.StartedAt = Convert.ToDateTime(startedAtAttribute.Value,
										DateTimeFormatInfo.InvariantInfo);
								}
				            }
				            else
				            {
					            project.StartedAt = DateTime.MinValue;
				            }
				            project.IsInPlace = Convert.ToBoolean(projectInfoTagItems.Attribute("IsInPlace").Value);
				            project.IsImported = Convert.ToBoolean(projectInfoTagItems.Attribute("IsImported").Value);
				            project.CreatedBy = projectInfoTagItems.Attribute("CreatedBy").Value;
				            if (projectInfoTagItems.Attribute("Description") != null)
				            {
					            project.Description = projectInfoTagItems.Attribute("Description").Value;
				            }
				            project.CreatedAt = Convert.ToDateTime(projectInfoTagItems.Attribute("CreatedAt").Value);
				            project.Name = projectInfoTagItems.Attribute("Name").Value;
				            project.Status = projectInfoTagItems.Attribute("Status").Value;

                    
				            //Modification ID: PH_2015-07-05T21:12:00
				            //Date: 2015-07-05
				            //Added by: Patrick Hartnett
				            //Begin Edit (PH_2015-07-05T21:12:00)
				            if (projectInfoTagItems.HasElements)
				            {
					            var customerElement = projectInfoTagItems.Element(projectCustomerItem);
					            if (customerElement != null)
					            {
						            var customerGuid = new Guid(customerElement.Attribute("Guid").Value);
						            var customerName = customerElement.Attribute("Name").Value;
						            var customerEmail = customerElement.Attribute("Email").Value;
						            project.Customer = new Customer(customerGuid, customerName, customerEmail);
					            }
				            }
				            //End Edit (PH_2015-07-05T21:12:00)

			            }
			            projects.Add(project);
		            }
	            }
            }

            var destinationProjectsXml = XElement.Load(destinationProjectsPath);
            var destinationProjects = destinationProjectsXml.Descendants(projectTagItem).Select(projectTagItems => new Project
            {
                Guid = new Guid(projectTagItems.Attribute("Guid").Value), ProjectFilePath = projectTagItems.Attribute("ProjectFilePath").Value
            }).ToList();


            return projects.Where(x=>destinationProjects.All(y => y.Guid != x.Guid)).ToList();
            
        }

        public string GetProjectsPath(StudioVersion studioVersion)
        {
             var myDocumnetsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return Path.Combine(myDocumnetsPath, string.Format("{0}\\Projects\\projects.xml", studioVersion.PublicVersion));
        }

        public string GetTranslationMemoryPath(StudioVersion studioVersion)
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderName = string.Format("{0}.0.0.0", studioVersion.ExecutableVersion.Major);

            return Path.Combine(appDataPath, string.Format("SDL\\SDL Trados Studio\\{0}\\TranslationMemoryRepository.xml", folderName));
        }

        
        public void MigrateProjects(List<Project> projects, List<Project> projectsToBeMoved, bool migrateCustomers, Action<int> reportProgress)
        {
            var destinationProjectPath = GetProjectsPath(_destinationStudioVersion);
            var projectsXml = XElement.Load(destinationProjectPath);

            if (migrateCustomers)
            {
                //if there customers associated with projects we need to make sure that we also migrate the customers
                //otherwise they will not appear
                MigrateCustomers(projectsXml);

            }
            MoveProjects(projectsToBeMoved,reportProgress);

            var projectsElement = projectsXml.Element("Projects");
            if (projectsElement == null)
            {
                var workFlowElement = projectsXml.Element("Workflow");
                
                projectsElement = new XElement("Projects");
	            workFlowElement?.AddAfterSelf(projectsElement);
            }


            foreach (var project in projectsToBeMoved)
            {
                var workingProject = project;
                var projectMoved = projectsToBeMoved.Find(x => x.Guid.Equals(project.Guid));
                if (projectMoved != null)
                {
                    workingProject = projectMoved;
                }
                var projectInfoItem = new XElement("ProjectInfo");

                if (workingProject.StartedAt != DateTime.MinValue)
                {
                    projectInfoItem.Add(new XAttribute("StartedAt", workingProject.StartedAt));
                }

                projectInfoItem.Add(new XAttribute("IsInPlace", workingProject.IsInPlace),
                    new XAttribute("IsImported", workingProject.IsImported),
                    new XAttribute("CreatedBy", workingProject.CreatedBy),
                    new XAttribute("CreatedAt", workingProject.CreatedAt),
                    new XAttribute("Name", workingProject.Name),
                    new XAttribute("Status", workingProject.Status));

                if (!string.IsNullOrEmpty(workingProject.Description))
                {
                    projectInfoItem.Add(new XAttribute("Description", workingProject.Description));
                }

                //Modification ID: PH_2015-07-05T21:12:00
                //Date: 2015-07-05
                //Added by: Patrick Hartnett
                //Begin Edit (PH_2015-07-05T31:12:00)
                if (project.Customer != null && migrateCustomers)
                {
                    var customertem = new XElement("Customer");

                    customertem.Add(new XAttribute("Guid", project.Customer.Guid));
                    customertem.Add(new XAttribute("Name", project.Customer.Name));
                    customertem.Add(new XAttribute("Email", project.Customer.Email));

                    projectInfoItem.Add(customertem);
                }
                //End Edit (PH_2015-07-05T31:12:00)

                var projectItem = new XElement("ProjectListItem", new XAttribute("Guid", workingProject.Guid),
                    new XAttribute("ProjectFilePath", workingProject.ProjectFilePath), projectInfoItem);
                projectsElement.Add(projectItem);
            }
         
            projectsXml.Save(destinationProjectPath);
            reportProgress(95);
        }

        
        private void MigrateCustomers(XElement destinationProjectsXml)
        {
            var sourceProjectsPath = GetProjectsPath(_sourceStudioVersion);
            var sourceProjectsXml = XElement.Load(sourceProjectsPath);

	        var xElement = sourceProjectsXml.Element("Customers");
	        if (xElement != null && !xElement.HasElements) return;

	        var element = destinationProjectsXml.Element("Customers");
	        foreach (var sourceDescendant in from sourceDescendant in sourceProjectsXml.Descendants("Customer")
                let sourcePath = sourceDescendant.Attribute("Guid")
                where
                    element != null && element
	                    .Descendants("Customer")
	                    .All(x => x.Attribute("Guid").Value != sourcePath.Value)
                select sourceDescendant)
            {
                //destinationProjectsXml.Element("Customers").Add(sourceDescendant);
                var xAttribute = sourceDescendant.Attribute("Name");
                var name = string.Empty;
                var email = string.Empty;
                if (xAttribute != null)
                {
                    name = xAttribute.Value;
                }
                var attribute = sourceDescendant.Attribute("Email");
                if (attribute != null)
                {
                    email = attribute.Value;
                }
                AddCustomers(name, email);

            }
        }

        private void AddCustomers(string name, string email)
        {
            var currentProject = GetProjectsController().CurrentProject;
            var type = currentProject.GetType();

            var internalProjectField = type.GetField("_project", BindingFlags.Instance | BindingFlags.NonPublic);
	        if (internalProjectField != null)
	        {
		        dynamic internalDynamicaProject = internalProjectField.GetValue(currentProject);
		        dynamic customersList = internalDynamicaProject.ProjectServer.Customers;
		        var existCustomer = false;
		        foreach (var customer in customersList)
		        {
			        if (customer.Name == name)
			        {
				        existCustomer = true;
			        }
		        }
		        if (existCustomer==false)
		        {
			        internalDynamicaProject.ProjectServer.AddCustomer(name, email);
		        }
	        }
        }

        private void MoveProjects(IEnumerable<Project> projectToBeMoved, Action<int> reportProgress)
        {
            var destinationProjectsPath = Path.GetDirectoryName(GetProjectsPath(_destinationStudioVersion));
            var sourceProjectsPath = Path.GetDirectoryName(GetProjectsPath(_sourceStudioVersion));
            var chunkSize = 90/(!projectToBeMoved.Any() ? 1 : projectToBeMoved.Count());
            var progress = 0;
            foreach (var project in projectToBeMoved)
            {
                var destinationProjectPath = Path.Combine(destinationProjectsPath, project.Name);
                var projectFilePath = project.ProjectFilePath;
                if (!Path.IsPathRooted(projectFilePath))
                {
                    projectFilePath = Path.Combine(sourceProjectsPath, project.ProjectFilePath);
                }
                DirectoryCopy(Path.GetDirectoryName(projectFilePath), destinationProjectPath);
                var fileName = Path.GetFileName(project.ProjectFilePath);
                project.ProjectFilePath = Path.Combine(destinationProjectPath, fileName);
                progress += chunkSize;
                reportProgress(progress);
            }
        }

        private void DirectoryCopy(string sourceDirectory, string destinationDirectory)
        {

            if (Directory.Exists(destinationDirectory))
            {
                return;
            }

            Directory.CreateDirectory(string.Format(destinationDirectory));

            var dir = new DirectoryInfo(sourceDirectory);
            var dirs = dir.GetDirectories();

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destinationDirectory, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        internal void MigrateTranslationMemories()
        {
            var sourceVersionTmPath = GetTranslationMemoryPath(_sourceStudioVersion);
            var destinationVersionTmPath = GetTranslationMemoryPath(_destinationStudioVersion);

            var sourceTmXml = XElement.Load(sourceVersionTmPath);
            var destinationTmXml = XElement.Load(destinationVersionTmPath);

            foreach (var sourceDescendant in sourceTmXml.Descendants("TranslationMemory"))
            {
	            var pathAttribute = sourceDescendant.Attribute("path");
	            if (pathAttribute != null)
	            {
		            var sourcePath = pathAttribute.Value;
		            if (destinationTmXml.Descendants("TranslationMemory").All(x => x.Attribute("path").Value != sourcePath))
		            {
			            var tmElement = destinationTmXml.Element("TranslationMemories");
			            tmElement?.Add(sourceDescendant);
		            }
	            }
            }

            foreach (var sourceDescendant in sourceTmXml.Descendants("LanguageResources"))
            {
	            var pathAttribute = sourceDescendant.Attribute("path");
	            if (pathAttribute != null)
	            {
		            var sourcePath = pathAttribute.Value;
		            if (destinationTmXml.Descendants("LanguageResources").All(x => x.Attribute("path").Value != sourcePath))
		            {
			            var languagteResourcesElement = destinationTmXml.Element("LanguageResourceGroups");
			            languagteResourcesElement?.Add(sourceDescendant);
		            }
	            }
            }

            destinationTmXml.Save(destinationVersionTmPath);
        }

        public string TryGetElementValue(XElement parentEl, string elementName, string defaultValue = null)
        {
            var foundEl = parentEl.Element(elementName);

            if (foundEl != null)
            {
                return foundEl.Value;
            }

            return defaultValue;
        }

       
    }
}
