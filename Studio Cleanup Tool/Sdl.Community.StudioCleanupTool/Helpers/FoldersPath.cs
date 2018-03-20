using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class FoldersPath
    {
	    private static string _backupFolderPath =
		    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");

		public static async Task<List<LocationDetails>> GetFoldersPath(string userName,
		    List<StudioVersionListItem> studioVersions,
		    List<StudioLocationListItem> locations)
	    {
			var foldersToBackup = new List<LocationDetails>();
		    foreach (var location in locations)
			{
				if (location.Alias != null)
				{
					if (location.Alias.Equals("projectsXml"))
					{
						var projectsXmlFolderPath =
							await Task.FromResult(DocumentsFolder.GetProjectsFolderPath(userName,location,studioVersions));
						foldersToBackup.AddRange(projectsXmlFolderPath);
					}
					if (location.Alias.Equals("projectTemplates"))
					{
						var projectTemplatesFolderPath = await Task.FromResult(DocumentsFolder.GetProjectTemplatesFolderPath(userName,location, studioVersions));
						foldersToBackup.AddRange(projectTemplatesFolderPath);

					}
					if (location.Alias.Equals("roamingMajor"))
					{
						var roamingMajorVersionFolderPath = await Task.FromResult(AppDataFolder.GetRoamingMajorFolderPath(userName, location,studioVersions));
						foldersToBackup.AddRange(roamingMajorVersionFolderPath);
					}
					if (location.Alias.Equals("roamingMajorFull"))
					{
						var roamingMajorFullVersionFolderPath = await Task.FromResult(AppDataFolder.GetRoamingMajorFullFolderPath(userName, location,studioVersions));
						foldersToBackup.AddRange(roamingMajorFullVersionFolderPath);
					}
					if (location.Alias.Equals("roamingProjectApi"))
					{
						var roamingProjectApiFolderPath = await Task.FromResult(AppDataFolder.GetRoamingProjectApiFolderPath(userName, location, studioVersions));
						foldersToBackup.AddRange(roamingProjectApiFolderPath);
					}
					if (location.Alias.Equals("localMajorFull"))
					{
						var localMajorFullFolderPath = await Task.FromResult(AppDataFolder.GetLocalMajorFullFolderPath(userName,location, studioVersions));
						foldersToBackup.AddRange(localMajorFullFolderPath);
					}
					if (location.Alias.Equals("localMajor"))
					{
						var localMajorFolderPath = await Task.FromResult(AppDataFolder.GetLocalMajorFolderPath(userName,location, studioVersions));
						foldersToBackup.AddRange(localMajorFolderPath);
					}
					if (location.Alias.Equals("programDataMajor"))
					{
						var programDataMajorFolderPath = await Task.FromResult(ProgramData.GetProgramDataMajorFolderPath(location,studioVersions));
						foldersToBackup.AddRange(programDataMajorFolderPath);
					}
					if (location.Alias.Equals("programDataMajorFull"))
					{
						var programDataMajorFullFolderPath = await Task.FromResult(ProgramData.GetProgramDataMajorFullFolderPath(location,studioVersions));
						foldersToBackup.AddRange(programDataMajorFullFolderPath);
					}
					if (location.Alias.Equals("programData"))
					{
						var programDataFolderPath = await Task.FromResult(ProgramData.GetProgramDataFolderPath(location,studioVersions));
						foldersToBackup.AddRange(programDataFolderPath);
					}
				}
				
			}
		    return foldersToBackup;
	    }

	    public static async Task<List<LocationDetails>> GetMultiTermFoldersPath(string userName,
		    List<MultiTermVersionListItem> multiTermVersions, List<MultiTermLocationListItem> locations)
	    {
		    var foldersLocationList = new List<LocationDetails>();
			foreach (var location in locations)
		    {
			    if (location.Alias != null)
			    {
				    if (location.Alias.Equals("packageCache"))
				    {
						var packageCacheLocations = await Task.FromResult(MultiTermFolders.GetPackageCachePaths(location,multiTermVersions));
						foldersLocationList.AddRange(packageCacheLocations);
					}
				    if (location.Alias.Equals("programFiles"))
				    {
					    var programFilesLocations = await Task.FromResult(MultiTermFolders.ProgramFilesPaths(location,multiTermVersions));
					    foldersLocationList.AddRange(programFilesLocations);
					}
				    if (location.Alias.Equals("appDataLocal"))
				    {
					    var appDataLocal = await Task.FromResult(MultiTermFolders.AppDataLocalPaths(location,userName,multiTermVersions));
					    foldersLocationList.AddRange(appDataLocal);
					}
				    if (location.Alias.Equals("appDataRoming"))
				    {
					    var appDataRoaming = await Task.FromResult(MultiTermFolders.AppDataRoamingPaths(location,userName,multiTermVersions));
					    foldersLocationList.AddRange(appDataRoaming);
					}
				}
		    }
		    return foldersLocationList;
	    }
    }
}
