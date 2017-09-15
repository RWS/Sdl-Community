using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using Sdl.Community.PostEdit.Versions.Structures;

namespace Sdl.Community.PostEdit.Versions
{
	public static class Helper
	{

		public static string GetUniqueName(string baseName, List<string> existingNames)
		{
			var rs = string.Empty;


			for (var i = 0; i < 1000; i++)
			{
				var newName = baseName + "_" + i.ToString().PadLeft(4, '0');
				var foundName = existingNames.Any(name => string.Compare(name, newName, StringComparison.OrdinalIgnoreCase) == 0);
				if (foundName) continue;
				rs = newName;
				break;
			}

			return rs;
		}

		public static string GetStringFromDateTime(DateTime dateTime)
		{
			return dateTime.Year
				+ "-" + dateTime.Month.ToString().PadLeft(2, '0')
				+ "-" + dateTime.Day.ToString().PadLeft(2, '0')
				+ "T" + dateTime.Hour.ToString().PadLeft(2, '0')
				+ "." + dateTime.Minute.ToString().PadLeft(2, '0')
				+ "." + dateTime.Second.ToString().PadLeft(2, '0');
		}

		public static DateTime GetDateTimeFromString(string strDateTime)
		{
			var dateTime = DateTime.Now;

			//2012-05-17
			var rDateTime = new Regex(@"(?<x1>\d{4})\-(?<x2>\d{2})\-(?<x3>\d{2})T(?<x4>\d{2})\.(?<x5>\d{2})\.(?<x6>\d{2})", RegexOptions.IgnoreCase);

			var mRDateTime = rDateTime.Match(strDateTime);
			if (!mRDateTime.Success) return dateTime;
			try
			{
				var yy = Convert.ToInt32(mRDateTime.Groups["x1"].Value);
				var mm = Convert.ToInt32(mRDateTime.Groups["x2"].Value);
				var dd = Convert.ToInt32(mRDateTime.Groups["x3"].Value);

				var hh = Convert.ToInt32(mRDateTime.Groups["x4"].Value);
				var MM = Convert.ToInt32(mRDateTime.Groups["x5"].Value);
				var ss = Convert.ToInt32(mRDateTime.Groups["x6"].Value);

				dateTime = new DateTime(yy, mm, dd, hh, MM, ss);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return dateTime;
		}

		public static List<PairedFiles.PairedFile> GetPairedFiles(VersionDetails versionDetails)
		{
			//var filesList = new List<string>();
			var originalFiles = new List<string>();
			var modifiedFiles = new List<string>();

			//get target files 
			foreach (var folder in versionDetails.TargetLanguages)
			{
				var originalFilesPath = Path.Combine(versionDetails.OriginalFileLocation,
					folder.id);
				originalFiles.AddRange(GetFilesFromFolder(originalFilesPath));

				var modifiedFilesPath = Path.Combine(versionDetails.ModifiedFileLocation,
					folder.id);
				modifiedFiles.AddRange(GetFilesFromFolder(modifiedFilesPath));

			}

			//get files from source folder for initial files
			var sourceOriginalPathFolder = Path.Combine(versionDetails.OriginalFileLocation,
				versionDetails.SourceLanguage.id);
			originalFiles.AddRange(GetFilesFromFolder(sourceOriginalPathFolder));

			//get files from source folder for modified files
			var modifiedOriginalPathFolder = Path.Combine(versionDetails.ModifiedFileLocation,
			versionDetails.SourceLanguage.id);
			modifiedFiles.AddRange(GetFilesFromFolder(modifiedOriginalPathFolder));


			var pairedFiles = CreatePairedFiles(originalFiles,modifiedFiles);

			return pairedFiles;
		}

		private static List<string> GetFilesFromFolder(string folderPath)
		{
			var files = new List<string>();
			if (Directory.Exists(folderPath))
			{
				files= Directory.GetFiles(folderPath).ToList();
			}
			return files;
		}
		private static List<PairedFiles.PairedFile> CreatePairedFiles(
			List<string> originalFiles, List<string> modifiedFiles)
		{
			var pairedFiles = new List<PairedFiles.PairedFile>();
			//add original file path 
			foreach (var filePath in originalFiles)
			{
				var piredFile = new PairedFiles.PairedFile
				{
					OriginalFilePath = new FileInfo(filePath)
				};
				pairedFiles.Add(piredFile);

			}

			//add modified files to pair list
			foreach (var modifiedFilePath in modifiedFiles)
			{
				var fileInfo = new FileInfo(modifiedFilePath);
				var directoryName = fileInfo.Directory.Name;

				var pairFile = pairedFiles.FirstOrDefault(
					p => p.OriginalFilePath.Directory.Name.Equals(directoryName) &&
					p.OriginalFilePath.Name.Equals(fileInfo.Name));

				if (pairFile != null)
				{
					pairFile.UpdatedFilePath = fileInfo;
				}

			}

			return pairedFiles;
		}

			

	public static VersionDetails CreateVersionDetails(Project project)
		{
			var versionDetails = new VersionDetails
			{
				OriginalFileLocation = project.projectVersions[0].location,
				ModifiedFileLocation = project.projectVersions[project.projectVersions.Count - 1].location,
				SourceLanguage=project.sourceLanguage,
				TargetLanguages = project.targetLanguages
			};

			return versionDetails;
		
		}
	}
}
