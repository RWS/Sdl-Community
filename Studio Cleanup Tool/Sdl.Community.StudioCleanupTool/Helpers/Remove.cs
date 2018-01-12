using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class Remove
    {
	     public static async Task FromSelectedLocations(List<string> pathsList)
	    {
		    try
		    {
			    foreach (var path in pathsList)
			    {
				    var directory = await Task.FromResult(IsDirectory(path));

				    if (!directory)
				    {
					    File.Delete(path);
				    }
				    else
				    {
						var directoryInfo = new DirectoryInfo(path);
					    await Task.Run(()=>Empty(directoryInfo));
				    }
			    }
		    }
		    catch (Exception e)
		    {
			    
		    }
	    }

	    private static void Empty(DirectoryInfo directoryInfo)
	    {
			
			//removes all files from root directory
		    foreach (var file in directoryInfo.GetFiles())
		    {
			    file.Delete();
		    }

			//removes all the directories from root directory
		    foreach (var directory in directoryInfo.GetDirectories())
		    {
			    directory.Delete(true);
		    }
	    }

	    private static bool IsDirectory(string path)
	    {
			
		    var attributes = File.GetAttributes(path);
		    return attributes.HasFlag(FileAttributes.Directory);
	    }
    }
}
