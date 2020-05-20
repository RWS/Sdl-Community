using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sdl.Community.InSource.Helpers;
using Sdl.Community.InSource.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.InSource
{
	public class ProjectCreator
    {
        public event ProgressChangedEventHandler ProgressChanged;
        public event EventHandler<ProjectMessageEventArgs> MessageReported;
        private double _currentProgress;
	    private readonly ProjectRequest _projectRequest;
		private readonly IMessageBoxService _messageBoxService;

		public static readonly Log Log = Log.Instance;

		public ProjectCreator(List<ProjectRequest> requests, ProjectTemplateInfo projectTemplate, IMessageBoxService messageBoxService)
        {
			_messageBoxService = messageBoxService;
			Requests = requests;
            ProjectTemplate = projectTemplate;
            SuccessfulRequests = new List<Tuple<ProjectRequest, FileBasedProject>>();
        }

	    public ProjectCreator(ProjectRequest projectRequest, ProjectTemplateInfo projectTemplate, IMessageBoxService messageBoxService)
	    {
		    _projectRequest = projectRequest;
			_messageBoxService = messageBoxService;
			ProjectTemplate = projectTemplate;
		}

		List<ProjectRequest> Requests { get; }

        public List<Tuple<ProjectRequest, FileBasedProject>> SuccessfulRequests { get; }

        ProjectTemplateInfo ProjectTemplate { get; }

	    public FileBasedProject Execute()
	    {
		    _currentProgress = 0;
		    if (Requests != null)
		    {
			    foreach (var request in Requests)
			    {
				    var project = CreateProject(request);
					if (project != null)
					{
						_currentProgress += 100.0 / Requests.Count;
						OnProgressChanged(_currentProgress);
					}		
					
				    return project;
			    }
		    }

		    var fileBasedProject = CreateProject(_projectRequest);
			if (fileBasedProject != null)
			{
				OnProgressChanged(100);
			}
			return fileBasedProject;
	    }

	    private FileBasedProject CreateProject(ProjectRequest request)
	    {
		    try
		    {
				if (request?.ProjectTemplate != null)
				{
					var projectInfo = new ProjectInfo
					{
						Name = request.Name,
						LocalProjectFolder = GetProjectFolderPath(request.Name, request.ProjectTemplate.Uri?.LocalPath)
					};
					var isProjectInfoValid = IsProjectInfoValid(projectInfo, request);
					if(!isProjectInfoValid)
					{
						return null;
					}
					var project = new FileBasedProject(projectInfo,	new ProjectTemplateReference(request.ProjectTemplate.Uri));

					OnMessageReported(project, $"Creating project {request.Name}");

					if (request.Files != null)
					{
						//path to subdirectory
						var subdirectoryPath = Path.GetDirectoryName(request.Files[0]);

						var projectFiles = project.AddFolderWithFiles(subdirectoryPath, true);
						project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);

						//when a template is created from a Single file project, task sequencies is null.
						try
						{
							var taskSequence = project.RunDefaultTaskSequence(projectFiles.GetIds(),
								(sender, e)
									=>
								{
									if (Requests != null)
									{
										OnProgressChanged(_currentProgress + (double)e.PercentComplete / Requests.Count);
									}
								}
								, (sender, e)
									=>
								{
									OnMessageReported(project, e.Message);
								});

							project.Save();

							if (taskSequence.Status == TaskStatus.Completed)
							{
								if (SuccessfulRequests != null)
								{
									SuccessfulRequests.Add(Tuple.Create(request, project));
									OnMessageReported(project, $"Project {request.Name} created successfully.");
									return project;
								}
							}
							else
							{
								OnMessageReported(project, $"Project {request.Name} creation failed.");
								return null;
							}
						}
						catch (Exception ex)
						{
							Log.Logger.Error($"ProjectCreator-> CreateProject method: {ex.Message}\n {ex.StackTrace}");
							_messageBoxService.ShowMessage(PluginResources.ProjectTemplateSequenceSelection_Message, string.Empty);
						}
					}
					return project;
				}

				_messageBoxService.ShowWarningMessage(PluginResources.ProjectTemplateSelection_Message, PluginResources.MissingProjectTemplate_Message);
			}
		    catch (Exception e)
		    {
				Log.Logger.Error($"ProjectCreator-> CreateProject method: {e.Message}\n {e.StackTrace}");
		    }
			return null;
	    }

		private bool IsProjectInfoValid(ProjectInfo projectInfo, ProjectRequest projectRequest)
		{
			var watchFolderName = Path.GetFileNameWithoutExtension(projectRequest.Path);
			var existsLanguageDirection = IsAnyLanguageDirection(projectRequest.ProjectTemplate.Uri?.LocalPath);

			if (string.IsNullOrEmpty(projectInfo.LocalProjectFolder))
			{
				_messageBoxService.ShowWarningMessage(string.Format(PluginResources.ProjectTemplateLocation_Message, projectRequest?.ProjectTemplate?.Name, watchFolderName), string.Empty);
				return false;
			}			
			else if (!existsLanguageDirection)
			{
				_messageBoxService.ShowWarningMessage(string.Format(PluginResources.ProjectTemplateLanguagePairs_Message, projectRequest?.ProjectTemplate?.Name, watchFolderName), string.Empty);
				return false;
			}
			return true;
		}

	    /// <summary>
        /// Reads the project folder location from selected template
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pathToTemplate"></param>
        /// <returns></returns>
        private string GetProjectFolderPath(string name,string pathToTemplate)
        {
	        try
	        {
		        var templateXml = XElement.Load(pathToTemplate);
		        var settingsGroup = templateXml.Descendants("SettingsGroup").Where(s => s.Attribute("Id").Value.Equals("ProjectTemplateSettings"));
		        var location = settingsGroup.Descendants("Setting").Where(id => id.Attribute("Id").Value.Equals("ProjectLocation")).Select(l => l.Value).FirstOrDefault();
				if(location == null)
				{
					return string.Empty;
				}
		        var rootFolder = location;
		        string folder;
		        int num = 1;
		        do
		        {
			        num++;
		        }
		        while (Directory.Exists(folder = Path.Combine(rootFolder, name + "-" + num)));
		        return folder;
			}
	        catch (Exception e)
			{
				Log.Logger.Error($"GetProjectFolderPath method: {e.Message}\n {e.StackTrace}");
			}
	        return string.Empty;
        }

		/// <summary>
		/// Validate if any language direction is configured on the custom template
		/// </summary>
		/// <param name="templatePath"></param>
		/// <returns></returns>
		private bool IsAnyLanguageDirection(string templatePath)
		{
			try
			{
				var templateXml = XElement.Load(templatePath);
				var languageDirection = templateXml.Descendants("LanguageDirections").Descendants("LanguageDirection")?.FirstOrDefault();				
				if (languageDirection == null)
				{
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"ExistLanguageDirection method: {e.Message}\n {e.StackTrace}");
			}
			return false;
		}

        private void OnProgressChanged(double progress)
        {
			ProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)progress, null));
		}

        private void OnMessageReported(FileBasedProject project, string message)
        {
			MessageReported?.Invoke(this, new ProjectMessageEventArgs { Project = project, Message = message });
		}

        private void OnMessageReported(FileBasedProject project,ExecutionMessage executionMessage)
        {
			MessageReported?.Invoke(this, new ProjectMessageEventArgs { Project = project, Message = executionMessage.Level + ": " + executionMessage.Message });
		}
    }

    public class ProjectMessageEventArgs : EventArgs
    {
        public FileBasedProject Project { get; set; }
        public string Message {get; set;}
    }

}
