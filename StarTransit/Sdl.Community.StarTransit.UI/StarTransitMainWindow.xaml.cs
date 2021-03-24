using System.Windows;
using System.Windows.Controls;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Controls;
using Sdl.Community.StarTransit.UI.ViewModels;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.UI
{
	/// <summary>
	/// Interaction logic for StarTransitMainWindow.xaml
	/// </summary>
	public partial class StarTransitMainWindow
	{
		private readonly PackageDetails _packageDetails;
		private readonly TranslationMemories _translationMemories;
        private readonly Finish _finish;

		public StarTransitMainWindow(PackageModel package)
		{
            InitializeComponent();

            IMessageBoxService messageBoxService = new MessageBoxService();
			var packageDetailsViewModel = new PackageDetailsViewModel(package, messageBoxService);
			_packageDetails = new PackageDetails(packageDetailsViewModel);

			var tmViewModel = new TranslationMemoriesViewModel(packageDetailsViewModel);
			_translationMemories = new TranslationMemories(tmViewModel);

			var finishViewModel = new FinishViewModel(tmViewModel, packageDetailsViewModel);
			_finish = new Finish(finishViewModel);

			var starTransitViewModel = new StarTransitMainWindowViewModel(
				packageDetailsViewModel,
				_packageDetails,
				_translationMemories,
				tmViewModel,
				finishViewModel,
                messageBoxService);

			DataContext = starTransitViewModel;

			if (starTransitViewModel.CloseAction == null)
			{
				starTransitViewModel.CloseAction = Close;
			}
		}

		private void ListViewItem_Selected(object sender, RoutedEventArgs e)
		{
			if (sender == null)
			{
				return;
			}

			var li = sender as ListViewItem;
			if (li == null)
			{
				return;
			}

			switch (li.Tag.ToString())
			{
				case "packageDetails":
					tcc.Content = _packageDetails;
					break;
				case "tm":
					tcc.Content = _translationMemories;
					break;
				case "finish":
					tcc.Content = _finish;
					break;
				default:
					tcc.Content = _packageDetails;
					break;
			}
		}

		private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
		{
			packageDetailsItem.IsSelected = true;
		}

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var starTransitViewModel = (StarTransitMainWindowViewModel)DataContext;
            if (starTransitViewModel == null) return;
            e.Cancel = starTransitViewModel.Active;
            if (starTransitViewModel.CreatedProject == null) return;
            var helpers = new Shared.Utils.Helpers();
            var projectsController = helpers.GetProjectsController();
            projectsController?.Open((FileBasedProject)starTransitViewModel.CreatedProject);
        }
	}
}