using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	public class RestOfFilesParser
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private bool _restOfFilesParsed;

		public void ParseRestOfFiles(ProjectsController projectController, ProjectFile[] taskFiles,
			AbstractBilingualContentProcessor contentProcessor)
		{
			if (_restOfFilesParsed) return;

			var unParsedProjectFiles = GetUnparsedFiles(projectController, taskFiles);

			CloseOpenedDocuments();

			foreach (var file in unParsedProjectFiles)
			{
				if (Path.GetExtension(file.LocalFilePath) != ".sdlxliff")
				{
					_logger.Error(PluginResources.FileIgnoredByParser, file.LocalFilePath);
					continue;
				}

				var converter = DefaultFileTypeManager.CreateInstance(true)
					.GetConverterToDefaultBilingual(file.LocalFilePath, file.LocalFilePath, null);

				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter.Parse();
			}

			_restOfFilesParsed = true;
		}

		private static void CloseOpenedDocuments()
		{
			var editor = SdlTradosStudio.Application.GetController<EditorController>();
			var activeDocs = editor.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				Application.Current.Dispatcher.Invoke(() => { editor.Close(activeDoc); });
			}
		}

		private List<ProjectFile> GetUnparsedFiles(ProjectsController projectController, ProjectFile[] taskFiles)
		{
			var project = projectController.CurrentProject ?? projectController.SelectedProjects.ToList()[0];
			var projectFiles = project.GetTargetLanguageFiles();

			var taskFilesIds = taskFiles.GetIds();
			var unparsedFiles = projectFiles.Where(file => !taskFilesIds.Contains(file.Id)).ToList();

			return unparsedFiles;
		}
	}
}