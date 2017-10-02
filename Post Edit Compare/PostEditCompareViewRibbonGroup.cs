using System;
using System.Windows.Forms;
using PostEdit.Compare;
using PostEdit.Compare.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Linq;
using System.Collections.Generic;
using PostEdit.Compare.Forms;
using Sdl.Community.PostEdit.Compare;
using static Sdl.Community.PostEdit.Compare.Core.Comparison.PairedFiles;
using Application = PostEdit.Compare.Cache.Application;
using System.IO;
using Sdl.Community.PostEdit.Compare.Core.Helper;
//using PostEdit.Compare;
//using PostEdit.Compare.Model;

namespace Sdl.Community.PostEdit.Versions
{

    [RibbonGroup("PostEditCompareMainProjectRibbonGroup", "PostEditCompareMainProjectRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 5)]
    internal class PostEditCompareMainProjectRibbonGroup : AbstractRibbonGroup
    {
    }


    [Action("CreateProjectVersionMainProject",
        typeof(ProjectsController),  
        Name = "CreateProjectVersion_Name",
        Description = "CreateProjectVersion_Description",
        Icon = "CreateProjectVersion_Icon")]
    [ActionLayout(typeof(PostEditCompareMainProjectRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.V)]
    public class CreateProjectVersionMainProject : AbstractViewControllerAction<PostEditCompareViewController>
    {

        public override void Initialize()
        {
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewProjectVersion;
        }
        protected override void Execute()
        {
            Controller.CreateNewProjectVersion(true);
        }
    }



	[Action("ContextCreateProjectVersionAction", typeof(PostEditCompareViewController), Name = "CreateProjectVersion_Name", Description = "CreateProjectVersion_Description", Icon = "CreateProjectVersion_Icon")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 3, DisplayType.Default, "", true)]
    [Shortcut(Keys.Control | Keys.Alt | Keys.V)]
    public class ContextCreateProjectVersionAction : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewProjectVersion;
        }
        protected override void Execute()
        {
            Controller.CreateNewProjectVersion(true);
        }
    }

