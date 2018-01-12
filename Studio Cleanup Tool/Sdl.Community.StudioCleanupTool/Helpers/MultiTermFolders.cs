using System;
using System.Collections.Generic;
using System.IO;
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
			    var versionName = string.Format("SDLMultiTermDesktop{0}", multiTermVersion.ReleaseNumber);
			    var packagePathList = SdlMultiTermDesktop(versionName);
			    if (packagePathList.Any())
			    {
				    packagePaths.AddRange(packagePathList);
			    }
				
		    }
		    return packagePaths;
	    }

	    private  static List<string> SdlMultiTermDesktop(string versionNumber)
	    {
		    var packageCachePath = @"C:\ProgramData\Package Cache\SDL";

		    if (Directory.Exists(packageCachePath))
		    {
			    var directoriesPath = new DirectoryInfo(@"C:\ProgramData\Package Cache\SDL").GetDirectories()
				    .Where(n => n.Name.Contains(versionNumber))
				    .Select(n => n.FullName).ToList();

			    return directoriesPath;
		    }
			return new List<string>();
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
