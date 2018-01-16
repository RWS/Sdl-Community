using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class ProgramData
    {
	    private static string _backupFolderPath =
		    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
		public static List<StudioDetails> GetProgramDataMajorFolderPath(StudioLocationListItem selectedLocation,List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}",studioVersion.MajorVersionNumber);
			    var directoryInfo = new DirectoryInfo(majorFolderPath);
			    var details = new StudioDetails
			    {
				    OriginalFilePath = majorFolderPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName,"ProgramData",directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    StudioVersion = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
			}
		    return studioDetails;
	    }

	    public static List<StudioDetails> GetProgramDataMajorFullFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
		    {
			    var majorFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}.0.0.0", studioVersion.MajorVersionNumber);
				var directoryInfo = new DirectoryInfo(majorFolderPath);
			    var details = new StudioDetails
			    {
				    OriginalFilePath = majorFolderPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName, "ProgramData",directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    StudioVersion = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
			}
			return studioDetails;
		}

	    public static List<StudioDetails> GetProgramDataFolderPath(StudioLocationListItem selectedLocation, List<StudioVersionListItem> studioVersions)
	    {
		    var studioDetails = new List<StudioDetails>();
			foreach (var studioVersion in studioVersions)
		    {
			    var programDataFolderPath = string.Format(@"C:\ProgramData\SDL\SDL Trados Studio\{0}", studioVersion.FolderName);
				var directoryInfo = new DirectoryInfo(programDataFolderPath);
			    var details = new StudioDetails
			    {
				    OriginalFilePath = programDataFolderPath,
				    BackupFilePath = Path.Combine(_backupFolderPath, studioVersion.DisplayName,"ProgramData", directoryInfo.Name),
				    Alias = selectedLocation.Alias,
				    StudioVersion = studioVersion.DisplayName
				};
			    studioDetails.Add(details);
			}
			return studioDetails;
		}
    }
}
