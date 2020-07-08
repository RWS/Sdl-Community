using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Versioning;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class VersionService
	{
		private const string SdlFolder = @"SDL\SDL Trados Studio";

		public List<StudioVersion> GetListOfStudioVersions()
		{
			var versionsDictionary = Versions.KnownStudioVersions.TakeWhile(s => !s.Value.Contains("Next"));

			var listOfStudioVersions = new List<StudioVersion>();

			foreach (var item in versionsDictionary)
			{
				var relativePluginPath = Path.Combine(SdlFolder, ExtractNumericVersion(item.Key));
				var relativeStudioPath = Path.Combine(SdlFolder, item.Key);

				listOfStudioVersions.Add(new StudioVersion
				{
					ProgramFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), item.Key),
					ProgramDataPaths = new[]
					{
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), relativePluginPath),
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), relativeStudioPath)
					},
					AppDataLocalPaths = new[]
					{
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), relativePluginPath),
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), relativeStudioPath)
					},
					AppDataRoamingPaths = new[]
					{
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), relativePluginPath),
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), relativeStudioPath)
					},
					DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), item.Value.Substring(18))
				});
			}

			return listOfStudioVersions;
		}

		private string ExtractNumericVersion(string version)
		{
			var regex = new Regex(@"\d+");
			return regex.Match(version).Value;
		}
	}
}