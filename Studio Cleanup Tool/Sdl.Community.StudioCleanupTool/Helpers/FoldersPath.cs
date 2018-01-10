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
						var roamingMajorVersionFolderPath = await Task.FromResult(RoamingFolder.GetRoamingMajorFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingMajorVersionFolderPath);
					}
					if (location.Alias.Equals("roamingMajorFull"))
					{
						var roamingMajorFullVersionFolderPath = await Task.FromResult(RoamingFolder.GetRoamingMajorFullFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingMajorFullVersionFolderPath);
					}
					if (location.Alias.Equals("roamingProjectApi"))
					{
						var roamingProjectApiFolderPath = await Task.FromResult(RoamingFolder.GetRoamingProjectApiFolderPath(userName, studioVersions));
						documentsFolderLocationList.AddRange(roamingProjectApiFolderPath);
					}
				}
				
			}
		    return documentsFolderLocationList;
	    }

	  

    }
}
