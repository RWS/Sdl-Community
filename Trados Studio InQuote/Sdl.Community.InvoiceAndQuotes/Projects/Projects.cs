using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace Sdl.Community.InvoiceAndQuotes.Projects
{
    static class Projects
    {
        private static String ProjectsFolder
        {
            get
            {
                return String.Format(@"{0}\Studio 2011\Projects", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
        }

        public static List<KeyValuePair<String, String>> GetAllProjects(string pathToProjectsXML)
        {
            var projects = new List<KeyValuePair<String, String>>();
            try
            {
                var projectsFile = new XPathDocument(Path.Combine(pathToProjectsXML, "projects.xml"));
                var nav = projectsFile.CreateNavigator();
                const string expression = "ProjectServer/Projects/ProjectListItem";
                var projectNodes = nav.Select(expression);
                while (projectNodes.MoveNext())
                {
                    String projectFilePath = String.Empty;
                    String projectName = String.Empty;

                    var selectSingleNode = projectNodes.Current.SelectSingleNode("@ProjectFilePath");
                    if (selectSingleNode != null)
                    {
                        projectFilePath = selectSingleNode.Value;
                    }
                    var xPathNavigator = projectNodes.Current.SelectSingleNode("ProjectInfo/@Name");
                    if (xPathNavigator != null)
                    {
                        try
                        {
                            projectName = Path.GetFileName(xPathNavigator.Value);
                        }
                        catch
                        {
                            projectName = xPathNavigator.Value;
                        }

                    }

                    projects.Add(new KeyValuePair<string, string>(projectFilePath, projectName));
                }
            } 
            catch
            {
            }

            return projects;
        }

        public static String GetProjectCustomer(String projectsFolder, String projectFilePath)
        {
            String path = GetProjectFilePath(projectsFolder, projectFilePath);
            if (String.IsNullOrEmpty(path))
                return String.Empty;
            var projectFile = new XPathDocument(path);
            var nav = projectFile.CreateNavigator();
            const string expression = "Project/GeneralProjectInfo/Customer/@Name";
            var selectSingleNode = nav.SelectSingleNode(expression);
            return selectSingleNode != null ? selectSingleNode.Value : String.Empty;
        }

        public static String GetProjectFilePath(String projectsFolder, String projectFilePath)
        {
            String path = String.Format(@"{0}\{1}", projectsFolder, projectFilePath);
            try
            {
                if (!File.Exists(path))
                {
                    try
                    {

                        if (!File.Exists(projectFilePath))
                            return null;
                        return projectFilePath;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return path;
            }
            catch
            {
                return null;
            }
        }
    }
}
