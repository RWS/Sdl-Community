using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.ViewModel;

namespace Trados.Transcreate.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class ProjectFilesView : UserControl, IUIControl
	{
		public ProjectFilesView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnLoaded;
			Refresh();
		}

		private void Refresh()
		{
			var context = DataContext as ProjectFilesViewModel;
			if (context?.ProjectFiles?.Count == 0)
			{
				// resolves know issue in binding properties on the view before tree is available
				Dispatcher.Invoke(delegate { context.ProjectFiles = new List<ProjectFile> {new ProjectFile()}; }, DispatcherPriority.ContextIdle);
				Dispatcher.Invoke(delegate { context.ProjectFiles = new List<ProjectFile>(); }, DispatcherPriority.ContextIdle);
			}
		}

		public void Dispose()
		{
		}
	}
}
