using System;
using System.Collections.Generic;
using System.IO;
using Trados.Community.Toolkit.Core;

namespace ChangeScalingBehavior
{
	internal class ResourceFilesManager
    {
        public bool NoStudioManifestFile(List<Sdl.Versioning.StudioVersion> installedStudioVersions)
        {
            var counter = 0;
            for (var i = 0; i < installedStudioVersions.Count; i++)
            {
                var isManifestFile = File.Exists(installedStudioVersions[i].InstallPath + "SDLTradosStudio.exe.manifest");
                if (!isManifestFile)
                    counter += 1;
            }
            var result = counter == installedStudioVersions.Count;

            return result;
        }

        public bool NoMultiTermManifestFile(List<MultiTermVersion> installedMultiTeVersions)
        {
            var counter = 0;
            for (var i = 0; i < installedMultiTeVersions.Count; i++)
            {
                var isManifestFile = File.Exists(installedMultiTeVersions[i].InstallPath + "MultiTerm.exe.manifest");
                if (!isManifestFile)
                    counter += 1;
            }
            var result = counter == installedMultiTeVersions.Count;

            return result;
        }

        public void RemoveResourceFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
