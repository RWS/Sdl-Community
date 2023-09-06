using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
	[Action("ExtractProjectTermsFiles", Name = "Extract Project Terms Files", Description = "ProjectTerms_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class ProjectTermsFilesControllerAction : AbstractViewControllerAction<FilesController>
    {
		private FilesController _filesController;
		public override void Initialize()
	    {
		    base.Initialize();
		    Text = "Extract Project Terms";			
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_filesController.SelectedFilesChanged += _filesController_SelectedFilesChanged;
		}
				
		protected override void Execute()
        {
            if (Utils.Utils.VerifySingleFileProjectType())
            {
                MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Info);
                return;
            }

            var projectTermsView = new ProjectTermsView();
            projectTermsView.ProjectSelected = false;
            var parent = projectTermsView.ParentForm;
            projectTermsView.ShowDialog(parent);
        }
		private void _filesController_SelectedFilesChanged(object sender, EventArgs e)
		{
			Enabled = false;
			// check if selected project having files available or not 
			var _selectedFiles = _filesController.SelectedFiles.FirstOrDefault();
			if (_selectedFiles != null)
			{
				// check file available or not
				if (File.Exists(_selectedFiles.LocalFilePath))
				{
					Enabled = true;
				}
			}
			else
			{
				var _currentVisibleFiles = _filesController.CurrentVisibleFiles.FirstOrDefault();
				if(_currentVisibleFiles != null )
				{
					// check file available or not
					if (File.Exists(_currentVisibleFiles.LocalFilePath))
					{
						Enabled = true;
					}
				}
			}
		}
	}
}
