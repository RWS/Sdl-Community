using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace Sdl.Community.BackupFiles
{
	public class BackupFiles
	{
		static void Main(string[] args)
		{
			LoadAssemblies();

			BackupFilesRecursive();
		}

		#region Private methods
		private static string GetAcceptedRequestsFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		private static void BackupFilesRecursive()
		{
			try
			{
				var service = new Service();
				var jsonResult = service.GetJsonInformation();
				var fileExtensions = new List<string>();

				if (jsonResult != null && jsonResult.BackupDetailsModelList.Any())
				{
					foreach (var fileExtension in jsonResult.BackupDetailsModelList)
					{
						fileExtensions.Add(fileExtension.BackupPattern);
					}
				}

				if (jsonResult != null && jsonResult.BackupModel != null)
				{
					var splittedSourcePathList = jsonResult.BackupModel.BackupFrom.Split(';').ToList<string>();
					var files = new List<string>().ToArray();

					foreach (var sourcePath in splittedSourcePathList)
					{
						if (!string.IsNullOrEmpty(sourcePath))
						{
							var acceptedRequestFolder = GetAcceptedRequestsFolder(sourcePath);

							// create the directory where to move files
							if (!Directory.Exists(jsonResult.BackupModel.BackupTo))
							{
								Directory.CreateDirectory(jsonResult.BackupModel.BackupTo);
							}

							// take files depending on defined action
							if (fileExtensions.Any())
							{
								// get all files which have extension set up depending on actions from TMBackupDetails grid
								files = Directory.GetFiles(sourcePath, "*.*")
													 .Where(f => fileExtensions
													 .Contains(Path.GetExtension(f)))
													 .ToArray();
							}
							else
							{
								// take all files
								files = Directory.GetFiles(sourcePath);
							}

							if (files.Length != 0)
							{
								MoveFilesToAcceptedFolder(files, jsonResult.BackupModel.BackupTo);
							} //that means we have a subfolder in watch folder
							else
							{
								var subdirectories = Directory.GetDirectories(sourcePath);
								foreach (var subdirectory in subdirectories)
								{
									var currentDirInfo = new DirectoryInfo(subdirectory);
									CheckForSubfolders(currentDirInfo, jsonResult.BackupModel.BackupTo);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageLogger.LogFileMessage(ex.Message);
			}
		}

		private static void CheckForSubfolders(DirectoryInfo directory, string root)
		{
			var service = new Service();
			var jsonResult = service.GetJsonInformation();

			if (jsonResult != null && jsonResult.BackupModel != null)
			{
				var sourcePath = jsonResult.BackupModel.BackupFrom;
				var subdirectories = directory.GetDirectories();
				var path = root + @"\" + directory.Parent;
				var subdirectoryFiles = Directory.GetFiles(directory.FullName);

				if (subdirectoryFiles.Length != 0)
				{
					MoveFilesToAcceptedFolder(subdirectoryFiles, path);
				}

				if (subdirectories.Length != 0)
				{
					foreach (var subdirectory in subdirectories)
					{
						CheckForSubfolders(subdirectory, path);
					}
				}
			}
		}

		private static void MoveFilesToAcceptedFolder(string[] files, string acceptedFolderPath)
		{
			foreach (var subFile in files)
			{
				var dirName = new DirectoryInfo(subFile).Name;
				var parentName = new DirectoryInfo(subFile).Parent != null ? new DirectoryInfo(subFile).Parent.Name : string.Empty;

				var fileName = subFile.Substring(subFile.LastIndexOf(@"\", StringComparison.Ordinal));
				var destinationPath = Path.Combine(acceptedFolderPath, parentName);
				if (!Directory.Exists(destinationPath))
				{
					Directory.CreateDirectory(destinationPath);
				}
				try
				{
					File.Copy(subFile, destinationPath + fileName, true);
				}
				catch (Exception ex)
				{
					MessageLogger.LogFileMessage(ex.Message);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadAssemblies()
		{
			var _assemblies = new Dictionary<string, Assembly>();
			Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
			{
				var shortName = new AssemblyName(args.Name).Name;
				if (_assemblies.TryGetValue(shortName, out var assembly))
				{
					return assembly;
				}
				return null;
			}
			var appAssembly = typeof(BackupFiles).Assembly;
			foreach (var resourceName in appAssembly.GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				{
					using (var stream = appAssembly.GetManifestResourceStream(resourceName))
					{
						var assemblyData = new byte[(int)stream.Length];
						stream.Read(assemblyData, 0, assemblyData.Length);
						var assembly = Assembly.Load(assemblyData);
						_assemblies.Add(assembly.GetName().Name, assembly);
					}
				}
			}
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}

		#endregion
	}
}