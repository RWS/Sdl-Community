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
using TMX_UI.Services;
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
			DataContext = vm;

			_refreshTimer.Tick += (s, a) => RefreshImportExport();

#if DEBUG
			// debug - make it easy to drag it
			WindowStyle = WindowStyle.ThreeDBorderWindow;
#endif
		}

		public OptionsView(IReadOnlyList<string> selectedDbNames, bool careForLocale) : this()
		{
			_initialSelection = selectedDbNames;
			ViewModel.CareForLocale = careForLocale;
		}

		private void RefreshImportExport() {
			ViewModel.ImportFromTmxFile.RefreshProgress();
			ViewModel.ExportToTmx.RefreshProgress();
			ImportService.Instance.GetImportReport(reportCtrl.ViewModel);
			GlobalSettings.Inst.Save();
		}

		private void ok_click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void cancel_click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		// https://www.mongodb.com/docs/manual/reference/limits/
		private static string ValidDbName(string name){
			name = new string(name.Where(ch => !"/\\. \"$*<>:|?".Contains(ch)).ToArray());
			// ... normally, it's 64 chars, but take into considerance the case when we need to append a suffix
			if (name.Length > 58)
				name = name.Substring(0, 58);
			return name;
		}

		private string NewDbName(string wantName)
		{
			for (int idx = 0; ; ++idx)
			{
				var name = wantName;
				if (idx > 0)
					wantName += $" ({idx + 1})";
				if (GlobalSettings.Inst.LocalTmxDatabases.All(db => db != name))
					return name;
			}
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
				if (ViewModel.ImportFromTmxFile.NewDatabaseName == "")
					ViewModel.ImportFromTmxFile.NewDatabaseName = NewDbName(ValidDbName( System.IO.Path.GetFileNameWithoutExtension(dialog.FileName)));
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

		private async void start_import_click(object sender, RoutedEventArgs e)
		{
			var Import = ViewModel.ImportFromTmxFile;
			var dbName = Import.ImportDatabaseName();
			await ImportService.Instance.ImportAsync(Import.FileName, dbName);
			GlobalSettings.Inst.AddTmxDatabase(dbName);

			Import.NewDatabaseName = "";
			Import.ImportComplete = true;
			await Task.Delay(5000);
			Import.ImportComplete = false;
			await RefreshDatabasesAsync();
		}

		private async void start_export_click(object sender, RoutedEventArgs e)
		{
			var Export = ViewModel.ExportToTmx;
			var dbName = Export.Databases[Export.DatabaseIdx].Name;
			await ExportService.Instance.ExportAsync(dbName, Export.FileName);

			try
			{
				System.Diagnostics.Process.Start(Export.FileName);
			}
			catch { 
				// maybe no app associated with .tmx files?
			}

			Export.ExportComplete = true;
			await Task.Delay(5000);
			Export.ExportComplete = false;
		}

		private async Task RefreshDatabasesAsync()
		{
			ViewModel.IsLoading = true;
			List<DatabaseItem> databases = new List<DatabaseItem>();
			foreach (var dbName in GlobalSettings.Inst.LocalTmxDatabases)
			{
				var db = TmxSearchServiceProvider.GetDatabase(dbName);
				try
				{
					await db.InitAsync();
					var languages = await db.GetAllLanguagesAsync();
					databases.Add(new DatabaseItem
					{
						Name = dbName,
						Languages = languages,
						IsSelected = _initialSelection.Contains(dbName),
					});
				}
				catch (Exception ex)
				{
					log.Error($"can't initialize database {dbName}");
				}
			}
			ViewModel.Databases = databases;
			var databasesForImport = databases.ToList();
			databasesForImport.Insert(0, new DatabaseItem
			{
				Name = TMX_UI.Properties.Resources.CreateNewDatabase,
			});
			ViewModel.ImportFromTmxFile.Databases = databasesForImport;
			ViewModel.ExportToTmx.Databases = databases;
			ViewModel.IsLoading = false;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await RefreshDatabasesAsync();
			_refreshTimer.Start();
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			_refreshTimer.Stop();
		}

		private void import_new_tmx_file(object sender, RoutedEventArgs e)
		{
			ViewModel.State = OptionsViewModel.StateType.ImportTmxFile;
		}

		private void download_mongodb_click(object sender, RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("https://www.mongodb.com/try/download/community");
			}
			catch
			{
			}
		}

	}
}
