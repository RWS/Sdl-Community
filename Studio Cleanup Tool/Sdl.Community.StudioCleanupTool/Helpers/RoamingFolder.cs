using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class RoamingFolder
    {
	    public static List<string> GetRoamingMajorFolderPath(string userName, List<StudioVersionListItem> studioVersions)
	    {
			var majorPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}", userName,
				    studioVersion.MajorVersionNumber);
				majorPaths.Add(majorFolderPath);

		    }
		    return majorPaths;
	    }

	    public static List<string> GetRoamingMajorFullFolderPath(string userName, List<StudioVersionListItem> studioVersions)
	    {
		    var majorPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\SDL Trados Studio\{1}.0.0.0", userName,
				    studioVersion.MajorVersionNumber);
			    majorPaths.Add(majorFolderPath);

		    }
		    return majorPaths;
	    }

	    public static List<string> GetRoamingProjectApiFolderPath(string userName, List<StudioVersionListItem> studioVersions)
	    {
			var roamninProjectApiPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var projectApiPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL\ProjectApi\{1}.0.0.0", userName,
				    studioVersion.MajorVersionNumber);
			    roamninProjectApiPaths.Add(projectApiPath);
			}
		    return roamninProjectApiPaths;
		}
    }
}
