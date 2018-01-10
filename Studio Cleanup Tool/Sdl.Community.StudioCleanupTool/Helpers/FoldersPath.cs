using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class FoldersPath
    {
	    public static async Task<List<string>> GetFoldersPath(string userName,
		    List<StudioVersionListItem> studioVersions,
		    List<StudioLocationListItem> locations)
	    {
			var documentsFolderLocationList = new List<string>();
		    foreach (var location in locations)
			{
				if (location.Alias != null)
				{
					if (location.Alias.Equals("projectsXml"))
					{
						var projectsXmlFolderPath =
							await Task.FromResult(DocumentsFolder.GetProjectsFolderPath(userName,
								studioVersions));
						documentsFolderLocationList.AddRange(projectsXmlFolderPath);
					}

					if (location.Alias.Equals("projectTemplates"))
					{
						var projectTemplatesFolderPath = await Task.FromResult(DocumentsFolder.GetProjectTemplatesFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(projectTemplatesFolderPath);

					}
					if (location.Alias.Equals("roamingMajor"))
					{
						var roamingMajorVersionFolderPath = await Task.FromResult(AppDataFolder.GetRoamingMajorFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingMajorVersionFolderPath);
					}
					if (location.Alias.Equals("roamingMajorFull"))
					{
						var roamingMajorFullVersionFolderPath = await Task.FromResult(AppDataFolder.GetRoamingMajorFullFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingMajorFullVersionFolderPath);
					}
					if (location.Alias.Equals("roamingProjectApi"))
					{
						var roamingProjectApiFolderPath = await Task.FromResult(AppDataFolder.GetRoamingProjectApiFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingProjectApiFolderPath);
					}
					if (location.Alias.Equals("localMajorFull"))
					{
						var localMajorFullFolderPath = await Task.FromResult(AppDataFolder.GetLocalMajorFullFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(localMajorFullFolderPath);
					}
					if (location.Alias.Equals("localMajor"))
					{
						var localMajorFolderPath = await Task.FromResult(AppDataFolder.GetLocalMajorFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(localMajorFolderPath);
					}
				}
				
			}
		    return documentsFolderLocationList;
	    }

	  

    }
}
