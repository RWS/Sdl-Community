using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using PostEdit.Compare;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Versions.Structures;

namespace Sdl.Community.PostEdit.Versions
{
	public static class Helper
	{
        public static string CreateDatetimePath(string path, string dateTime) =>
            Path.Combine(path, dateTime.Replace(':', '.'));
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

        public static string GetStringFromDateTime(DateTime dateTime) => dateTime.ToString(CultureInfo.InvariantCulture);

        public static DateTime GetDateTimeFromString(string strDateTime) =>
            DateTime.TryParse(strDateTime, out var dateTime) ? dateTime : Common.DateNull;

        public static List<PairedFiles.PairedFile> GetPairedFiles(VersionDetails versionDetails,Project project)
		{
			var originalFiles = new List<string>();
			var modifiedFiles = new List<string>();

			var pathToLanguageFolder = Path.Combine(versionDetails.OriginalFileLocation, versionDetails.SourceLanguage.id);
			if (Directory.Exists(pathToLanguageFolder)){
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
			}
			else
			{
				// that means the project is created as "Translate single document" - only one target language is available

				var originalFile = Directory.GetFiles(versionDetails.OriginalFileLocation, "*.sdlxliff").FirstOrDefault();
				if (!string.IsNullOrEmpty(originalFile))
				{
					originalFiles.Add(originalFile);
				}

				var updatedFile = Directory.GetFiles(versionDetails.ModifiedFileLocation, "*.sdlxliff").FirstOrDefault();
				if (!string.IsNullOrEmpty(updatedFile))
				{
					modifiedFiles.Add(updatedFile);
				}
			}
			
	


			var pairedFiles = CreatePairedFiles(originalFiles, modifiedFiles);

			return pairedFiles;
		}

		private static List<string> GetFilesFromFolder(string folderPath)
		{
			var files = new List<string>();
			if (Directory.Exists(folderPath))
			{
				files = Directory.GetFiles(folderPath).ToList();
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
				else
				{
					//this is for the case when project is created with "Translate as single document"
					pairedFiles[0].UpdatedFilePath = fileInfo;
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
				SourceLanguage = project.sourceLanguage,
				TargetLanguages = project.targetLanguages
			};

			return versionDetails;

		}

		public static bool WorksheetExists(ExcelPackage xlPackage, string worksheetName)
		{
			var smallerSheetName = ExcelReportHelper.NormalizeWorksheetName(worksheetName);

			var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault(w =>w.Name.Equals(smallerSheetName));

			if (worksheet != null)
			{
				return true;
			}
			return false;
		}

		public static void AddNewWorksheetToReport(ExcelPackage xlPackage, string worksheetName)
		{
			xlPackage.Workbook.Worksheets.Add(worksheetName);
			xlPackage.Save();
		}
	}
	
}
