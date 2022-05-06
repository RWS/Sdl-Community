using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.View;
using Sdl.Community.StarTransit.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;
using PackageDetails = Sdl.Community.StarTransit.View.PackageDetails;

namespace Sdl.Community.StarTransit
{
	[RibbonGroup("Sdl.Community.StarTransit", Name = "StarTransit", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class StarTransitRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.StarTransit", Name = "Open StarTransit Package", Icon = "open_package", Description = "Open a StarTransit package")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]
	public class StarTransitOpenPackageAction : AbstractAction
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly StudioVersionService _studioVersionService = new StudioVersionService();
		private readonly IEventAggregatorService _eventAggregatorService = new EventAggregatorService(TransitApplicationInitializer.EventAggregator);
		private ProjectsControllerService _projectControllerService;
		private ProjectsController _projectsController;

		private ObservableCollection<IProgressHeaderItem> CreatePages(IWizardModel wizardModel)
		{
			var packageService = new PackageService(_eventAggregatorService);
			var studioService = new StudioService(_projectsController);
			var dialogService = new DialogService();
			var projectService = new ProjectService(_eventAggregatorService);

			var shortStudioVersion = _studioVersionService.GetStudioVersion()?.ShortVersion;
			var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				$@"Studio {shortStudioVersion}\Projects\projects.xml");

			return new ObservableCollection<IProgressHeaderItem>
			{
				new PackageDetailsViewModel(wizardModel, packageService, dialogService, studioService,projectsPath,_eventAggregatorService, new PackageDetails()),
				new TmsViewModel(wizardModel,dialogService,new Tms()),
				new CreateProjectViewModel(wizardModel,projectService,_eventAggregatorService,new CreateProject())
			};
		}

		protected override void Execute()
		{
			try
			{
				_projectsController = SdlTradosStudio.Application?.GetController<ProjectsController>();
				_projectControllerService = new ProjectsControllerService(_projectsController);

				var fileDialog = new OpenFileDialog { Filter = @"Transit Project Package Files (*.ppf)|*.ppf;" };
				var dialogResult = fileDialog.ShowDialog();
				if (dialogResult != DialogResult.OK) return;

				var pathToTempFolder = CreateTempPackageFolder();
				var wizardModel = new WizardModel
				{
					TransitFilePathLocation = fileDialog.FileName,
					PathToTempFolder = pathToTempFolder
				};
				var pages = CreatePages(wizardModel);

				using (var wizard = new ImportWizard(pages, _eventAggregatorService, _projectControllerService))
				{
					wizard.ShowDialog();
				}
			}

			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// We need to delete all the files from subfolders before deleteing the main directory. Otherwise sometimes it throws an error
		/// </summary>
		private string CreateTempPackageFolder()
		{
			var tempFolder = $@"C:\Users\{Environment.UserName}\StarTransit";
			var pathToTempPackageFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString());

			//Delete existing folder where we extract transit packages
			try
			{
				if (Directory.Exists(tempFolder))
				{
					DeleteDirectory(tempFolder);
				}
				Directory.CreateDirectory(pathToTempPackageFolder);
			}
			catch (IOException)
			{
				Directory.Delete(tempFolder, true);
			}
			catch (UnauthorizedAccessException)
			{
				Directory.Delete(tempFolder, true);
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}

			return pathToTempPackageFolder;
		}

		private void DeleteDirectory(string directoryPath)
		{
			//Delete all files from folder before deleting the folder
			var files = Directory.GetFiles(directoryPath);
			var dirs = Directory.GetDirectories(directoryPath);

			foreach (var file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (var dir in dirs)
			{
				DeleteDirectory(dir);
			}
			Directory.Delete(directoryPath, false);
		}
	}

	[Action("Sdl.Community.StarTransit.Return", Name = "StarTransit return package", Icon = "return_package", Description = "StarTransit return package")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]

	public class ReturnPackageAction : AbstractAction
	{
		private IMessageBoxService _messageBoxService;
		private ProjectsControllerService _projectsControllerService;
		private ProjectsController _projectsController;
		private readonly IEventAggregatorService _eventAggregatorService = new EventAggregatorService(TransitApplicationInitializer.EventAggregator);

		protected override void Execute()
		{
			_messageBoxService = new MessageBoxService();
			_projectsController = SdlTradosStudio.Application?.GetController<ProjectsController>();
			_projectsControllerService = new ProjectsControllerService(_projectsController);
			var returnService = new ReturnPackageService(_projectsControllerService);
			var dialogService = new DialogService();

			var returnPackage = returnService.GetPackage();
			if (returnPackage.Item1 is null) return;
			if (!string.IsNullOrEmpty(returnPackage.Item2))
			{
				_messageBoxService.ShowWarningMessage(returnPackage.Item2, "Warning");
			}
			else if (returnPackage.Item1.FileBasedProject != null && returnPackage.Item1.TargetFiles.Count > 0)
			{
				var xliffFiles = returnPackage.Item1.TargetFiles?.Any(file => file.Name.EndsWith(".sdlxliff"));

				if (xliffFiles.Value)
				{
					using (var returnPackageWindow = new ReturnPackageWindow(_eventAggregatorService))
					{
						var returnViewModel =
							new ReturnPackageWindowViewModel(returnPackage.Item1, returnService, dialogService,_eventAggregatorService);
						returnPackageWindow.DataContext = returnViewModel;
						returnPackageWindow.ShowDialog();
					}
				}
				else
				{
					_messageBoxService.ShowWarningMessage("The target file(s) has already been returned. In order to repeat the process, you need to revert to .sdlxliff(s) from the Files view.", "Warning");
				}
			}
		}
	}

	[Action("Sdl.Community.StarTransit.Contribute", Name = "Contribute to project", Icon = "opensourceimage", Description = "Contribute to project")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]
	public class ContributeToProjectAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/tree/master/StarTransit");
		}
	}
}