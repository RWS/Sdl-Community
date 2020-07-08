using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class DocumentsFolder
    {
	    private static readonly string BackupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetProjectTemplatesFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var projectTemplatePath =
						$@"C:\Users\{userName}\Documents\{studioVersion.DisplayName}\Project Templates";
					var directoryInfo = new DirectoryInfo(projectTemplatePath);
					var details = new LocationDetails
					{
						OriginalFilePath = projectTemplatePath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				};
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProjectTemplatesFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
	    }

	    public static List<LocationDetails> GetProjectsFolderPath(string userName, StudioLocationListItem selectedLocation,List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var projectsXmlPath =
						$@"C:\Users\{userName}\Documents\{studioVersion.DisplayName}\Projects\projects.xml";
					var details = new LocationDetails
					{
						OriginalFilePath = projectsXmlPath,
						BackupFilePath = Path.Combine(BackupFolderPath, studioVersion.DisplayName, "Projects", "projects.xml"),
						Alias = selectedLocation?.Alias,
						Version = studioVersion?.DisplayName
					};
					studioDetails.Add(details);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.GetProjectsFolderPath} {ex.Message}\n {ex.StackTrace}");
			}
			return studioDetails;
		}
	}
}