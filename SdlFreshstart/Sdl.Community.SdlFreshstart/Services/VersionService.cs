using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Model;
using Sdl.Community.Toolkit.Core;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.Versioning;
using StudioVersion = Sdl.Community.SdlFreshstart.Model.StudioVersion;
using StudioVersionService = Sdl.Versioning.StudioVersionService;

namespace Sdl.Community.SdlFreshstart.Services
{
	public class VersionService
	{
		private readonly string _logPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"SDL/Chainer/Logs");
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public VersionService()
		{
			_logger.Info("Version service initialized");
		}

		public List<StudioVersion> GetListOfStudioVersions()
		{
			var versionsDictionary = Versions.KnownStudioVersions.Skip(1).TakeWhile(s => !s.Value.Contains("Next"));
			
			return versionsDictionary.Select(item => new StudioVersion(item.Key, item.Value)).ToList();
		}

		public List<StudioVersion> GetInstalledStudioVersions()
		{
			var studioVersionService = new StudioVersionService();
			var installedStudioVersions = studioVersionService.GetInstalledStudioVersions();
			_logger.Info("Installed Trados Studio Versions");

			var installedVersions = installedStudioVersions
				?.Select(v => new StudioVersion(v.Version, v.PublicVersion, v.Edition)).ToList();
			
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

			installedVersions?.Sort((item1, item2) =>
				item1.MajorVersion < item2.MajorVersion ? 1 :
					item1.MajorVersion > item2.MajorVersion ? -1 : 0);

			return installedVersions;
		}

		public List<MultitermVersion> GetInstalledMultitermVersions()
		{
			var multitermVersioningService = new MultiTermVersionService();
			var multitermVersions = multitermVersioningService.GetInstalledMultiTermVersions().Select(mv => new MultitermVersion(mv.PublicVersion))
				.ToList();

			multitermVersions.Sort((item1, item2) =>
				item1.MajorVersion < item2.MajorVersion
					? 1
					: item1.MajorVersion > item2.MajorVersion ? -1 : 0);

			return multitermVersions;
		}
	}
}