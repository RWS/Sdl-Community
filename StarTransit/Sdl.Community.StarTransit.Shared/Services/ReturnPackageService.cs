﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using NLog;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class ReturnPackageService
	{
		private readonly ProjectsController _projectsController;
		private readonly ReturnPackage _returnPackage;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ReturnPackageService()
		{
			_returnPackage = new ReturnPackage();
			var helpers = new Helpers();
			_projectsController = helpers.GetProjectsController();
		}

		/// <summary>
		/// Returns a list of StarTransit return package and  true if the projects selected are a StarTransit projects 
		/// </summary>
		/// <returns></returns>
		public Tuple<IReturnPackage, string> GetPackage()
		{
			try
			{
				var projects = _projectsController?.SelectedProjects.ToList();
				if (projects != null)
				{
					if (projects.Count > 1)
					{
						return new Tuple<IReturnPackage, string>(null, "Please select only one project.");
					}

					var project = projects[0];
					var targetFiles = project.GetTargetLanguageFiles().ToList();
					var isTransit = IsTransitProject(targetFiles);

					if (!isTransit)
						return new Tuple<IReturnPackage, string>(_returnPackage,
							"Please select a StarTransit project");

					_returnPackage.FileBasedProject = project;
					_returnPackage.ProjectLocation = Path.GetDirectoryName(project.FilePath); //project.FilePath;
					_returnPackage.TargetFiles = targetFiles;
					//we take only the first file location, because the other files are in the same location
					_returnPackage.LocalFilePath = targetFiles[0].LocalFilePath;
					_returnPackage.PathToPrjFile = GetPathToPrjFile(project.FilePath);
					_returnPackage.ReturnFilesDetails = new List<ReturnFileDetails>();

					foreach (var targetFile in targetFiles)
					{
						var fileDetails = new ReturnFileDetails
						{
							FileName = targetFile.Name, Path = targetFile.LocalFilePath, Id = targetFile.Id
						};
						_returnPackage.ReturnFilesDetails.Add(fileDetails);
					}
					return new Tuple<IReturnPackage, string>(_returnPackage, string.Empty);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		public Tuple<ReturnPackage, string> GetReturnPackage()
		{
			try
			{
				var projects = _projectsController?.SelectedProjects.ToList();
				if (projects != null)
				{
					if (projects.Count > 1)
					{
						return new Tuple<ReturnPackage, string>(null, "Please select only one project.");
					}

					var project = projects[0];
					var targetFiles = project.GetTargetLanguageFiles().ToList();
					var isTransit = IsTransitProject(targetFiles);

					if (!isTransit)
						return new Tuple<ReturnPackage, string>(_returnPackage,
							"Please select a StarTransit project");

					_returnPackage.FileBasedProject = project;
					_returnPackage.ProjectLocation = Path.GetDirectoryName(project.FilePath); //project.FilePath;
					_returnPackage.TargetFiles = targetFiles;
					//we take only the first file location, because the other files are in the same location
					_returnPackage.LocalFilePath = targetFiles[0].LocalFilePath;
					_returnPackage.PathToPrjFile = GetPathToPrjFile(project.FilePath);
					return new Tuple<ReturnPackage, string>(_returnPackage, string.Empty);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}
		/// <summary>
		/// Check to see if the file type is the same with the Transit File Type
		/// </summary>
		public bool IsTransitProject(List<ProjectFile> filesPath)
		{
			if (filesPath != null)
			{
				return filesPath.Any(f => f.FileTypeId != null && f.FileTypeId.Equals("Transit File Type 1.0.0.0"));
			}
			return false;
		}

		public void ExportFiles(ReturnPackage package)
		{
			var taskSequence = package.FileBasedProject.RunAutomaticTasks(package.TargetFiles.GetIds(),
				new string[] {AutomaticTaskTemplateIds.GenerateTargetTranslations});

			var outputFiles = taskSequence.OutputFiles.ToList();
			CreateArchive(package);
		}

		private string GetPathToPrjFile(string pathToProject)
		{
			var prjPath = Path.Combine(pathToProject.Substring(0, pathToProject.LastIndexOf(@"\", StringComparison.Ordinal)), "StarTransitMetadata");
			if (!Directory.Exists(prjPath)) return string.Empty;
			var filesPath = Directory.GetFiles(prjPath).ToList();
			var prjFilePath = filesPath.FirstOrDefault(p => p.EndsWith("PRJ"));
			return prjFilePath;
		}

		/// <summary>
		/// Creates an archive in the Return Package folder and add project files to it
		/// For the moment we add the files without runing any task on them
		/// </summary>
		private void CreateArchive(ReturnPackage package)
		{
			try
			{
				ChangeMetadataFile(package.PathToPrjFile);

				var prjFileName = Path.GetFileNameWithoutExtension(package.PathToPrjFile);
				var archivePath = Path.Combine(package.FolderLocation, prjFileName + ".tpf");

				foreach (var targetFile in package.TargetFiles)
				{
					var pathToTargetFileFolder = targetFile.LocalFilePath.Substring(0, targetFile.LocalFilePath.LastIndexOf(@"\", StringComparison.Ordinal));

					if (!File.Exists(archivePath))
					{
						//create the archive, and add files to it
						using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
						{
							archive.CreateEntryFromFile(package.PathToPrjFile, string.Concat(prjFileName, ".PRJ"), CompressionLevel.Optimal);
							foreach (var file in package.TargetFiles)
							{
								pathToTargetFileFolder = file.LocalFilePath.Substring(0, file.LocalFilePath.LastIndexOf(@"\", StringComparison.Ordinal));
								var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);

								archive.CreateEntryFromFile(Path.Combine(pathToTargetFileFolder, fileName), fileName, CompressionLevel.Optimal);
							}
						}
					}
					else
					{
						UpdateArchive(archivePath, prjFileName, package, pathToTargetFileFolder);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		private void UpdateArchive(string archivePath, string prjFileName, ReturnPackage returnPackagePackage, string pathToTargetFileFolder)
		{
			try
			{
				// open the archive and delete old files
				// archive in update mode not overrides existing files 
				using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
				{
					var entriesCollection = new ObservableCollection<ZipArchiveEntry>(archive.Entries);
					foreach (var entry in entriesCollection)
					{

						if (entry.Name.Equals(string.Concat(prjFileName, ".PRJ")))
						{
							entry.Delete();
						}

						foreach (var project in returnPackagePackage.TargetFiles)
						{
							var projectFromArchiveToBeDeleted = archive.Entries.FirstOrDefault(n => n.Name.Equals(Path.GetFileNameWithoutExtension(project.Name)));
							projectFromArchiveToBeDeleted?.Delete();
						}
					}
				}

				//add files to archive
				using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
				{
					archive.CreateEntryFromFile(returnPackagePackage.PathToPrjFile, string.Concat(prjFileName, ".PRJ"), CompressionLevel.Optimal);
					foreach (var file in returnPackagePackage.TargetFiles)
					{
						var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);
						pathToTargetFileFolder = file.LocalFilePath.Substring(0, file.LocalFilePath.LastIndexOf(@"\", StringComparison.Ordinal));
						archive.CreateEntryFromFile(Path.Combine(pathToTargetFileFolder, fileName), fileName, CompressionLevel.Optimal);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Reads the prj file and check if an update has been made already
		/// </summary>
		private void ChangeMetadataFile(string pathToPrjFile)
		{
			var metadata = new Metadata
			{
				ExchangedDate = CustomDateTime.CustomExchangeDate(DateTime.Now),
				ExchangedTime = CustomDateTime.CustomExchangeTime(DateTime.Now),
				LastChangedDate = CustomDateTime.CreateCustomDate(DateTime.Now)
			};

			using (var reader = new StreamReader(pathToPrjFile))
			{
				var fileContent = reader.ReadToEnd();
				reader.Close();

				MetadataBuilder(pathToPrjFile, metadata, !fileContent.Contains("PromptForNewWorkingDir"));
			}
		}

		/// <summary>
		/// Changes the metadata from prj file
		/// </summary>
		private void MetadataBuilder(string pathToPrjFile, Metadata metadata, bool createNewMetadata)
		{
			try
			{
				var builder = new StringBuilder();
				var lines = File.ReadAllLines(pathToPrjFile).ToList();
				foreach (var line in lines)
				{
					if (line.StartsWith("LastChangedDate"))
					{
						var newLine = line.Replace(line, string.Concat("LastChangedDate=", metadata.LastChangedDate));
						builder.Append(newLine + Environment.NewLine);
						continue;
					}

					//check to see if the project was edited before
					//if yes, that means we don't need to add this line anymore
					if (createNewMetadata)
					{
						if (line.Equals("[Exchange]"))
						{
							builder.AppendFormat("{0}{1}{2}{3}", "[Exchange]", Environment.NewLine, "PromptForNewWorkingDir=0", Environment.NewLine);
							continue;
						}
					}

					if (line.StartsWith("IsExchange"))
					{
						var newLine = line.Replace(line, "IsExchange=1");
						builder.Append(newLine + Environment.NewLine);
						continue;
					}
					if (line.StartsWith("ExchangeDate"))
					{
						var date = line.Replace(line, string.Concat("ExchangeDate=", metadata.ExchangedDate));
						builder.Append(date + Environment.NewLine);
						continue;
					}
					if (line.StartsWith("ExchangeTime"))
					{
						var time = line.Replace(line, string.Concat("ExchangeTime=", metadata.ExchangedTime));
						builder.Append(time + Environment.NewLine);
						continue;
					}
					builder.Append(line + Environment.NewLine);
				}

				File.WriteAllText(pathToPrjFile, builder.ToString());
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}
	}
}