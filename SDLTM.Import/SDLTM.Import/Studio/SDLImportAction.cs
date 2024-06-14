using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;
using SDLTM.Import.Service;
using SDLTM.Import.View;
using SDLTM.Import.ViewModel;

namespace SDLTM.Import.Studio
{
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	[RibbonGroup("Sdl.Community.SDLImportTm", Name = "SDLTM Import Plus", Icon = "importplus", Description = "Launch the SDLTM Import Plus application")]
	public class SdlImportRibbonGroup : AbstractRibbonGroup
	{
    }

	[Action("SdlTmImportPlus",
		Name = "SDLTM Import Plus",
		Icon = "importplus",
		Description = "SDLTM Import Plus")]
	[ActionLayout(typeof(SdlImportRibbonGroup), 20, DisplayType.Large)]
	public class SdlImportAction : AbstractAction
	{
		protected override void Execute()
		{
			var wizardModel = new WizardModel
			{
				TmsList = new ObservableCollection<TmDetails>(),
				FilesList = new ObservableCollection<FileDetails>(),
				ImportSettings = new Settings()
			};
			
			var pages = CreatePages(wizardModel);
			var wizard = new Wizard(pages);

			ElementHost.EnableModelessKeyboardInterop(wizard);
			wizard.ShowDialog();
		}

		private ObservableCollection<IProgressHeaderItem> CreatePages(WizardModel wizardModel)
		{
			var dataService = new TmVmService();
			var openFileDialogService = new OpenFileDialogService();
			var selectFolderDialogService = new SelectFolderDialogService();
			var messageBoxService = new MessageBoxService();
			var translantionMemoryService = new TranslationMemoryService();
			var importViewModelService = new ImportViewModelService();
			var filesService = new FilesDialogService(openFileDialogService,selectFolderDialogService,messageBoxService);
			
			return new ObservableCollection<IProgressHeaderItem>
			{
				new TmViewModel(wizardModel,dataService,filesService,new TmsView()),
				new XliffCustomFieldsViewModel(wizardModel,new XliffCustomFieldsView()),
				new FileNameCustomFieldViewModel(wizardModel,new FileNameCustomFieldView()),
				new ImportSettingsViewModel(wizardModel,new ImportSettingsView()),
				new ImportViewModel(wizardModel,translantionMemoryService,importViewModelService,new ImportView())
			};
		}
	}
}
