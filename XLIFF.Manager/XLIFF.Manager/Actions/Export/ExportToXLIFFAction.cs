using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.LanguageMapping;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions.Export
{
	[Action("XLIFFManager_ExportToXLIFF_Action", typeof(XLIFFManagerViewController),
		Name = "XLIFFManager_ExportToXLIFF_Name",
		Icon = "Export",
		Description = "XLIFFManager_ExportToXLIFF_Description")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 5, DisplayType.Large)]
	public class ExportToXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private Controllers _controllers;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private IDialogService _dialogService;
		private SegmentBuilder _segmentBuilder;
		private ILanguageProvider _languageProvider;
		
		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _pathInfo, _customerProvider,
				 _imageService, _controllers, _segmentBuilder, GetSettings(), _dialogService, _languageProvider);

			var wizardContext = wizardService.ShowWizard(Controller, out var message);
			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.XLIFFManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			_controllers.XliffManagerController.UpdateProjectData(wizardContext);
		}

		public void LaunchWizard()
		{
			Execute();
		}

		public override void Initialize()
		{
			_controllers = new Controllers();
			SetupXliffManagerController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_languageProvider = new LanguageProvider(_pathInfo);

			Enabled = true;
		}

		private Settings GetSettings()
		{			
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private void SetupXliffManagerController()
		{
			_controllers.XliffManagerController.ProjectSelectionChanged += OnProjectSelectionChanged;		
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			Enabled = e.SelectedProject != null;
		}
	}
}
