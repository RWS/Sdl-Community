using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using NLog;
using TMX_Lib.Search;
using TMX_Lib.Utils;
using TMX_UI.ViewModel;

namespace TMX_UI.View
{
	/// <summary>
	/// Interaction logic for OptionsView.xaml
	/// </summary>
	public partial class OptionsView : Window
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		public OptionsViewModel ViewModel => DataContext as OptionsViewModel;
		private IReadOnlyList<string> _initialSelection = new List<string>();
		private DispatcherTimer _refreshTimer = new DispatcherTimer { 
			Interval = TimeSpan.FromMilliseconds(1000),
		};

		public OptionsView()
		{
			InitializeComponent();
			var vm = new OptionsViewModel();
			vm = new OptionsViewModel { 
				Databases = new[] {
					new DatabaseItem { Name = "first db", Languages = new[]{ "en-US", "fr-fr"}  },
					new DatabaseItem { Name = "second db", Languages = new[]{ "en-gb", "ro-ro"}  },
					new DatabaseItem { Name = "third db", Languages = new[]{ "en-gb", "es-ES"}  },
				}
			};
			vm.ImportFromTmxFile.Databases = new[] {
					new DatabaseItem { Name = "[new]", Languages = new[]{ "en-US", "fr-fr"}  },
					new DatabaseItem { Name = "first db", Languages = new[]{ "en-US", "fr-fr"}  },
					new DatabaseItem { Name = "second db", Languages = new[]{ "en-gb", "ro-ro"}  },
					new DatabaseItem { Name = "third db", Languages = new[]{ "en-gb", "es-ES"}  },
				};
			vm.ExportToTmx.Databases = new[] {
					new DatabaseItem { Name = "first db", Languages = new[]{ "en-US", "fr-fr"}  },
					new DatabaseItem { Name = "second db", Languages = new[]{ "en-gb", "ro-ro"}  },
					new DatabaseItem { Name = "third db", Languages = new[]{ "en-gb", "es-ES"}  },
				};
			DataContext = vm;

			_refreshTimer.Tick += (s, a) => RefreshImportExport();
		}

		public OptionsView(IReadOnlyList<string> selectedDbNames) : this()
		{
			_initialSelection = selectedDbNames;
		}

		private void RefreshImportExport() {
			ViewModel.ImportFromTmxFile.RefreshProgress();
			ViewModel.ExportToTmx.RefreshProgress();
		}

		private void ok_click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void cancel_click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void browse_import_file_click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.FileName = ""; // Default file name
			dialog.DefaultExt = ".tmx"; // Default file extension
			dialog.Filter = "TMX Translation documents (.tmx)|*.tmx";

			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				ViewModel.ImportFromTmxFile.FileName = dialog.FileName;
			}
		}

		private void browse_export_file_click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog();
			dialog.FileName = ""; // Default file name
			dialog.DefaultExt = ".tmx"; // Default file extension
			dialog.Filter = "TMX Translation documents (.tmx)|*.tmx";

			bool? result = dialog.ShowDialog();
			if (result == true)
			{
				ViewModel.ExportToTmx.FileName = dialog.FileName;
			}
		}

		private void start_import_click(object sender, RoutedEventArgs e)
		{

		}

		private void start_export_click(object sender, RoutedEventArgs e)
		{

		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_refreshTimer.Start();
			ViewModel.IsLoading = true;
			List<DatabaseItem> databases = new List<DatabaseItem>(); 
			foreach (var dbName in GlobalSettings.Inst.LocalTmxDatabases) {
				var db = TmxSearchServiceProvider.GetSearchService(dbName);
				try
				{
					await db.InitAsync();
					databases.Add( new DatabaseItem { 
						Name = dbName,
						Languages = db.Languages,
						IsSelected = _initialSelection.Contains(dbName),
					});
				}
				catch(Exception ex) {
					log.Error($"can't initialize database {dbName}");
				}
			}
			ViewModel.IsLoading = false;
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			_refreshTimer.Stop();
		}
	}
}
