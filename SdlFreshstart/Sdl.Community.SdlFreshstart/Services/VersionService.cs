using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using Sdl.Community.SdlFreshstart.Model;
using Trados.Community.Toolkit.Core.Services;
using StudioVersion = Sdl.Community.SdlFreshstart.Model.StudioVersion;
using StudioVersionService = Sdl.Versioning.StudioVersionService;

namespace Sdl.Community.SdlFreshstart.Services
{
    public class VersionService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _packageCacheSdl = @"C:\ProgramData\Package Cache\SDL";
        private readonly string _packageCacheTrados = @"C:\ProgramData\Package Cache\Trados";
        private readonly List<string> _possiblePackageCacheFolderName;
        private List<StudioVersion> _installedVersions;

        public VersionService()
        {
            _logger.Info("Version service initialized");
            _possiblePackageCacheFolderName = new List<string>
            {
                "{0}_SR{1}_cu{2}",
                "{0}_SR{1}_cu{2}_Beta",
                "{0}_SR{1}",
                "{0}_SR{1}_Beta",
                "{0}SR{1}",
                "{0}SR{1}_Beta",
                "{0}SR{1}_cu{2}",
                "{0}SR{1}_cu{2}_Beta"
            };
        }

        public List<StudioVersion> GetInstalledStudioVersions()
        {
            if (_installedVersions != null) return _installedVersions;

            var studioVersionService = new StudioVersionService();
            var installedStudioVersions = studioVersionService.GetInstalledStudioVersions();
            _logger.Info("Installed Trados Studio Versions");

            _installedVersions = installedStudioVersions
                ?.Select(v => new StudioVersion(v)).ToList();

            if (installedStudioVersions != null)
            {
                foreach (var studioVersion in installedStudioVersions)
                {
                    _logger.Info(
                        $"Installed Version:{studioVersion.Version} Public version: {studioVersion.PublicVersion}");
                }
            }
            else
            {
                _logger.Info("Cannot find any Trados Version installed on the machine");
            }

            _installedVersions?.Sort((item1, item2) =>
                item1.ExecutableVersion.Major < item2.ExecutableVersion.Major ? 1 :
                    item1.ExecutableVersion.Major > item2.ExecutableVersion.Major ? -1 : 0);

            return _installedVersions;
        }

        public List<MultitermVersion> GetInstalledMultitermVersions()
        {
            var multitermVersioningService = new MultiTermVersionService();
            var multiTermVersions = multitermVersioningService.GetInstalledMultiTermVersions().Select(mv => new MultitermVersion(mv.PublicVersion, mv.ExecutableVersion))
                .ToList();

            if (multiTermVersions.Any())
            {
                foreach (var multiTermVersion in multiTermVersions)
                {
                    _logger.Info(
                        $"Installed Version:{multiTermVersion.VersionName} Public version: {multiTermVersion.PublicVersion}");
                }
            }
            else
            {
                _logger.Info("Cannot find any Multiterm Version installed on the machine");

            }

            multiTermVersions.Sort((item1, item2) =>
                item1.MajorVersion < item2.MajorVersion
                    ? 1
                    : item1.MajorVersion > item2.MajorVersion ? -1 : 0);

            return multiTermVersions;
        }

        public string GetPackageCacheCurrentFolder(Version executableVersion, string versionName, bool isBeta)
        {
            var possibleVersions = new List<string>();
            var folderPath = string.Empty;

            //For folder name variants for SR and CU
            if (executableVersion.Minor > 0)
            {
                possibleVersions = isBeta
                    ? _possiblePackageCacheFolderName.Where(v => v.Contains("Beta")).ToList()
                    : _possiblePackageCacheFolderName.Where(v => !v.Contains("Beta")).ToList();
                foreach (var possibleVersion in possibleVersions)
                {
                    var folderName = string.Format(possibleVersion, versionName, executableVersion.Minor,
                        executableVersion.Build);
                    folderPath = Path.Combine(_packageCacheTrados, folderName);
                    if (Directory.Exists(folderPath)) return folderPath;
                }
            }

            if (executableVersion.Minor == 0)
            {
                if (isBeta)
                {
                    versionName = $"{versionName}_Beta";
                }

                folderPath = Path.Combine(_packageCacheSdl, versionName);
                if (Directory.Exists(folderPath))
                {
                    return folderPath;
                }

                folderPath = Path.Combine(_packageCacheTrados, versionName.Replace("SDL", string.Empty));
                if (Directory.Exists(folderPath))
                {
                    return folderPath;
                }
            }

            return folderPath;
        }

        public void RunRepairMsi(string pathToModuleFolder, string msiFileName)
        {
            if (Directory.Exists(pathToModuleFolder))
            {
                var msiFile = Path.Combine(pathToModuleFolder, msiFileName);
                if (File.Exists(msiFile))
                {
                    var process = new ProcessStartInfo
                    {
                        FileName = "msiexec",
                        WorkingDirectory = pathToModuleFolder ,
                        Arguments = "/fa " + msiFileName,
                        Verb = "runas",
                        UseShellExecute = false
                    };
                    Process.Start(process);
                }
            }
            else
            {
                _logger.Info($"Could not find Module folder at the following path: {pathToModuleFolder}");
            }
        }
    }
}