using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class DocumentsFolder
    {
	    public static List<string> GetProjectTemplatesFolderPath(string userName, List<StudioVersionListItem> studioVersions)
	    {
		    var projectTempletesPath = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var ptojectTemplate = string.Format(@"C:\Users\{0}\Documents\{1}\Project Templates", userName,
				    studioVersion.DisplayName);
			    projectTempletesPath.Add(ptojectTemplate);
		    }
		    return projectTempletesPath;
	    }

	    public static List<string> GetProjectsFolderPath(string userName, List<StudioVersionListItem> studioVersions)
	    {
		    var xmlProjectsPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var projectsXmlPath = string.Format(@"C:\Users\{0}\Documents\{1}\Projects\projects.xml", userName,
				    studioVersion.DisplayName);
			    xmlProjectsPaths.Add(projectsXmlPath);
		    }

		    return xmlProjectsPaths;
	    }
	}
}
