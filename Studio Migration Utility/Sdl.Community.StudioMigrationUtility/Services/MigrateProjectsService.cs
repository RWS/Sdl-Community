using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using Sdl.Community.StudioMigrationUtility.Model;

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
                var project = new Project
                {
                    Guid = new Guid(projectTagItems.Attribute("Guid").Value),
                    ProjectFilePath = projectTagItems.Attribute("ProjectFilePath").Value
                };
                foreach (var projectInfoTagItems in projectTagItems.Descendants(projectInfoTagItem))
                {
                    
                    if (projectInfoTagItems.Descendants(projectCustomerItem) != null)
                    {
                        var CustomerElement = projectInfoTagItems.Descendants(projectCustomerItem);
                    }

                    if (projectInfoTagItems.Attribute("StartedAt") != null)
                    {
                        project.StartedAt = Convert.ToDateTime(projectInfoTagItems.Attribute("StartedAt").Value,
                            DateTimeFormatInfo.InvariantInfo);
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
                            Guid c_Guid = new Guid(customerElement.Attribute("Guid").Value);
                            string c_Name = customerElement.Attribute("Name").Value;
                            string c_Email = customerElement.Attribute("Email").Value;
                            project.Customer = new Customer(c_Guid, c_Name, c_Email);
                        }
                    }
                    //End Edit (PH_2015-07-05T21:12:00)

                }
                projects.Add(project);
            }

            var destinationProjectsXml = XElement.Load(destinationProjectsPath);
            var destinationProjects = destinationProjectsXml.Descendants(projectTagItem).Select(projectTagItems => new Project
            {
                Guid = new Guid(projectTagItems.Attribute("Guid").Value), ProjectFilePath = projectTagItems.Attribute("ProjectFilePath").Value
            }).ToList();


            return projects.Where(x=>destinationProjects.All(y => y.Guid != x.Guid)).ToList();
            
        }

        public String GetProjectsPath(StudioVersion studioVersion)
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

        public void MigrateProjects(List<Project> projects, List<Project> projectToBeMoved, Action<int> reportProgress)
        {
            var destinationProjectPath = GetProjectsPath(_destinationStudioVersion);

            MoveProjects(projectToBeMoved,reportProgress);

            var projectsXml = XElement.Load(destinationProjectPath);
            var projectsElement = projectsXml.Element("Projects");
            if (projectsElement == null)
            {
                var workFlowElement = projectsXml.Element("Workflow");
                
                projectsElement = new XElement("Projects");
                workFlowElement.AddAfterSelf(projectsElement);
            }


            foreach (var project in projects)
            {
                var workingProject = project;
                var projectMoved = projectToBeMoved.Find(x => x.Guid.Equals(project.Guid));
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
                if (project.Customer != null)
                {
                    var customertem = new XElement("Customer");

                    customertem.Add(new XAttribute("Guid", project.Customer.Guid));
                    customertem.Add(new XAttribute("Name", project.Customer.Name));
                    customertem.Add(new XAttribute("Email", project.Customer.Email));

                    projectInfoItem.Add(customertem);
                }
                //End Edit (PH_2015-07-05T31:12:00)

                var projectItem = new XElement("ProjectListItem", new XAttribute("Guid", workingProject.Guid),
                    new XAttribute("ProjectFilePath", workingProject.ProjectFilePath), projectInfoItem
                    );
                projectsElement.Add(projectItem);
            }

            projectsXml.Save(destinationProjectPath);

            reportProgress(95);
        }

        private void MoveProjects(IEnumerable<Project> projectToBeMoved, Action<int> reportProgress)
        {
            var destinationProjectsPath = Path.GetDirectoryName(GetProjectsPath(_destinationStudioVersion));
            var sourceProjectsPath = Path.GetDirectoryName(GetProjectsPath(_sourceStudioVersion));
            var chunkSize = 90/projectToBeMoved.Count();
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

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destinationDirectory, subdir.Name);
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
                var sourcePath = sourceDescendant.Attribute("path").Value;
                if (destinationTmXml.Descendants("TranslationMemory").All(x => x.Attribute("path").Value != sourcePath))
                {
                    destinationTmXml.Element("TranslationMemories").Add(sourceDescendant);
                }
            }

            foreach (var sourceDescendant in sourceTmXml.Descendants("LanguageResources"))
            {
                var sourcePath = sourceDescendant.Attribute("path").Value;
                if (destinationTmXml.Descendants("LanguageResources").All(x => x.Attribute("path").Value != sourcePath))
                {
                    destinationTmXml.Element("LanguageResourceGroups").Add(sourceDescendant);
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
