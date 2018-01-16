using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class DocumentsFolder
    {
	    private static string _backupFolderPath =
		    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static List<StudioDetails> GetProjectTemplatesFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
			var studioDetails = new List<StudioDetails>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var projectTemplatePath = string.Format(@"C:\Users\{0}\Documents\{1}\Project Templates", userName,
				    studioVersion.DisplayName);
			    var directoryInfo = new DirectoryInfo(projectTemplatePath);
				var details = new StudioDetails
				{
					OriginalFilePath = projectTemplatePath,
					BackupFilePath = Path.Combine(_backupFolderPath,studioVersion.DisplayName,directoryInfo.Name),
					Alias = selectedLocation.Alias,
					StudioVersion = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
		    };
		    return studioDetails;

	    }

	    public static List<StudioDetails> GetProjectsFolderPath(string userName, StudioLocationListItem selectedLocation,List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
		    {
				var projectsXmlPath = string.Format(@"C:\Users\{0}\Documents\{1}\Projects", userName,
					studioVersion.DisplayName);
				var directoryInfo = new DirectoryInfo(projectsXmlPath);
				var details = new StudioDetails
			    {
				    OriginalFilePath = projectsXmlPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    StudioVersion = studioVersion.DisplayName
				};
				studioDetails.Add(details);
		    }
		    return studioDetails;
		}
	}
}
