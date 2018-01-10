using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class MultiTermFolders
    {
	    public static List<string> GetPackageCachePaths(List<MultiTermVersionListItem> multiTermVersions)
	    {
		  var packagePaths = new List<string>();
		    foreach (var multiTermVersion in multiTermVersions)
		    {
			    var packagePath = string.Format(@"C:\ProgramData\Package Cache\SDL\SDLMultiTermDesktop{0}",
				    multiTermVersion.ReleaseNumber);
				packagePaths.Add(packagePath);
		    }
		    return packagePaths;
	    }

	    public static List<string> ProgramFilesPaths(List<MultiTermVersionListItem> multiTermVersions)
	    {
			var programFilesPaths = new List<string>();
		    foreach (var multiTermVersion in multiTermVersions)
		    {
			    var programFilePath = string.Format(@"c:\Program Files (x86)\SDL\SDL MultiTerm\MultiTerm{0}",
				    multiTermVersion.MajorVersionNumber);
			    programFilesPaths.Add(programFilePath);
		    }
		    return programFilesPaths;
		}

	    public static List<string> AppDataLocalPaths(string userName, List<MultiTermVersionListItem> multiTermVersions)
	    {
			var appDataPaths = new List<string>();
		    foreach (var multiTermVersion in multiTermVersions)
		    {
			    var appDataFilePath = string.Format(@"c:\Users\{0}\AppData\Local\SDL\SDL MultiTerm\MultiTerm{1}",userName,
				    multiTermVersion.MajorVersionNumber);
			    appDataPaths.Add(appDataFilePath);
		    }
		    return appDataPaths;
		}

	    public static List<string> AppDataRoamingPaths(string userName, List<MultiTermVersionListItem> multiTermVersions)
	    {
			var appDataPaths = new List<string>();
		    foreach (var multiTermVersion in multiTermVersions)
		    {
			    var appDataFilePath = string.Format(@"c:\Users\{0}\AppData\Roaming\SDL\SDL MultiTerm\MultiTerm{1}", userName,
				    multiTermVersion.MajorVersionNumber);
			    appDataPaths.Add(appDataFilePath);
		    }
		    return appDataPaths;
		}
    }
}
