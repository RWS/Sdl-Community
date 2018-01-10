using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class ProgramData
    {
	    public static List<string> GetProgramDataMajorFolderPath(List<StudioVersionListItem> studioVersions)
	    {
			var majorPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}",studioVersion.MajorVersionNumber);
			    majorPaths.Add(majorFolderPath);
		    }
		    return majorPaths;
		}

	    public static List<string> GetProgramDataMajorFullFolderPath(List<StudioVersionListItem> studioVersions)
	    {
			var majorPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}.0.0.0", studioVersion.MajorVersionNumber);
			    majorPaths.Add(majorFolderPath);
		    }
		    return majorPaths;
		}

	    public static List<string> GetProgramDataFolderPath(List<StudioVersionListItem> studioVersions)
	    {
			var programDataPaths = new List<string>();
		    foreach (var studioVersion in studioVersions)
		    {
			    var programDataFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}", studioVersion.FolderName);
			    programDataPaths.Add(programDataFolderPath);
		    }
		    return programDataPaths;
		}
    }
}