	[Action("CreateReport",
  typeof(PostEditCompareViewController),
  Name = "Create Comparison Report",
  Description = "Create Comparison Report",
  Icon = "CompareProjects_Icon")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class CreateProjectReport : AbstractViewControllerAction<PostEditCompareViewController>
	{
		protected override void Execute()
		{
			IModel mModel = new Model();
			var postEditCompare = new FormMain(mModel);

			var skipWindow = new SkipSettingsWindow();
			skipWindow.ShowDialog();

			var reportWizard = new ReportWizard();
			postEditCompare.InitializeReportWizard(reportWizard);

			if (skipWindow.CustomizeSettings)
			{
				reportWizard.IsFromProjectsViewCall = true;
				reportWizard.ShowDialog();
				postEditCompare.SetPriceGroup(reportWizard);
				
				CreateReport(postEditCompare);
			}
			if (skipWindow.SkipSettings)
			{
				postEditCompare.SetPriceGroup(reportWizard);

				CreateReport(postEditCompare);
			}
		}

		private void CreateReport(FormMain postEditCompare)
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedProjectsId = new List<string>();
			var comparer = postEditCompare.CreateProcessor();
			var cancel = false;
			foreach (var studioProject in projectController.SelectedProjects)
			{
				var id = studioProject.GetProjectInfo().Id.ToString();
				selectedProjectsId.Add(id);
			}

			var projectsFromSettings = Controller.Settings.projects;
			var selectedVersionProjects = projectsFromSettings
				.Where(proj => selectedProjectsId.Any(p => p.Equals(proj.id))).ToList();
			//var reportPathAutoSave = Application.Settings.ReportsAutoSaveFullPath;
			//aici ar trebui sa creez fisierul excel ca sa fiu sigura ca se creeaza un singur fisier pt mai multe proiecte selectate
			// dar de fiecare data cand se selecteaza alte proiecte ma asigur ca  se creeaza alt raport

			//var excelSavePath = postEditCompare.SetAutoSavePath();
			var reportPathAutoSave = Application.Settings.ReportsAutoSaveFullPath;
			var reportNameAutoSave = postEditCompare.SetAutoSavePath();
			reportNameAutoSave = postEditCompare.GetAutoSaveFileName(reportNameAutoSave);

			if (Application.Settings.ReportsCreateMonthlySubFolders)
			{
				reportPathAutoSave = Path.Combine(reportPathAutoSave,
								   DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0'), "Excel Reports");
				if (!Directory.Exists(reportPathAutoSave))
					Directory.CreateDirectory(reportPathAutoSave);
			}
			else
			{
				reportPathAutoSave = Path.Combine(reportPathAutoSave, "Excel Reports");
			}

			var excelReportFullPath = Path.Combine(reportPathAutoSave, reportNameAutoSave+".xlsx");
			postEditCompare.SetExcelReportPath(excelReportFullPath);

			// create excel report
			ExcelReportHelper.CreateExcelReport(excelReportFullPath, selectedVersionProjects[0].name);

			

			foreach (var project in selectedVersionProjects)
			{
				var package = ExcelReportHelper.GetExcelPackage(excelReportFullPath);
				var normalizedName = ExcelReportHelper.NormalizeWorksheetName(project.name);
				var worksheetExists = Helper.WorksheetExists(package, project.name);
				if (!worksheetExists)
				{
					Helper.AddNewWorksheetToReport(package, normalizedName);
				}

				postEditCompare.SetExcelSheetName(normalizedName);
				var versionDetails = Helper.CreateVersionDetails(project);

				var filesPairs = Helper.GetPairedFiles(versionDetails);

				postEditCompare.ParseContentFromFiles(comparer, filesPairs, ref cancel);
				postEditCompare.CreateComparisonReport(cancel, comparer);
			}
		}
	}



	[RibbonGroup("PostEditCompareRibbonGroup", "PostEditCompareRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 0)]
    class PostEditCompareRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("CompareProjectVersionsAction", typeof(PostEditCompareViewController), Name = "CompareProjectVersions_Name", Description = "CompareProjectVersions_Description", Icon = "CompareProjects_Icon")]
    [ActionLayout(typeof(PostEditCompareRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    public class CompareProjectVersionsAction : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCompare;
        }
        protected override void Execute()
        {

            Controller.CompareProjectVersions();
        }
    }

    [Action("PostEditCompareOpenCompare", typeof(PostEditCompareViewController), Name = "PostEditCompareOpenCompare_Name", Description = "PostEditCompareOpenCompare_Description", Icon = "PostEditCompare_Icon")]
    [ActionLayout(typeof(PostEditCompareRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Large)]
    public class PostEditCompareOpenCompare : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {

        }


        protected override void Execute()
        {

            IModel mModel = new Model();
            var postEditCompare = new FormMain(mModel);
            postEditCompare.ShowDialog();
        }
    }

    [RibbonGroup("PostEditCompareViewContactRibbonGroup", "PostEditCompareViewContactRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex =1)]
    internal class PostEditCompareViewContactRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("PostEditCompareContactAbout", typeof(PostEditCompareViewController), Name = "PostEditCompareContactAbout_Name", Description = "PostEditCompareContactAbout_Description", Icon = "PostEditCompareContactAbout_Icon")]
    [ActionLayout(typeof(PostEditCompareViewContactRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Large)]
    public class PostEditCompareContactAbout : AbstractViewControllerAction<PostEditCompareViewController>
    {
        protected override void Execute()
        {
            Controller.ViewAboutInfo();
        }
    }

    [Action("PostEditCompareContactHelp", typeof(PostEditCompareViewController), Name = "PostEditCompareContactHelp_Name", Description = "PostEditCompareContactHelp_Description", Icon = "PostEditCompareContactHelp_Icon")]
    [ActionLayout(typeof(PostEditCompareViewContactRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class PostEditCompareContactHelp : AbstractViewControllerAction<PostEditCompareViewController>
    {
        protected override void Execute()
        {
            Controller.ViewOnlineHelp();
        }
    }


    [RibbonGroup("PostEditCompareViewRibbonGroup", "PostEditCompareViewRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 2)]
    internal class PostEditCompareViewRibbonGroup : AbstractRibbonGroup
    {
       
    }

 
    [Action("CreateProjectVersionAction", typeof(PostEditCompareViewController), Name = "CreateProjectVersion_Name", Description = "CreateProjectVersion_Description", Icon = "CreateProjectVersion_Icon")]
    [ActionLayout(typeof(PostEditCompareViewRibbonGroup), ZIndex = 4, DisplayType = DisplayType.Large)]
    public class CreateProjectsAction : AbstractViewControllerAction<PostEditCompareViewController>
    {

        public override void Initialize()
        {
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledCreateNewProjectVersion;
        }
        protected override void Execute()
        {
            Controller.CreateNewProjectVersion(false);
        }
    }

    [Action("EditProjectVersionAction", typeof(PostEditCompareViewController), Name = "EditProjectVersion_Name", Description = "EditProjectVersion_Description", Icon = "EditProjectVersion_Icon")]
    [ActionLayout(typeof(PostEditCompareViewRibbonGroup), ZIndex = 3, DisplayType = DisplayType.Normal)]
    public class EditProjectsAction : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledEditProjectVersion;       
        }

        protected override void Execute()
        {
            Controller.EditProjectVersion();
        }
    }


    [Action("RemoveProjectVersionAction", typeof(PostEditCompareViewController), Name = "RemoveProjectVersion_Name", Description = "RemoveProjectVersion_Description", Icon = "RemoveProjectVersion_Icon")]
    [ActionLayout(typeof(PostEditCompareViewRibbonGroup), ZIndex = 2, DisplayType = DisplayType.Normal)]
    public class RemoveProjectsAction : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledDeleteProjectVersion;
        }
        protected override void Execute()
        {
            Controller.RemoveProjectVersions();

        }
    }


    [Action("RestoreProjectVersionsAction", typeof(PostEditCompareViewController), Name = "RestoreProjectVersion_Name", Description = "RestoreProjectVersion_Description", Icon = "RestoreProjectVersion_Icon")]
    [ActionLayout(typeof(PostEditCompareViewRibbonGroup), ZIndex = 1, DisplayType = DisplayType.Normal)]
    public class RestoreProjectVersionsAction : AbstractViewControllerAction<PostEditCompareViewController>
    {
        public override void Initialize()
        {
            Enabled = false;
            Controller.CheckEnabledObjectsEvent += Controller_CheckEnabledObjectsEvent;
        }

        private void Controller_CheckEnabledObjectsEvent(object sender, EventArgs e)
        {
            Enabled = Controller.IsEnabledRestoreProjectVersion;
        }
        protected override void Execute()
        {

            Controller.RestoreProjectVersion();
        }
    }




    [RibbonGroup("PostEditCompareViewConfigurationRibbonGroup", "PostEditCompareViewConfigurationRibbonGroup_Name")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation), ZIndex = 3)]
    internal class PostEditCompareViewConfigurationRibbonGroup : AbstractRibbonGroup
    {
    }




    [Action("PostEditCompareActionsConfiguration", typeof(PostEditCompareViewController), Name = "PostEditCompareActionsConfiguration_Name", Description = "PostEditCompareActionsConfiguration_Description", Icon = "PostEditCompareActionsConfiguration_Icon")]
    [ActionLayout(typeof(PostEditCompareViewConfigurationRibbonGroup), 1, DisplayType.Large)]
    public class PostEditCompareActionsConfiguration : AbstractViewControllerAction<PostEditCompareViewController>
    {
        protected override void Execute()
        {
            Controller.LoadConfigurationSettings();
        }
    }





}
