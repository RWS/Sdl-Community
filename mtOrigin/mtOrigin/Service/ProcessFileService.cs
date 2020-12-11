using System.Collections.Generic;
using Sdl.Community.mtOrigin.Interface;
using Sdl.Community.mtOrigin.Studio;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.mtOrigin.Service
{
	public class ProcessFileService:IProcessFileService
	{
		public void RemoveTranslationOrigin(List<FileBasedProject> selectedProjects, string newOrigin)
		{
			foreach (var selectedProject in selectedProjects)
			{
				var files = selectedProject.GetTargetLanguageFiles();
				foreach (var file in files)
				{
					ProcessFile(file.LocalFilePath,newOrigin);
				}
				selectedProject.Save();
			}
		}

		private void ProcessFile(string filePath, string newOrigin)
		{
			var converter = DefaultFileTypeManager.CreateInstance(true)
				.GetConverterToDefaultBilingual(filePath, filePath, null);
			var fileProcessor = new FileProcessor(newOrigin);
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(fileProcessor));
			converter.Parse();
		}
	}
}
