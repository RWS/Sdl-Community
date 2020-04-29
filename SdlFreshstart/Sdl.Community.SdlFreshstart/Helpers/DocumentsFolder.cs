using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class DocumentsFolder
    {
	    private static string _backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static readonly Log Log = Log.Instance;

		public static List<LocationDetails> GetProjectTemplatesFolderPath(string userName, StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
			var studioDetails = new List<LocationDetails>();
			try
			{
				foreach (var studioVersion in studioVersions)
				{
					var projectTemplatePath = string.Format(@"C:\Users\{0}\Documents\{1}\Project Templates", userName, studioVersion.DisplayName);
					var directoryInfo = new DirectoryInfo(projectTemplatePath);
					var details = new LocationDetails
					{
						OriginalFilePath = projectTemplatePath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, directoryInfo.Name),
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
					var projectsXmlPath = string.Format(@"C:\Users\{0}\Documents\{1}\Projects\projects.xml", userName, studioVersion.DisplayName);
					var details = new LocationDetails
					{
						OriginalFilePath = projectsXmlPath,
						BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "Projects", "projects.xml"),
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