using System.Collections.ObjectModel;
using ProjectWizardExample.Model;
using ProjectWizardExample.Wizard.API;
using ProjectWizardExample.Wizard.View;
using ProjectWizardExample.WizardPages.View;
using ProjectWizardExample.WizardPages.ViewModel;

namespace ProjectWizardExample
{
	public class Startup
	{
		public void Execute()
		{
			var project = CreateProject();
			var pages = CreatePages(project);

			var projectWizard = new ProjectWizard(pages);
			projectWizard.Show();
		}

		private static Project CreateProject()
		{
			var project = new Project
			{
				Name = "Sample Project",
				Description = "Sample Project Description",
				ClientName = "Sample Client Name"
			};
			return project;
		}

		private static ObservableCollection<IProgressHeaderItem> CreatePages(Project project)
		{
			var pages = new ObservableCollection<IProgressHeaderItem>
			{
				new Page01ViewModel(project, new Page01View()),
				new Page02ViewModel(project, new Page02View()),
				new Page03ViewModel(project, new Page03View()){ IsUpdated = true},
				new Page04ViewModel(project, new Page04View()),
				new Page05ViewModel(project, new Page05View())
			};

			return pages;
		}
	}
}
