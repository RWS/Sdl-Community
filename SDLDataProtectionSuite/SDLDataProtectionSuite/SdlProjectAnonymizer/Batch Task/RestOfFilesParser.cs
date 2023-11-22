using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	public class RestOfFilesParser
	{
		private readonly List<string> _ignoredList = new List<string>();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private bool _restOfFilesParsed;

		public void ParseRestOfFiles(IProject project, ProjectFile[] taskFiles,
			AbstractBilingualContentProcessor contentProcessor, out List<string> ignoredFiles)
		{
			ignoredFiles = _ignoredList;
			if (_restOfFilesParsed) return;

			var unParsedProjectFiles = GetUnparsedFiles(project, taskFiles);

			CloseOpenedDocuments();

			foreach (var file in unParsedProjectFiles)
			{
				if (Path.GetExtension(file.LocalFilePath) != ".sdlxliff")
				{
					_logger.Info(PluginResources.FileIgnoredByParser, file.LocalFilePath);
					ignoredFiles.Add(file.LocalFilePath);
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
			var editor = TradosApp.GetController<EditorController>();
			if (editor == null)
				return;
			var activeDocs = editor.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				Application.Current.Dispatcher.Invoke(() => { editor.Close(activeDoc); });
			}
		}

		private IEnumerable<ProjectFile> GetUnparsedFiles(IProject project, IEnumerable<ProjectFile> taskFiles)
		{
			var projectFiles = project?.GetTargetLanguageFiles();
			var taskFilesIds = taskFiles.GetIds();
			var unparsedFiles = projectFiles?.Where(file => !taskFilesIds.Contains(file.Id)).ToList();

			return unparsedFiles;
		}
	}
}